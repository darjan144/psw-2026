using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities.ProblemEvents;

public class ProblemRejectedStateEvent : ProblemStateEvent
{
    public ProblemRejectedStateEvent(long? causedByUserId) : base(causedByUserId) { }

    private ProblemRejectedStateEvent() : base() { } // EF Core

    public override ProblemStatus ApplyTo(ProblemStatus current)
    {
        if (current != ProblemStatus.InReview)
            throw new InvalidOperationException("Only problems in review can be rejected.");
        return ProblemStatus.Rejected;
    }
}
