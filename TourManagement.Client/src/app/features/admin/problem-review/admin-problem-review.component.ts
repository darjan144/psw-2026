import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';

import { ProblemService } from '../../../core/services/problem.service';
import { AdminService } from '../../../core/services/admin.service';
import { ToastService } from '../../../core/services/toast.service';
import { Problem, ProblemEvent } from '../../../core/models/problem.model';

@Component({
  selector: 'app-admin-problem-review',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './admin-problem-review.component.html',
})
export class AdminProblemReviewComponent implements OnInit {
  private readonly problemService = inject(ProblemService);
  private readonly toast = inject(ToastService);

  readonly problems = signal<Problem[]>([]);
  readonly loading = signal(true);
  readonly expandedHistory = signal<Map<number, ProblemEvent[]>>(new Map());

  ngOnInit(): void {
    this.problemService.getInReview().subscribe({
      next: (problems) => {
        this.problems.set(problems);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  returnToGuide(problemId: number): void {
    this.problemService.returnToGuide(problemId).subscribe({
      next: (updated) => {
        this.problems.update((list) =>
          list.map((p) => (p.id === problemId ? updated : p))
        );
        this.toast.success('Problem returned to guide');
      },
    });
  }

  reject(problemId: number): void {
    this.problemService.reject(problemId).subscribe({
      next: (updated) => {
        this.problems.update((list) =>
          list.map((p) => (p.id === problemId ? updated : p))
        );
        this.toast.success('Problem rejected');
      },
    });
  }

  toggleHistory(problemId: number): void {
    const map = this.expandedHistory();
    if (map.has(problemId)) {
      const updated = new Map(map);
      updated.delete(problemId);
      this.expandedHistory.set(updated);
    } else {
      this.problemService.getHistory(problemId).subscribe({
        next: (events) => {
          const updated = new Map(map);
          updated.set(problemId, events);
          this.expandedHistory.set(updated);
        },
      });
    }
  }

  getHistory(problemId: number): ProblemEvent[] | undefined {
    return this.expandedHistory().get(problemId);
  }
}
