namespace WeaveIntel.Models.DTOs;

public class ProductSummaryDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public LifecycleStatus Status { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime? LaunchDate { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalUnits { get; set; }
    public int ActiveAvailabilities { get; set; }
}

public class ProductDetailDto : ProductSummaryDto
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AvailabilityDto> Availabilities { get; set; } = new();
}

public class AvailabilityDto
{
    public int Id { get; set; }
    public string Route { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public SalesChannel Channel { get; set; }
    public bool IsActive { get; set; }
    public decimal? RoutePrice { get; set; }
}

public class StatusChangeDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public LifecycleStatus? FromStatus { get; set; }
    public LifecycleStatus ToStatus { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string? Comment { get; set; }
}

public class RouteDto
{
    public int Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public RouteRegion Region { get; set; }
}
