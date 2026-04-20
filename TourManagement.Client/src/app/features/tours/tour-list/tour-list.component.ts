import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';

import { TourService } from '../../../core/services/tour.service';
import { CartService } from '../../../core/services/cart.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Tour } from '../../../core/models/tour.model';
import { UserRole } from '../../../core/models/user.model';

@Component({
  selector: 'app-tour-list',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './tour-list.component.html',
})
export class TourListComponent implements OnInit {
  private readonly tourService = inject(TourService);
  private readonly cartService = inject(CartService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly tours = signal<Tour[]>([]);
  readonly loading = signal(true);
  readonly isTourist = this.auth.role() === UserRole.Tourist;

  ngOnInit(): void {
    this.tourService.getPublishedTours().subscribe({
      next: (tours) => {
        this.tours.set(tours);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  addToCart(tour: Tour): void {
    const touristId = this.auth.userId();
    if (!touristId) return;
    this.cartService.addItem(tour.id, touristId).subscribe({
      next: () => this.toast.success(`"${tour.name}" added to cart`),
    });
  }
}
