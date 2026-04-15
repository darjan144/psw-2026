import { Injectable, signal } from '@angular/core';

export type ToastLevel = 'success' | 'error' | 'info' | 'warning';

export interface Toast {
  id: number;
  message: string;
  level: ToastLevel;
}

@Injectable({ providedIn: 'root' })
export class ToastService {
  private readonly _toasts = signal<Toast[]>([]);
  private _nextId = 1;

  readonly toasts = this._toasts.asReadonly();

  show(message: string, level: ToastLevel = 'info', autoDismissMs = 4000): number {
    const id = this._nextId++;
    this._toasts.update((list) => [...list, { id, message, level }]);
    if (autoDismissMs > 0) {
      setTimeout(() => this.dismiss(id), autoDismissMs);
    }
    return id;
  }

  success(message: string, autoDismissMs = 4000): number {
    return this.show(message, 'success', autoDismissMs);
  }

  error(message: string, autoDismissMs = 6000): number {
    return this.show(message, 'error', autoDismissMs);
  }

  info(message: string, autoDismissMs = 4000): number {
    return this.show(message, 'info', autoDismissMs);
  }

  warning(message: string, autoDismissMs = 5000): number {
    return this.show(message, 'warning', autoDismissMs);
  }

  dismiss(id: number): void {
    this._toasts.update((list) => list.filter((t) => t.id !== id));
  }

  clear(): void {
    this._toasts.set([]);
  }
}
