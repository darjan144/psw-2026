using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class AddToCartCommandHandlerTests
{
    private readonly Mock<IShoppingCartRepository> _cartRepoMock;
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddToCartCommandHandler _handler;

    public AddToCartCommandHandlerTests()
    {
        _cartRepoMock = new Mock<IShoppingCartRepository>();
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AddToCartCommandHandler(_cartRepoMock.Object, _tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    private Tour CreatePublishedTour(long id = 1)
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 1);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        tour.Publish();
        return tour;
    }

    [Fact]
    public async Task Handle_NewCart_ShouldCreateCartAndAddItem()
    {
        var tour = CreatePublishedTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((ShoppingCart?)null);
        var command = new AddToCartCommand(TourId: 1, TouristId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].TourName.Should().Be("Tura");
        _cartRepoMock.Verify(r => r.AddAsync(It.IsAny<ShoppingCart>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingCart_ShouldAddItem()
    {
        var tour = CreatePublishedTour();
        var cart = new ShoppingCart(touristId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        var command = new AddToCartCommand(TourId: 1, TouristId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        _cartRepoMock.Verify(r => r.AddAsync(It.IsAny<ShoppingCart>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new AddToCartCommand(TourId: 99, TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_TourNotPublished_ShouldThrow()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new AddToCartCommand(TourId: 1, TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*published*");
    }

    [Fact]
    public async Task Handle_ShouldSave()
    {
        var tour = CreatePublishedTour();
        var cart = new ShoppingCart(touristId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        var command = new AddToCartCommand(TourId: 1, TouristId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
