import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { TourService } from '../../../core/services/tour.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Interest } from '../../../core/models/user.model';
import { TourDifficulty } from '../../../core/models/tour.model';

@Component({
  selector: 'app-tour-create',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './tour-create.component.html',
})
export class TourCreateComponent {
  private readonly fb = inject(FormBuilder);
  private readonly tourService = inject(TourService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly difficulties = Object.values(TourDifficulty);
  readonly categories = Object.values(Interest);

  readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: ['', Validators.required],
    difficulty: [TourDifficulty.Easy, Validators.required],
    category: [Interest.Nature, Validators.required],
    price: [0, [Validators.required, Validators.min(0)]],
    scheduledDate: ['', Validators.required],
  });

  readonly submitting = signal(false);

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);

    const val = this.form.getRawValue();
    this.tourService
      .createTour({
        ...val,
        scheduledDate: new Date(val.scheduledDate).toISOString(),
        guideId: this.auth.userId()!,
      })
      .subscribe({
        next: (tour) => {
          this.toast.success('Tour created as draft. Add key points to publish.');
          this.router.navigate(['/guide/tours', tour.id]);
        },
        error: () => this.submitting.set(false),
      });
  }
}
