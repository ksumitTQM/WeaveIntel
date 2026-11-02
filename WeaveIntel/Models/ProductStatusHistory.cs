namespace WeaveIntel.Models;

/// <summary>Audit entry written whenever a product is created or its lifecycle status changes.</summary>
public class ProductStatusHistory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    /// <summary>Null for the initial entry written at creation time.</summary>
    public LifecycleStatus? FromStatus { get; set; }

    public LifecycleStatus ToStatus { get; set; }

    public string ChangedBy { get; set; } = string.Empty;

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    public string? Comment { get; set; }
}
