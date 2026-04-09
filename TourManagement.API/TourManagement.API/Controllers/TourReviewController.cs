using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Commands;
using TourManagement.Application.DTOs;
using TourManagement.Application.Queries;

namespace TourManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourReviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public TourReviewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = "Tourist")]
    public async Task<ActionResult<TourReviewDto>> Create(
        [FromBody] CreateTourReviewCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Created("", result);
    }

    [HttpGet("{tourId}")]
    [Authorize]
    public async Task<ActionResult<List<TourReviewDto>>> GetByTour(
        long tourId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTourReviewsQuery(tourId), cancellationToken);
        return Ok(result);
    }
}
