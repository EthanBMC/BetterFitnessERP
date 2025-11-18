using System;

namespace BetterFitnessERP.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int? InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
        public string? Method { get; set; }
        public string? Notes { get; set; }
    }
}
