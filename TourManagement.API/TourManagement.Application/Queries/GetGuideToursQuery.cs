using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetGuideToursQuery(long GuideId, bool SortAscending = true) : IRequest<List<TourDto>>;
