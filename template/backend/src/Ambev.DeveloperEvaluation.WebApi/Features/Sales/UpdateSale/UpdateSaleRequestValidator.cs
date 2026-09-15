using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(sale => sale.Id).NotEqual(Guid.Empty);
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
