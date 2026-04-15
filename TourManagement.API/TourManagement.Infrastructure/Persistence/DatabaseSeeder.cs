using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var db = sp.GetRequiredService<TourManagementDbContext>();
        var hasher = sp.GetRequiredService<IPasswordHasher>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        if (await db.Users.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Users already exist — skipping seed.");
            return;
        }

        logger.LogInformation("Seeding initial users (admin + 3 guides)...");

        var seeds = new (string Username, string First, string Last, string Email, string Password, UserRole Role)[]
        {
            ("admin",  "Site",   "Admin",   "admin@psw.test",  "Admin123!",  UserRole.Administrator),
            ("guide1", "Guide",  "One",     "guide1@psw.test", "Guide123!",  UserRole.Guide),
            ("guide2", "Guide",  "Two",     "guide2@psw.test", "Guide123!",  UserRole.Guide),
            ("guide3", "Guide",  "Three",   "guide3@psw.test", "Guide123!",  UserRole.Guide),
        };

        foreach (var s in seeds)
        {
            var user = new User(
                username: s.Username,
                firstName: s.First,
                lastName: s.Last,
                email: s.Email,
                passwordHash: hasher.Hash(s.Password),
                role: s.Role
            );
            await db.Users.AddAsync(user, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} users.", seeds.Length);
    }
}
