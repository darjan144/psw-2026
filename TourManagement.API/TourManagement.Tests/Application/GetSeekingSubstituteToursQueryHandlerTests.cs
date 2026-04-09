using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class GetSeekingSubstituteToursQueryHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly GetSeekingSubstituteToursQueryHandler _handler;

    public GetSeekingSubstituteToursQueryHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _handler = new GetSeekingSubstituteToursQueryHandler(_tourRepoMock.Object);
    }

    [Fact]
    public async Task Handle_WithTours_ShouldReturnDtos()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        tour.Publish();
        tour.MarkSeekingSubstitute();
        _tourRepoMock.Setup(r => r.GetSeekingSubstituteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour> { tour });

        var result = await _handler.Handle(new GetSeekingSubstituteToursQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].SeekingSubstitute.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_NoTours_ShouldReturnEmpty()
    {
        _tourRepoMock.Setup(r => r.GetSeekingSubstituteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

        var result = await _handler.Handle(new GetSeekingSubstituteToursQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
