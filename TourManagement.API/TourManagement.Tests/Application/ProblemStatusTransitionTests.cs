using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class ProblemStatusTransitionTests
{
    private readonly Mock<IProblemRepository> _problemRepoMock;
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ProblemStatusTransitionTests()
    {
        _problemRepoMock = new Mock<IProblemRepository>();
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private Problem CreatePendingProblem()
    {
        return new Problem("Problem", "Opis", tourId: 1, touristId: 1);
    }

    private Problem CreateInReviewProblem()
    {
        var problem = CreatePendingProblem();
        problem.SendToReview();
        return problem;
    }

    // --- ResolveProblemCommandHandler ---

    [Fact]
    public async Task Resolve_PendingProblem_ShouldSetResolved()
    {
        var problem = CreatePendingProblem();
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 5);
        _problemRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problem);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var handler = new ResolveProblemCommandHandler(_problemRepoMock.Object, _tourRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new ResolveProblemCommand(1, GuideId: 5), CancellationToken.None);

        result.Status.Should().Be("Resolved");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Resolve_WrongGuide_ShouldThrow()
    {
        var problem = CreatePendingProblem();
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 5);
        _problemRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problem);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var handler = new ResolveProblemCommandHandler(_problemRepoMock.Object, _tourRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new ResolveProblemCommand(1, GuideId: 99), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*authorize*");
    }

    [Fact]
    public async Task Resolve_ProblemNotFound_ShouldThrow()
    {
        _problemRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Problem?)null);
        var handler = new ResolveProblemCommandHandler(_problemRepoMock.Object, _tourRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new ResolveProblemCommand(99, GuideId: 1), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    // --- SendProblemToReviewCommandHandler ---

    [Fact]
    public async Task SendToReview_PendingProblem_ShouldSetInReview()
    {
        var problem = CreatePendingProblem();
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 5);
        _problemRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problem);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var handler = new SendProblemToReviewCommandHandler(_problemRepoMock.Object, _tourRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new SendProblemToReviewCommand(1, GuideId: 5), CancellationToken.None);

        result.Status.Should().Be("InReview");
    }

    [Fact]
    public async Task SendToReview_WrongGuide_ShouldThrow()
    {
        var problem = CreatePendingProblem();
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 5);
        _problemRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problem);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var handler = new SendProblemToReviewCommandHandler(_problemRepoMock.Object, _tourRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new SendProblemToReviewCommand(1, GuideId: 99), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*authorize*");
    }

    // --- ReturnProblemToGuideCommandHandler ---

    [Fact]
    public async Task ReturnToGuide_InReviewProblem_ShouldSetPending()
    {
        var problem = CreateInReviewProblem();
        _problemRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problem);
        var handler = new ReturnProblemToGuideCommandHandler(_problemRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new ReturnProblemToGuideCommand(1), CancellationToken.None);

        result.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task ReturnToGuide_ProblemNotFound_ShouldThrow()
    {
        _problemRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Problem?)null);
        var handler = new ReturnProblemToGuideCommandHandler(_problemRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new ReturnProblemToGuideCommand(99), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    // --- RejectProblemCommandHandler ---

    [Fact]
    public async Task Reject_InReviewProblem_ShouldSetRejected()
    {
        var problem = CreateInReviewProblem();
        _problemRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problem);
        var handler = new RejectProblemCommandHandler(_problemRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new RejectProblemCommand(1), CancellationToken.None);

        result.Status.Should().Be("Rejected");
    }

    [Fact]
    public async Task Reject_ProblemNotFound_ShouldThrow()
    {
        _problemRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Problem?)null);
        var handler = new RejectProblemCommandHandler(_problemRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new RejectProblemCommand(99), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
