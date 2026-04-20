import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Tour, KeyPoint } from '../models/tour.model';

@Injectable({ providedIn: 'root' })
export class TourService {
  private readonly _publishedTours = signal<Tour[]>([]);
  readonly publishedTours = this._publishedTours.asReadonly();

  constructor(private readonly http: HttpClient) {}

  getPublishedTours(): Observable<Tour[]> {
    return this.http
      .get<Tour[]>(`${environment.apiUrl}/tour/published`)
      .pipe(tap((tours) => this._publishedTours.set(tours)));
  }

  getMyTours(guideId: number, sortAscending = true): Observable<Tour[]> {
    const params = new HttpParams()
      .set('guideId', guideId)
      .set('sortAscending', sortAscending);
    return this.http.get<Tour[]>(`${environment.apiUrl}/tour/my`, { params });
  }

  createTour(cmd: {
    name: string;
    description: string;
    difficulty: string;
    category: string;
    price: number;
    scheduledDate: string;
    guideId: number;
  }): Observable<Tour> {
    return this.http.post<Tour>(`${environment.apiUrl}/tour`, cmd);
  }

  addKeyPoint(
    tourId: number,
    cmd: {
      name: string;
      description: string;
      latitude: number;
      longitude: number;
      imageUrl: string;
      guideId: number;
    }
  ): Observable<KeyPoint> {
    return this.http.post<KeyPoint>(
      `${environment.apiUrl}/tour/${tourId}/keypoints`,
      cmd
    );
  }

  publishTour(tourId: number, guideId: number): Observable<Tour> {
    return this.http.put<Tour>(`${environment.apiUrl}/tour/${tourId}/publish`, {
      tourId,
      guideId,
    });
  }

  findById(id: number): Tour | undefined {
    return this._publishedTours().find((t) => t.id === id);
  }
}
