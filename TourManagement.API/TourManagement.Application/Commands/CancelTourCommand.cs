using MediatR;

namespace TourManagement.Application.Commands;

public record CancelTourCommand(long TourId, long GuideId) : IRequest;
