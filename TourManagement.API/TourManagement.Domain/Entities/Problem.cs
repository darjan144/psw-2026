using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;

namespace TourManagement.Domain.Entities;

public class Problem : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public ProblemStatus Status { get; private set; }
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Problem() { } // EF Core

    public Problem(string title, string description, long tourId, long touristId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Problem title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Problem description is required.", nameof(description));

        Title = title;
        Description = description;
        TourId = tourId;
        TouristId = touristId;
        Status = ProblemStatus.Pending;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ProblemCreatedEvent(Id, tourId, touristId));
    }

    public void Resolve()
    {
        if (Status != ProblemStatus.Pending)
            throw new InvalidOperationException("Only pending problems can be resolved.");

        var oldStatus = Status;
        Status = ProblemStatus.Resolved;
        AddDomainEvent(new ProblemStatusChangedEvent(Id, oldStatus, Status));
    }

    public void SendToReview()
    {
        if (Status != ProblemStatus.Pending)
            throw new InvalidOperationException("Only pending problems can be sent to review.");

        var oldStatus = Status;
        Status = ProblemStatus.InReview;
        AddDomainEvent(new ProblemStatusChangedEvent(Id, oldStatus, Status));
    }

    public void ReturnToGuide()
    {
        if (Status != ProblemStatus.InReview)
            throw new InvalidOperationException("Only problems in review can be returned to guide.");

        var oldStatus = Status;
        Status = ProblemStatus.Pending;
        AddDomainEvent(new ProblemStatusChangedEvent(Id, oldStatus, Status));
    }

    public void Reject()
    {
        if (Status != ProblemStatus.InReview)
            throw new InvalidOperationException("Only problems in review can be rejected.");

        var oldStatus = Status;
        Status = ProblemStatus.Rejected;
        AddDomainEvent(new ProblemStatusChangedEvent(Id, oldStatus, Status));
    }
}
