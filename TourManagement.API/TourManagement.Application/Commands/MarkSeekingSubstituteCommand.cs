using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record MarkSeekingSubstituteCommand(long TourId, long GuideId) : IRequest<TourDto>;
