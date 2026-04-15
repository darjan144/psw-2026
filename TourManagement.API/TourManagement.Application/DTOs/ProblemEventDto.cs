namespace TourManagement.Application.DTOs;

public record ProblemEventDto(
    long Id,
    int SequenceNumber,
    string EventType,
    DateTime OccurredAt,
    long? CausedByUserId
);
