import { Routes } from '@angular/router';

export const TOUR_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./tour-list/tour-list.component').then((m) => m.TourListComponent),
  },
  {
    path: ':tourId',
    loadComponent: () =>
      import('./tour-detail/tour-detail.component').then((m) => m.TourDetailComponent),
  },
];
