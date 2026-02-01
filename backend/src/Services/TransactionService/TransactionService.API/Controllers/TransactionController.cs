using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models;
using TransactionService.Application.Commands.CreateTransaction;
using TransactionService.Application.Dto;
using TransactionService.Application.Queries.GetTransactionById;
using TransactionService.Application.Queries.GetTransactionsFiltered;

namespace TransactionService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionController(IMediator mediator, ILogger<TransactionController> logger) : ControllerBase
{
    [HttpPost("filtered")]
    public async Task<IActionResult> GetFiltered([FromBody] GetTransactionsFilteredQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failed(result.Error));

        return Ok(ApiResponse<PagedResult<TransactionDto>>.Succeeded(result.Data));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        
        if (!result.IsSuccess)
            return BadRequest(ApiResponse.Failed(result.Error));

        return Ok(ApiResponse<TransactionDto>.Succeeded(result.Data));
    }
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var query = new GetTransactionByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(ApiResponse.Failed(result.Error));

        return Ok(ApiResponse<TransactionDto>.Succeeded(result.Data));
    }
}