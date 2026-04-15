using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities.ProblemEvents;

public class ProblemCreatedStateEvent : ProblemStateEvent
{
    public ProblemCreatedStateEvent(long? causedByUserId) : base(causedByUserId) { }

    private ProblemCreatedStateEvent() : base() { } // EF Core

    public override ProblemStatus ApplyTo(ProblemStatus current) => ProblemStatus.Pending;
}
