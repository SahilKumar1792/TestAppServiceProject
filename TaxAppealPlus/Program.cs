using Microsoft.EntityFrameworkCore;
using TaxAppealPlus.Models;
using Serilog;
using TaxAppealPlus.Filters;
// Add this using directive to enable UseSqlServer extension method
var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Map("Controller", (controller, wt) =>
        wt.File(
            path: $"Logs/{(string.IsNullOrWhiteSpace(controller) ? "General" : controller)}/error-.log",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
            retainedFileCountLimit: 30))
    .CreateLogger();

builder.Host.UseSerilog();

// EF Core and caching
builder.Services.AddDbContext<BlogDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDatabase"));
});
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionLoggingFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Set caching headers for static files
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000"); // 1 year for images, CSS, JS
    }
});

app.UseRouting();

app.UseAuthorization();

// Blog routes
app.MapControllerRoute(
    name: "blog-details",
    pattern: "blog/{slug}",
    defaults: new { controller = "BlogPage", action = "Details" },
    constraints: new { slug = "[a-z0-9-]+" }
);

app.MapControllerRoute(
    name: "blog-index",
    pattern: "blog",
    defaults: new { controller = "BlogPage", action = "Index" }
);

app.MapControllerRoute(name: "default", pattern: "{action=Index}/{id?}");

app.Run();
