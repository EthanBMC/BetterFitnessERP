using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using BetterFitnessERP.Data;
using BetterFitnessERP.Models;

namespace BetterFitnessERP.Pages
{
    // Restrict access to users in the "Customer" role only
    [Authorize(Roles = "Customer")]
    public class CRMModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CRMModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // List view
        public IList<Customer> Customers { get; set; } = new List<Customer>();

        // Bound property for Create/Edit/Details
        [BindProperty]
        public Customer Customer { get; set; } = new Customer();

        [TempData]
        public string StatusMessage { get; set; }

        // GET: /CRM
        public async Task<IActionResult> OnGetAsync()
        {
            Customers = await _context.Customers.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
            return Page();
        }

        // GET: /CRM?handler=Details&id=5
        public async Task<IActionResult> OnGetDetailsAsync(int? id)
        {
            if (id == null) return NotFound();

            Customer = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id.Value);
            if (Customer == null) return NotFound();

            return Page();
        }

        // GET: /CRM?handler=Create
        public IActionResult OnGetCreate()
        {
            return Page();
        }

        // POST: /CRM?handler=Create
        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            StatusMessage = "Customer created successfully.";
            return RedirectToPage();
        }

        // GET: /CRM?handler=Edit&id=5
        public async Task<IActionResult> OnGetEditAsync(int? id)
        {
            if (id == null) return NotFound();

            Customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id.Value);
            if (Customer == null) return NotFound();

            return Page();
        }

        // POST: /CRM?handler=Edit
        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!CustomerExists(Customer.Id))
            {
                return NotFound();
            }

            try
            {
                _context.Attach(Customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                StatusMessage = "Customer updated successfully.";
                return RedirectToPage();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(Customer.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // POST: /CRM?handler=Delete&id=5
        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _context.Customers.FindAsync(id.Value);
            if (customer == null) return NotFound();

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            StatusMessage = "Customer deleted successfully.";
            return RedirectToPage();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
