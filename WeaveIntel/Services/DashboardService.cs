using Microsoft.EntityFrameworkCore;
using WeaveIntel.Data;
using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Services;

public class DashboardService : IDashboardService
{
    private readonly WeaveIntelDbContext _db;

    public DashboardService(WeaveIntelDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var products = await _db.Products.AsNoTracking().ToListAsync();
        var records = await _db.PerformanceRecords.AsNoTracking().ToListAsync();
        var productById = products.ToDictionary(p => p.Id);

        var byStatus = Enum.GetValues<LifecycleStatus>()
            .Select(s => new ChartPoint
            {
                Label = s.DisplayName(),
                Value = products.Count(p => p.Status == s)
            })
            .ToList();

        var topProducts = records
            .GroupBy(r => r.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Revenue = g.Sum(r => r.Revenue),
                Units = g.Sum(r => r.UnitsSold)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(5)
            .Where(x => productById.ContainsKey(x.ProductId))
            .Select(x => new TopProductDto
            {
                Id = x.ProductId,
                Code = productById[x.ProductId].Code,
                Name = productById[x.ProductId].Name,
                Revenue = x.Revenue,
                Units = x.Units
            })
            .ToList();

        var revenueByCategory = Enum.GetValues<ProductCategory>()
            .Select(c => new ChartPoint
            {
                Label = c.DisplayName(),
                Value = records
                    .Where(r => productById.TryGetValue(r.ProductId, out var p) && p.Category == c)
                    .Sum(r => r.Revenue)
            })
            .ToList();

        var revenueByMonth = records
            .GroupBy(r => r.Month)
            .OrderBy(g => g.Key)
            .Select(g => new ChartPoint
            {
                Label = g.Key.ToString("yyyy-MM"),
                Value = g.Sum(r => r.Revenue)
            })
            .ToList();

        var recentChanges = await _db.StatusHistory
            .AsNoTracking()
            .OrderByDescending(h => h.ChangedAt)
            .Take(8)
            .Select(h => new StatusChangeDto
            {
                Id = h.Id,
                ProductId = h.ProductId,
                ProductCode = h.Product.Code,
                ProductName = h.Product.Name,
                FromStatus = h.FromStatus,
                ToStatus = h.ToStatus,
                ChangedBy = h.ChangedBy,
                ChangedAt = h.ChangedAt,
                Comment = h.Comment
            })
            .ToListAsync();

        return new DashboardDto
        {
            TotalProducts = products.Count,
            LiveProducts = products.Count(p => p.Status == LifecycleStatus.Live),
            TotalRevenue = records.Sum(r => r.Revenue),
            TotalUnits = records.Sum(r => r.UnitsSold),
            AverageAttachRatePercent = records.Count == 0 ? 0 : Math.Round(records.Average(r => r.AttachRatePercent), 1),
            ProductsByStatus = byStatus,
            TopProducts = topProducts,
            RevenueByCategory = revenueByCategory,
            RevenueByMonth = revenueByMonth,
            RecentChanges = recentChanges
        };
    }
}
