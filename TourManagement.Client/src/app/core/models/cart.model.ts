export interface CartItem {
  id: number;
  tourId: number;
  tourName: string;
  price: number;
}

export interface Cart {
  id: number;
  touristId: number;
  items: CartItem[];
  totalPrice: number;
}
