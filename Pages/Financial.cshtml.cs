using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BetterFitnessERP.Data;
using BetterFitnessERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BetterFitnessERP.Pages
{
    public class FinancialModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public FinancialModel(ApplicationDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public decimal TotalRevenue { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance { get; set; }
    // Additional overview metrics
    public int AccountsCount { get; set; }
    public int CustomersCount { get; set; }
    public int VendorsCount { get; set; }
    public int PartnersCount { get; set; }
    public int InvoicesCount { get; set; }
    public decimal OpenAR { get; set; }
    public decimal OpenAP { get; set; }
    public int PaymentsCount { get; set; }
    public int JournalLinesCount { get; set; }
    public int TaxRatesCount { get; set; } = 3; // static sample

        [BindProperty(SupportsGet = true)]
        public string FilterCategory { get; set; } = "All";

        [BindProperty(SupportsGet = true)]
        public string FilterType { get; set; } = "All";

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [BindProperty]
        public InputTransactionModel InputTransaction { get; set; } = new InputTransactionModel();

        public List<string> Categories { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            await LoadAndComputeAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCategoriesAsync();

            if (!ModelState.IsValid)
            {
                await LoadAndComputeAsync();
                return Page();
            }

            var transaction = new Transaction
            {
                Date = InputTransaction.Date ?? DateTime.UtcNow,
                Description = InputTransaction.Description?.Trim(),
                Category = InputTransaction.Category?.Trim() ?? "Uncategorized",
                Amount = InputTransaction.Amount ?? 0m,
                IsExpense = InputTransaction.IsExpense
            };

            _db.Transactions.Add(transaction);
            await _db.SaveChangesAsync();

            var routeValues = new
            {
                FilterCategory = string.IsNullOrWhiteSpace(FilterCategory) ? "All" : FilterCategory,
                FilterType = string.IsNullOrWhiteSpace(FilterType) ? "All" : FilterType,
                StartDate = StartDate?.ToString("yyyy-MM-dd"),
                EndDate = EndDate?.ToString("yyyy-MM-dd")
            };

            return RedirectToPage("./Financial", routeValues);
        }

        private async Task LoadAndComputeAsync()
        {
            await LoadCategoriesAsync();

            var query = _db.Transactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(FilterCategory) && FilterCategory != "All")
            {
                query = query.Where(t => t.Category == FilterCategory);
            }

            if (!string.IsNullOrWhiteSpace(FilterType) && FilterType != "All")
            {
                if (FilterType.Equals("Income", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(t => !t.IsExpense);
                else if (FilterType.Equals("Expense", StringComparison.OrdinalIgnoreCase))
                    query = query.Where(t => t.IsExpense);
            }

            if (StartDate.HasValue)
            {
                var sd = StartDate.Value.Date;
                query = query.Where(t => t.Date.Date >= sd);
            }

            if (EndDate.HasValue)
            {
                var ed = EndDate.Value.Date;
                query = query.Where(t => t.Date.Date <= ed);
            }

            Transactions = await query.OrderByDescending(t => t.Date).ThenBy(t => t.Description).ToListAsync();

            TotalRevenue = Transactions.Where(t => !t.IsExpense).Sum(t => t.Amount);
            TotalExpense = Transactions.Where(t => t.IsExpense).Sum(t => t.Amount);
            NetBalance = TotalRevenue - TotalExpense;

            // Load counts for overview cards
            AccountsCount = await _db.Accounts.CountAsync();
            CustomersCount = await _db.Customers.CountAsync();
            VendorsCount = await _db.Vendors.CountAsync();
            PartnersCount = CustomersCount + VendorsCount;
            InvoicesCount = await _db.Invoices.CountAsync();
            // Open AR = sum of unpaid invoices
            var unpaidTotals = await _db.Invoices
                .Where(i => i.Status == "Unpaid")
                .Select(i => i.Total)
                .ToListAsync();
            OpenAR = unpaidTotals.Any() ? unpaidTotals.Sum() : 0m;

            // Open AP = sum of expenses (compute client-side to avoid SQLite decimal Sum translation issue)
            var expenseAmounts = await _db.Expenses
                .Select(e => e.Amount)
                .ToListAsync();
            OpenAP = expenseAmounts.Any() ? expenseAmounts.Sum() : 0m;
            PaymentsCount = await _db.Payments.CountAsync();
            JournalLinesCount = await _db.Transactions.CountAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var raw = await _db.Transactions
                .Select(t => t.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
            Categories = raw.Where(c => c != null).Select(c => c!).ToList();

            if (!Categories.Contains("All"))
                Categories.Insert(0, "All");
        }

        public class InputTransactionModel
        {
            [DataType(DataType.Date)]
            public DateTime? Date { get; set; }

            [Required(ErrorMessage = "Description is required")]
            [StringLength(200)]
            public string? Description { get; set; }

            [Required(ErrorMessage = "Category is required")]
            [StringLength(100)]
            public string? Category { get; set; }

            [Required(ErrorMessage = "Amount is required")]
            [Range(0.01, 100_000_000, ErrorMessage = "Amount must be a positive number")]
            public decimal? Amount { get; set; }

            // True = Expense
            public bool IsExpense { get; set; } = true;
        }
    }
}
