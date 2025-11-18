using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BetterFitnessERP.Data;
using BetterFitnessERP.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register employee repository service so HRM page can resolve it
builder.Services.AddSingleton<IEmployeeRepository, InMemoryEmployeeRepository>();

// Register EF Core DbContext using connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Try to detect whether the configured SQL Server (LocalDB) is reachable. If not, fall back to SQLite.
var useSqlServer = false;
try
{
    // Try a short-timeout connection to verify availability
    var sb = new SqlConnectionStringBuilder(connectionString)
    {
        ConnectTimeout = 2
    };
    using var testConn = new SqlConnection(sb.ConnectionString);
    testConn.Open();
    testConn.Close();
    useSqlServer = true;
    Console.WriteLine("Database detection: SQL Server reachable. Using LocalDB/SQL Server provider.");
}
catch (Exception ex)
{
    Console.WriteLine($"Database detection: SQL Server not reachable, falling back to SQLite. ({ex.Message})");
}

if (useSqlServer)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    var sqliteConn = "Data Source=betterfitness.db";
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConn));
}

// Authentication & Cookie setup (existing)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Index";
        options.AccessDeniedPath = "/Index";
    });

var app = builder.Build();

// Ensure DB is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.EnsureCreated();

    // Seed all financial tables (safe to call multiple times)
    DataSeeder.Seed(db);

    // Log counts to verify seeding
    Console.WriteLine($"Seeded Customers: {db.Customers.Count()}");
    Console.WriteLine($"Seeded Vendors: {db.Vendors.Count()}");
    Console.WriteLine($"Seeded Accounts: {db.Accounts.Count()}");
    Console.WriteLine($"Seeded Invoices: {db.Invoices.Count()}");
    Console.WriteLine($"Seeded InvoiceItems: {db.InvoiceItems.Count()}");
    Console.WriteLine($"Seeded Payments: {db.Payments.Count()}");
    Console.WriteLine($"Seeded Expenses: {db.Expenses.Count()}");
    Console.WriteLine($"Seeded Transactions: {db.Transactions.Count()}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error ensuring database: {ex.Message}");
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

// Add lifecycle logging to help diagnose unexpected shutdowns
var lifetime = app.Lifetime;
lifetime.ApplicationStarted.Register(() => Console.WriteLine("APPLICATION_STARTED"));
lifetime.ApplicationStopping.Register(() => Console.WriteLine("APPLICATION_STOPPING"));
lifetime.ApplicationStopped.Register(() => Console.WriteLine("APPLICATION_STOPPED"));

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception during host run: {ex}");
    throw;
}
