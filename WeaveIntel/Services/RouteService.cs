using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeaveIntel.Data;
using WeaveIntel.Models;

namespace WeaveIntel.Services;

public class RouteService : IRouteService
{
    private readonly WeaveIntelDbContext _db;

    public RouteService(WeaveIntelDbContext db)
    {
        _db = db;
    }

    public async Task<List<FlightRoute>> GetAllAsync()
    {
        return await _db.FlightRoutes
            .AsNoTracking()
            .Include(r => r.Availabilities)
            .OrderBy(r => r.Origin)
            .ThenBy(r => r.Destination)
            .ToListAsync();
    }

    public async Task<FlightRoute?> GetByIdAsync(int id)
    {
        return await _db.FlightRoutes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<FlightRoute> CreateAsync(FlightRoute route)
    {
        route.Origin = route.Origin.Trim().ToUpperInvariant();
        route.Destination = route.Destination.Trim().ToUpperInvariant();
        _db.FlightRoutes.Add(route);
        await _db.SaveChangesAsync();
        return route;
    }

    public async Task<bool> UpdateAsync(FlightRoute route)
    {
        var existing = await _db.FlightRoutes.FirstOrDefaultAsync(r => r.Id == route.Id);
        if (existing == null)
        {
            return false;
        }

        existing.Origin = route.Origin.Trim().ToUpperInvariant();
        existing.Destination = route.Destination.Trim().ToUpperInvariant();
        existing.Region = route.Region;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var route = await _db.FlightRoutes
            .Include(r => r.Availabilities)
            .Include(r => r.PerformanceRecords)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (route == null)
        {
            return false;
        }

        _db.ProductAvailabilities.RemoveRange(route.Availabilities);
        _db.PerformanceRecords.RemoveRange(route.PerformanceRecords);
        _db.FlightRoutes.Remove(route);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(string origin, string destination, int? excludeId = null)
    {
        var o = origin.Trim().ToUpperInvariant();
        var d = destination.Trim().ToUpperInvariant();
        return await _db.FlightRoutes.AnyAsync(r =>
            r.Origin == o && r.Destination == d && (excludeId == null || r.Id != excludeId));
    }

    public async Task<List<SelectListItem>> GetSelectItemsAsync()
    {
        var routes = await _db.FlightRoutes
            .AsNoTracking()
            .OrderBy(r => r.Origin)
            .ThenBy(r => r.Destination)
            .ToListAsync();

        return routes
            .Select(r => new SelectListItem($"{r.Code} ({r.Region.DisplayName()})", r.Id.ToString()))
            .ToList();
    }
}
