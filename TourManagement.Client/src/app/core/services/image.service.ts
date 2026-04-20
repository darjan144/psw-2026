import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ImageService {
  constructor(private readonly http: HttpClient) {}

  upload(file: File): Observable<string> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http
      .post<{ url: string }>(`${environment.apiUrl}/image`, formData)
      .pipe(map((res) => res.url));
  }
}
