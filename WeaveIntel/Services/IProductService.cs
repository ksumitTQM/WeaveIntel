using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Services;

public interface IProductService
{
    Task<List<ProductSummaryDto>> GetSummariesAsync(ProductCategory? category, LifecycleStatus? status, string? search);

    /// <summary>Returns the product with availabilities (and routes), performance records and status history loaded.</summary>
    Task<Product?> GetByIdAsync(int id);

    Task<ProductDetailDto?> GetDetailDtoAsync(int id);

    Task<Product> CreateAsync(Product product, string? changedBy, string? comment);

    /// <summary>Updates a product. A lifecycle history entry is written automatically when the status changes.</summary>
    Task<bool> UpdateAsync(Product product, string? changedBy, string? comment);

    Task<bool> DeleteAsync(int id);

    Task<bool> CodeExistsAsync(string code, int? excludeId = null);

    Task<List<StatusChangeDto>> GetStatusHistoryAsync(int productId);
}
