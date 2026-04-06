using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record LoginUserCommand(string Username, string Password) : IRequest<LoginResponse>;
