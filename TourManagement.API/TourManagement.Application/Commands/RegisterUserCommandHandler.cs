using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var existingByUsername = await _userRepository.GetByUsernameAsync(command.Username, cancellationToken);
        if (existingByUsername != null)
            throw new InvalidOperationException("Username is already taken.");

        var existingByEmail = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingByEmail != null)
            throw new InvalidOperationException("Email is already in use.");

        var passwordHash = _passwordHasher.Hash(command.Password);

        var user = new User(
            command.Username,
            command.FirstName,
            command.LastName,
            command.Email,
            passwordHash,
            UserRole.Tourist
        );

        if (command.Interests.Count > 0)
            user.SetInterests(command.Interests);

        if (command.RecommendationsEnabled)
            user.EnableRecommendations();

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResponse(user.Id, user.Username, user.Email);
    }
}
