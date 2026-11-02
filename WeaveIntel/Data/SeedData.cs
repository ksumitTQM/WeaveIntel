using WeaveIntel.Models;

namespace WeaveIntel.Data;

public static class SeedData
{
    private sealed record ProductSeed(
        string Code,
        string Name,
        ProductCategory Category,
        string Description,
        decimal Price,
        LifecycleStatus Status,
        string Owner,
        int LaunchOffsetDays,
        int LastChangeDaysAgo,
        double Volume);

    public static void Initialize(WeaveIntelDbContext db)
    {
        if (db.Products.Any())
        {
            return;
        }

        var rnd = new Random(42);
        var now = DateTime.UtcNow;
        var thisMonth = new DateTime(now.Year, now.Month, 1);

        var routes = new List<FlightRoute>
        {
            new() { Origin = "LHR", Destination = "JFK", Region = RouteRegion.NorthAmerica },
            new() { Origin = "LHR", Destination = "DXB", Region = RouteRegion.MiddleEast },
            new() { Origin = "FRA", Destination = "SIN", Region = RouteRegion.Asia },
            new() { Origin = "CDG", Destination = "GRU", Region = RouteRegion.SouthAmerica },
            new() { Origin = "AMS", Destination = "NBO", Region = RouteRegion.Africa },
            new() { Origin = "MAD", Destination = "MIA", Region = RouteRegion.NorthAmerica },
            new() { Origin = "LHR", Destination = "SYD", Region = RouteRegion.Oceania },
            new() { Origin = "FRA", Destination = "BKK", Region = RouteRegion.Asia },
            new() { Origin = "MUC", Destination = "LIS", Region = RouteRegion.Europe },
            new() { Origin = "CDG", Destination = "FCO", Region = RouteRegion.Europe },
            new() { Origin = "LHR", Destination = "LAX", Region = RouteRegion.NorthAmerica },
            new() { Origin = "DXB", Destination = "BOM", Region = RouteRegion.Asia }
        };
        db.FlightRoutes.AddRange(routes);

        var seeds = new List<ProductSeed>
        {
            new("FB-LITE", "Economy Lite", ProductCategory.FareBundle,
                "Entry fare with cabin bag only. Seats and checked baggage sold separately.",
                129m, LifecycleStatus.Live, "Priya Nair", -720, 720, 1.6),
            new("FB-CLASSIC", "Economy Classic", ProductCategory.FareBundle,
                "Standard fare including one checked bag and a snack.",
                189m, LifecycleStatus.Live, "Priya Nair", -690, 690, 1.3),
            new("FB-FLEX", "Economy Flex", ProductCategory.FareBundle,
                "Fully flexible fare with free changes and priority boarding.",
                289m, LifecycleStatus.Live, "Marcus Hale", -540, 540, 0.9),
            new("FB-PREMIUM", "Premium Economy Plus", ProductCategory.FareBundle,
                "Premium cabin fare with lounge access and extra legroom.",
                449m, LifecycleStatus.Live, "Marcus Hale", -300, 300, 0.5),
            new("AN-SEAT", "Seat Selection", ProductCategory.Ancillary,
                "Pre-select standard, extra-legroom or front-row seats at booking or check-in.",
                18m, LifecycleStatus.Live, "Elena Rossi", -800, 800, 2.4),
            new("AN-BAG1", "Extra Baggage 23kg", ProductCategory.Ancillary,
                "Pre-purchase an additional 23kg checked bag at a discount to airport rates.",
                45m, LifecycleStatus.Live, "Elena Rossi", -620, 620, 1.8),
            new("AN-LOUNGE", "Lounge Access", ProductCategory.Ancillary,
                "Single-entry access to partner lounges at the departure airport.",
                35m, LifecycleStatus.Live, "Daniel Okoye", -400, 400, 0.7),
            new("AN-FASTTRACK", "Fast Track Security", ProductCategory.Ancillary,
                "Priority security lane access at selected hubs.",
                14m, LifecycleStatus.Concept, "Daniel Okoye", 200, 12, 1.0),
            new("AN-PET", "Pet in Cabin", ProductCategory.Ancillary,
                "Carry a small pet in the cabin. Retired after low uptake and operational complexity.",
                75m, LifecycleStatus.Retired, "Elena Rossi", -900, 95, 0.2),
            new("OB-MEAL", "Premium Onboard Meal", ProductCategory.Onboard,
                "Pre-order a chef-designed hot meal with a drink.",
                22m, LifecycleStatus.Live, "Sofia Lindgren", -240, 240, 1.4),
            new("OB-WIFI", "Inflight Wi-Fi", ProductCategory.Onboard,
                "Streaming-grade Wi-Fi for the full flight. Piloting on long-haul routes.",
                12m, LifecycleStatus.Pilot, "Sofia Lindgren", 45, 26, 1.1),
            new("OB-SLEEP", "Sleeper Kit", ProductCategory.Onboard,
                "Eye mask, ear plugs, blanket and neck pillow for overnight flights.",
                28m, LifecycleStatus.Development, "Sofia Lindgren", 120, 9, 1.0),
            new("UP-BID", "Bid for Upgrade", ProductCategory.Upgrade,
                "Passengers bid for a cabin upgrade ahead of departure.",
                220m, LifecycleStatus.Pilot, "Marcus Hale", 30, 18, 0.5),
            new("UP-CASH", "Cash Upgrade at Airport", ProductCategory.Upgrade,
                "Fixed-price upgrade offered at check-in and the gate.",
                150m, LifecycleStatus.Live, "Marcus Hale", -150, 150, 0.4),
            new("LY-MILES", "Miles Booster", ProductCategory.Loyalty,
                "Buy bonus loyalty miles at checkout for the booked journey.",
                60m, LifecycleStatus.Development, "Aisha Rahman", 90, 33, 0.8)
        };

        var path = new[]
        {
            LifecycleStatus.Concept,
            LifecycleStatus.Development,
            LifecycleStatus.Pilot,
            LifecycleStatus.Live,
            LifecycleStatus.Retired
        };

        var comments = new[]
        {
            "Concept approved by the product board",
            "Business case signed off, development started",
            "Pilot launched on a limited set of routes",
            "Full launch approved after pilot review",
            "Retired after sustained low uptake"
        };

        var productIndex = 0;
        foreach (var seed in seeds)
        {
            var product = new Product
            {
                Code = seed.Code,
                Name = seed.Name,
                Category = seed.Category,
                Description = seed.Description,
                BasePrice = seed.Price,
                Currency = "EUR",
                Status = seed.Status,
                Owner = seed.Owner,
                LaunchDate = now.Date.AddDays(seed.LaunchOffsetDays),
                CreatedAt = now.AddDays(-Math.Max(seed.LastChangeDaysAgo, 1) - 45 * Array.IndexOf(path, seed.Status))
            };

            // Lifecycle history up to the current status.
            var lastIndex = Array.IndexOf(path, seed.Status);
            var lastChange = now.AddDays(-seed.LastChangeDaysAgo);
            for (var i = 0; i <= lastIndex; i++)
            {
                product.StatusHistory.Add(new ProductStatusHistory
                {
                    FromStatus = i == 0 ? null : path[i - 1],
                    ToStatus = path[i],
                    ChangedBy = path[i] == LifecycleStatus.Live ? "Commercial Director" : seed.Owner,
                    ChangedAt = lastChange.AddDays(-(lastIndex - i) * 45),
                    Comment = comments[i]
                });
            }

            // Availability and sales performance.
            var routeCount = seed.Status switch
            {
                LifecycleStatus.Live => 6 + productIndex % 4,
                LifecycleStatus.Pilot => 3,
                LifecycleStatus.Retired => 2,
                _ => 0
            };

            IEnumerable<int> monthsBack = seed.Status switch
            {
                LifecycleStatus.Live => Enumerable.Range(0, 6),
                LifecycleStatus.Pilot => Enumerable.Range(0, 3),
                LifecycleStatus.Retired => Enumerable.Range(3, 3),
                _ => Enumerable.Empty<int>()
            };
            var monthList = monthsBack.ToList();

            for (var j = 0; j < routeCount; j++)
            {
                var route = routes[(productIndex + j) % routes.Count];
                var routePrice = Math.Round(seed.Price * (decimal)(0.9 + rnd.NextDouble() * 0.3), 2);

                var channels = new List<SalesChannel> { SalesChannel.Web, SalesChannel.Mobile };
                if (seed.Status == LifecycleStatus.Live)
                {
                    channels.Add(j % 2 == 0 ? SalesChannel.GDS : SalesChannel.Airport);
                    if (j % 3 == 0)
                    {
                        channels.Add(SalesChannel.CallCentre);
                    }
                }
                else if (seed.Status == LifecycleStatus.Retired)
                {
                    channels.Clear();
                    channels.Add(SalesChannel.Web);
                }

                foreach (var channel in channels)
                {
                    product.Availabilities.Add(new ProductAvailability
                    {
                        FlightRoute = route,
                        Channel = channel,
                        IsActive = seed.Status != LifecycleStatus.Retired && !(channel == SalesChannel.CallCentre && j % 2 == 1),
                        RoutePrice = routePrice
                    });
                }

                foreach (var m in monthList)
                {
                    var growth = 1.0 + (5 - m) * 0.04;
                    var units = (int)(rnd.Next(40, 240) * seed.Volume * growth);
                    var revenue = Math.Round(units * routePrice * (decimal)(0.9 + rnd.NextDouble() * 0.2), 2);
                    var attach = Math.Round((decimal)(5 + rnd.NextDouble() * 40), 1);

                    product.PerformanceRecords.Add(new PerformanceRecord
                    {
                        FlightRoute = route,
                        Month = thisMonth.AddMonths(-m),
                        UnitsSold = units,
                        Revenue = revenue,
                        AttachRatePercent = attach
                    });
                }
            }

            db.Products.Add(product);
            productIndex++;
        }

        db.SaveChanges();
    }
}
