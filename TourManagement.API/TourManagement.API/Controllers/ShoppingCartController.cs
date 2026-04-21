using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Commands;
using TourManagement.Application.DTOs;
using TourManagement.Application.Queries;

namespace TourManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Tourist")]
public class ShoppingCartController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShoppingCartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("purchases")]
    public async Task<ActionResult<List<TouristPurchaseDto>>> GetPurchases(
        [FromQuery] long touristId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTouristPurchasesQuery(touristId), cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(
        [FromQuery] long touristId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCartQuery(touristId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddToCart(
        [FromBody] AddToCartCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("items/{tourId}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(
        long tourId,
        [FromQuery] long touristId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RemoveFromCartCommand(tourId, touristId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("purchase")]
    public async Task<ActionResult<List<PurchaseDto>>> Purchase(
        [FromBody] PurchaseToursCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
