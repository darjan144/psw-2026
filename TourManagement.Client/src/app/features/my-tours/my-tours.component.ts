import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

import { CartService } from '../../core/services/cart.service';
import { ProblemService } from '../../core/services/problem.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { TouristPurchase } from '../../core/models/purchase.model';

@Component({
  selector: 'app-my-tours',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FormsModule, RouterLink],
  templateUrl: './my-tours.component.html',
})
export class MyToursComponent implements OnInit {
  private readonly cartService = inject(CartService);
  private readonly problemService = inject(ProblemService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly purchases = signal<TouristPurchase[]>([]);
  readonly loading = signal(true);
  readonly reportingForTourId = signal<number | null>(null);

  problemTitle = '';
  problemDescription = '';

  ngOnInit(): void {
    const userId = this.auth.userId();
    if (!userId) return;

    this.cartService.getMyPurchases(userId).subscribe({
      next: (purchases) => {
        this.purchases.set(purchases);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  canReportProblem(purchase: TouristPurchase): boolean {
    const scheduled = new Date(purchase.scheduledDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    scheduled.setHours(0, 0, 0, 0);
    return scheduled <= today;
  }

  openReportForm(tourId: number): void {
    this.reportingForTourId.set(tourId);
    this.problemTitle = '';
    this.problemDescription = '';
  }

  cancelReport(): void {
    this.reportingForTourId.set(null);
  }

  submitProblem(tourId: number): void {
    const touristId = this.auth.userId();
    if (!touristId || !this.problemTitle.trim()) return;

    this.problemService
      .createProblem({
        tourId,
        touristId,
        title: this.problemTitle.trim(),
        description: this.problemDescription.trim(),
      })
      .subscribe({
        next: () => {
          this.toast.success('Problem reported');
          this.reportingForTourId.set(null);
        },
      });
  }
}
