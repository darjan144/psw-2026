import { Component, inject } from '@angular/core';
import { NgClass } from '@angular/common';

import { ToastService, ToastLevel } from '../../../core/services/toast.service';

@Component({
  selector: 'app-toast-container',
  standalone: true,
  imports: [NgClass],
  template: `
    <div
      class="pointer-events-none fixed inset-x-0 top-4 z-50 flex flex-col items-center gap-2 px-4"
      aria-live="polite"
      aria-atomic="true"
    >
      @for (t of toast.toasts(); track t.id) {
        <div
          class="pointer-events-auto flex w-full max-w-md items-start gap-3 rounded-lg border px-4 py-3 shadow-md"
          [ngClass]="classesFor(t.level)"
          role="alert"
        >
          <span class="flex-1 text-sm font-medium">{{ t.message }}</span>
          <button
            type="button"
            class="text-lg leading-none opacity-60 hover:opacity-100"
            (click)="toast.dismiss(t.id)"
            aria-label="Dismiss"
          >
            &times;
          </button>
        </div>
      }
    </div>
  `,
})
export class ToastContainerComponent {
  readonly toast = inject(ToastService);

  classesFor(level: ToastLevel): string {
    switch (level) {
      case 'success':
        return 'border-green-300 bg-green-50 text-green-900';
      case 'error':
        return 'border-red-300 bg-red-50 text-red-900';
      case 'warning':
        return 'border-amber-300 bg-amber-50 text-amber-900';
      case 'info':
      default:
        return 'border-sky-300 bg-sky-50 text-sky-900';
    }
  }
}
