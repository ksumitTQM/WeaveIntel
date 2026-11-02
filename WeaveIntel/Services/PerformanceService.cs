using Microsoft.EntityFrameworkCore;
using WeaveIntel.Data;
using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Services;

public class PerformanceService : IPerformanceService
{
    private readonly WeaveIntelDbContext _db;

    public PerformanceService(WeaveIntelDbContext db)
    {
        _db = db;
    }

    public async Task<ProductPerformanceDto?> GetProductPerformanceAsync(int productId)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            return null;
        }

        var records = await _db.PerformanceRecords
            .AsNoTracking()
            .Include(r => r.FlightRoute)
            .Where(r => r.ProductId == productId)
            .ToListAsync();

        var monthly = records
            .GroupBy(r => r.Month)
            .OrderBy(g => g.Key)
            .Select(g => new MonthlyPerformanceDto
            {
                Month = g.Key.ToString("yyyy-MM"),
                Units = g.Sum(r => r.UnitsSold),
                Revenue = g.Sum(r => r.Revenue),
                AttachRatePercent = Math.Round(g.Average(r => r.AttachRatePercent), 1)
            })
            .ToList();

        var byRoute = records
            .GroupBy(r => r.FlightRoute.Code)
            .Select(g => new RoutePerformanceDto
            {
                Route = g.Key,
                Units = g.Sum(r => r.UnitsSold),
                Revenue = g.Sum(r => r.Revenue)
            })
            .OrderByDescending(r => r.Revenue)
            .ToList();

        return new ProductPerformanceDto
        {
            ProductId = product.Id,
            ProductCode = product.Code,
            ProductName = product.Name,
            TotalUnits = records.Sum(r => r.UnitsSold),
            TotalRevenue = records.Sum(r => r.Revenue),
            AverageAttachRatePercent = records.Count == 0 ? 0 : Math.Round(records.Average(r => r.AttachRatePercent), 1),
            Monthly = monthly,
            ByRoute = byRoute
        };
    }

    public async Task<List<PerformanceRecord>> GetRecentRecordsAsync(int productId, int count)
    {
        return await _db.PerformanceRecords
            .AsNoTracking()
            .Include(r => r.FlightRoute)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.Month)
            .ThenBy(r => r.FlightRoute.Origin)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> AddAsync(PerformanceRecord record)
    {
        record.Month = new DateTime(record.Month.Year, record.Month.Month, 1);

        var duplicate = await _db.PerformanceRecords.AnyAsync(r =>
            r.ProductId == record.ProductId &&
            r.FlightRouteId == record.FlightRouteId &&
            r.Month == record.Month);

        if (duplicate)
        {
            return false;
        }

        _db.PerformanceRecords.Add(record);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var record = await _db.PerformanceRecords.FirstOrDefaultAsync(r => r.Id == id);
        if (record == null)
        {
            return false;
        }

        _db.PerformanceRecords.Remove(record);
        await _db.SaveChangesAsync();
        return true;
    }
}
