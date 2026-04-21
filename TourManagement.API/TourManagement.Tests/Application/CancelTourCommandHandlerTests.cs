using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class CancelTourCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CancelTourCommandHandler _handler;

    public CancelTourCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _purchaseRepoMock.Setup(r => r.GetByTourIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourPurchase>());
        _handler = new CancelTourCommandHandler(
            _tourRepoMock.Object, _purchaseRepoMock.Object,
            _userRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_CancelsDraftTour()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        await _handler.Handle(new CancelTourCommand(1, 1), CancellationToken.None);

        tour.Status.Should().Be(TourStatus.Archived);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CancelsPublishedTour()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        tour.AddKeyPoint(new KeyPoint("A", "d", new Coordinate(45, 19), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("B", "d", new Coordinate(45, 19), "img.jpg", 2));
        tour.Publish();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        await _handler.Handle(new CancelTourCommand(1, 1), CancellationToken.None);

        tour.Status.Should().Be(TourStatus.Archived);
    }

    [Fact]
    public async Task Handle_AwardsBonusPointsToPurchasers()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 1000,
            DateTime.UtcNow.AddDays(1), guideId: 1);
        tour.AddKeyPoint(new KeyPoint("A", "d", new Coordinate(45, 19), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("B", "d", new Coordinate(45, 19), "img.jpg", 2));
        tour.Publish();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var tourist = new User("turista", "Marko", "M", "m@test.com", "hash", UserRole.Tourist);
        var purchase = new TourPurchase(touristId: 5, tourId: 1, pricePaid: 1000);
        _purchaseRepoMock.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourPurchase> { purchase });
        _userRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(tourist);

        await _handler.Handle(new CancelTourCommand(1, 1), CancellationToken.None);

        tourist.BonusPoints.Should().Be(1000);
    }

    [Fact]
    public async Task Handle_WrongGuide_Throws()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var act = () => _handler.Handle(new CancelTourCommand(1, 999), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not authorized*");
    }

    [Fact]
    public async Task Handle_TourNotFound_Throws()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var act = () => _handler.Handle(new CancelTourCommand(99, 1), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
