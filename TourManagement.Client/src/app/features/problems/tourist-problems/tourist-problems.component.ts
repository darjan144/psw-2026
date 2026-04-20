import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe } from '@angular/common';

import { ProblemService } from '../../../core/services/problem.service';
import { AuthService } from '../../../core/services/auth.service';
import { Problem, ProblemEvent } from '../../../core/models/problem.model';

@Component({
  selector: 'app-tourist-problems',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './tourist-problems.component.html',
})
export class TouristProblemsComponent implements OnInit {
  private readonly problemService = inject(ProblemService);
  private readonly auth = inject(AuthService);

  readonly problems = signal<Problem[]>([]);
  readonly loading = signal(true);
  readonly expandedHistory = signal<Map<number, ProblemEvent[]>>(new Map());

  ngOnInit(): void {
    this.problemService.getByTourist(this.auth.userId()!).subscribe({
      next: (problems) => {
        this.problems.set(problems);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
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

  statusClass(status: string): string {
    switch (status) {
      case 'Resolved': return 'bg-green-100 text-green-700';
      case 'Rejected': return 'bg-red-100 text-red-700';
      case 'InReview': return 'bg-amber-100 text-amber-700';
      default: return 'bg-slate-100 text-slate-700';
    }
  }
}
