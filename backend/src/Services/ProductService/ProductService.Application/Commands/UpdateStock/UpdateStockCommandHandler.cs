using System.Runtime.Intrinsics.X86;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.Dto;
using ProductService.Application.Mappings;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;

namespace ProductService.Application.Commands.UpdateStock;

public class UpdateStockCommandHandler(IProductRepository repository, ILogger<UpdateStockCommandHandler> logger)
    : IRequestHandler<UpdateStockCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Starting update stock {request.Quantity} to {request.Id}");

        var exist = await repository.TableAsNoTracking.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);

        if (exist is null)
        {
            logger.LogError("Product with ID {ProductId} is not registered!", request.Id);
            return Result<ProductDto>.Failure($"Product with ID {request.Id} not exist!");
        }

        var currentStock = exist.Stock;
        var newStock = exist.Stock + request.Quantity;
        
        if (newStock < 0 ) 
        {
            logger.LogError("The new stock cannot be set negative");
            return Result<ProductDto>.Failure("The new stock cannot be set negative");
        }
        
        exist.Stock = newStock;
        exist.UpdatedAt = DateTime.Now;
        
        await repository.UpdateAsync(exist, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Stock updated successfully to product {ProductId}!", request.Id);

        return Result<ProductDto>.Success(exist.ToDto());
    }
}