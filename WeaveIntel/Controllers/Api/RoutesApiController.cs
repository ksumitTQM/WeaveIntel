using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models.DTOs;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers.Api;

[ApiController]
[Route("api/routes")]
[Produces("application/json")]
public class RoutesApiController : ControllerBase
{
    private readonly IRouteService _routes;

    public RoutesApiController(IRouteService routes)
    {
        _routes = routes;
    }

    /// <summary>Lists all routes.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RouteDto>>> GetAll()
    {
        var routes = await _routes.GetAllAsync();
        var dtos = routes.Select(r => new RouteDto
        {
            Id = r.Id,
            Origin = r.Origin,
            Destination = r.Destination,
            Code = r.Code,
            Region = r.Region
        });

        return Ok(dtos);
    }

    /// <summary>Gets a single route.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RouteDto>> GetById(int id)
    {
        var route = await _routes.GetByIdAsync(id);
        if (route == null)
        {
            return NotFound();
        }

        return Ok(new RouteDto
        {
            Id = route.Id,
            Origin = route.Origin,
            Destination = route.Destination,
            Code = route.Code,
            Region = route.Region
        });
    }
}
