using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Enums;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.HttpClients;

namespace TransactionService.Application.Commands.UpdateTransaction;

public class UpdateTransactionCommandHandler(ITransactionRepository repository, IProductServiceClient productClient) : IRequestHandler<UpdateTransactionCommand, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await repository.GetByIdAsync(request.Id);
        if (transaction == null) return Result<TransactionDto>.Failure("Transaction not found");

        var product = await productClient.GetProductAsync(transaction.ProductId, cancellationToken);
        
        int stockDifference = request.Quantity - transaction.Quantity;
        int stockChange = transaction.Type == TransactionType.Sale ? -stockDifference : stockDifference;
        
        if (transaction.Type == TransactionType.Sale && product.Stock < stockChange * -1)
            return Result<TransactionDto>.Failure("Insufficient stock");

        transaction.Quantity = request.Quantity;
        transaction.UnitPrice = request.UnitPrice;
        transaction.TotalPrice = request.Quantity * request.UnitPrice;
        transaction.Details = request.Details ?? transaction.Details;

        await repository.UpdateAsync(transaction, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        // await productClient.UpdateStockAsync(transaction.ProductId, stockChange, cancellationToken);
        
        var dto = new TransactionDto
        {
            Id = transaction.Id,
            ProductId = transaction.ProductId,
            ProductName = product.Name,
            CurrentStock = product.Stock, 
            Type = transaction.Type,
            Quantity = transaction.Quantity,
            UnitPrice = transaction.UnitPrice,
            TotalPrice = transaction.TotalPrice,
            Details = transaction.Details,
            TransactionDate = transaction.TransactionDate,
            CreatedAt = transaction.CreatedAt,
            StockAfterTransaction = transaction.StockAfterTransaction
        };

        return Result<TransactionDto>.Success(dto);
    }
}