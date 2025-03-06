using Microsoft.EntityFrameworkCore;
using QRCodeBasedMetroTicketingSystem.Application.Services.Implementations;
using QRCodeBasedMetroTicketingSystem.Application.Services.Interfaces;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Data;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Services.Implementations;
using QRCodeBasedMetroTicketingSystem.Infrastructure.Services.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

builder.Services.AddScoped<IDistanceService, DistanceService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
    defaults: new { area = "Admin" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
