using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Account
    {
        public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Type { get; set; }

        public decimal Balance { get; set; }
    }
}
