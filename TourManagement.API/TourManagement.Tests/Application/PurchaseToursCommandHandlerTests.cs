using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class PurchaseToursCommandHandlerTests
{
    private readonly Mock<IShoppingCartRepository> _cartRepoMock;
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly PurchaseToursCommandHandler _handler;

    public PurchaseToursCommandHandlerTests()
    {
        _cartRepoMock = new Mock<IShoppingCartRepository>();
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new PurchaseToursCommandHandler(
            _cartRepoMock.Object,
            _purchaseRepoMock.Object,
            _userRepoMock.Object,
            _tourRepoMock.Object,
            _unitOfWorkMock.Object,
            _emailServiceMock.Object);
    }

    private User CreateTourist()
    {
        return new User("turista", "Marko", "Markovic", "marko@test.com", "hash", UserRole.Tourist);
    }

    [Fact]
    public async Task Handle_ValidCart_ShouldCreatePurchases()
    {
        var user = CreateTourist();
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura 1", 1500);
        cart.AddItem(2, "Tura 2", 2000);
        var tour1 = new Tour("Tura 1", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        var tour2 = new Tour("Tura 2", "Opis", TourDifficulty.Medium, Interest.Art, 2000, DateTime.UtcNow.AddDays(14), 1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(tour2);
        var command = new PurchaseToursCommand(TouristId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().HaveCount(2);
        _purchaseRepoMock.Verify(r => r.AddAsync(It.IsAny<TourPurchase>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Handle_ValidCart_ShouldClearCart()
    {
        var user = CreateTourist();
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PurchaseToursCommand(TouristId: 1);

        await _handler.Handle(command, CancellationToken.None);

        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ValidCart_ShouldSendEmail()
    {
        var user = CreateTourist();
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PurchaseToursCommand(TouristId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendPurchaseConfirmationAsync(
            "marko@test.com", "Marko",
            It.IsAny<List<PurchasedTourInfo>>(),
            It.IsAny<double>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyCart_ShouldThrow()
    {
        var user = CreateTourist();
        var cart = new ShoppingCart(touristId: 1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        var command = new PurchaseToursCommand(TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*empty*");
    }

    [Fact]
    public async Task Handle_CartNotFound_ShouldThrow()
    {
        var user = CreateTourist();
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((ShoppingCart?)null);
        var command = new PurchaseToursCommand(TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrow()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var command = new PurchaseToursCommand(TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WithBonusPoints_ShouldApplyDiscount()
    {
        var user = CreateTourist();
        user.AddBonusPoints(500);
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PurchaseToursCommand(TouristId: 1, UseBonusPoints: true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result[0].PricePaid.Should().Be(1000);
        user.BonusPoints.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldSave()
    {
        var user = CreateTourist();
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new PurchaseToursCommand(TouristId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AlreadyPurchasedTour_ShouldThrow()
    {
        var user = CreateTourist();
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura 1", 1500);
        cart.AddItem(2, "Tura 2", 2000);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 2, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var command = new PurchaseToursCommand(TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*already purchased*");
        _purchaseRepoMock.Verify(r => r.AddAsync(It.IsAny<TourPurchase>(), It.IsAny<CancellationToken>()), Times.Never);
        _emailServiceMock.Verify(e => e.SendPurchaseConfirmationAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PurchasedTourInfo>>(),
            It.IsAny<double>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_AlreadyPurchasedTourWithBonusPoints_ShouldNotConsumePoints()
    {
        var user = CreateTourist();
        user.AddBonusPoints(500);
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura 1", 1500);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var command = new PurchaseToursCommand(TouristId: 1, UseBonusPoints: true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        user.BonusPoints.Should().Be(500);
    }
}
