using Microsoft.AspNetCore.Mvc.Rendering;
using WeaveIntel.Models;

namespace WeaveIntel.Services;

public interface IRouteService
{
    /// <summary>Returns all routes ordered by code, with availabilities loaded.</summary>
    Task<List<FlightRoute>> GetAllAsync();

    Task<FlightRoute?> GetByIdAsync(int id);

    Task<FlightRoute> CreateAsync(FlightRoute route);

    Task<bool> UpdateAsync(FlightRoute route);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(string origin, string destination, int? excludeId = null);

    Task<List<SelectListItem>> GetSelectItemsAsync();
}
