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
  token: string;
}

export enum Interest {
  Nature = 'Nature',
  Art = 'Art',
  Sports = 'Sports',
  Shopping = 'Shopping',
  Food = 'Food',
}

export enum UserRole {
  Tourist = 'Tourist',
  Guide = 'Guide',
  Admin = 'Admin',
}
