import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';

import { SubstituteService } from '../../../core/services/substitute.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Tour } from '../../../core/models/tour.model';

@Component({
  selector: 'app-substitute-list',
  standalone: true,
  imports: [DatePipe, DecimalPipe],
  templateUrl: './substitute-list.component.html',
})
export class SubstituteListComponent implements OnInit {
  private readonly substituteService = inject(SubstituteService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly tours = signal<Tour[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.substituteService.getAvailable().subscribe({
      next: (tours) => {
        this.tours.set(tours);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  accept(tourId: number): void {
    this.substituteService.assignSubstitute(tourId, this.auth.userId()!).subscribe({
      next: () => {
        this.tours.update((list) => list.filter((t) => t.id !== tourId));
        this.toast.success('You accepted the tour!');
      },
    });
  }
}
