using Microsoft.AspNetCore.Mvc;
using WeaveIntel.Models;
using WeaveIntel.Models.ViewModels;
using WeaveIntel.Services;

namespace WeaveIntel.Controllers;

public class RoutesController : Controller
{
    private readonly IRouteService _routes;

    public RoutesController(IRouteService routes)
    {
        _routes = routes;
    }

    public async Task<IActionResult> Index()
    {
        var routes = await _routes.GetAllAsync();
        return View(routes);
    }

    public IActionResult Create()
    {
        return View(new RouteFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RouteFormViewModel model)
    {
        await ValidateRouteAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var route = await _routes.CreateAsync(model.ToEntity());
        TempData["Success"] = $"Route {route.Code} was created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var route = await _routes.GetByIdAsync(id);
        if (route == null)
        {
            return NotFound();
        }

        return View(RouteFormViewModel.FromRoute(route));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, RouteFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        await ValidateRouteAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updated = await _routes.UpdateAsync(model.ToEntity());
        if (!updated)
        {
            return NotFound();
        }

        TempData["Success"] = "Route was updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var route = await _routes.GetByIdAsync(id);
        if (route == null)
        {
            return NotFound();
        }

        return View(route);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _routes.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        TempData["Success"] = "Route was deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task ValidateRouteAsync(RouteFormViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Origin) || string.IsNullOrWhiteSpace(model.Destination))
        {
            return;
        }

        if (string.Equals(model.Origin.Trim(), model.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.Destination), "Destination must differ from the origin.");
            return;
        }

        var excludeId = model.Id == 0 ? (int?)null : model.Id;
        if (await _routes.ExistsAsync(model.Origin, model.Destination, excludeId))
        {
            ModelState.AddModelError(string.Empty, "That route already exists.");
        }
    }
}
