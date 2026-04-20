export interface Problem {
  id: number;
  title: string;
  description: string;
  status: string;
  tourId: number;
  touristId: number;
  createdAt: string;
}

export interface ProblemEvent {
  id: number;
  sequenceNumber: number;
  eventType: string;
  occurredAt: string;
  causedByUserId: number | null;
}
