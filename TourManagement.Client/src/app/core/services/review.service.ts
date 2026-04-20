import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { TourReview } from '../models/review.model';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  constructor(private readonly http: HttpClient) {}

  getReviewsForTour(tourId: number): Observable<TourReview[]> {
    return this.http.get<TourReview[]>(
      `${environment.apiUrl}/tourreview/${tourId}`
    );
  }

  createReview(cmd: {
    tourId: number;
    touristId: number;
    rating: number;
    comment: string | null;
  }): Observable<TourReview> {
    return this.http.post<TourReview>(`${environment.apiUrl}/tourreview`, cmd);
  }
}
