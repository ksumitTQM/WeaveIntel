namespace WeaveIntel.Models;

/// <summary>A product offered on a route through a specific sales channel.</summary>
public class ProductAvailability
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int FlightRouteId { get; set; }

    public FlightRoute FlightRoute { get; set; } = null!;

    public SalesChannel Channel { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Optional route-specific price; falls back to the product base price when null.</summary>
    public decimal? RoutePrice { get; set; }
}
