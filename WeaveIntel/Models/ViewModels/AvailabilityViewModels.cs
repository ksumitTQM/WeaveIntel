using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WeaveIntel.Models.ViewModels;

public class AvailabilityFormViewModel
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Select a route.")]
    [Display(Name = "Route")]
    public int? FlightRouteId { get; set; }

    public SalesChannel Channel { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "Route price (optional)")]
    [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100,000.")]
    public decimal? RoutePrice { get; set; }
}

public class AvailabilityManageViewModel
{
    public Product Product { get; set; } = null!;
    public List<ProductAvailability> Items { get; set; } = new();
    public AvailabilityFormViewModel Form { get; set; } = new();
    public List<SelectListItem> Routes { get; set; } = new();
}
