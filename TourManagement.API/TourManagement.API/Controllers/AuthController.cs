using MediatR;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Commands;
using TourManagement.Application.DTOs;

namespace TourManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            request.Username,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Interests,
            request.RecommendationsEnabled
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Register), new { id = result.Id }, result);
    }
}
