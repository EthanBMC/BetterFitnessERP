using System;
using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public int? VendorId { get; set; }
        public Vendor? Vendor { get; set; }

        public string? Category { get; set; }

        public decimal Amount { get; set; }
    }
}
