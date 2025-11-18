using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string? Name { get; set; }

        public string? ContactInfo { get; set; }
    }
}
