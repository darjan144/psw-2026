using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Enums;

namespace TourManagement.Application.Commands;

public record UpdateProfileCommand(long TouristId, List<Interest> Interests, bool RecommendationsEnabled) : IRequest<ProfileDto>;
