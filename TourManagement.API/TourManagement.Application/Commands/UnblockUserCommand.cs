using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record UnblockUserCommand(long UserId) : IRequest<BlockedUserDto>;
