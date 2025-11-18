using System;
using System.ComponentModel.DataAnnotations;

namespace BetterFitnessERP.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Category { get; set; }

        public decimal Amount { get; set; }

        // True = Expense, False = Income
        public bool IsExpense { get; set; }
    }
}
