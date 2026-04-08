using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record AddKeyPointCommand(
    long TourId,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string ImageUrl,
    long GuideId
) : IRequest<KeyPointDto>;
