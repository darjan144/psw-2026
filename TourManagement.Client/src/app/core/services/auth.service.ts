import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  UserRole,
} from '../models/user.model';

interface CurrentUser {
  id: number;
  username: string;
  role: UserRole;
  token: string;
}

const TOKEN_KEY = 'token';
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const NAMEID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';
const NAME_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _currentUser = signal<CurrentUser | null>(null);

  readonly currentUser = this._currentUser.asReadonly();
  readonly isLoggedIn = computed(() => this._currentUser() !== null);
  readonly role = computed(() => this._currentUser()?.role ?? null);
  readonly userId = computed(() => this._currentUser()?.id ?? null);
  readonly username = computed(() => this._currentUser()?.username ?? null);

  constructor(private readonly http: HttpClient) {
    this.hydrateFromStorage();
  }

  login(req: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/auth/login`, req)
      .pipe(tap((res) => this.setSession(res)));
  }

  register(req: RegisterRequest): Observable<unknown> {
    return this.http.post(`${environment.apiUrl}/auth/register`, req);
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this._currentUser.set(null);
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.token);
    this._currentUser.set({
      id: res.id,
      username: res.username,
      role: res.role,
      token: res.token,
    });
  }

  private hydrateFromStorage(): void {
    const token = localStorage.getItem(TOKEN_KEY);
    if (!token) return;
    const user = this.decode(token);
    if (user) this._currentUser.set(user);
  }

  private decode(token: string): CurrentUser | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = (payload[ROLE_CLAIM] ?? payload['role']) as UserRole | undefined;
      const idRaw = payload[NAMEID_CLAIM] ?? payload['nameid'] ?? payload['sub'];
      const name = payload[NAME_CLAIM] ?? payload['unique_name'] ?? payload['name'] ?? '';
      if (!role || idRaw == null) return null;
      return { id: Number(idRaw), username: String(name), role, token };
    } catch {
      return null;
    }
  }
}
