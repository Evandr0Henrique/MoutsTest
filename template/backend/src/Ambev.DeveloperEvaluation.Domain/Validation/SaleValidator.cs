using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required.");

        RuleFor(sale => sale.SaleDate)
            .NotEqual(default(DateTime)).WithMessage("Sale date is required.");

        RuleFor(sale => sale.CustomerId)
            .NotEqual(Guid.Empty).WithMessage("Customer is required.");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty().WithMessage("Customer name is required.");

        RuleFor(sale => sale.BranchId)
            .NotEqual(Guid.Empty).WithMessage("Branch is required.");

        RuleFor(sale => sale.BranchName)
            .NotEmpty().WithMessage("Branch name is required.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("Sale must contain at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new SaleItemValidator());
    }
}
