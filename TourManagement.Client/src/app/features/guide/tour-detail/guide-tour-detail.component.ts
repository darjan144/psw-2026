import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe, DecimalPipe } from '@angular/common';

import { switchMap } from 'rxjs';

import { TourService } from '../../../core/services/tour.service';
import { SubstituteService } from '../../../core/services/substitute.service';
import { ImageService } from '../../../core/services/image.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Tour, KeyPoint } from '../../../core/models/tour.model';
import { LeafletMapComponent } from '../../../shared/components/leaflet-map/leaflet-map.component';

@Component({
  selector: 'app-guide-tour-detail',
  standalone: true,
  imports: [ReactiveFormsModule, DatePipe, DecimalPipe, RouterLink, LeafletMapComponent],
  templateUrl: './guide-tour-detail.component.html',
})
export class GuideTourDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly tourService = inject(TourService);
  private readonly substituteService = inject(SubstituteService);
  private readonly imageService = inject(ImageService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  readonly tour = signal<Tour | null>(null);
  readonly clickedLatLng = signal<{ lat: number; lng: number } | null>(null);
  readonly selectedFile = signal<File | null>(null);
  readonly uploading = signal(false);

  readonly kpForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
  });

  ngOnInit(): void {
    const tourId = Number(this.route.snapshot.paramMap.get('tourId'));
    const guideId = this.auth.userId()!;
    this.tourService.getMyTours(guideId).subscribe({
      next: (tours) => {
        const t = tours.find((x) => x.id === tourId) ?? null;
        this.tour.set(t);
      },
    });
  }

  onMapClick(event: { lat: number; lng: number }): void {
    this.clickedLatLng.set(event);
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.selectedFile.set(input.files?.[0] ?? null);
  }

  addKeyPoint(): void {
    const t = this.tour();
    const coords = this.clickedLatLng();
    const file = this.selectedFile();
    if (!t || !coords || this.kpForm.invalid || !file) return;

    this.uploading.set(true);
    const val = this.kpForm.getRawValue();

    this.imageService
      .upload(file)
      .pipe(
        switchMap((imageUrl) =>
          this.tourService.addKeyPoint(t.id, {
            ...val,
            imageUrl,
            latitude: coords.lat,
            longitude: coords.lng,
            guideId: this.auth.userId()!,
          })
        )
      )
      .subscribe({
        next: (kp) => {
          this.tour.set({
            ...t,
            keyPoints: [...t.keyPoints, kp],
          });
          this.kpForm.reset();
          this.selectedFile.set(null);
          this.clickedLatLng.set(null);
          this.uploading.set(false);
          this.toast.success('Key point added');
        },
        error: () => this.uploading.set(false),
      });
  }

  publish(): void {
    const t = this.tour();
    if (!t) return;
    this.tourService.publishTour(t.id, this.auth.userId()!).subscribe({
      next: (updated) => {
        this.tour.set(updated);
        this.toast.success('Tour published!');
      },
    });
  }

  seekSubstitute(): void {
    const t = this.tour();
    if (!t) return;
    this.substituteService.seekSubstitute(t.id, this.auth.userId()!).subscribe({
      next: () => {
        this.tour.set({ ...t, seekingSubstitute: true });
        this.toast.success('Tour marked as seeking substitute');
      },
    });
  }
}
