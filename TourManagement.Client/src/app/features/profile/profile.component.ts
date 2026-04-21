import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';

import { ProfileService } from '../../core/services/profile.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { Interest } from '../../core/models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './profile.component.html',
})
export class ProfileComponent implements OnInit {
  private readonly profileService = inject(ProfileService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly fb = inject(FormBuilder);

  readonly interests = Object.values(Interest);
  readonly username = this.auth.username();
  readonly saving = signal(false);
  readonly loading = signal(true);
  readonly bonusPoints = signal(0);

  readonly form = this.fb.nonNullable.group({
    interests: this.fb.nonNullable.group(
      Object.values(Interest).reduce(
        (acc, i) => ({ ...acc, [i]: false }),
        {} as Record<string, boolean>
      )
    ),
    recommendationsEnabled: [true],
  });

  ngOnInit(): void {
    const userId = this.auth.userId();
    if (!userId) return;

    this.profileService.getProfile(userId).subscribe({
      next: (profile) => {
        const interestFlags = Object.values(Interest).reduce(
          (acc, i) => ({ ...acc, [i]: profile.interests.includes(i) }),
          {} as Record<string, boolean>
        );
        this.form.patchValue({
          interests: interestFlags,
          recommendationsEnabled: profile.recommendationsEnabled,
        });
        this.bonusPoints.set(profile.bonusPoints);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  save(): void {
    this.saving.set(true);
    const val = this.form.getRawValue();
    const selectedInterests = Object.entries(val.interests)
      .filter(([, checked]) => checked)
      .map(([key]) => key as Interest);

    this.profileService
      .updateProfile({
        touristId: this.auth.userId()!,
        interests: selectedInterests,
        recommendationsEnabled: val.recommendationsEnabled,
      })
      .subscribe({
        next: () => {
          this.toast.success('Profile updated');
          this.saving.set(false);
        },
        error: () => this.saving.set(false),
      });
  }
}
