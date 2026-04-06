using FluentAssertions;
using TourManagement.Domain.Entities;
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
    public void Create_ShouldRaiseProblemCreatedEvent()
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

    // --- Status transitions ---

    [Fact]
    public void Resolve_WhenPending_ShouldSetResolved()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.ClearDomainEvents();

        problem.Resolve();

        problem.Status.Should().Be(ProblemStatus.Resolved);
    }

    [Fact]
    public void Resolve_ShouldRaiseStatusChangedEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.ClearDomainEvents();

        problem.Resolve();

        problem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProblemStatusChangedEvent>()
            .Which.OldStatus.Should().Be(ProblemStatus.Pending);
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
        problem.ClearDomainEvents();

        problem.SendToReview();

        problem.Status.Should().Be(ProblemStatus.InReview);
    }

    [Fact]
    public void SendToReview_ShouldRaiseStatusChangedEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.ClearDomainEvents();

        problem.SendToReview();

        problem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProblemStatusChangedEvent>();
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
        problem.ClearDomainEvents();

        problem.ReturnToGuide();

        problem.Status.Should().Be(ProblemStatus.Pending);
    }

    [Fact]
    public void ReturnToGuide_ShouldRaiseStatusChangedEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview();
        problem.ClearDomainEvents();

        problem.ReturnToGuide();

        problem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProblemStatusChangedEvent>()
            .Which.NewStatus.Should().Be(ProblemStatus.Pending);
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
        problem.ClearDomainEvents();

        problem.Reject();

        problem.Status.Should().Be(ProblemStatus.Rejected);
    }

    [Fact]
    public void Reject_ShouldRaiseStatusChangedEvent()
    {
        var problem = new Problem("Problem", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview();
        problem.ClearDomainEvents();

        problem.Reject();

        problem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProblemStatusChangedEvent>()
            .Which.NewStatus.Should().Be(ProblemStatus.Rejected);
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
}
