using FluentValidation;

namespace ProductService.Application.Commands.UpdateStock;

public class UpdateStockCommandValidator : AbstractValidator<UpdateStockCommand>
{
    public UpdateStockCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(-10000).WithMessage("Quantity cannot be less -10,000")
            .NotEqual(0).WithMessage("Quantity cannot be 0")
            .LessThan(10000).WithMessage("Quantity must be less than 10,000");
    }
}