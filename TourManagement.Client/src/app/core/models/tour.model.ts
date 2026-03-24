export interface Tour {
  id: number;
  name: string;
  description: string;
  difficulty: TourDifficulty;
  category: string;
  price: number;
  date: string;
  status: TourStatus;
  guideId: number;
  keyPoints: KeyPoint[];
}

export interface KeyPoint {
  id: number;
  name: string;
  description: string;
  latitude: number;
  longitude: number;
  imageUrl: string;
  tourId: number;
}

export enum TourDifficulty {
  Easy = 'Easy',
  Medium = 'Medium',
  Hard = 'Hard',
}

export enum TourStatus {
  Draft = 'Draft',
  Published = 'Published',
  Cancelled = 'Cancelled',
}
