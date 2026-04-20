import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';

import { ProblemService } from '../../../core/services/problem.service';
import { TourService } from '../../../core/services/tour.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../core/services/toast.service';
import { Problem } from '../../../core/models/problem.model';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-guide-problem-list',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './guide-problem-list.component.html',
})
export class GuideProblemListComponent implements OnInit {
  private readonly problemService = inject(ProblemService);
  private readonly tourService = inject(TourService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly problems = signal<Problem[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    const guideId = this.auth.userId()!;
    this.tourService.getMyTours(guideId).subscribe({
      next: (tours) => {
        if (tours.length === 0) {
          this.loading.set(false);
          return;
        }
        const requests = tours.map((t) => this.problemService.getByTour(t.id));
        forkJoin(requests).subscribe({
          next: (results) => {
            this.problems.set(results.flat());
            this.loading.set(false);
          },
          error: () => this.loading.set(false),
        });
      },
      error: () => this.loading.set(false),
    });
  }

  resolve(problemId: number): void {
    this.problemService.resolve(problemId, this.auth.userId()!).subscribe({
      next: (updated) => {
        this.problems.update((list) =>
          list.map((p) => (p.id === problemId ? updated : p))
        );
        this.toast.success('Problem resolved');
      },
    });
  }

  sendToReview(problemId: number): void {
    this.problemService.sendToReview(problemId, this.auth.userId()!).subscribe({
      next: (updated) => {
        this.problems.update((list) =>
          list.map((p) => (p.id === problemId ? updated : p))
        );
        this.toast.success('Problem sent to admin for review');
      },
    });
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Resolved': return 'bg-green-100 text-green-700';
      case 'Rejected': return 'bg-red-100 text-red-700';
      case 'InReview': return 'bg-amber-100 text-amber-700';
      default: return 'bg-slate-100 text-slate-700';
    }
  }
}
