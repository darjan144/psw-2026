using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities.ProblemEvents;

public class ProblemReturnedToGuideStateEvent : ProblemStateEvent
{
    public ProblemReturnedToGuideStateEvent(long? causedByUserId) : base(causedByUserId) { }

    private ProblemReturnedToGuideStateEvent() : base() { } // EF Core

    public override ProblemStatus ApplyTo(ProblemStatus current)
    {
        if (current != ProblemStatus.InReview)
            throw new InvalidOperationException("Only problems in review can be returned to guide.");
        return ProblemStatus.Pending;
    }
}
