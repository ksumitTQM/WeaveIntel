using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models.DTOs;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public class DashboardApiController : ControllerBase
{
    private readonly IDashboardService _dashboard;

    public DashboardApiController(IDashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    /// <summary>Gets the portfolio summary used by the dashboard.</summary>
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get()
    {
        return Ok(await _dashboard.GetDashboardAsync());
    }
}
