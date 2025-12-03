using Microsoft.EntityFrameworkCore;
using BetterFitnessERP.Models;

namespace BetterFitnessERP.Data
{
    public class ApplicationDbContext : DbContext
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
