using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEqual(Guid.Empty).WithMessage("Product is required.");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(SaleItem.MaxQuantity)
            .WithMessage($"It's not possible to sell above {SaleItem.MaxQuantity} identical items.");

        RuleFor(item => item.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
    }
}
