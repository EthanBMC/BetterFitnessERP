using System;
using System.Collections.Generic;
using System.Linq;
using BetterFitnessERP.Models;

namespace BetterFitnessERP.Services
{
    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAll();
        IEnumerable<Employee> GetByDepartment(string department);
        void Add(Employee employee);
        IEnumerable<string> GetDepartments();
    }

    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees;
        private readonly List<string> _departments;

        public InMemoryEmployeeRepository()
        {
            _departments = new List<string> { "All", "Fitness", "HR", "Sales", "IT", "Operations" };
            _employees = new List<Employee>();

            var rnd = new Random();
            for (int i = 1; i <= 8; i++)
            {
                _employees.Add(new Employee
                {
                    FirstName = $"Employee {i}",
                    LastName = $"Placeholder",
                    Department = _departments[(i % (_departments.Count - 1)) + 1],
                    Email = $"employee{i}@example.com",
                    Salary = Math.Round((decimal)(30000 + rnd.NextDouble() * 70000), 2),
                    HireDate = DateTime.UtcNow.AddDays(-rnd.Next(30, 3650))
                });
            }
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList();
        }

        public IEnumerable<Employee> GetByDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department) || department == "All")
                return GetAll();

            return _employees
                .Where(e => string.Equals(e.Department, department, StringComparison.OrdinalIgnoreCase))
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ToList();
        }

        public void Add(Employee employee)
        {
            if (employee == null) throw new ArgumentNullException(nameof(employee));
            employee.Id = Guid.NewGuid();
            employee.FirstName = employee.FirstName?.Trim();
            employee.LastName = employee.LastName?.Trim();
            employee.Email = employee.Email?.Trim();
            _employees.Add(employee);
            if (!string.IsNullOrWhiteSpace(employee.Department) && !_departments.Contains(employee.Department))
                _departments.Add(employee.Department);
        }

        public IEnumerable<string> GetDepartments()
        {
            return _departments.ToList();
        }
    }
}