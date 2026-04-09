using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetBlockedUsersQuery() : IRequest<List<BlockedUserDto>>;
