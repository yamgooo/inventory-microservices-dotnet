
using TransactionService.Domain.Enums;

namespace TransactionService.Domain.Entities;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public TransactionType Type { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }

    public string Details { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime CreatedAt { get; set; }
}