namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// Request model for cancelling a single sale item
/// </summary>
public class CancelSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}
