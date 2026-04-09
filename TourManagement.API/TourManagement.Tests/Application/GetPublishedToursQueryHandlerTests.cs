using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class GetPublishedToursQueryHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly GetPublishedToursQueryHandler _handler;

    public GetPublishedToursQueryHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _handler = new GetPublishedToursQueryHandler(_tourRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WithTours_ShouldReturnDtos()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        tour.Publish();
        _tourRepoMock.Setup(r => r.GetPublishedAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour> { tour });

        var result = await _handler.Handle(new GetPublishedToursQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be("Published");
    }

    [Fact]
    public async Task Handle_NoTours_ShouldReturnEmpty()
    {
        _tourRepoMock.Setup(r => r.GetPublishedAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

        var result = await _handler.Handle(new GetPublishedToursQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
