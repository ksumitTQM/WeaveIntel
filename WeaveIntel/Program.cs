using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using WeaveIntel.Data;
using WeaveIntel.Services;

// Keep number/date formatting stable regardless of the host machine's regional settings.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<WeaveIntelDbContext>(options =>
    options.UseInMemoryDatabase("WeaveIntelDb"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WeaveIntel API",
        Version = "v1",
        Description = "Read-only JSON API for airline product tracking."
    });

    // Only document the JSON API, not the MVC pages.
    options.DocInclusionPredicate((_, apiDescription) =>
        apiDescription.RelativePath?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) ?? false);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WeaveIntelDbContext>();
    SeedData.Initialize(db);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "WeaveIntel API v1");
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
