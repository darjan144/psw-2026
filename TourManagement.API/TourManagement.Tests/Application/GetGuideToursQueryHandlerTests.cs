using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class GetGuideToursQueryHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly GetGuideToursQueryHandler _handler;

    public GetGuideToursQueryHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _handler = new GetGuideToursQueryHandler(_tourRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnGuideTours()
    {
        var tours = new List<Tour>
        {
            new Tour("Tura 1", "Opis", TourDifficulty.Easy, Interest.Nature, 1000, DateTime.UtcNow.AddDays(3), 1),
            new Tour("Tura 2", "Opis", TourDifficulty.Hard, Interest.Art, 2000, DateTime.UtcNow.AddDays(7), 1)
        };
        _tourRepoMock.Setup(r => r.GetByGuideIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _handler.Handle(new GetGuideToursQuery(1), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_SortAscending_ShouldSortByDateAsc()
    {
        var tours = new List<Tour>
        {
            new Tour("Later", "Opis", TourDifficulty.Easy, Interest.Nature, 1000, DateTime.UtcNow.AddDays(7), 1),
            new Tour("Earlier", "Opis", TourDifficulty.Easy, Interest.Nature, 1000, DateTime.UtcNow.AddDays(1), 1)
        };
        _tourRepoMock.Setup(r => r.GetByGuideIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _handler.Handle(new GetGuideToursQuery(1, SortAscending: true), CancellationToken.None);

        result[0].Name.Should().Be("Earlier");
        result[1].Name.Should().Be("Later");
    }

    [Fact]
    public async Task Handle_SortDescending_ShouldSortByDateDesc()
    {
        var tours = new List<Tour>
        {
            new Tour("Earlier", "Opis", TourDifficulty.Easy, Interest.Nature, 1000, DateTime.UtcNow.AddDays(1), 1),
            new Tour("Later", "Opis", TourDifficulty.Easy, Interest.Nature, 1000, DateTime.UtcNow.AddDays(7), 1)
        };
        _tourRepoMock.Setup(r => r.GetByGuideIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var result = await _handler.Handle(new GetGuideToursQuery(1, SortAscending: false), CancellationToken.None);

        result[0].Name.Should().Be("Later");
        result[1].Name.Should().Be("Earlier");
    }

    [Fact]
    public async Task Handle_NoTours_ShouldReturnEmpty()
    {
        _tourRepoMock.Setup(r => r.GetByGuideIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour>());

        var result = await _handler.Handle(new GetGuideToursQuery(1), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
