using TourManagement.Domain.Entities.ProblemEvents;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;

namespace TourManagement.Domain.Entities;

public class Problem : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<ProblemStateEvent> _events = new();
    public IReadOnlyCollection<ProblemStateEvent> Events => _events.AsReadOnly();

    public ProblemStatus Status => ReplayStatus();

    private Problem() { } // EF Core

    public Problem(string title, string description, long tourId, long touristId, long? causedByUserId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Problem title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Problem description is required.", nameof(description));

        Title = title;
        Description = description;
        TourId = tourId;
        TouristId = touristId;
        CreatedAt = DateTime.UtcNow;

        var createdEvent = new ProblemCreatedStateEvent(causedByUserId ?? touristId);
        createdEvent.SequenceNumber = 1;
        _events.Add(createdEvent);

        AddDomainEvent(new ProblemCreatedEvent(Id, tourId, touristId, title));
    }

    public static Problem Reconstitute(
        long id,
        string title,
        string description,
        long tourId,
        long touristId,
        DateTime createdAt,
        IEnumerable<ProblemStateEvent> events)
    {
        var problem = new Problem
        {
            Id = id,
            Title = title,
            Description = description,
            TourId = tourId,
            TouristId = touristId,
            CreatedAt = createdAt
        };
        var seq = 1;
        foreach (var e in events)
        {
            if (e.SequenceNumber == 0) e.SequenceNumber = seq;
            seq++;
            problem._events.Add(e);
        }
        return problem;
    }

    public void Resolve(long? causedByUserId = null)
    {
        AppendEvent(new ProblemResolvedStateEvent(causedByUserId));
    }

    public void SendToReview(long? causedByUserId = null)
    {
        AppendEvent(new ProblemSentToReviewStateEvent(causedByUserId));
    }

    public void ReturnToGuide(long? causedByUserId = null)
    {
        AppendEvent(new ProblemReturnedToGuideStateEvent(causedByUserId));
    }

    public void Reject(long? causedByUserId = null)
    {
        AppendEvent(new ProblemRejectedStateEvent(causedByUserId));
    }

    private void AppendEvent(ProblemStateEvent stateEvent)
    {
        // Validate transition against replayed state before appending.
        _ = stateEvent.ApplyTo(Status);
        stateEvent.SequenceNumber = _events.Count + 1;
        _events.Add(stateEvent);
    }

    private ProblemStatus ReplayStatus()
    {
        var status = ProblemStatus.Pending;
        foreach (var e in _events.OrderBy(x => x.SequenceNumber))
        {
            status = e.ApplyTo(status);
        }
        return status;
    }
}
