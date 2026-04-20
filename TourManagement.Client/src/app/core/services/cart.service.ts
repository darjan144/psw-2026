import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Cart } from '../models/cart.model';

export interface PurchaseResult {
  id: number;
  touristId: number;
  tourId: number;
  pricePaid: number;
  purchasedAt: string;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  constructor(private readonly http: HttpClient) {}

  getCart(touristId: number): Observable<Cart> {
    const params = new HttpParams().set('touristId', touristId);
    return this.http.get<Cart>(`${environment.apiUrl}/shoppingcart`, { params });
  }

  addItem(tourId: number, touristId: number): Observable<Cart> {
    return this.http.post<Cart>(`${environment.apiUrl}/shoppingcart/items`, {
      tourId,
      touristId,
    });
  }

  removeItem(tourId: number, touristId: number): Observable<Cart> {
    const params = new HttpParams().set('touristId', touristId);
    return this.http.delete<Cart>(
      `${environment.apiUrl}/shoppingcart/items/${tourId}`,
      { params }
    );
  }

  purchase(touristId: number, useBonusPoints: boolean): Observable<PurchaseResult[]> {
    return this.http.post<PurchaseResult[]>(
      `${environment.apiUrl}/shoppingcart/purchase`,
      { touristId, useBonusPoints }
    );
  }
}
