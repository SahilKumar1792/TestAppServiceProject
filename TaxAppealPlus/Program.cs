var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(name: "default", pattern: "{action=Index}/{id?}");

app.Run();
