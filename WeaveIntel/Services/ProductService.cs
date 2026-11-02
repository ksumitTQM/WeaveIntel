using Microsoft.EntityFrameworkCore;
using WeaveIntel.Data;
using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Services;

public class ProductService : IProductService
{
    private readonly WeaveIntelDbContext _db;

    public ProductService(WeaveIntelDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductSummaryDto>> GetSummariesAsync(ProductCategory? category, LifecycleStatus? status, string? search)
    {
        var query = _db.Products.AsNoTracking().AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(p =>
                p.Code.ToLower().Contains(term) ||
                p.Name.ToLower().Contains(term) ||
                p.Owner.ToLower().Contains(term));
        }

        return await query
            .OrderBy(p => p.Code)
            .Select(p => new ProductSummaryDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Category = p.Category,
                Status = p.Status,
                Owner = p.Owner,
                BasePrice = p.BasePrice,
                Currency = p.Currency,
                LaunchDate = p.LaunchDate,
                TotalRevenue = p.PerformanceRecords.Sum(r => r.Revenue),
                TotalUnits = p.PerformanceRecords.Sum(r => r.UnitsSold),
                ActiveAvailabilities = p.Availabilities.Count(a => a.IsActive)
            })
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products
            .AsNoTracking()
            .Include(p => p.Availabilities).ThenInclude(a => a.FlightRoute)
            .Include(p => p.PerformanceRecords)
            .Include(p => p.StatusHistory)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProductDetailDto?> GetDetailDtoAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product == null)
        {
            return null;
        }

        return new ProductDetailDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Category = product.Category,
            Status = product.Status,
            Owner = product.Owner,
            BasePrice = product.BasePrice,
            Currency = product.Currency,
            LaunchDate = product.LaunchDate,
            Description = product.Description,
            CreatedAt = product.CreatedAt,
            TotalRevenue = product.PerformanceRecords.Sum(r => r.Revenue),
            TotalUnits = product.PerformanceRecords.Sum(r => r.UnitsSold),
            ActiveAvailabilities = product.Availabilities.Count(a => a.IsActive),
            Availabilities = product.Availabilities
                .OrderBy(a => a.FlightRoute.Origin)
                .ThenBy(a => a.FlightRoute.Destination)
                .ThenBy(a => a.Channel)
                .Select(a => new AvailabilityDto
                {
                    Id = a.Id,
                    Route = a.FlightRoute.Code,
                    Origin = a.FlightRoute.Origin,
                    Destination = a.FlightRoute.Destination,
                    Channel = a.Channel,
                    IsActive = a.IsActive,
                    RoutePrice = a.RoutePrice
                })
                .ToList()
        };
    }

    public async Task<Product> CreateAsync(Product product, string? changedBy, string? comment)
    {
        var now = DateTime.UtcNow;
        product.Code = product.Code.Trim().ToUpperInvariant();
        product.CreatedAt = now;

        product.StatusHistory.Add(new ProductStatusHistory
        {
            FromStatus = null,
            ToStatus = product.Status,
            ChangedBy = ResolveActor(changedBy, product.Owner),
            ChangedAt = now,
            Comment = string.IsNullOrWhiteSpace(comment) ? "Product created" : comment.Trim()
        });

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product, string? changedBy, string? comment)
    {
        var existing = await _db.Products.FirstOrDefaultAsync(p => p.Id == product.Id);
        if (existing == null)
        {
            return false;
        }

        var previousStatus = existing.Status;

        existing.Code = product.Code.Trim().ToUpperInvariant();
        existing.Name = product.Name;
        existing.Category = product.Category;
        existing.Description = product.Description;
        existing.BasePrice = product.BasePrice;
        existing.Currency = product.Currency;
        existing.Owner = product.Owner;
        existing.LaunchDate = product.LaunchDate;
        existing.Status = product.Status;

        if (previousStatus != product.Status)
        {
            _db.StatusHistory.Add(new ProductStatusHistory
            {
                ProductId = existing.Id,
                FromStatus = previousStatus,
                ToStatus = product.Status,
                ChangedBy = ResolveActor(changedBy, product.Owner),
                ChangedAt = DateTime.UtcNow,
                Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim()
            });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products
            .Include(p => p.Availabilities)
            .Include(p => p.PerformanceRecords)
            .Include(p => p.StatusHistory)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
        {
            return false;
        }

        _db.ProductAvailabilities.RemoveRange(product.Availabilities);
        _db.PerformanceRecords.RemoveRange(product.PerformanceRecords);
        _db.StatusHistory.RemoveRange(product.StatusHistory);
        _db.Products.Remove(product);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var normalized = code.Trim().ToUpperInvariant();
        return await _db.Products.AnyAsync(p =>
            p.Code == normalized && (excludeId == null || p.Id != excludeId));
    }

    public async Task<List<StatusChangeDto>> GetStatusHistoryAsync(int productId)
    {
        return await _db.StatusHistory
            .AsNoTracking()
            .Where(h => h.ProductId == productId)
            .OrderByDescending(h => h.ChangedAt)
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
    }

    private static string ResolveActor(string? changedBy, string owner) =>
        string.IsNullOrWhiteSpace(changedBy) ? owner : changedBy.Trim();
}
