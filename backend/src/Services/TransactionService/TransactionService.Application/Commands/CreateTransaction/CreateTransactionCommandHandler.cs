using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Application.Services;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Enums;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Application.Commands.CreateTransaction;

public class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IProductServiceClient productServiceClient,
    ILogger<CreateTransactionCommandHandler> logger)
    : IRequestHandler<CreateTransactionCommand, Result<TransactionDto>>
{
    public async Task<Result<TransactionDto>> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(
                "Creating {TransactionType} transaction for product {ProductId}, quantity: {Quantity}",
                request.Type,
                request.ProductId,
                request.Quantity);

            var product = await productServiceClient.GetProductAsync(
                request.ProductId, 
                cancellationToken);

            if (product == null)
            {
                logger.LogWarning("Product {ProductId} not found", request.ProductId);
                return Result<TransactionDto>.Failure(
                    $"Product with ID {request.ProductId} not found");
            }

            if (request.Type == TransactionType.Sale)
            {
                if (product.Stock < request.Quantity)
                {
                    logger.LogWarning(
                        "Insufficient stock for product {ProductId}. Available: {Stock}, Requested: {Quantity}",
                        request.ProductId,
                        product.Stock,
                        request.Quantity);

                    return Result<TransactionDto>.Failure(
                        $"Insufficient stock. Available: {product.Stock}, Requested: {request.Quantity}");
                }
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Type = request.Type,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                TotalPrice = request.Quantity * request.UnitPrice,
                Details = request.Details,
                TransactionDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await transactionRepository.AddAsync(transaction, cancellationToken);
            await transactionRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Transaction {TransactionId} created successfully",
                transaction.Id);

            var stockChange = request.Type == TransactionType.Sale 
                ? -request.Quantity  
                : +request.Quantity;

            var stockUpdated = await productServiceClient.UpdateStockAsync(
                request.ProductId,
                stockChange,
                cancellationToken);

            if (!stockUpdated)
            {
                logger.LogError(
                    "Failed to update stock in ProductService for transaction {TransactionId}",
                    transaction.Id);
            }
            
            var dto = new TransactionDto
            {
                Id = transaction.Id,
                ProductId = transaction.ProductId,
                ProductName = product.Name,
                CurrentStock = product.Stock + stockChange, 
                Type = TransactionType.Sale,
                Quantity = transaction.Quantity,
                UnitPrice = transaction.UnitPrice,
                TotalPrice = transaction.TotalPrice,
                Details = transaction.Details,
                TransactionDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt
            };

            logger.LogInformation(
                "Transaction {TransactionId} completed successfully. New stock for product {ProductId}: {NewStock}",
                transaction.Id,
                request.ProductId,
                dto.CurrentStock);

            return Result<TransactionDto>.Success(dto);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Service communication error while creating transaction");
            return Result<TransactionDto>.Failure(
                "Unable to communicate with ProductService. Please try again later.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating transaction for product {ProductId}", request.ProductId);
            return Result<TransactionDto>.Failure(
                "An error occurred while creating the transaction");
        }
    }
}