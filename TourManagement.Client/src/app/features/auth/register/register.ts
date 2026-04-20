import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Interest } from '../../../core/models/user.model';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toast = inject(ToastService);

  readonly interests = Object.values(Interest);

  readonly form = this.fb.nonNullable.group(
    {
      username: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required],
      interests: this.fb.nonNullable.group(
        Object.values(Interest).reduce(
          (acc, i) => ({ ...acc, [i]: false }),
          {} as Record<string, boolean>
        )
      ),
      recommendationsEnabled: [true],
    },
    { validators: [this.passwordsMatch] }
  );

  readonly submitting = signal(false);

  onSubmit(): void {
    if (this.form.invalid) return;
    this.submitting.set(true);

    const val = this.form.getRawValue();
    const selectedInterests = Object.entries(val.interests)
      .filter(([, checked]) => checked)
      .map(([key]) => key as Interest);

    this.auth
      .register({
        username: val.username,
        password: val.password,
        firstName: val.firstName,
        lastName: val.lastName,
        email: val.email,
        interests: selectedInterests,
        recommendationsEnabled: val.recommendationsEnabled,
      })
      .subscribe({
        next: () => {
          this.toast.success('Account created! Please sign in.');
          this.router.navigate(['/login']);
        },
        error: () => {
          this.submitting.set(false);
        },
      });
  }

  private passwordsMatch(control: AbstractControl): ValidationErrors | null {
    const pw = control.get('password')?.value;
    const confirm = control.get('confirmPassword')?.value;
    return pw === confirm ? null : { passwordsMismatch: true };
  }
}
