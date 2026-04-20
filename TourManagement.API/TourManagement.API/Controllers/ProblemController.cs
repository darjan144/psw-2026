using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Commands;
using TourManagement.Application.DTOs;
using TourManagement.Application.Queries;

namespace TourManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Tourist")]
    public async Task<ActionResult<ProblemDto>> Create(
        [FromBody] CreateProblemCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Created("", result);
    }

    [HttpPut("{problemId}/resolve")]
    [Authorize(Roles = "Guide")]
    public async Task<ActionResult<ProblemDto>> Resolve(
        long problemId,
        [FromQuery] long guideId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ResolveProblemCommand(problemId, guideId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{problemId}/send-to-review")]
    [Authorize(Roles = "Guide")]
    public async Task<ActionResult<ProblemDto>> SendToReview(
        long problemId,
        [FromQuery] long guideId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SendProblemToReviewCommand(problemId, guideId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{problemId}/return-to-guide")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ProblemDto>> ReturnToGuide(
        long problemId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ReturnProblemToGuideCommand(problemId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{problemId}/reject")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ProblemDto>> Reject(
        long problemId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectProblemCommand(problemId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("in-review")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<List<ProblemDto>>> GetInReview(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProblemsInReviewQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("tourist/{touristId}")]
    [Authorize]
    public async Task<ActionResult<List<ProblemDto>>> GetByTourist(
        long touristId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProblemsByTouristQuery(touristId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("tour/{tourId}")]
    [Authorize]
    public async Task<ActionResult<List<ProblemDto>>> GetByTour(
        long tourId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProblemsByTourQuery(tourId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{problemId}/history")]
    [Authorize]
    public async Task<ActionResult<List<ProblemEventDto>>> GetHistory(
        long problemId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProblemHistoryQuery(problemId), cancellationToken);
        return Ok(result);
    }
}
