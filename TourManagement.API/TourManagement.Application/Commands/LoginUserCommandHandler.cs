using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(command.Username, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("Invalid username or password.");

        if (user.IsBlocked)
            throw new InvalidOperationException("Account is blocked. Contact administrator.");

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            user.RegisterFailedLogin();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Invalid username or password.");
        }

        user.ResetFailedLoginAttempts();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);

        return new LoginResponse(user.Id, user.Username, user.Role.ToString(), token);
    }
}
