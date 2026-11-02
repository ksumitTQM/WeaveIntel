namespace WeaveIntel.Models.DTOs;

public class ProductPerformanceDto
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TotalUnits { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageAttachRatePercent { get; set; }
    public List<MonthlyPerformanceDto> Monthly { get; set; } = new();
    public List<RoutePerformanceDto> ByRoute { get; set; } = new();
}

public class MonthlyPerformanceDto
{
    /// <summary>Month formatted as yyyy-MM.</summary>
    public string Month { get; set; } = string.Empty;
    public int Units { get; set; }
    public decimal Revenue { get; set; }
    public decimal AttachRatePercent { get; set; }
}

public class RoutePerformanceDto
{
    public string Route { get; set; } = string.Empty;
    public int Units { get; set; }
    public decimal Revenue { get; set; }
}
