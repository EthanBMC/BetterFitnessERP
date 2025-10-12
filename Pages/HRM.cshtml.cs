using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LoginPageExample.Pages
{
    public class Employee
    {
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public decimal Salary { get; set; }
    }

    [Authorize]
    public class HRMModel : PageModel
    {
        // Properties for section cards
        public int TotalEmployees { get; set; }
        public int TotalDepartments { get; set; }
        public decimal AverageSalary { get; set; }
        public int OpenPositions { get; set; }

        // Employee directory properties
    public List<Employee> Employees { get; set; } = new List<Employee>();
    public List<SelectListItem> DepartmentList { get; set; } = new List<SelectListItem>();
    [BindProperty(SupportsGet = true)]
    public string? SelectedDepartment { get; set; }

        public void OnGet()
        {
            // Initialize sample data
            InitializeSampleData();

            // Filter employees if department is selected
            if (!string.IsNullOrEmpty(SelectedDepartment))
            {
                Employees = Employees.Where(e => e.Department == SelectedDepartment).ToList();
            }

            // Calculate statistics (guard against empty lists)
            TotalEmployees = Employees.Count;
            TotalDepartments = Employees.Select(e => e.Department).Where(d => !string.IsNullOrEmpty(d)).Distinct().Count();
            AverageSalary = Employees.Any() ? Employees.Average(e => e.Salary) : 0m;
            OpenPositions = 5; // Hardcoded sample value
        }

        private void InitializeSampleData()
        {
            // Initialize sample employees
            Employees = new List<Employee>
            {
                new Employee { Name = "John Doe", Department = "IT", Role = "Software Developer", Address = "123 Main St", Phone = "555-0101", Salary = 85000 },
                new Employee { Name = "Jane Smith", Department = "HR", Role = "HR Manager", Address = "456 Oak Ave", Phone = "555-0102", Salary = 75000 },
                new Employee { Name = "Bob Wilson", Department = "Finance", Role = "Accountant", Address = "789 Pine Rd", Phone = "555-0103", Salary = 65000 },
                new Employee { Name = "Alice Brown", Department = "IT", Role = "System Admin", Address = "321 Elm St", Phone = "555-0104", Salary = 70000 },
                new Employee { Name = "Charlie Davis", Department = "Marketing", Role = "Marketing Manager", Address = "654 Maple Dr", Phone = "555-0105", Salary = 72000 }
            };

            // Initialize department list for dropdown
            DepartmentList = Employees
                .Select(e => e.Department)
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct()
                .Select(d => new SelectListItem { Value = d!, Text = d! })
                .ToList();

            // Prepend an All Departments option
            DepartmentList.Insert(0, new SelectListItem { Value = "", Text = "All Departments" });
        }
    }
}