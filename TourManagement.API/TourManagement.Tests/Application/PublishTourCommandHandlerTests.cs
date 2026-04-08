using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class PublishTourCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PublishTourCommandHandler _handler;

    public PublishTourCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new PublishTourCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    private Tour CreatePublishableTour(long guideId = 1)
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        return tour;
    }

    [Fact]
    public async Task Handle_ValidTour_ShouldPublish()
    {
        var tour = CreatePublishableTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PublishTourCommand(1, GuideId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be("Published");
    }

    [Fact]
    public async Task Handle_ValidTour_ShouldSave()
    {
        var tour = CreatePublishableTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PublishTourCommand(1, GuideId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new PublishTourCommand(99, GuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WrongGuide_ShouldThrow()
    {
        var tour = CreatePublishableTour(guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PublishTourCommand(1, GuideId: 999);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not authorized*");
    }

    [Fact]
    public async Task Handle_LessThanTwoKeyPoints_ShouldThrow()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 1);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PublishTourCommand(1, GuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*2 key points*");
    }
}
