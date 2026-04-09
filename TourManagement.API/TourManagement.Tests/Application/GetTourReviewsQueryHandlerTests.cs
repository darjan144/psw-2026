using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class GetTourReviewsQueryHandlerTests
{
    private readonly Mock<ITourReviewRepository> _reviewRepoMock;
    private readonly GetTourReviewsQueryHandler _handler;

    public GetTourReviewsQueryHandlerTests()
    {
        _reviewRepoMock = new Mock<ITourReviewRepository>();
        _handler = new GetTourReviewsQueryHandler(_reviewRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WithReviews_ShouldReturnDtos()
    {
        var reviews = new List<TourReview>
        {
            new TourReview(1, 1, 5, "Super", DateTime.UtcNow.AddDays(-3)),
            new TourReview(1, 2, 4, null, DateTime.UtcNow.AddDays(-3))
        };
        _reviewRepoMock.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(reviews);

        var result = await _handler.Handle(new GetTourReviewsQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].Rating.Should().Be(5);
        result[1].Rating.Should().Be(4);
    }

    [Fact]
    public async Task Handle_NoReviews_ShouldReturnEmpty()
    {
        _reviewRepoMock.Setup(r => r.GetByTourIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourReview>());

        var result = await _handler.Handle(new GetTourReviewsQuery(99), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
