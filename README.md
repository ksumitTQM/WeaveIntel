# WeaveIntel

WeaveIntel is a demo web portal for airline product tracking. An airline's commercial and product team uses it to follow its products (fare bundles, ancillaries such as seat selection, extra baggage, lounge access, Wi-Fi and onboard meals, upgrades and loyalty add-ons) across routes and sales channels, through their lifecycle, and into their sales performance.

It is an ASP.NET Core MVC application on .NET 8 with a small read-only JSON Web API alongside the UI. Data is stored in an EF Core InMemory database that is seeded with sample data at startup, so nothing else needs to be installed. Data resets every time the app restarts.

## Features

- **Dashboard** with Chart.js charts (loaded from a CDN): products by lifecycle stage, top 5 products by revenue, revenue by category, revenue by month, KPI tiles and the most recent status changes.
- **Products**: list with filters (category, lifecycle status, free-text search), details page, create, edit and delete. Each product has a code, name, category (Fare Bundle, Ancillary, Upgrade, Onboard, Loyalty), description, base price, currency, lifecycle status (Concept, Development, Pilot, Live, Retired), owner and launch date.
- **Lifecycle history and audit**: every status change is written automatically to a history log with who, when and an optional comment. The product details page shows a tracking timeline (status changes, launch date, first recorded sales).
- **Routes**: create, edit and delete routes (origin and destination IATA codes plus region).
- **Availability**: manage where each product is sold, per route and channel (Web, Mobile, GDS, Airport, Call Centre), with an active flag and an optional route-specific price. Entries can be added, activated or deactivated and removed.
- **Performance records**: add monthly sales records per product and route (units sold, revenue, attach rate %), and see monthly and per-route aggregates.
- Antiforgery tokens on all MVC form posts, server-side validation with client-side unobtrusive validation, Bootstrap 5 UI with an aviation-themed stylesheet.
- Swagger UI for the API when running in the Development environment.

## API endpoints

All endpoints return JSON. Enums are serialised as strings.

| Method | Path | Description |
| --- | --- | --- |
| GET | `/api/products` | List products. Optional query: `category`, `status`, `search`. |
| GET | `/api/products/{id}` | Product detail including route/channel availability. |
| GET | `/api/products/{id}/performance` | Totals, monthly series and per-route breakdown. |
| GET | `/api/products/{id}/history` | Lifecycle status change history, newest first. |
| GET | `/api/routes` | List routes. |
| GET | `/api/routes/{id}` | Single route. |
| GET | `/api/dashboard` | Portfolio summary used by the dashboard. |

Swagger UI is available at `/swagger` in the Development environment. The API is read-only and does not use antiforgery tokens; the antiforgery protection applies to the MVC form posts.

Revenue figures are treated as being in the reporting currency (EUR).

## Project structure

```
WeaveIntel.sln
README.md
.gitignore
WeaveIntel/
  WeaveIntel.csproj
  Program.cs
  appsettings.json
  appsettings.Development.json
  Properties/launchSettings.json
  Controllers/        MVC controllers (Home, Products, Routes, Availability, Performance)
    Api/              JSON API controllers
  Models/             Entities and enums
    DTOs/             API / reporting data transfer objects
    ViewModels/       Form and page view models
  Data/               WeaveIntelDbContext and SeedData
  Services/           Service interfaces and implementations (registered via DI)
  Views/              Razor views (Shared/_Layout uses Bootstrap 5 from a CDN)
  wwwroot/css/        site.css
```

## How to run

Prerequisite: the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). An internet connection is needed in the browser for the Bootstrap, jQuery and Chart.js CDN files.

From the solution folder (the one containing `WeaveIntel.sln`):

```
dotnet run --project WeaveIntel
```

Or open `WeaveIntel.sln` in Visual Studio 2022 and press F5.

By default the app listens on `http://localhost:5080` (and `https://localhost:7080` with the https profile). Swagger UI is at `/swagger`.

NuGet packages used: `Microsoft.EntityFrameworkCore.InMemory` 8.0.11 and `Swashbuckle.AspNetCore` 6.6.2.
