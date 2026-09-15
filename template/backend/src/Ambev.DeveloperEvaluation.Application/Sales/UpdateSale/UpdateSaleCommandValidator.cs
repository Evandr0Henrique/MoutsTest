using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(sale => sale.Id).NotEqual(Guid.Empty).WithMessage("Sale ID is required.");

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

        RuleForEach(sale => sale.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEqual(Guid.Empty).WithMessage("Product is required.");
            item.RuleFor(i => i.ProductName).NotEmpty().WithMessage("Product name is required.");
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
                .LessThanOrEqualTo(20).WithMessage("It's not possible to sell above 20 identical items.");
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative.");
        });
    }
}
