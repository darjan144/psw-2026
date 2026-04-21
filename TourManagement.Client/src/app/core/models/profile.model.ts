import { Interest } from './user.model';

export interface Profile {
  id: number;
  username: string;
  email: string;
  interests: Interest[];
  recommendationsEnabled: boolean;
  bonusPoints: number;
}
