using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class RemoveFromCartCommandHandlerTests
{
    private readonly Mock<IShoppingCartRepository> _cartRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoveFromCartCommandHandler _handler;

    public RemoveFromCartCommandHandlerTests()
    {
        _cartRepoMock = new Mock<IShoppingCartRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoveFromCartCommandHandler(_cartRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidItem_ShouldRemoveAndReturnCart()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        cart.AddItem(2, "Tura 2", 2000);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        var command = new RemoveFromCartCommand(TourId: 1, TouristId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items[0].TourId.Should().Be(2);
    }

    [Fact]
    public async Task Handle_CartNotFound_ShouldThrow()
    {
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((ShoppingCart?)null);
        var command = new RemoveFromCartCommand(TourId: 1, TouristId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_ShouldSave()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);
        var command = new RemoveFromCartCommand(TourId: 1, TouristId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
