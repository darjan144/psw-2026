namespace TourManagement.Application.DTOs;

public record PurchaseDto(
    long Id,
    long TouristId,
    long TourId,
    double PricePaid,
    DateTime PurchasedAt
);
