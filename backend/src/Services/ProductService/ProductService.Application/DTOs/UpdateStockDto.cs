namespace ProductService.Application.DTOs;

public class UpdateStockDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}