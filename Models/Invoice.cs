using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public decimal Total { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public List<InvoiceItem>? Items { get; set; }
    }
}
