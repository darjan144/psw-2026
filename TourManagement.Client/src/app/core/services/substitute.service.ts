import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Tour } from '../models/tour.model';

@Injectable({ providedIn: 'root' })
export class SubstituteService {
  constructor(private readonly http: HttpClient) {}

  seekSubstitute(tourId: number, guideId: number): Observable<Tour> {
    const params = new HttpParams().set('guideId', guideId);
    return this.http.put<Tour>(
      `${environment.apiUrl}/substitute/${tourId}/seek`,
      null,
      { params }
    );
  }

  getAvailable(): Observable<Tour[]> {
    return this.http.get<Tour[]>(`${environment.apiUrl}/substitute`);
  }

  assignSubstitute(tourId: number, newGuideId: number): Observable<Tour> {
    const params = new HttpParams().set('newGuideId', newGuideId);
    return this.http.put<Tour>(
      `${environment.apiUrl}/substitute/${tourId}/assign`,
      null,
      { params }
    );
  }
}
