using WeaveIntel.Models;

namespace WeaveIntel.Services;

public interface IAvailabilityService
{
    Task<List<ProductAvailability>> GetForProductAsync(int productId);

    /// <summary>Adds an availability. Fails when the product/route/channel combination already exists.</summary>
    Task<(bool Success, string? Error)> AddAsync(ProductAvailability availability);

    Task<bool> ToggleAsync(int id);

    Task<bool> DeleteAsync(int id);
}
