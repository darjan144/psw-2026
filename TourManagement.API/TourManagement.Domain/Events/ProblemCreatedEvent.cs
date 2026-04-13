using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Events;

public class ProblemCreatedEvent : DomainEvent
{
    public long ProblemId { get; }
    public long TourId { get; }
    public long TouristId { get; }
    public string ProblemTitle { get; }

    public ProblemCreatedEvent(long problemId, long tourId, long touristId, string problemTitle)
    {
        ProblemId = problemId;
        TourId = tourId;
        TouristId = touristId;
        ProblemTitle = problemTitle;
    }
}
