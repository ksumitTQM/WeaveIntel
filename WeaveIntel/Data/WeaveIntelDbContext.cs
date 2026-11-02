using Microsoft.EntityFrameworkCore;
using WeaveIntel.Models;

namespace WeaveIntel.Data;

public class WeaveIntelDbContext : DbContext
{
    public WeaveIntelDbContext(DbContextOptions<WeaveIntelDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<FlightRoute> FlightRoutes => Set<FlightRoute>();

    public DbSet<ProductAvailability> ProductAvailabilities => Set<ProductAvailability>();

    public DbSet<PerformanceRecord> PerformanceRecords => Set<PerformanceRecord>();

    public DbSet<ProductStatusHistory> StatusHistory => Set<ProductStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Code).IsUnique();
            entity.Property(p => p.BasePrice).HasPrecision(18, 2);

            entity.HasMany(p => p.Availabilities)
                .WithOne(a => a.Product)
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.PerformanceRecords)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.StatusHistory)
                .WithOne(h => h.Product)
                .HasForeignKey(h => h.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FlightRoute>(entity =>
        {
            entity.HasIndex(r => new { r.Origin, r.Destination }).IsUnique();

            entity.HasMany(r => r.Availabilities)
                .WithOne(a => a.FlightRoute)
                .HasForeignKey(a => a.FlightRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.PerformanceRecords)
                .WithOne(p => p.FlightRoute)
                .HasForeignKey(p => p.FlightRouteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductAvailability>(entity =>
        {
            entity.Property(a => a.RoutePrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<PerformanceRecord>(entity =>
        {
            entity.Property(p => p.Revenue).HasPrecision(18, 2);
            entity.Property(p => p.AttachRatePercent).HasPrecision(5, 2);
        });
    }
}
