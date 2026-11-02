using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models;
using WeaveIntel.Models.ViewModels;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers;

public class PerformanceController : Controller
{
    private readonly IProductService _products;
    private readonly IPerformanceService _performance;
    private readonly IRouteService _routes;

    public PerformanceController(IProductService products, IPerformanceService performance, IRouteService routes)
    {
        _products = products;
        _performance = performance;
        _routes = routes;
    }

    public async Task<IActionResult> Create(int productId)
    {
        var model = new PerformanceFormViewModel { ProductId = productId };
        if (!await PopulateAsync(model))
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PerformanceFormViewModel model)
    {
        if (!DateTime.TryParseExact(model.Month, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var month))
        {
            ModelState.AddModelError(nameof(model.Month), "Enter the month as yyyy-MM.");
        }

        if (ModelState.IsValid)
        {
            var added = await _performance.AddAsync(new PerformanceRecord
            {
                ProductId = model.ProductId,
                FlightRouteId = model.FlightRouteId!.Value,
                Month = month,
                UnitsSold = model.UnitsSold,
                Revenue = model.Revenue,
                AttachRatePercent = model.AttachRatePercent
            });

            if (added)
            {
                TempData["Success"] = "Performance record added.";
                return RedirectToAction("Details", "Products", new { id = model.ProductId });
            }

            ModelState.AddModelError(string.Empty, "A record for this product, route and month already exists.");
        }

        if (!await PopulateAsync(model))
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int productId)
    {
        if (!await _performance.DeleteAsync(id))
        {
            return NotFound();
        }

        TempData["Success"] = "Performance record deleted.";
        return RedirectToAction("Details", "Products", new { id = productId });
    }

    private async Task<bool> PopulateAsync(PerformanceFormViewModel model)
    {
        var product = await _products.GetByIdAsync(model.ProductId);
        if (product == null)
        {
            return false;
        }

        model.ProductName = $"{product.Code} - {product.Name}";
        model.Routes = await _routes.GetSelectItemsAsync();
        return true;
    }
}
