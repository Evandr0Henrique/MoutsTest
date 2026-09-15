namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Represents a product item within a sale, used across Create/Update sale commands and results.
/// </summary>
public class SaleItemDto
{
    /// <summary>
    /// Gets or sets the identifier of an existing sale item (used for updates).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets the external identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }
}
