import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { anonymousGuard } from './core/guards/anonymous.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: 'login',
    canActivate: [anonymousGuard],
    loadComponent: () =>
      import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'register',
    canActivate: [anonymousGuard],
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
    path: 'profile',
    canActivate: [authGuard, roleGuard('Tourist')],
    loadComponent: () =>
      import('./features/profile/profile.component').then((m) => m.ProfileComponent),
  },
  {
    path: 'problems/mine',
    canActivate: [authGuard, roleGuard('Tourist')],
    loadComponent: () =>
      import('./features/problems/tourist-problems/tourist-problems.component').then(
        (m) => m.TouristProblemsComponent
      ),
  },
  {
    path: 'guide',
    canActivate: [authGuard, roleGuard('Guide')],
    loadChildren: () =>
      import('./features/guide/guide.routes').then((m) => m.GUIDE_ROUTES),
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
