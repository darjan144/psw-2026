import { Routes } from '@angular/router';

export const ADMIN_ROUTES: Routes = [
  {
    path: 'blocked-users',
    loadComponent: () =>
      import('./blocked-users/blocked-users.component').then((m) => m.BlockedUsersComponent),
  },
  {
    path: 'problems',
    loadComponent: () =>
      import('./problem-review/admin-problem-review.component').then(
        (m) => m.AdminProblemReviewComponent
      ),
  },
];
