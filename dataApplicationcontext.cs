using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BetterFitnessERP.Models;

namespace BetterFitnessERP.Data
{
    // Use IdentityDbContext so Identity types (User, Roles, etc.) are mapped into your EF model.
    // If you already have a different Identity DbContext in the project, merge the DbSets below into it instead of replacing.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }

        // Added Customers DbSet so CRM page can operate against Customer entities.
        public DbSet<Customer> Customers { get; set; }
    }
}
