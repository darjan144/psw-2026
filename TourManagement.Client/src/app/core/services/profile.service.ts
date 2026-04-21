import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Profile } from '../models/profile.model';
import { Interest } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  constructor(private readonly http: HttpClient) {}

  getProfile(touristId: number): Observable<Profile> {
    return this.http.get<Profile>(`${environment.apiUrl}/profile/${touristId}`);
  }

  updateProfile(cmd: {
    touristId: number;
    interests: Interest[];
    recommendationsEnabled: boolean;
  }): Observable<Profile> {
    return this.http.put<Profile>(`${environment.apiUrl}/profile`, cmd);
  }
}
