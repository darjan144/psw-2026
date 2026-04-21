using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record ArchiveTourCommand(long TourId, long GuideId) : IRequest<TourDto>;
