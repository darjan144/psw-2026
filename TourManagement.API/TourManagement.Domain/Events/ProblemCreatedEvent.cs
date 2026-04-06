using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Events;

public class ProblemCreatedEvent : DomainEvent
{
    public long ProblemId { get; }
    public long TourId { get; }
    public long TouristId { get; }

    public ProblemCreatedEvent(long problemId, long tourId, long touristId)
    {
        ProblemId = problemId;
        TourId = tourId;
        TouristId = touristId;
    }
}
