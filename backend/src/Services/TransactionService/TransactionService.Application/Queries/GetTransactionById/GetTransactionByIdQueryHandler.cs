using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.HttpClients;

namespace TransactionService.Application.Queries.GetTransactionById;

public record GetTransactionByIdQueryHandler(
    ITransactionRepository repository,
    IProductServiceClient productClient,
    ILogger<GetTransactionByIdQueryHandler> logger)
    : IRequestHandler<GetTransactionByIdQuery, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Search product with Id {ProductId}", request.Id);
            
            if (request.Id == Guid.Empty)
            {
                logger.LogError("ProductId is empty");
                return Result<TransactionDto>.Failure($"ProductId is required");
            }
            
            var transaction = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (transaction is null)
            {
                logger.LogError("Product with Id {ProductId} not found!", request.Id);
                return Result<TransactionDto>.Failure($"Product with Id {request.Id} not found!");
            }

            var product = await productClient.GetProductAsync(
                transaction.ProductId,
                cancellationToken);

            var dto = new TransactionDto
            {
                Id = transaction.Id,
                ProductId = transaction.ProductId,
                ProductName = product?.Name ?? "Product not available",
                CurrentStock = product?.Stock ?? 0,
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
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting transaction {Id}", request.Id);
            return Result<TransactionDto>.Failure(
                "An error occurred while retrieving the transaction");
        }
    }
}