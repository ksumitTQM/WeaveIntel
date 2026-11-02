using Microsoft.EntityFrameworkCore;
using WeaveIntel.Data;
using WeaveIntel.Models;

namespace WeaveIntel.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly WeaveIntelDbContext _db;

    public AvailabilityService(WeaveIntelDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductAvailability>> GetForProductAsync(int productId)
    {
        return await _db.ProductAvailabilities
            .AsNoTracking()
            .Include(a => a.FlightRoute)
            .Where(a => a.ProductId == productId)
            .OrderBy(a => a.FlightRoute.Origin)
            .ThenBy(a => a.FlightRoute.Destination)
            .ThenBy(a => a.Channel)
            .ToListAsync();
    }

    public async Task<(bool Success, string? Error)> AddAsync(ProductAvailability availability)
    {
        var productExists = await _db.Products.AnyAsync(p => p.Id == availability.ProductId);
        if (!productExists)
        {
            return (false, "Product not found.");
        }

        var routeExists = await _db.FlightRoutes.AnyAsync(r => r.Id == availability.FlightRouteId);
        if (!routeExists)
        {
            return (false, "Route not found.");
        }

        var duplicate = await _db.ProductAvailabilities.AnyAsync(a =>
            a.ProductId == availability.ProductId &&
            a.FlightRouteId == availability.FlightRouteId &&
            a.Channel == availability.Channel);

        if (duplicate)
        {
            return (false, "This product is already configured for that route and channel.");
        }

        _db.ProductAvailabilities.Add(availability);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<bool> ToggleAsync(int id)
    {
        var item = await _db.ProductAvailabilities.FirstOrDefaultAsync(a => a.Id == id);
        if (item == null)
        {
            return false;
        }

        item.IsActive = !item.IsActive;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _db.ProductAvailabilities.FirstOrDefaultAsync(a => a.Id == id);
        if (item == null)
        {
            return false;
        }

        _db.ProductAvailabilities.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}
