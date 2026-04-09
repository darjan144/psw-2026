using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetCartQuery(long TouristId) : IRequest<CartDto>;
