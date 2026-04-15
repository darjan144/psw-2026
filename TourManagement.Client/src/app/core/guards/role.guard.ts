import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

export const roleGuard =
  (...allowedRoles: string[]): CanActivateFn =>
  () => {
    const router = inject(Router);
    const auth = inject(AuthService);

    if (!auth.isLoggedIn()) {
      router.navigate(['/login']);
      return false;
    }

    const role = auth.role();
    if (role && allowedRoles.includes(role)) {
      return true;
    }

    router.navigate(['/']);
    return false;
  };
