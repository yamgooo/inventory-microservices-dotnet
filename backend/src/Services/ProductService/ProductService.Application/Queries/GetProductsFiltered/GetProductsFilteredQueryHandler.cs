// -------------------------------------------------------------
// Get Products Filtered Query Handler
// Author: Erik Portilla
// Created: 2026-01-31
// Description: Aplica filtros dinámicos usando IQueryable del repositorio
// -------------------------------------------------------------

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs;
using ProductService.Application.Mappings;
using ProductService.Domain.Interfaces;
using Shared.Common.Models;
using Shared.Infrastructure.Pagination;

namespace ProductService.Application.Queries.GetProductsFiltered;

public class GetProductsFilteredQueryHandler 
    : IRequestHandler<GetProductsFilteredQuery, Result<PagedResult<ProductDto>>>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<GetProductsFilteredQueryHandler> _logger;

    public GetProductsFilteredQueryHandler(
        IProductRepository repository,
        ILogger<GetProductsFilteredQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<ProductDto>>> Handle(
        GetProductsFilteredQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Filtering products with criteria");

            var query = _repository.TableAsNoTracking
                .Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(p => p.Name.Contains(request.Name));

            if (!string.IsNullOrWhiteSpace(request.Category))
                query = query.Where(p => p.Category == request.Category);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            if (request.MinStock.HasValue)
                query = query.Where(p => p.Stock >= request.MinStock.Value);

            if (request.MaxStock.HasValue)
                query = query.Where(p => p.Stock <= request.MaxStock.Value);

            query = query.OrderBy(p => p.Name);

            var pagedResult = await query.ToPagedResultAsync(
                request.Page, 
                request.PageSize, 
                cancellationToken);

            var dtos = pagedResult.Items.Select(p => p.ToDto()).ToList();

            var result = new PagedResult<ProductDto>
            {
                Items = dtos,
                TotalCount = pagedResult.TotalCount,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };

            return Result<PagedResult<ProductDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error filtering products");
            return Result<PagedResult<ProductDto>>.Failure(
                "An error occurred while filtering products");
        }
    }
}