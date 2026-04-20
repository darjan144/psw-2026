import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';

import { TourService } from '../../../core/services/tour.service';
import { AuthService } from '../../../core/services/auth.service';
import { Tour } from '../../../core/models/tour.model';

@Component({
  selector: 'app-guide-tour-list',
  standalone: true,
  imports: [RouterLink, DatePipe, DecimalPipe],
  templateUrl: './guide-tour-list.component.html',
})
export class GuideTourListComponent implements OnInit {
  private readonly tourService = inject(TourService);
  private readonly auth = inject(AuthService);

  readonly tours = signal<Tour[]>([]);
  readonly loading = signal(true);
  readonly sortAscending = signal(true);

  ngOnInit(): void {
    this.loadTours();
  }

  toggleSort(): void {
    this.sortAscending.update((v) => !v);
    this.loadTours();
  }

  private loadTours(): void {
    const guideId = this.auth.userId()!;
    this.tourService.getMyTours(guideId, this.sortAscending()).subscribe({
      next: (tours) => {
        this.tours.set(tours);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
