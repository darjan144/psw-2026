namespace TourManagement.Application.DTOs;

public record LoginResponse(long Id, string Username, string Role, string Token);
