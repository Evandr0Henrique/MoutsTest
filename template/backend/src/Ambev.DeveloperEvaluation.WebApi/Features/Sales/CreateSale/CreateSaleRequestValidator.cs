using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleRequest that defines validation rules for sale creation.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(sale => sale.SaleDate).NotEqual(default(DateTime));
        RuleFor(sale => sale.CustomerId).NotEqual(Guid.Empty);
        RuleFor(sale => sale.CustomerName).NotEmpty();
        RuleFor(sale => sale.BranchId).NotEqual(Guid.Empty);
        RuleFor(sale => sale.BranchName).NotEmpty();
        RuleFor(sale => sale.Items).NotEmpty();

        RuleForEach(sale => sale.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEqual(Guid.Empty);
            item.RuleFor(i => i.ProductName).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(20);
            item.RuleFor(i => i.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}
