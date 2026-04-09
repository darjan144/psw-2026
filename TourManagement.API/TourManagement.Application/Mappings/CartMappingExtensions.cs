using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

public static class CartMappingExtensions
{
    public static CartDto ToDto(this ShoppingCart cart)
    {
        return new CartDto(
            cart.Id,
            cart.TouristId,
            cart.Items.Select(i => i.ToDto()).ToList(),
            cart.GetTotalPrice()
        );
    }

    public static CartItemDto ToDto(this CartItem item)
    {
        return new CartItemDto(
            item.Id,
            item.TourId,
            item.TourName,
            item.Price
        );
    }

    public static PurchaseDto ToDto(this TourPurchase purchase)
    {
        return new PurchaseDto(
            purchase.Id,
            purchase.TouristId,
            purchase.TourId,
            purchase.PricePaid,
            purchase.PurchasedAt
        );
    }
}
