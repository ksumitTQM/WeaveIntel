namespace WeaveIntel.Models;

/// <summary>Monthly sales performance of a product on a route.</summary>
public class PerformanceRecord
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int FlightRouteId { get; set; }

    public FlightRoute FlightRoute { get; set; } = null!;

    /// <summary>First day of the month the record applies to.</summary>
    public DateTime Month { get; set; }

    public int UnitsSold { get; set; }

    public decimal Revenue { get; set; }

    public decimal AttachRatePercent { get; set; }
}
