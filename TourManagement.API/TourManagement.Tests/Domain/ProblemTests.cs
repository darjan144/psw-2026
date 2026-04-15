using FluentAssertions;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Entities.ProblemEvents;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;

namespace TourManagement.Tests.Domain;

public class ProblemTests
{
    // --- Creation ---

    [Fact]
    public void Create_WithValidData_ShouldCreatePendingProblem()
    {
        var problem = new Problem("Loša organizacija", "Vodič je kasnio", tourId: 1, touristId: 1);

        problem.Title.Should().Be("Loša organizacija");
        problem.Description.Should().Be("Vodič je kasnio");
        problem.Status.Should().Be(ProblemStatus.Pending);
        problem.TourId.Should().Be(1);
        problem.TouristId.Should().Be(1);
    }

    [Fact]
    public void Create_ShouldAppendProblemCreatedStateEvent()
    {
        var problem = new Problem("Problem", "Opis problema", tourId: 1, touristId: 1);

        problem.Events.Should().ContainSingle()
            .Which.Should().BeOfType<ProblemCreatedStateEvent>();
    }

    [Fact]
    public void Create_ShouldRaiseProblemCreatedDomainEventForNotifications()
    {
        var problem = new Problem("Problem", "Opis problema", tourId: 1, touristId: 1);

        problem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProblemCreatedEvent>();
    }

    [Theory]
    [InlineData("", "Opis")]
    [InlineData(null, "Opis")]
    [InlineData("   ", "Opis")]
    public void Create_WithInvalidTitle_ShouldThrow(string? title, string description)
    {
        var act = () => new Problem(title!, description, tourId: 1, touristId: 1);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("Naslov", "")]
    [InlineData("Naslov", null)]
    [InlineData("Naslov", "   ")]
    public void Create_WithInvalidDescription_ShouldThrow(string title, string? description)
    {
        var act = () => new Problem(title, description!, tourId: 1, touristId: 1);

        act.Should().Throw<ArgumentException>();
    }

    // --- Status transitions (event sourced) ---

    [Fact]
    public void Resolve_WhenPending_ShouldSetResolved()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        problem.Resolve(causedByUserId: 10);

        problem.Status.Should().Be(ProblemStatus.Resolved);
    }

    [Fact]
    public void Resolve_ShouldAppendProblemResolvedStateEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        problem.Resolve(causedByUserId: 10);

        problem.Events.Should().HaveCount(2);
        problem.Events.Last().Should().BeOfType<ProblemResolvedStateEvent>();
        problem.Events.Last().CausedByUserId.Should().Be(10);
    }

    [Fact]
    public void Resolve_WhenNotPending_ShouldThrow()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.Resolve();

        var act = () => problem.Resolve();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void SendToReview_WhenPending_ShouldSetInReview()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        problem.SendToReview(causedByUserId: 10);

        problem.Status.Should().Be(ProblemStatus.InReview);
    }

    [Fact]
    public void SendToReview_ShouldAppendStateEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        problem.SendToReview(causedByUserId: 10);

        problem.Events.Last().Should().BeOfType<ProblemSentToReviewStateEvent>();
    }

    [Fact]
    public void SendToReview_WhenNotPending_ShouldThrow()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.Resolve();

        var act = () => problem.SendToReview();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ReturnToGuide_WhenInReview_ShouldSetPending()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview();

        problem.ReturnToGuide(causedByUserId: 99);

        problem.Status.Should().Be(ProblemStatus.Pending);
    }

    [Fact]
    public void ReturnToGuide_ShouldAppendStateEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview();

        problem.ReturnToGuide(causedByUserId: 99);

        problem.Events.Last().Should().BeOfType<ProblemReturnedToGuideStateEvent>();
    }

    [Fact]
    public void ReturnToGuide_WhenNotInReview_ShouldThrow()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        var act = () => problem.ReturnToGuide();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Reject_WhenInReview_ShouldSetRejected()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview();

        problem.Reject(causedByUserId: 99);

        problem.Status.Should().Be(ProblemStatus.Rejected);
    }

    [Fact]
    public void Reject_ShouldAppendStateEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview();

        problem.Reject(causedByUserId: 99);

        problem.Events.Last().Should().BeOfType<ProblemRejectedStateEvent>();
    }

    [Fact]
    public void Reject_WhenNotInReview_ShouldThrow()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        var act = () => problem.Reject();

        act.Should().Throw<InvalidOperationException>();
    }

    // --- Full flow: Pending → InReview → Pending → Resolved ---

    [Fact]
    public void FullFlow_PendingToInReviewToPendingToResolved_ShouldSucceed()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        problem.SendToReview();
        problem.Status.Should().Be(ProblemStatus.InReview);

        problem.ReturnToGuide();
        problem.Status.Should().Be(ProblemStatus.Pending);

        problem.Resolve();
        problem.Status.Should().Be(ProblemStatus.Resolved);
    }

    [Fact]
    public void FullFlow_ShouldHaveFourEventsInOrder()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);

        problem.SendToReview();
        problem.ReturnToGuide();
        problem.Resolve();

        problem.Events.Should().HaveCount(4);
        problem.Events.ElementAt(0).Should().BeOfType<ProblemCreatedStateEvent>();
        problem.Events.ElementAt(1).Should().BeOfType<ProblemSentToReviewStateEvent>();
        problem.Events.ElementAt(2).Should().BeOfType<ProblemReturnedToGuideStateEvent>();
        problem.Events.ElementAt(3).Should().BeOfType<ProblemResolvedStateEvent>();
    }

    // --- Reconstitution from event stream (simulating DB load) ---

    [Fact]
    public void Reconstitute_FromEventStream_ShouldReplayStatusCorrectly()
    {
        var events = new List<ProblemStateEvent>
        {
            new ProblemCreatedStateEvent(causedByUserId: 1),
            new ProblemSentToReviewStateEvent(causedByUserId: 1),
            new ProblemRejectedStateEvent(causedByUserId: 2)
        };

        var problem = Problem.Reconstitute(
            id: 42,
            title: "T",
            description: "D",
            tourId: 1,
            touristId: 1,
            createdAt: DateTime.UtcNow,
            events: events);

        problem.Status.Should().Be(ProblemStatus.Rejected);
        problem.Events.Should().HaveCount(3);
    }

    [Fact]
    public void Reconstitute_WithOnlyCreatedEvent_ShouldBePending()
    {
        var events = new List<ProblemStateEvent>
        {
            new ProblemCreatedStateEvent(causedByUserId: 1)
        };

        var problem = Problem.Reconstitute(
            id: 1, title: "T", description: "D", tourId: 1, touristId: 1,
            createdAt: DateTime.UtcNow, events: events);

        problem.Status.Should().Be(ProblemStatus.Pending);
    }
}
