using System.ComponentModel.DataAnnotations;

namespace WeaveIntel.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ProductCategory Category { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(0, 100000)]
    public decimal BasePrice { get; set; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "EUR";

    public LifecycleStatus Status { get; set; } = LifecycleStatus.Concept;

    [Required, StringLength(100)]
    public string Owner { get; set; } = string.Empty;

    public DateTime? LaunchDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductAvailability> Availabilities { get; set; } = new List<ProductAvailability>();

    public ICollection<PerformanceRecord> PerformanceRecords { get; set; } = new List<PerformanceRecord>();

    public ICollection<ProductStatusHistory> StatusHistory { get; set; } = new List<ProductStatusHistory>();
}
