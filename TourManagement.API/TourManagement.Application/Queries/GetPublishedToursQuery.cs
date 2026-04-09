using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetPublishedToursQuery() : IRequest<List<TourDto>>;
