using FluentAssertions;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class TourCancellationServiceTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public TourCancellationServiceTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private Tour CreateSeekingSubstituteTour(DateTime scheduledDate, long guideId = 1)
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, scheduledDate, guideId);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        tour.Publish();
        tour.MarkSeekingSubstitute();
        return tour;
    }

    [Fact]
    public async Task CancelOverdueTours_ShouldCancelTourWithin24h()
    {
        var tour = CreateSeekingSubstituteTour(DateTime.UtcNow.AddHours(12));
        _tourRepoMock.Setup(r => r.GetSeekingSubstituteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour> { tour });
        _purchaseRepoMock.Setup(r => r.GetByTourIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourPurchase>());

        var service = new TourCancellationProcessor(_tourRepoMock.Object, _purchaseRepoMock.Object, _userRepoMock.Object, _unitOfWorkMock.Object);
        await service.ProcessCancellationsAsync(CancellationToken.None);

        tour.Status.Should().Be(TourStatus.Archived);
        tour.SeekingSubstitute.Should().BeFalse();
    }

    [Fact]
    public async Task CancelOverdueTours_ShouldDistributeBonusPoints()
    {
        var tour = CreateSeekingSubstituteTour(DateTime.UtcNow.AddHours(12));
        var tourist = new User("turista", "Marko", "Markovic", "marko@test.com", "hash", UserRole.Tourist);
        var purchase = new TourPurchase(touristId: 1, tourId: 1, pricePaid: 1500);
        _tourRepoMock.Setup(r => r.GetSeekingSubstituteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour> { tour });
        _purchaseRepoMock.Setup(r => r.GetByTourIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<TourPurchase> { purchase });
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tourist);

        var service = new TourCancellationProcessor(_tourRepoMock.Object, _purchaseRepoMock.Object, _userRepoMock.Object, _unitOfWorkMock.Object);
        await service.ProcessCancellationsAsync(CancellationToken.None);

        tourist.BonusPoints.Should().Be(1500);
    }

    [Fact]
    public async Task CancelOverdueTours_ShouldSkipToursNotWithin24h()
    {
        var tour = CreateSeekingSubstituteTour(DateTime.UtcNow.AddDays(3));
        _tourRepoMock.Setup(r => r.GetSeekingSubstituteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Tour> { tour });

        var service = new TourCancellationProcessor(_tourRepoMock.Object, _purchaseRepoMock.Object, _userRepoMock.Object, _unitOfWorkMock.Object);
        await service.ProcessCancellationsAsync(CancellationToken.None);

        tour.Status.Should().Be(TourStatus.Published);
        tour.SeekingSubstitute.Should().BeTrue();
    }
}
