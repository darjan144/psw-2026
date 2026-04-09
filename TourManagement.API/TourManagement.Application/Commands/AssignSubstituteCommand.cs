using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record AssignSubstituteCommand(long TourId, long NewGuideId) : IRequest<TourDto>;
