using FluentAssertions;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Domain;

public class ShoppingCartTests
{
    [Fact]
    public void Create_ShouldCreateEmptyCart()
    {
        var cart = new ShoppingCart(touristId: 1);

        cart.TouristId.Should().Be(1);
        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddTourToCart()
    {
        var cart = new ShoppingCart(touristId: 1);

        cart.AddItem(tourId: 10, "Tura po Novom Sadu", 1500);

        cart.Items.Should().HaveCount(1);
        cart.Items.First().TourId.Should().Be(10);
        cart.Items.First().TourName.Should().Be("Tura po Novom Sadu");
        cart.Items.First().Price.Should().Be(1500);
    }

    [Fact]
    public void AddItem_DuplicateTour_ShouldThrow()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 10, "Tura", 1500);

        var act = () => cart.AddItem(tourId: 10, "Tura", 1500);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already in the cart*");
    }

    [Fact]
    public void RemoveItem_ShouldRemoveTourFromCart()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 10, "Tura", 1500);

        cart.RemoveItem(tourId: 10);

        cart.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_WhenTourNotInCart_ShouldThrow()
    {
        var cart = new ShoppingCart(touristId: 1);

        var act = () => cart.RemoveItem(tourId: 99);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*not in the cart*");
    }

    [Fact]
    public void GetTotalPrice_ShouldSumAllItems()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 1, "Tura 1", 1000);
        cart.AddItem(tourId: 2, "Tura 2", 2000);
        cart.AddItem(tourId: 3, "Tura 3", 500);

        cart.GetTotalPrice().Should().Be(3500);
    }

    [Fact]
    public void ApplyBonusPoints_ShouldReducePrice()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 1, "Tura", 1500);

        var finalPrice = cart.ApplyBonusPoints(500);

        finalPrice.Should().Be(1000);
    }

    [Fact]
    public void ApplyBonusPoints_WhenMoreThanTotal_ShouldReturnZero()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 1, "Tura", 1500);

        var finalPrice = cart.ApplyBonusPoints(2000);

        finalPrice.Should().Be(0);
    }

    [Fact]
    public void ApplyBonusPoints_WithZeroPoints_ShouldReturnFullPrice()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 1, "Tura", 1500);

        var finalPrice = cart.ApplyBonusPoints(0);

        finalPrice.Should().Be(1500);
    }

    [Fact]
    public void Clear_ShouldRemoveAllItems()
    {
        var cart = new ShoppingCart(touristId: 1);
        cart.AddItem(tourId: 1, "Tura 1", 1000);
        cart.AddItem(tourId: 2, "Tura 2", 2000);

        cart.Clear();

        cart.Items.Should().BeEmpty();
    }
}
