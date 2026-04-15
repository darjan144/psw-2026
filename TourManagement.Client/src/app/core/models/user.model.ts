export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  firstName: string;
  lastName: string;
  email: string;
  interests: Interest[];
  recommendationsEnabled: boolean;
}

export interface AuthResponse {
  id: number;
  username: string;
  role: UserRole;
  token: string;
}

export enum Interest {
  Nature = 'Nature',
  Art = 'Art',
  Sport = 'Sport',
  Shopping = 'Shopping',
  Food = 'Food',
}

export enum UserRole {
  Tourist = 'Tourist',
  Guide = 'Guide',
  Administrator = 'Administrator',
}
