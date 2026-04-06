using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Authentication;
using TourManagement.Infrastructure.Persistence;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TourManagementDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, TourManagementDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ITourReviewRepository, TourReviewRepository>();
        services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
        services.AddScoped<IProblemRepository, ProblemRepository>();
        services.AddScoped<ITourPurchaseRepository, TourPurchaseRepository>();

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
