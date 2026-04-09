using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetSeekingSubstituteToursQuery() : IRequest<List<TourDto>>;
