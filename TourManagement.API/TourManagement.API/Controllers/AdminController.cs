using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Application.Commands;
using TourManagement.Application.DTOs;
using TourManagement.Application.Queries;

namespace TourManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("blocked-users")]
    public async Task<ActionResult<List<BlockedUserDto>>> GetBlockedUsers(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBlockedUsersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPut("unblock/{userId}")]
    public async Task<ActionResult<BlockedUserDto>> UnblockUser(
        long userId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UnblockUserCommand(userId), cancellationToken);
        return Ok(result);
    }
}
