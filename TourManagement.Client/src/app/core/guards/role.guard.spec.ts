import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';

import { roleGuard } from './role.guard';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

describe('roleGuard', () => {
  let router: { navigate: ReturnType<typeof vi.fn> };
  let roleSig: ReturnType<typeof signal<UserRole | null>>;
  let loggedInSig: ReturnType<typeof signal<boolean>>;

  beforeEach(() => {
    router = { navigate: vi.fn() };
    roleSig = signal<UserRole | null>(null);
    loggedInSig = signal(false);
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: router },
        { provide: AuthService, useValue: { role: roleSig, isLoggedIn: loggedInSig } },
      ],
    });
  });

  function run(...allowed: string[]): boolean | undefined {
    return TestBed.runInInjectionContext(() => roleGuard(...allowed)({} as any, {} as any)) as any;
  }

  it('redirects to /login if not logged in', () => {
    const result = run('Tourist');
    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('allows when role matches', () => {
    loggedInSig.set(true);
    roleSig.set(UserRole.Administrator);
    const result = run('Administrator');
    expect(result).toBe(true);
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('rejects and redirects to / when role mismatches', () => {
    loggedInSig.set(true);
    roleSig.set(UserRole.Tourist);
    const result = run('Administrator');
    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });
});
