import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Problem, ProblemEvent } from '../models/problem.model';

@Injectable({ providedIn: 'root' })
export class ProblemService {
  constructor(private readonly http: HttpClient) {}

  createProblem(cmd: {
    tourId: number;
    touristId: number;
    title: string;
    description: string;
  }): Observable<Problem> {
    return this.http.post<Problem>(`${environment.apiUrl}/problem`, cmd);
  }

  getInReview(): Observable<Problem[]> {
    return this.http.get<Problem[]>(
      `${environment.apiUrl}/problem/in-review`
    );
  }

  getByTourist(touristId: number): Observable<Problem[]> {
    return this.http.get<Problem[]>(
      `${environment.apiUrl}/problem/tourist/${touristId}`
    );
  }

  getByTour(tourId: number): Observable<Problem[]> {
    return this.http.get<Problem[]>(
      `${environment.apiUrl}/problem/tour/${tourId}`
    );
  }

  getHistory(problemId: number): Observable<ProblemEvent[]> {
    return this.http.get<ProblemEvent[]>(
      `${environment.apiUrl}/problem/${problemId}/history`
    );
  }

  resolve(problemId: number, guideId: number): Observable<Problem> {
    const params = new HttpParams().set('guideId', guideId);
    return this.http.put<Problem>(
      `${environment.apiUrl}/problem/${problemId}/resolve`,
      null,
      { params }
    );
  }

  sendToReview(problemId: number, guideId: number): Observable<Problem> {
    const params = new HttpParams().set('guideId', guideId);
    return this.http.put<Problem>(
      `${environment.apiUrl}/problem/${problemId}/send-to-review`,
      null,
      { params }
    );
  }

  returnToGuide(problemId: number): Observable<Problem> {
    return this.http.put<Problem>(
      `${environment.apiUrl}/problem/${problemId}/return-to-guide`,
      null
    );
  }

  reject(problemId: number): Observable<Problem> {
    return this.http.put<Problem>(
      `${environment.apiUrl}/problem/${problemId}/reject`,
      null
    );
  }
}
