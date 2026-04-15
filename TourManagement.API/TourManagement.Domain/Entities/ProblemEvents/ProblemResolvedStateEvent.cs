using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities.ProblemEvents;

public class ProblemResolvedStateEvent : ProblemStateEvent
{
    public ProblemResolvedStateEvent(long? causedByUserId) : base(causedByUserId) { }

    private ProblemResolvedStateEvent() : base() { } // EF Core

    public override ProblemStatus ApplyTo(ProblemStatus current)
    {
        if (current != ProblemStatus.Pending)
            throw new InvalidOperationException("Only pending problems can be resolved.");
        return ProblemStatus.Resolved;
    }
}
