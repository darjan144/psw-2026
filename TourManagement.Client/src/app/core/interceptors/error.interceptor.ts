import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toast = inject(ToastService);
  const router = inject(Router);

  return next(req).pipe(
    catchError((err: unknown) => {
      if (err instanceof HttpErrorResponse) {
        if (err.status === 401) {
          localStorage.removeItem('token');
          router.navigate(['/login']);
        } else {
          toast.error(extractMessage(err));
        }
      }
      return throwError(() => err);
    })
  );
};

function extractMessage(err: HttpErrorResponse): string {
  const body = err.error;
  if (typeof body === 'string' && body.trim().length > 0) return body;
  if (body && typeof body === 'object') {
    if (typeof body.message === 'string') return body.message;
    if (typeof body.title === 'string') return body.title;
    if (typeof body.detail === 'string') return body.detail;
  }
  return err.statusText || `HTTP ${err.status}`;
}
