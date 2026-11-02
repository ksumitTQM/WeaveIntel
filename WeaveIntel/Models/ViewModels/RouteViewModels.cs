using System.ComponentModel.DataAnnotations;

namespace WeaveIntel.Models.ViewModels;

public class RouteFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(3, MinimumLength = 3, ErrorMessage = "Use a 3-letter IATA code.")]
    [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "Use a 3-letter IATA code.")]
    [Display(Name = "Origin (IATA)")]
    public string Origin { get; set; } = string.Empty;

    [Required, StringLength(3, MinimumLength = 3, ErrorMessage = "Use a 3-letter IATA code.")]
    [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "Use a 3-letter IATA code.")]
    [Display(Name = "Destination (IATA)")]
    public string Destination { get; set; } = string.Empty;

    public RouteRegion Region { get; set; }

    public static RouteFormViewModel FromRoute(FlightRoute route) => new()
    {
        Id = route.Id,
        Origin = route.Origin,
        Destination = route.Destination,
        Region = route.Region
    };

    public FlightRoute ToEntity() => new()
    {
        Id = Id,
        Origin = Origin.Trim().ToUpperInvariant(),
        Destination = Destination.Trim().ToUpperInvariant(),
        Region = Region
    };
}
