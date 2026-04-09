using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class UnblockUserCommandHandler : IRequestHandler<UnblockUserCommand, BlockedUserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnblockUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BlockedUserDto> Handle(UnblockUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        user.Unblock();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToBlockedDto();
    }
}
