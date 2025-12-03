using System;
using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
