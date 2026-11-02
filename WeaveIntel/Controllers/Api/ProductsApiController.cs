using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers.Api;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _products;
    private readonly IPerformanceService _performance;

    public ProductsApiController(IProductService products, IPerformanceService performance)
    {
        _products = products;
        _performance = performance;
    }

    /// <summary>Lists products, optionally filtered by category, status or a search term.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductSummaryDto>>> GetAll(
        [FromQuery] ProductCategory? category,
        [FromQuery] LifecycleStatus? status,
        [FromQuery] string? search)
    {
        var items = await _products.GetSummariesAsync(category, status, search);
        return Ok(items);
    }

    /// <summary>Gets a single product including its route/channel availability.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDetailDto>> GetById(int id)
    {
        var product = await _products.GetDetailDtoAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>Gets aggregated sales performance (monthly and by route) for a product.</summary>
    [HttpGet("{id:int}/performance")]
    public async Task<ActionResult<ProductPerformanceDto>> GetPerformance(int id)
    {
        var performance = await _performance.GetProductPerformanceAsync(id);
        if (performance == null)
        {
            return NotFound();
        }

        return Ok(performance);
    }

    /// <summary>Gets the lifecycle status change history of a product, newest first.</summary>
    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IEnumerable<StatusChangeDto>>> GetHistory(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var history = await _products.GetStatusHistoryAsync(id);
        return Ok(history);
    }
}
