using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record AddToCartCommand(
    long TourId,
    long TouristId
) : IRequest<CartDto>;
