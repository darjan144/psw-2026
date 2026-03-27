using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Persistence;

public class TourManagementDbContext : DbContext
{
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TourManagementDbContext).Assembly);
    }
}
