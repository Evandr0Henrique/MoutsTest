using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesCommandValidator : AbstractValidator<GetSalesCommand>
{
    public GetSalesCommandValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page must be greater than zero.");
        RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("PageSize must be greater than zero.");
    }
}
