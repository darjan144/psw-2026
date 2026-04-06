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
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<KeyPoint> KeyPoints => Set<KeyPoint>();
    public DbSet<TourReview> TourReviews => Set<TourReview>();
    public DbSet<TourPurchase> TourPurchases => Set<TourPurchase>();
    public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Problem> Problems => Set<Problem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TourManagementDbContext).Assembly);
    }
}
