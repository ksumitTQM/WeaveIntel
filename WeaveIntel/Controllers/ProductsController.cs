using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models;
using WeaveIntel.Models.DTOs;
using WeaveIntel.Models.ViewModels;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _products;
    private readonly IPerformanceService _performance;

    public ProductsController(IProductService products, IPerformanceService performance)
    {
        _products = products;
        _performance = performance;
    }

    public async Task<IActionResult> Index(ProductCategory? category, LifecycleStatus? status, string? search)
    {
        var model = new ProductListViewModel
        {
            Category = category,
            Status = status,
            Search = search,
            Products = await _products.GetSummariesAsync(category, status, search)
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var performance = await _performance.GetProductPerformanceAsync(id) ?? new ProductPerformanceDto();
        var recent = await _performance.GetRecentRecordsAsync(id, 12);

        var model = new ProductDetailsViewModel
        {
            Product = product,
            Availabilities = product.Availabilities
                .OrderBy(a => a.FlightRoute.Origin)
                .ThenBy(a => a.FlightRoute.Destination)
                .ThenBy(a => a.Channel)
                .ToList(),
            Performance = performance,
            RecentRecords = recent,
            Timeline = BuildTimeline(product)
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new ProductFormViewModel
        {
            LaunchDate = null,
            Status = LifecycleStatus.Concept
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && await _products.CodeExistsAsync(model.Code))
        {
            ModelState.AddModelError(nameof(model.Code), "A product with this code already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = model.ToEntity();
        await _products.CreateAsync(product, model.ChangedBy, model.ChangeComment);

        TempData["Success"] = $"Product {product.Code} was created.";
        return RedirectToAction(nameof(Details), new { id = product.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(ProductFormViewModel.FromProduct(product));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(model.Code) && await _products.CodeExistsAsync(model.Code, model.Id))
        {
            ModelState.AddModelError(nameof(model.Code), "A product with this code already exists.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _products.UpdateAsync(model.ToEntity(), model.ChangedBy, model.ChangeComment);
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Product was updated.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _products.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _products.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Product was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private static List<TimelineItem> BuildTimeline(Product product)
    {
        var items = new List<TimelineItem>();

        foreach (var entry in product.StatusHistory)
        {
            var title = entry.FromStatus == null
                ? $"Created as {entry.ToStatus.DisplayName()}"
                : $"{entry.FromStatus.Value.DisplayName()} to {entry.ToStatus.DisplayName()}";

            items.Add(new TimelineItem
            {
                Date = entry.ChangedAt,
                Title = title,
                Detail = string.IsNullOrWhiteSpace(entry.Comment)
                    ? $"by {entry.ChangedBy}"
                    : $"{entry.Comment} (by {entry.ChangedBy})",
                Kind = "status",
                Status = entry.ToStatus
            });
        }

        if (product.LaunchDate.HasValue)
        {
            var launch = product.LaunchDate.Value;
            items.Add(new TimelineItem
            {
                Date = launch,
                Title = launch.Date > DateTime.UtcNow.Date ? "Planned launch date" : "Launch date",
                Detail = null,
                Kind = "launch"
            });
        }

        var firstSales = product.PerformanceRecords
            .OrderBy(r => r.Month)
            .FirstOrDefault();

        if (firstSales != null)
        {
            items.Add(new TimelineItem
            {
                Date = firstSales.Month,
                Title = "First sales recorded",
                Detail = $"{firstSales.UnitsSold:N0} units in {firstSales.Month:MMM yyyy}",
                Kind = "sales"
            });
        }

        return items.OrderByDescending(i => i.Date).ToList();
    }
}
