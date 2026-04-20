import { Routes } from '@angular/router';

export const GUIDE_ROUTES: Routes = [
  {
    path: 'tours',
    loadComponent: () =>
      import('./tour-list/guide-tour-list.component').then((m) => m.GuideTourListComponent),
  },
  {
    path: 'tours/new',
    loadComponent: () =>
      import('./tour-create/tour-create.component').then((m) => m.TourCreateComponent),
  },
  {
    path: 'tours/:tourId',
    loadComponent: () =>
      import('./tour-detail/guide-tour-detail.component').then((m) => m.GuideTourDetailComponent),
  },
  {
    path: 'substitutes',
    loadComponent: () =>
      import('./substitutes/substitute-list.component').then((m) => m.SubstituteListComponent),
  },
  {
    path: 'problems',
    loadComponent: () =>
      import('./problems/guide-problem-list.component').then((m) => m.GuideProblemListComponent),
  },
];
