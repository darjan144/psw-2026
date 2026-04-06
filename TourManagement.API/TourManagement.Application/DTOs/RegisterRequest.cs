using TourManagement.Domain.Enums;

namespace TourManagement.Application.DTOs;

public record RegisterRequest(
    string Username,
    string FirstName,
    string LastName,
    string Email,
    string Password,
    List<Interest> Interests,
    bool RecommendationsEnabled
);
