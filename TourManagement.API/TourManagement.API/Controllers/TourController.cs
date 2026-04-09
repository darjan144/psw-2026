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
public class TourController : ControllerBase
{
    private readonly IMediator _mediator;

    public TourController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<TourDto>> Create(
        [FromBody] CreateTourCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMyTours), result);
    }

    [HttpPost("{tourId}/keypoints")]
    public async Task<ActionResult<KeyPointDto>> AddKeyPoint(
        long tourId,
        [FromBody] AddKeyPointCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { TourId = tourId }, cancellationToken);
        return Created("", result);
    }

    [HttpPut("{tourId}/publish")]
    public async Task<ActionResult<TourDto>> Publish(
        long tourId,
        [FromBody] PublishTourCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command with { TourId = tourId }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("published")]
    [AllowAnonymous]
    public async Task<ActionResult<List<TourDto>>> GetPublished(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPublishedToursQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<TourDto>>> GetMyTours(
        [FromQuery] long guideId,
        [FromQuery] bool sortAscending = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetGuideToursQuery(guideId, sortAscending), cancellationToken);
        return Ok(result);
    }
}
