namespace TourManagement.Application.DTOs;

public record ProfileDto(long Id, string Username, string Email, List<string> Interests, bool RecommendationsEnabled);
