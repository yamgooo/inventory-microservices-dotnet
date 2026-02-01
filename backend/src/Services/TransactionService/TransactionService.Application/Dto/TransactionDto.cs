using TransactionService.Domain.Enums;

namespace TransactionService.Application.Dto;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;  
    public int CurrentStock { get; set; }           
    public TransactionType Type { get; set; }
    public string TypeDescription => Type == TransactionType.Purchase ? "Compra" : "Venta";
    public int Quantity { get; set; }
    public int StockAfterTransaction { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Details { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}