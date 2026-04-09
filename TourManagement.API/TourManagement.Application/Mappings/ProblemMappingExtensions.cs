using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

public static class ProblemMappingExtensions
{
    public static ProblemDto ToDto(this Problem problem)
    {
        return new ProblemDto(
            problem.Id,
            problem.Title,
            problem.Description,
            problem.Status.ToString(),
            problem.TourId,
            problem.TouristId,
            problem.CreatedAt
        );
    }
}
