using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public class GetSaleValidator : AbstractValidator<GetSaleCommand>
{
    public GetSaleValidator()
    {
        RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("Sale ID is required.");
    }
}
