using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeaveIntel.Models;

public class FlightRoute
{
    public int Id { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Origin { get; set; } = string.Empty;

    [Required, StringLength(3, MinimumLength = 3)]
    public string Destination { get; set; } = string.Empty;

    public RouteRegion Region { get; set; }

    [NotMapped]
    public string Code => $"{Origin}-{Destination}";

    public ICollection<ProductAvailability> Availabilities { get; set; } = new List<ProductAvailability>();

    public ICollection<PerformanceRecord> PerformanceRecords { get; set; } = new List<PerformanceRecord>();
}
