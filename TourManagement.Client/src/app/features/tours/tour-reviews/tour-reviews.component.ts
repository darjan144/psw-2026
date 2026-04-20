import { Component, inject, input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { DatePipe } from '@angular/common';

import { ReviewService } from '../../../core/services/review.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { TourReview } from '../../../core/models/review.model';
import { UserRole } from '../../../core/models/user.model';

@Component({
  selector: 'app-tour-reviews',
  standalone: true,
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './tour-reviews.component.html',
})
export class TourReviewsComponent implements OnInit {
  private readonly reviewService = inject(ReviewService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  readonly tourId = input.required<number>();
  readonly tourScheduledDate = input.required<string>();

  readonly reviews = signal<TourReview[]>([]);
  readonly showForm = signal(false);

  readonly form = this.fb.nonNullable.group({
    rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    comment: [''],
  });

  ngOnInit(): void {
    this.reviewService.getReviewsForTour(this.tourId()).subscribe({
      next: (reviews) => {
        this.reviews.set(reviews);
        this.checkEligibility();
      },
    });
  }

  get commentRequired(): boolean {
    return this.form.get('rating')?.value! <= 2;
  }

  submitReview(): void {
    if (this.form.invalid) return;
    if (this.commentRequired && !this.form.get('comment')?.value?.trim()) return;

    const val = this.form.getRawValue();
    this.reviewService
      .createReview({
        tourId: this.tourId(),
        touristId: this.auth.userId()!,
        rating: val.rating,
        comment: val.comment?.trim() || null,
      })
      .subscribe({
        next: (review) => {
          this.reviews.update((list) => [...list, review]);
          this.showForm.set(false);
          this.toast.success('Review submitted');
        },
      });
  }

  private checkEligibility(): void {
    if (this.auth.role() !== UserRole.Tourist) return;

    const tourDate = new Date(this.tourScheduledDate());
    const now = new Date();
    const daysSinceTour = (now.getTime() - tourDate.getTime()) / (1000 * 60 * 60 * 24);
    const alreadyReviewed = this.reviews().some(
      (r) => r.touristId === this.auth.userId()
    );

    if (daysSinceTour > 0 && daysSinceTour <= 7 && !alreadyReviewed) {
      this.showForm.set(true);
    }
  }
}
