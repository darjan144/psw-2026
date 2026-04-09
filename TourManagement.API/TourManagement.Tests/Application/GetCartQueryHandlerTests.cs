using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class GetCartQueryHandlerTests
{
    private readonly Mock<IShoppingCartRepository> _cartRepoMock;
    private readonly GetCartQueryHandler _handler;

    public GetCartQueryHandlerTests()
    {
        _cartRepoMock = new Mock<IShoppingCartRepository>();
        _handler = new GetCartQueryHandler(_cartRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingCart_ShouldReturnDto()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(1, "Tura", 1500);
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(cart);

        var result = await _handler.Handle(new GetCartQuery(1), CancellationToken.None);

        result.TouristId.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.TotalPrice.Should().Be(1500);
    }

    [Fact]
    public async Task Handle_NoCart_ShouldReturnEmpty()
    {
        _cartRepoMock.Setup(r => r.GetByTouristIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((ShoppingCart?)null);

        var result = await _handler.Handle(new GetCartQuery(1), CancellationToken.None);

        result.TouristId.Should().Be(1);
        result.Items.Should().BeEmpty();
        result.TotalPrice.Should().Be(0);
    }
}
