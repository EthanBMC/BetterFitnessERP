using System;
using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Employee
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name must be at most 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name must be at most 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(100)]
        public string Department { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        [Range(0, 10_000_000, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }
    }
}