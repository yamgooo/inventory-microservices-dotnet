using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Common.Models;
using TransactionService.Application.Dto;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.HttpClients;

namespace TransactionService.Application.Queries.GetTransactionsFiltered;

public class GetTransactionsFilteredQueryHandler(
    ITransactionRepository repository,
    IProductServiceClient productServiceClient,
    ILogger<GetTransactionsFilteredQueryHandler> logger)
    : IRequestHandler<GetTransactionsFilteredQuery, Result<PagedResult<TransactionDto>>>
{
    public async Task<Result<PagedResult<TransactionDto>>> Handle(
        GetTransactionsFilteredQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting transaction filtered search process...");

            var query = repository.TableAsNoTracking;  

            if (request.ProductId.HasValue)
            {
                logger.LogInformation("Filtering by ProductId: {ProductId}", request.ProductId);
                query = query.Where(t => t.ProductId == request.ProductId.Value);
            }

            if (request.TransactionType.HasValue)
            {
                logger.LogInformation("Filtering by TransactionType: {Type}", request.TransactionType);
                query = query.Where(t => t.Type == request.TransactionType.Value);
            }

            if (request.StartDate.HasValue)
            {
                logger.LogInformation("Filtering by StartDate: {StartDate}", request.StartDate);
                query = query.Where(t => t.TransactionDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                logger.LogInformation("Filtering by EndDate: {EndDate}", request.EndDate);
                var endOfDay = request.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.TransactionDate <= endOfDay);
            }

            if (request.MinAmount.HasValue)
            {
                logger.LogInformation("Filtering by MinAmount: {MinAmount}", request.MinAmount);
                query = query.Where(t => t.TotalPrice >= request.MinAmount.Value);
            }

            if (request.MaxAmount.HasValue)
            {
                logger.LogInformation("Filtering by MaxAmount: {MaxAmount}", request.MaxAmount);
                query = query.Where(t => t.TotalPrice <= request.MaxAmount.Value);
            }

            query = query.OrderByDescending(t => t.TransactionDate);

            var totalCount = await query.CountAsync(cancellationToken);

            logger.LogInformation(
                "Found {TotalCount} transactions matching filters", 
                totalCount);

            var transactions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            
            var productIds = transactions.Select(p => p.ProductId).Distinct();
            
            var productsDict = await productServiceClient.GetProductsByIdsAsync(
                productIds, 
                cancellationToken);

            var dtos = transactions.Select(transaction =>
            {
                var hasProduct = productsDict.TryGetValue(
                    transaction.ProductId, 
                    out var product);

                return new TransactionDto
                {
                    Id = transaction.Id,
                    ProductId = transaction.ProductId,
                    Type = transaction.Type,
                    Quantity = transaction.Quantity,
                    UnitPrice = transaction.UnitPrice,
                    TotalPrice = transaction.TotalPrice,
                    Details = transaction.Details,
                    TransactionDate = transaction.TransactionDate,
                    CreatedAt = transaction.CreatedAt,
                    StockAfterTransaction = transaction.StockAfterTransaction,
                    ProductName = product?.Name ?? "Producto no disponible",
                    CurrentStock = product?.Stock ?? 0  
                };
            }).ToList();

            var pagedResult = new PagedResult<TransactionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };

            logger.LogInformation(
                "Returning page {Page}/{TotalPages} with {ItemCount} transactions",
                request.Page,
                pagedResult.TotalPages,
                dtos.Count);

            return Result<PagedResult<TransactionDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error filtering transactions");
            return Result<PagedResult<TransactionDto>>.Failure(
                "An error occurred while filtering transactions");
        }
    }
}