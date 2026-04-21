using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetProfileQuery(long TouristId) : IRequest<ProfileDto>;
