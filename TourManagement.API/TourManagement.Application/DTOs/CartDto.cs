namespace TourManagement.Application.DTOs;

public record CartDto(
    long Id,
    long TouristId,
    List<CartItemDto> Items,
    double TotalPrice
);

public record CartItemDto(
    long Id,
    long TourId,
    string TourName,
    double Price
);
