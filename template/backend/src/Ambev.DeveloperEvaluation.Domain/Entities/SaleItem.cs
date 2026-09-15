using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item (product line) belonging to a sale.
/// Uses the External Identity pattern to reference the Product with a denormalized description.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets the foreign key to the parent Sale.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets the external identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets the denormalized product description/name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets the discount percentage applied to this item (e.g. 0.10 for 10%).
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets the total amount for this item after discount (Quantity * UnitPrice * (1 - DiscountPercentage)).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets whether this item has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Maximum quantity allowed for a single product in a sale.
    /// </summary>
    public const int MaxQuantity = 20;

    /// <summary>
    /// Minimum quantity required to be eligible for any discount.
    /// </summary>
    public const int MinQuantityForDiscount = 4;

    /// <summary>
    /// Minimum quantity required for the higher discount tier.
    /// </summary>
    public const int MinQuantityForHigherDiscount = 10;

    public SaleItem()
    {
    }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        SetQuantityAndPrice(quantity, unitPrice);
    }

    /// <summary>
    /// Sets the quantity and unit price, recalculating the discount and total amount
    /// according to the business rules.
    /// </summary>
    public void SetQuantityAndPrice(int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > MaxQuantity)
            throw new DomainException($"It's not possible to sell above {MaxQuantity} identical items.");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative.");

        Quantity = quantity;
        UnitPrice = unitPrice;
        DiscountPercentage = CalculateDiscountPercentage(quantity);
        TotalAmount = CalculateTotalAmount();
    }

    /// <summary>
    /// Calculates the applicable discount percentage based on the quantity business rules:
    /// - Below 4 items: no discount
    /// - 4 to 9 items: 10% discount
    /// - 10 to 20 items: 20% discount
    /// - Above 20 items: not allowed
    /// </summary>
    public static decimal CalculateDiscountPercentage(int quantity)
    {
        if (quantity > MaxQuantity)
            throw new DomainException($"It's not possible to sell above {MaxQuantity} identical items.");

        if (quantity >= MinQuantityForHigherDiscount)
            return 0.20m;

        if (quantity >= MinQuantityForDiscount)
            return 0.10m;

        return 0m;
    }

    private decimal CalculateTotalAmount()
    {
        var grossAmount = Quantity * UnitPrice;
        var discountAmount = grossAmount * DiscountPercentage;
        return grossAmount - discountAmount;
    }

    /// <summary>
    /// Cancels this sale item.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
    }
}
