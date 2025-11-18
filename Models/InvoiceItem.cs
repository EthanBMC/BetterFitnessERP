using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
