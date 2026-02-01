using FluentValidation;

namespace TransactionService.Application.Commands.CreateTransaction;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is requires");
        
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Transaction type must be Purchase or Sale");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0")
            .LessThan(10000).WithMessage("Quantity must be less than 10,000");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative")
            .LessThan(1000000).WithMessage("Unit price must be less than 1,000,000");

        RuleFor(x => x.Details)
            .MaximumLength(500).WithMessage("Details cannot exceed 500 characters");
    }
    
}