import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register').then((m) => m.Register),
  },
  {
    path: 'tours',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/tours/tours.routes').then((m) => m.TOUR_ROUTES),
  },
  {
    path: 'cart',
    canActivate: [authGuard, roleGuard('Tourist')],
    loadComponent: () =>
      import('./features/cart/cart.component').then((m) => m.CartComponent),
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard('Administrator')],
    loadChildren: () =>
      import('./features/admin/admin.routes').then((m) => m.ADMIN_ROUTES),
  },
  { path: '', redirectTo: '/tours', pathMatch: 'full' },
  { path: '**', redirectTo: '/tours' },
];
