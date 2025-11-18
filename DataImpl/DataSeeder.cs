using System;
using System.Collections.Generic;
using System.Linq;
using BetterFitnessERP.Models;

namespace BetterFitnessERP.Data
{
    public static class DataSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (!db.Customers.Any())
            {
                db.Customers.AddRange(GetSampleCustomers());
            }

            if (!db.Vendors.Any())
            {
                db.Vendors.AddRange(GetSampleVendors());
            }

            if (!db.Accounts.Any())
            {
                db.Accounts.AddRange(GetSampleAccounts());
            }

            List<Invoice>? invoices = null;
            if (!db.Invoices.Any())
            {
                invoices = GetSampleInvoices();
                db.Invoices.AddRange(invoices);
                // Save now to ensure invoices have database Ids before adding dependent InvoiceItems
                db.SaveChanges();
            }

            if (!db.InvoiceItems.Any())
            {
                var items = GetSampleInvoiceItems();
                // If we just added invoices above use them; otherwise load existing invoices to map IDs
                if (invoices == null)
                {
                    invoices = db.Invoices.OrderBy(i => i.Id).ToList();
                }

                // Assign each item to a corresponding invoice by index (wrap if counts differ)
                for (int i = 0; i < items.Count; i++)
                {
                    var invoice = invoices.Count > 0 ? invoices[i % invoices.Count] : null;
                    if (invoice != null)
                    {
                        items[i].InvoiceId = invoice.Id;
                    }
                }

                db.InvoiceItems.AddRange(items);
            }

            if (!db.Payments.Any())
            {
                db.Payments.AddRange(GetSamplePayments());
            }

            if (!db.Expenses.Any())
            {
                db.Expenses.AddRange(GetSampleExpenses());
            }

            if (!db.Transactions.Any())
            {
                db.Transactions.AddRange(GetSampleTransactions());
            }

