import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const roleGuard =
  (...allowedRoles: string[]): CanActivateFn =>
  () => {
    const router = inject(Router);
    const token = localStorage.getItem('token');

    if (!token) {
      router.navigate(['/login']);
      return false;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const userRole =
        payload[
          'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
        ] || payload['role'];

      if (allowedRoles.includes(userRole)) {
        return true;
      }
    } catch {
      // Invalid token
    }

    router.navigate(['/']);
    return false;
  };
