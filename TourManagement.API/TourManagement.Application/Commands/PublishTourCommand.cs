using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record PublishTourCommand(long TourId, long GuideId) : IRequest<TourDto>;
