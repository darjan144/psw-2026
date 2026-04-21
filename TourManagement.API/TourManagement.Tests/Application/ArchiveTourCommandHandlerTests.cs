using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class ArchiveTourCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ArchiveTourCommandHandler _handler;

    public ArchiveTourCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new ArchiveTourCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ArchivesPublishedTour()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        tour.AddKeyPoint(new KeyPoint("A", "d", new Coordinate(45, 19), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("B", "d", new Coordinate(45, 19), "img.jpg", 2));
        tour.Publish();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var result = await _handler.Handle(new ArchiveTourCommand(1, 1), CancellationToken.None);

        result.Status.Should().Be("Archived");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DraftTour_Throws()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var act = () => _handler.Handle(new ArchiveTourCommand(1, 1), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Only published*");
    }

    [Fact]
    public async Task Handle_WrongGuide_Throws()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        tour.AddKeyPoint(new KeyPoint("A", "d", new Coordinate(45, 19), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("B", "d", new Coordinate(45, 19), "img.jpg", 2));
        tour.Publish();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var act = () => _handler.Handle(new ArchiveTourCommand(1, 999), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not authorized*");
    }
}
