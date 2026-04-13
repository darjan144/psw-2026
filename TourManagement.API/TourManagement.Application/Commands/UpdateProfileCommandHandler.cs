using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ProfileDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfileDto> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.TouristId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        user.SetInterests(command.Interests);

        if (command.RecommendationsEnabled)
            user.EnableRecommendations();
        else
            user.DisableRecommendations();

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToProfileDto();
    }
}
