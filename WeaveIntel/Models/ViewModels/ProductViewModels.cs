using System.ComponentModel.DataAnnotations;
using WeaveIntel.Models.DTOs;

namespace WeaveIntel.Models.ViewModels;

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(20)]
    [RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "Code may contain letters, digits and hyphens only.")]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public ProductCategory Category { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Display(Name = "Base price")]
    [Range(0, 100000, ErrorMessage = "Base price must be between 0 and 100,000.")]
    public decimal BasePrice { get; set; }

    [Required, StringLength(3, MinimumLength = 3, ErrorMessage = "Currency must be a 3-letter ISO code.")]
    [RegularExpression("^[A-Za-z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code.")]
    public string Currency { get; set; } = "EUR";

    [Display(Name = "Lifecycle status")]
    public LifecycleStatus Status { get; set; } = LifecycleStatus.Concept;

    [Required, StringLength(100)]
    public string Owner { get; set; } = string.Empty;

    [Display(Name = "Launch date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? LaunchDate { get; set; }

    [Display(Name = "Changed by")]
    [StringLength(100)]
    public string? ChangedBy { get; set; }

    [Display(Name = "Status change comment")]
    [StringLength(500)]
    public string? ChangeComment { get; set; }

    public static ProductFormViewModel FromProduct(Product product) => new()
    {
        Id = product.Id,
        Code = product.Code,
        Name = product.Name,
        Category = product.Category,
        Description = product.Description,
        BasePrice = product.BasePrice,
        Currency = product.Currency,
        Status = product.Status,
        Owner = product.Owner,
        LaunchDate = product.LaunchDate
    };

    public Product ToEntity() => new()
    {
        Id = Id,
        Code = Code.Trim().ToUpperInvariant(),
        Name = Name.Trim(),
        Category = Category,
        Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim(),
        BasePrice = BasePrice,
        Currency = Currency.Trim().ToUpperInvariant(),
        Status = Status,
        Owner = Owner.Trim(),
        LaunchDate = LaunchDate
    };
}

public class ProductListViewModel
{
    public List<ProductSummaryDto> Products { get; set; } = new();
    public ProductCategory? Category { get; set; }
    public LifecycleStatus? Status { get; set; }
    public string? Search { get; set; }

    public bool HasFilters => Category.HasValue || Status.HasValue || !string.IsNullOrWhiteSpace(Search);
}

public class TimelineItem
{
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Detail { get; set; }

    /// <summary>One of: status, launch, sales.</summary>
    public string Kind { get; set; } = "status";

    public LifecycleStatus? Status { get; set; }
}

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;
    public List<ProductAvailability> Availabilities { get; set; } = new();
    public ProductPerformanceDto Performance { get; set; } = new();
    public List<PerformanceRecord> RecentRecords { get; set; } = new();
    public List<TimelineItem> Timeline { get; set; } = new();
}
