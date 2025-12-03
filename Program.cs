using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BetterFitnessERP.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register EF Core DbContext using connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity (registers UserManager, RoleManager, Identity cookies, etc.)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        // configure identity options if desired
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure the application cookie (login/access denied paths)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Index";
    options.AccessDeniedPath = "/Index";
});

var app = builder.Build();

// Seed roles, optionally assign a user to a role, and ensure DB is migrated & seeded.
// Because top-level statements support await, we can use await here.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Ensure database is created/migrated
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // Prefer migrations for production. Use MigrateAsync to apply pending migrations.
        await db.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error migrating database: {ex.Message}");
        // don't rethrow here — allow app to continue so you can inspect errors during startup
    }

    // Seed roles and optionally assign a user if Identity is being used
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = new[] { "Customer", "Admin" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Optional: assign an existing user to the Customer role by email
        var email = "example@example.com";
        var user = await userManager.FindByEmailAsync(email);
        if (user != null && !await userManager.IsInRoleAsync(user, "Customer"))
        {
            var result = await userManager.AddToRoleAsync(user, "Customer");
            if (!result.Succeeded)
            {
                Console.WriteLine($"Failed to add user '{email}' to role 'Customer': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("Identity services are not correctly registered. Make sure AddIdentity was called and ApplicationDbContext is configured for Identity.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding roles/users: {ex.Message}");
    }

    // Seed application data (Transactions sample) if needed
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        // Ensure DataSeeder and GetSampleTransactions exist in your project
        if (!db.Transactions.Any())
        {
            db.Transactions.AddRange(DataSeeder.GetSampleTransactions());
            await db.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error ensuring sample data: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
