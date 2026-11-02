using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}
