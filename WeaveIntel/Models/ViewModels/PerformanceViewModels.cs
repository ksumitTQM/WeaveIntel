using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WeaveIntel.Models.ViewModels;

public class PerformanceFormViewModel
{
    public int ProductId { get; set; }

    /// <summary>Display only; reloaded from the database on every request.</summary>
    public string? ProductName { get; set; }

    [Required(ErrorMessage = "Select a route.")]
    [Display(Name = "Route")]
    public int? FlightRouteId { get; set; }

    /// <summary>Month in yyyy-MM format (matches the HTML month input).</summary>
    [Required(ErrorMessage = "Enter a month.")]
    [RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])$", ErrorMessage = "Enter the month as yyyy-MM.")]
    public string Month { get; set; } = DateTime.UtcNow.ToString("yyyy-MM");

    [Display(Name = "Units sold")]
    [Range(0, 10000000, ErrorMessage = "Units sold must be between 0 and 10,000,000.")]
    public int UnitsSold { get; set; }

    [Range(typeof(decimal), "0", "1000000000", ErrorMessage = "Revenue must be zero or more.")]
    public decimal Revenue { get; set; }

    [Display(Name = "Attach rate (%)")]
    [Range(typeof(decimal), "0", "100", ErrorMessage = "Attach rate must be between 0 and 100.")]
    public decimal AttachRatePercent { get; set; }

    public List<SelectListItem> Routes { get; set; } = new();
}
