export interface TourReview {
  id: number;
  tourId: number;
  touristId: number;
  rating: number;
  comment: string | null;
  createdAt: string;
}
