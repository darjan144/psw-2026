using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record SendProblemToReviewCommand(long ProblemId, long GuideId) : IRequest<ProblemDto>;
