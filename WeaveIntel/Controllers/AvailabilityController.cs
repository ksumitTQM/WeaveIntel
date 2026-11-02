using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models;
using WeaveIntel.Models.ViewModels;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers;

public class AvailabilityController : Controller
{
    private readonly IProductService _products;
    private readonly IAvailabilityService _availability;
    private readonly IRouteService _routes;

    public AvailabilityController(IProductService products, IAvailabilityService availability, IRouteService routes)
    {
        _products = products;
        _availability = availability;
        _routes = routes;
    }

    /// <summary>Manage availability for the product with the given id.</summary>
    public async Task<IActionResult> Manage(int id)
    {
        var model = await BuildModelAsync(id, new AvailabilityFormViewModel { ProductId = id });
        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind(Prefix = "Form")] AvailabilityFormViewModel form)
    {
        if (ModelState.IsValid)
        {
            var (success, error) = await _availability.AddAsync(new ProductAvailability
            {
                ProductId = form.ProductId,
                FlightRouteId = form.FlightRouteId!.Value,
                Channel = form.Channel,
                IsActive = form.IsActive,
                RoutePrice = form.RoutePrice
            });

            if (success)
            {
                TempData["Success"] = "Availability added.";
                return RedirectToAction(nameof(Manage), new { id = form.ProductId });
            }

            ModelState.AddModelError(string.Empty, error ?? "Unable to add availability.");
        }

        var model = await BuildModelAsync(form.ProductId, form);
        if (model == null)
        {
            return NotFound();
        }

        return View("Manage", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, int productId)
    {
        if (!await _availability.ToggleAsync(id))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Manage), new { id = productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, int productId)
    {
        if (!await _availability.DeleteAsync(id))
        {
            return NotFound();
        }

        TempData["Success"] = "Availability removed.";
        return RedirectToAction(nameof(Manage), new { id = productId });
    }

    private async Task<AvailabilityManageViewModel?> BuildModelAsync(int productId, AvailabilityFormViewModel form)
    {
        var product = await _products.GetByIdAsync(productId);
        if (product == null)
        {
            return null;
        }

        form.ProductId = productId;

        return new AvailabilityManageViewModel
        {
            Product = product,
            Items = await _availability.GetForProductAsync(productId),
            Form = form,
            Routes = await _routes.GetSelectItemsAsync()
        };
    }
}
