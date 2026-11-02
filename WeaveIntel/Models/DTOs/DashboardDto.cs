namespace WeaveIntel.Models.DTOs;

public class ChartPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class TopProductDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int Units { get; set; }
}

public class DashboardDto
{
    public int TotalProducts { get; set; }
    public int LiveProducts { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalUnits { get; set; }
    public decimal AverageAttachRatePercent { get; set; }
    public List<ChartPoint> ProductsByStatus { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
    public List<ChartPoint> RevenueByCategory { get; set; } = new();
    public List<ChartPoint> RevenueByMonth { get; set; } = new();
    public List<StatusChangeDto> RecentChanges { get; set; } = new();
}
