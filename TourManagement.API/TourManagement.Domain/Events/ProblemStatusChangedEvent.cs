using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Events;

public class ProblemStatusChangedEvent : DomainEvent
{
    public long ProblemId { get; }
    public ProblemStatus OldStatus { get; }
    public ProblemStatus NewStatus { get; }

    public ProblemStatusChangedEvent(long problemId, ProblemStatus oldStatus, ProblemStatus newStatus)
    {
        ProblemId = problemId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}
