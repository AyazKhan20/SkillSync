using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartJobRecommender.Data;
using SmartJobRecommender.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Configure ASP.NET Identity
// This sets up the default Identity system to use ApplicationDbContext and IdentityUser
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IResumeService, ResumeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Configure Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

// 4. Map Endpoints (Razor Pages and MVC Controllers)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// This line is CRITICAL for Identity Razor Pages to be discoverable
app.MapRazorPages();

app.Run();