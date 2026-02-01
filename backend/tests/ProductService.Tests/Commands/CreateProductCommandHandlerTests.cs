using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repositories;
using ProductService.Tests.Helpers;

namespace ProductService.Tests.Commands;

public class CreateProductCommandHandlerTests: IDisposable
{
    private readonly ProductContext _context;
    private readonly ProductRepository _repository;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _repository = new ProductRepository(_context);
        Mock<ILogger<CreateProductCommandHandler>> loggerMock = new();
        _handler = new CreateProductCommandHandler(_repository, loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidProduct_ShouldCreateSuccessfully()
    {
        var command = new CreateProductCommand(
            Name: "Laptop Dell XPS 15",
            Description: "High-performance laptop",
            Category: "Electronics",
            ImageUrl: "https://example.com/laptop.jpg",
            Price: 1500.00m,
            Stock: 10
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Laptop Dell XPS 15");
        result.Data.Price.Should().Be(1500.00m);
        result.Data.Stock.Should().Be(10);
        result.Data.Id.Should().NotBeEmpty();

        var productInDb = await _repository.GetByIdAsync(result.Data.Id);
        productInDb.Should().NotBeNull();
        productInDb!.Name.Should().Be("Laptop Dell XPS 15");
    }

    public void Dispose()
    {
        TestDbContextFactory.Destroy(_context);
    }
}