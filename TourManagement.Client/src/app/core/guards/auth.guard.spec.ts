import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';

import { authGuard } from './auth.guard';
import { AuthService } from '../services/auth.service';

describe('authGuard', () => {
  let router: { navigate: ReturnType<typeof vi.fn> };
  let loggedInSig: ReturnType<typeof signal<boolean>>;

  beforeEach(() => {
    router = { navigate: vi.fn() };
    loggedInSig = signal(false);
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: router },
        { provide: AuthService, useValue: { isLoggedIn: loggedInSig } },
      ],
    });
  });

  function run(): boolean {
    return TestBed.runInInjectionContext(() => authGuard({} as any, {} as any)) as boolean;
  }

  it('redirects to /login when logged out', () => {
    expect(run()).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('allows when logged in', () => {
    loggedInSig.set(true);
    expect(run()).toBe(true);
  });
});
