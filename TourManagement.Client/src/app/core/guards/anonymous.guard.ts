import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

export const anonymousGuard: CanActivateFn = () => {
  const router = inject(Router);
  const auth = inject(AuthService);

  if (!auth.isLoggedIn()) return true;

  router.navigate([landingFor(auth.role())]);
  return false;
};

export function landingFor(role: UserRole | null): string {
  switch (role) {
    case UserRole.Guide:
      return '/guide/tours';
    case UserRole.Administrator:
      return '/admin/blocked-users';
    case UserRole.Tourist:
    default:
      return '/tours';
  }
}
