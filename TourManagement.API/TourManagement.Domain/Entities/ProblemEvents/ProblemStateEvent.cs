using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities.ProblemEvents;

public abstract class ProblemStateEvent : Entity
{
    public long ProblemId { get; internal set; }
    public int SequenceNumber { get; internal set; }
    public DateTime OccurredAt { get; protected set; }
    public long? CausedByUserId { get; protected set; }

    protected ProblemStateEvent(long? causedByUserId)
    {
        OccurredAt = DateTime.UtcNow;
        CausedByUserId = causedByUserId;
    }

    protected ProblemStateEvent() { } // EF Core

    public abstract ProblemStatus ApplyTo(ProblemStatus current);
}