            db.SaveChanges();
        }

        public static List<Customer> GetSampleCustomers()
        {
            return new List<Customer>
            {
                new Customer { Name = "Acme Fitness", ContactInfo = "acme@example.com" },
                new Customer { Name = "Wellness Co", ContactInfo = "contact@wellnessco.com" },
                new Customer { Name = "Local Gym", ContactInfo = "info@localgym.com" },
                new Customer { Name = "Studio One", ContactInfo = "hello@studioone.com" },
                new Customer { Name = "Peak Performance", ContactInfo = "sales@peak.com" }
            };
        }

        public static List<Vendor> GetSampleVendors()
        {
            return new List<Vendor>
            {
                new Vendor { Name = "SupplyCo", ContactInfo = "orders@supplyco.com" },
                new Vendor { Name = "OfficeMart", ContactInfo = "sales@officemart.com" },
                new Vendor { Name = "EquipRentals", ContactInfo = "rentals@equip.com" },
                new Vendor { Name = "Cleaners Inc", ContactInfo = "billing@cleaners.com" },
                new Vendor { Name = "Utilities Ltd", ContactInfo = "support@utils.com" }
            };
        }

        public static List<Account> GetSampleAccounts()
        {
            return new List<Account>
            {
                new Account { Name = "Checking - Main", Type = "Bank", Balance = 12000m },
                new Account { Name = "Savings", Type = "Bank", Balance = 50000m },
                new Account { Name = "Undeposited Funds", Type = "Current", Balance = 0m },
                new Account { Name = "Petty Cash", Type = "Cash", Balance = 250m },
                new Account { Name = "Credit Card", Type = "Liability", Balance = -1500m }
            };
        }

        public static List<Invoice> GetSampleInvoices()
        {
            var customers = GetSampleCustomers();
            return new List<Invoice>
            {
                new Invoice { Date = DateTime.UtcNow.AddDays(-30), Description = "Monthly membership - Acme", Total = 500m, Status = "Paid", Customer = customers[0] },
                new Invoice { Date = DateTime.UtcNow.AddDays(-20), Description = "Equipment rental - Wellness", Total = 1200m, Status = "Unpaid", Customer = customers[1] },
                new Invoice { Date = DateTime.UtcNow.AddDays(-10), Description = "Personal training package", Total = 800m, Status = "Paid", Customer = customers[2] },
                new Invoice { Date = DateTime.UtcNow.AddDays(-5), Description = "Studio booking", Total = 300m, Status = "Unpaid", Customer = customers[3] },
                new Invoice { Date = DateTime.UtcNow.AddDays(-2), Description = "Event charge", Total = 1500m, Status = "Paid", Customer = customers[4] }
            };
        }

        public static List<InvoiceItem> GetSampleInvoiceItems()
        {
            return new List<InvoiceItem>
            {
                new InvoiceItem { Description = "Membership - 1 month", UnitPrice = 100m, Quantity = 5 },
                new InvoiceItem { Description = "Rental - equipment A", UnitPrice = 300m, Quantity = 1 },
                new InvoiceItem { Description = "Training sessions", UnitPrice = 50m, Quantity = 16 },
                new InvoiceItem { Description = "Studio hourly rate", UnitPrice = 75m, Quantity = 4 },
                new InvoiceItem { Description = "Event fee", UnitPrice = 500m, Quantity = 3 }
            };
        }

        public static List<Payment> GetSamplePayments()
        {
            return new List<Payment>
            {
                new Payment { Date = DateTime.UtcNow.AddDays(-29), Amount = 500m, Method = "Credit Card", Notes = "Membership" },
                new Payment { Date = DateTime.UtcNow.AddDays(-9), Amount = 800m, Method = "ACH", Notes = "Training" },
                new Payment { Date = DateTime.UtcNow.AddDays(-1), Amount = 1500m, Method = "Check", Notes = "Event" },
                new Payment { Date = DateTime.UtcNow.AddDays(-15), Amount = 250m, Method = "Cash", Notes = "Studio" },
                new Payment { Date = DateTime.UtcNow.AddDays(-3), Amount = 300m, Method = "Credit Card", Notes = "Misc" }
            };
        }

        public static List<Expense> GetSampleExpenses()
        {
            var vendors = GetSampleVendors();
            return new List<Expense>
            {
                new Expense { Date = DateTime.UtcNow.AddDays(-27), Description = "Equipment purchase", Vendor = vendors[0], Category = "Capital", Amount = 2000m },
                new Expense { Date = DateTime.UtcNow.AddDays(-21), Description = "Office supplies", Vendor = vendors[1], Category = "Office", Amount = 120m },
                new Expense { Date = DateTime.UtcNow.AddDays(-12), Description = "Cleaning service", Vendor = vendors[3], Category = "Operations", Amount = 300m },
                new Expense { Date = DateTime.UtcNow.AddDays(-8), Description = "Utility bill", Vendor = vendors[4], Category = "Utilities", Amount = 450m },
                new Expense { Date = DateTime.UtcNow.AddDays(-4), Description = "Rental charge", Vendor = vendors[2], Category = "Rent", Amount = 700m }
            };
        }

        public static List<Transaction> GetSampleTransactions()
        {
            return new List<Transaction>
            {
                new Transaction { Date = DateTime.UtcNow.AddDays(-30), Description = "Membership payments", Category = "Membership", Amount = 500m, IsExpense = false },
                new Transaction { Date = DateTime.UtcNow.AddDays(-21), Description = "Equipment purchase", Category = "Capital", Amount = 2000m, IsExpense = true },
                new Transaction { Date = DateTime.UtcNow.AddDays(-12), Description = "Office supplies", Category = "Office", Amount = 120m, IsExpense = true },
                new Transaction { Date = DateTime.UtcNow.AddDays(-7), Description = "Training income", Category = "Services", Amount = 800m, IsExpense = false },
                new Transaction { Date = DateTime.UtcNow.AddDays(-2), Description = "Event income", Category = "Events", Amount = 1500m, IsExpense = false }
            };
        }
    }
}
