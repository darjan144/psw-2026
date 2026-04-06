using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Enums;

namespace TourManagement.Application.Commands;

public record RegisterUserCommand(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    List<Interest> Interests,
    bool RecommendationsEnabled
) : IRequest<RegisterResponse>;
