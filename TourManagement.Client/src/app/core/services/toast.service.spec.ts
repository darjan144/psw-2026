import { TestBed } from '@angular/core/testing';
import { ToastService, ToastLevel } from './toast.service';

describe('ToastService', () => {
  let service: ToastService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ToastService);
  });

  it('starts with empty toasts', () => {
    expect(service.toasts()).toEqual([]);
  });

  it('adds a toast via show()', () => {
    service.show('hello', 'info');
    const toasts = service.toasts();
    expect(toasts.length).toBe(1);
    expect(toasts[0].message).toBe('hello');
    expect(toasts[0].level).toBe<ToastLevel>('info');
  });

  it('assigns unique ids to each toast', () => {
    service.show('a', 'info');
    service.show('b', 'error');
    const toasts = service.toasts();
    expect(toasts[0].id).not.toBe(toasts[1].id);
  });

  it('dismisses a toast by id', () => {
    service.show('a', 'info');
    const id = service.toasts()[0].id;
    service.dismiss(id);
    expect(service.toasts()).toEqual([]);
  });

  it('provides convenience success/error/info methods', () => {
    service.success('ok');
    service.error('bad');
    service.info('fyi');
    const levels = service.toasts().map((t) => t.level);
    expect(levels).toEqual(['success', 'error', 'info']);
  });
});
