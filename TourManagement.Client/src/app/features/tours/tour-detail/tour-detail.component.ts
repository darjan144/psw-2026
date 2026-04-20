import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

import { TourService } from '../../../core/services/tour.service';
import { CartService } from '../../../core/services/cart.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Tour } from '../../../core/models/tour.model';
import { UserRole } from '../../../core/models/user.model';
import { LeafletMapComponent } from '../../../shared/components/leaflet-map/leaflet-map.component';

@Component({
  selector: 'app-tour-detail',
  standalone: true,
  imports: [DatePipe, RouterLink, LeafletMapComponent],
  templateUrl: './tour-detail.component.html',
})
export class TourDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly tourService = inject(TourService);
  private readonly cartService = inject(CartService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly tour = signal<Tour | null>(null);
  readonly isTourist = this.auth.role() === UserRole.Tourist;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('tourId'));
    const cached = this.tourService.findById(id);
    if (cached) {
      this.tour.set(cached);
    } else {
      this.tourService.getPublishedTours().subscribe({
        next: () => this.tour.set(this.tourService.findById(id) ?? null),
      });
    }
  }

  addToCart(): void {
    const t = this.tour();
    const touristId = this.auth.userId();
    if (!t || !touristId) return;
    this.cartService.addItem(t.id, touristId).subscribe({
      next: () => this.toast.success(`"${t.name}" added to cart`),
    });
  }
}
