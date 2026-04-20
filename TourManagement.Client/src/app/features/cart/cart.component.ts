import { Component, inject, OnInit, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';

import { CartService } from '../../core/services/cart.service';
import { AuthService } from '../../core/services/auth.service';
import { ToastService } from '../../core/services/toast.service';
import { Cart } from '../../core/models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [DecimalPipe],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss',
})
export class CartComponent implements OnInit {
  private readonly cartService = inject(CartService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(ToastService);

  readonly cart = signal<Cart | null>(null);
  readonly loading = signal(true);
  readonly purchasing = signal(false);
  readonly useBonusPoints = signal(false);

  private get touristId(): number {
    return this.auth.userId()!;
  }

  ngOnInit(): void {
    this.loadCart();
  }

  removeItem(tourId: number): void {
    this.cartService.removeItem(tourId, this.touristId).subscribe({
      next: (cart) => this.cart.set(cart),
    });
  }

  toggleBonusPoints(): void {
    this.useBonusPoints.update((v) => !v);
  }

  purchase(): void {
    this.purchasing.set(true);
    this.cartService.purchase(this.touristId, this.useBonusPoints()).subscribe({
      next: () => {
        this.toast.success('Purchase successful! Check your email for confirmation.');
        this.cart.set(null);
        this.purchasing.set(false);
      },
      error: () => this.purchasing.set(false),
    });
  }

  private loadCart(): void {
    this.cartService.getCart(this.touristId).subscribe({
      next: (cart) => {
        this.cart.set(cart);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
