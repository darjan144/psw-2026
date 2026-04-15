using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities.ProblemEvents;

public class ProblemSentToReviewStateEvent : ProblemStateEvent
{
    public ProblemSentToReviewStateEvent(long? causedByUserId) : base(causedByUserId) { }

    private ProblemSentToReviewStateEvent() : base() { } // EF Core

    public override ProblemStatus ApplyTo(ProblemStatus current)
    {
        if (current != ProblemStatus.Pending)
            throw new InvalidOperationException("Only pending problems can be sent to review.");
        return ProblemStatus.InReview;
    }
}
