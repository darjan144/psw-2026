import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { AuthService } from './auth.service';
import { UserRole } from '../models/user.model';
import { environment } from '../../../environments/environment';

function makeToken(payload: Record<string, unknown>): string {
  const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
  const body = btoa(JSON.stringify(payload));
  return `${header}.${body}.sig`;
}

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('starts logged out when no token', () => {
    expect(service.isLoggedIn()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });

  it('hydrates currentUser from stored token on init', () => {
    const token = makeToken({ sub: '42', nameid: '42', unique_name: 'alice', role: 'Tourist' });
    localStorage.setItem('token', token);
    const fresh = TestBed.runInInjectionContext(() => new AuthService(TestBed.inject(HttpClient)));
    expect(fresh.isLoggedIn()).toBe(true);
    expect(fresh.userId()).toBe(42);
    expect(fresh.role()).toBe(UserRole.Tourist);
  });

  it('login stores token and populates currentUser', () => {
    const token = makeToken({ nameid: '7', unique_name: 'bob', role: 'Guide' });
    let called = false;
    service.login({ username: 'bob', password: 'pw' }).subscribe((res) => {
      called = true;
      expect(res.token).toBe(token);
    });
    const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({ id: 7, username: 'bob', role: 'Guide', token });
    expect(called).toBe(true);
    expect(localStorage.getItem('token')).toBe(token);
    expect(service.isLoggedIn()).toBe(true);
    expect(service.role()).toBe(UserRole.Guide);
    expect(service.userId()).toBe(7);
    expect(service.username()).toBe('bob');
  });

  it('register POSTs to /auth/register', () => {
    const body = {
      username: 'c',
      password: 'p',
      firstName: 'f',
      lastName: 'l',
      email: 'e@e.e',
      interests: [],
      recommendationsEnabled: false,
    };
    service.register(body).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/auth/register`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({ id: 1 });
  });

  it('logout clears token and state', () => {
    localStorage.setItem('token', makeToken({ nameid: '1', unique_name: 'x', role: 'Tourist' }));
    const s = TestBed.runInInjectionContext(() => new AuthService(TestBed.inject(HttpClient)));
    s.logout();
    expect(localStorage.getItem('token')).toBeNull();
    expect(s.isLoggedIn()).toBe(false);
    expect(s.currentUser()).toBeNull();
  });
});
