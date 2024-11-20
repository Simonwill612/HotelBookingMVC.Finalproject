using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IRoomService, RoomService>();

// Configure DbContext for HotelBooking
builder.Services.AddDbContext<HotelBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelBookingDbContextConnection")));

// Configure DbContext for Identity
builder.Services.AddDbContext<HotelIdentityDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HotelIdentityDBContextConnection")));

// Configure Identity services
builder.Services.AddDefaultIdentity<HotelUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add roles support
    .AddEntityFrameworkStores<HotelIdentityDBContext>();

//// Configure upload limits
//builder.Services.Configure<FormOptions>(options =>
//{
//    options.MultipartBodyLengthLimit = 314572800; // 300 MB
//});

// Configure Identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

// Application cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add email service
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services.AddScoped<EmailService>();

// Configure session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Role management function
async Task CreateRoles(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "Admin", "Manager", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

var app = builder.Build(); // Build the app after adding services

// Ensure roles are created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRoles(services);
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Add session middleware

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hotels}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();