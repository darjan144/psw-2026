import { Interest } from './user.model';

export interface Tour {
  id: number;
  name: string;
  description: string;
  difficulty: TourDifficulty;
  category: Interest;
  price: number;
  status: TourStatus;
  scheduledDate: string;
  publishedDate: string | null;
  guideId: number;
  seekingSubstitute: boolean;
  keyPoints: KeyPoint[];
}

export interface KeyPoint {
  id: number;
  name: string;
  description: string;
  latitude: number;
  longitude: number;
  imageUrl: string;
  order: number;
}

export enum TourDifficulty {
  Easy = 'Easy',
  Medium = 'Medium',
  Hard = 'Hard',
}

export enum TourStatus {
  Draft = 'Draft',
  Published = 'Published',
  Archived = 'Archived',
}
