import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { BlockedUser } from '../models/admin.model';

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private readonly http: HttpClient) {}

  getBlockedUsers(): Observable<BlockedUser[]> {
    return this.http.get<BlockedUser[]>(
      `${environment.apiUrl}/admin/blocked-users`
    );
  }

  unblockUser(userId: number): Observable<BlockedUser> {
    return this.http.put<BlockedUser>(
      `${environment.apiUrl}/admin/unblock/${userId}`,
      null
    );
  }
}
