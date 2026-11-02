using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Services;

public interface IPerformanceService
{
    Task<ProductPerformanceDto?> GetProductPerformanceAsync(int productId);

    /// <summary>Latest records first, with the route loaded.</summary>
    Task<List<PerformanceRecord>> GetRecentRecordsAsync(int productId, int count);

    /// <summary>Adds a record. Returns false when the product/route/month combination already exists.</summary>
    Task<bool> AddAsync(PerformanceRecord record);

    Task<bool> DeleteAsync(int id);
}
