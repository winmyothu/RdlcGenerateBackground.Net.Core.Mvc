using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using RdlcSignalRDemo.Data;
using RdlcSignalRDemo.Hubs;
using RdlcSignalRDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSignalR();
builder.Services.AddScoped<ReportGeneratorService>();
builder.Services.AddHostedService<BackgroundReportService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(); // For wwwroot

// Add static file serving for Reports/Output
var outputDir = Path.Combine(app.Environment.ContentRootPath, "Reports", "Output");
if (!Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(outputDir),
    RequestPath = "/Reports/Output"
});


app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Reports}/{action=Index}/{id?}");

app.MapHub<ReportHub>("/reportHub");

app.Run();
