using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Commands.DeleteProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.DTOs;
using ProductService.Application.Queries.GetProductById;
using ProductService.Application.Queries.GetProducts;
using ProductService.Application.Queries.GetProductsFiltered;
using Shared.Common.Models;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController(IMediator mediator, ILogger<ProductsController> logger) : ControllerBase
{
    private readonly ILogger<ProductsController> _logger = logger;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failed(result.Error));

        return Ok(ApiResponse<PagedResult<ProductDto>>.Succeeded(result.Data));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.Failed(result.Error));

        return Ok(ApiResponse<ProductDto>.Succeeded(result.Data));
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFiltered(
        [FromBody] GetProductsFilteredQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failed(result.Error));

        return Ok(ApiResponse<PagedResult<ProductDto>>.Succeeded(result.Data));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failed(result.Errors));

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Data.Id },
            ApiResponse<ProductDto>.Succeeded(result.Data));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
            return BadRequest(ApiResponse.Failed("ID mismatch"));

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess) return Ok(ApiResponse<ProductDto>.Succeeded(result.Data));
        if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(ApiResponse.Failed(result.Error));

        return BadRequest(ApiResponse.Failed(result.Errors));

    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteProductCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.Failed(result.Error));

        return NoContent();
    }
}