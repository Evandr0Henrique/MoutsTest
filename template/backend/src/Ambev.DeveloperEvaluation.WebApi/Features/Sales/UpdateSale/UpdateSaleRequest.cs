using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents a request to update an existing sale.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to update (bound from route).
    /// </summary>
    public Guid Id { get; set; }

    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public List<SaleItemRequest> Items { get; set; } = new();
}
