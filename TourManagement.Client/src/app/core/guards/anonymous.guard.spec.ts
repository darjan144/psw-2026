import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { signal } from '@angular/core';

import { anonymousGuard } from './anonymous.guard';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

describe('anonymousGuard', () => {
  let router: { navigate: ReturnType<typeof vi.fn> };
  let loggedInSig: ReturnType<typeof signal<boolean>>;
  let roleSig: ReturnType<typeof signal<UserRole | null>>;

  beforeEach(() => {
    router = { navigate: vi.fn() };
    loggedInSig = signal(false);
    roleSig = signal<UserRole | null>(null);
    TestBed.configureTestingModule({
      providers: [
        { provide: Router, useValue: router },
        { provide: AuthService, useValue: { isLoggedIn: loggedInSig, role: roleSig } },
      ],
    });
  });

  function run(): boolean {
    return TestBed.runInInjectionContext(() => anonymousGuard({} as any, {} as any)) as boolean;
  }

  it('allows when logged out', () => {
    expect(run()).toBe(true);
  });

  it('redirects tourists to /tours when already logged in', () => {
    loggedInSig.set(true);
    roleSig.set(UserRole.Tourist);
    expect(run()).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/tours']);
  });

  it('redirects guides to /guide/tours', () => {
    loggedInSig.set(true);
    roleSig.set(UserRole.Guide);
    run();
    expect(router.navigate).toHaveBeenCalledWith(['/guide/tours']);
  });

  it('redirects admins to /admin/blocked-users', () => {
    loggedInSig.set(true);
    roleSig.set(UserRole.Administrator);
    run();
    expect(router.navigate).toHaveBeenCalledWith(['/admin/blocked-users']);
  });
});
