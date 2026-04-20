using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class GetProblemsQueryHandlerTests
{
    private readonly Mock<IProblemRepository> _problemRepoMock;

    public GetProblemsQueryHandlerTests()
    {
        _problemRepoMock = new Mock<IProblemRepository>();
    }

    [Fact]
    public async Task GetByTourist_WithProblems_ShouldReturnDtos()
    {
        var problems = new List<Problem>
        {
            new Problem("Problem 1", "Opis 1", tourId: 1, touristId: 1),
            new Problem("Problem 2", "Opis 2", tourId: 2, touristId: 1)
        };
        _problemRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problems);
        var handler = new GetProblemsByTouristQueryHandler(_problemRepoMock.Object);

        var result = await handler.Handle(new GetProblemsByTouristQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByTour_WithProblems_ShouldReturnDtos()
    {
        var problems = new List<Problem>
        {
            new Problem("Problem 1", "Opis 1", tourId: 1, touristId: 1)
        };
        _problemRepoMock.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(problems);
        var handler = new GetProblemsByTourQueryHandler(_problemRepoMock.Object);

        var result = await handler.Handle(new GetProblemsByTourQuery(1), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Problem 1");
    }

    [Fact]
    public async Task GetByTourist_NoProblems_ShouldReturnEmpty()
    {
        _problemRepoMock.Setup(r => r.GetByTouristIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Problem>());
        var handler = new GetProblemsByTouristQueryHandler(_problemRepoMock.Object);

        var result = await handler.Handle(new GetProblemsByTouristQuery(99), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInReview_WithProblems_ShouldReturnOnlyInReview()
    {
        var problem = new Problem("Review me", "Opis", tourId: 1, touristId: 1);
        problem.SendToReview(2);
        _problemRepoMock.Setup(r => r.GetByStatusAsync(ProblemStatus.InReview, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Problem> { problem });
        var handler = new GetProblemsInReviewQueryHandler(_problemRepoMock.Object);

        var result = await handler.Handle(new GetProblemsInReviewQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be("InReview");
    }

    [Fact]
    public async Task GetInReview_NoProblems_ShouldReturnEmpty()
    {
        _problemRepoMock.Setup(r => r.GetByStatusAsync(ProblemStatus.InReview, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Problem>());
        var handler = new GetProblemsInReviewQueryHandler(_problemRepoMock.Object);

        var result = await handler.Handle(new GetProblemsInReviewQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
