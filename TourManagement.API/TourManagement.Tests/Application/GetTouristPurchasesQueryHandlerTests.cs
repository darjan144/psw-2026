using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class GetTouristPurchasesQueryHandlerTests
{
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly GetTouristPurchasesQueryHandler _handler;

    public GetTouristPurchasesQueryHandlerTests()
    {
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _tourRepoMock = new Mock<ITourRepository>();
        _handler = new GetTouristPurchasesQueryHandler(_purchaseRepoMock.Object, _tourRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPurchasesWithTourDetails()
    {
        var tour = new Tour("City Walk", "A nice walk", TourDifficulty.Easy, Interest.Art, 500, new DateTime(2026, 4, 21), 10);
        typeof(Entity).GetProperty("Id")!.SetValue(tour, 1L);

        var purchase = new TourPurchase(5, 1, 450);
        typeof(Entity).GetProperty("Id")!.SetValue(purchase, 100L);

        _purchaseRepoMock.Setup(r => r.GetByTouristIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourPurchase> { purchase });
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        var result = await _handler.Handle(new GetTouristPurchasesQuery(5), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TourName.Should().Be("City Walk");
        result[0].Category.Should().Be("Art");
        result[0].PricePaid.Should().Be(450);
        result[0].TourStatus.Should().Be("Draft");
    }

    [Fact]
    public async Task Handle_CancelledTour_ReturnsArchivedStatus()
    {
        var tour = new Tour("Cancelled Tour", "Opis", TourDifficulty.Easy, Interest.Nature, 500, DateTime.UtcNow.AddDays(1), 10);
        typeof(Entity).GetProperty("Id")!.SetValue(tour, 1L);
        tour.Cancel();

        var purchase = new TourPurchase(5, 1, 500);

        _purchaseRepoMock.Setup(r => r.GetByTouristIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourPurchase> { purchase });
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        var result = await _handler.Handle(new GetTouristPurchasesQuery(5), CancellationToken.None);

        result[0].TourStatus.Should().Be("Archived");
    }

    [Fact]
    public async Task Handle_NoPurchases_ReturnsEmpty()
    {
        _purchaseRepoMock.Setup(r => r.GetByTouristIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourPurchase>());

        var result = await _handler.Handle(new GetTouristPurchasesQuery(99), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_SkipsPurchasesWhereTourDeleted()
    {
        var purchase = new TourPurchase(5, 999, 100);

        _purchaseRepoMock.Setup(r => r.GetByTouristIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourPurchase> { purchase });
        _tourRepoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        var result = await _handler.Handle(new GetTouristPurchasesQuery(5), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
