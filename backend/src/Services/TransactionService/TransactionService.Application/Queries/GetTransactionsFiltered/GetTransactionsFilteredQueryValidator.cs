using FluentValidation;

namespace TransactionService.Application.Queries.GetTransactionsFiltered;

public class GetTransactionsFilteredQueryValidator : AbstractValidator<GetTransactionsFilteredQuery>
{

    public GetTransactionsFilteredQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");

        RuleFor(x => x.TransactionType)
            .IsInEnum()
            .When(x => x.TransactionType.HasValue)
            .WithMessage("Transaction type must be Sale (2) or Purchase (1)");

        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate ?? DateTime.MaxValue)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before or equal to end date");

        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinAmount.HasValue)
            .WithMessage("Minimum amount cannot be negative");

        RuleFor(x => x.MaxAmount)
            .GreaterThanOrEqualTo(x => x.MinAmount ?? 0)
            .When(x => x.MaxAmount.HasValue && x.MinAmount.HasValue)
            .WithMessage("Maximum amount must be greater than or equal to minimum amount");
    }
    
}