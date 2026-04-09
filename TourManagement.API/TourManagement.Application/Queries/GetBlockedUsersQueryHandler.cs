using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetBlockedUsersQueryHandler : IRequestHandler<GetBlockedUsersQuery, List<BlockedUserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetBlockedUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<BlockedUserDto>> Handle(GetBlockedUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetBlockedUsersAsync(cancellationToken);
        return users.Select(u => u.ToBlockedDto()).ToList();
    }
}
