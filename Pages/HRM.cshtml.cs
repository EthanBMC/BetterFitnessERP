using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using BetterFitnessERP.Models;
using BetterFitnessERP.Services;

namespace BetterFitnessERP.Pages
{
    public class HRMModel : PageModel
    {
        private readonly IEmployeeRepository _repo;

        public HRMModel(IEmployeeRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<string> Departments { get; set; } = new List<string>();

        public int TotalEmployees { get; set; }
        public int DeptCount { get; set; }
        public decimal AvgSalary { get; set; }
        public decimal MonthlyPayroll { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchDept { get; set; }

        [BindProperty]
        public Employee InputEmployee { get; set; }

        public void OnGet()
        {
            Departments = _repo.GetDepartments().ToList();

            var query = string.IsNullOrWhiteSpace(SearchDept) || SearchDept == "All"
                ? _repo.GetAll()
                : _repo.GetByDepartment(SearchDept);

            Employees = query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();

            ComputeMetrics();
        }

        public IActionResult OnPost()
        {
            Departments = _repo.GetDepartments().ToList();

            if (!ModelState.IsValid)
            {
                var query = string.IsNullOrWhiteSpace(SearchDept) || SearchDept == "All"
                    ? _repo.GetAll()
                    : _repo.GetByDepartment(SearchDept);

                Employees = query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();
                ComputeMetrics();
                return Page();
            }

            _repo.Add(InputEmployee);

            return RedirectToPage(new { SearchDept = this.SearchDept });
        }

        private void ComputeMetrics()
        {
            TotalEmployees = Employees.Count;
            DeptCount = Employees.Select(e => e.Department).Where(d => !string.IsNullOrWhiteSpace(d)).Distinct().Count();
            var totalSalary = Employees.Sum(e => (decimal?)e.Salary) ?? 0m;
            AvgSalary = TotalEmployees > 0 ? Math.Round(totalSalary / TotalEmployees, 2) : 0m;
            MonthlyPayroll = Math.Round(totalSalary / 12m, 2);
        }
    }
}
