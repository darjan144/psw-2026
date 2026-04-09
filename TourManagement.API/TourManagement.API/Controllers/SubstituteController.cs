using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Commands;
using TourManagement.Application.DTOs;
using TourManagement.Application.Queries;

namespace TourManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Guide")]
public class SubstituteController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubstituteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("{tourId}/seek")]
    public async Task<ActionResult<TourDto>> MarkSeeking(
        long tourId,
        [FromQuery] long guideId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MarkSeekingSubstituteCommand(tourId, guideId), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{tourId}/assign")]
    public async Task<ActionResult<TourDto>> Assign(
        long tourId,
        [FromQuery] long newGuideId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AssignSubstituteCommand(tourId, newGuideId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<TourDto>>> GetSeeking(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSeekingSubstituteToursQuery(), cancellationToken);
        return Ok(result);
    }
}
