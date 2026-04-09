namespace TourManagement.Application.DTOs;

public record BlockedUserDto(
    long Id,
    string Username,
    string Email,
    int BlockCount
);
