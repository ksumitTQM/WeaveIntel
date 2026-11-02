using System.ComponentModel.DataAnnotations;

namespace WeaveIntel.Models;

public enum ProductCategory
{
    [Display(Name = "Fare Bundle")]
    FareBundle,

    [Display(Name = "Ancillary")]
    Ancillary,

    [Display(Name = "Upgrade")]
    Upgrade,

    [Display(Name = "Onboard")]
    Onboard,

    [Display(Name = "Loyalty")]
    Loyalty
}

public enum LifecycleStatus
{
    [Display(Name = "Concept")]
    Concept,

    [Display(Name = "Development")]
    Development,

    [Display(Name = "Pilot")]
    Pilot,

    [Display(Name = "Live")]
    Live,

    [Display(Name = "Retired")]
    Retired
}

public enum SalesChannel
{
    [Display(Name = "Web")]
    Web,

    [Display(Name = "Mobile")]
    Mobile,

    [Display(Name = "GDS")]
    GDS,

    [Display(Name = "Airport")]
    Airport,

    [Display(Name = "Call Centre")]
    CallCentre
}

public enum RouteRegion
{
    [Display(Name = "Europe")]
    Europe,

    [Display(Name = "North America")]
    NorthAmerica,

    [Display(Name = "South America")]
    SouthAmerica,

    [Display(Name = "Asia")]
    Asia,

    [Display(Name = "Middle East")]
    MiddleEast,

    [Display(Name = "Africa")]
    Africa,

    [Display(Name = "Oceania")]
    Oceania
}
