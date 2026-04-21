namespace TourManagement.Application.DTOs;

public record TouristPurchaseDto(
    long PurchaseId,
    long TourId,
    string TourName,
    string Category,
    DateTime ScheduledDate,
    double PricePaid,
    DateTime PurchasedAt
);
