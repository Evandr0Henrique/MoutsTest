using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command for cancelling a sale
/// </summary>
public record CancelSaleCommand : IRequest<CancelSaleResponse>
{
    public Guid Id { get; }

    public CancelSaleCommand(Guid id)
    {
        Id = id;
    }
}
