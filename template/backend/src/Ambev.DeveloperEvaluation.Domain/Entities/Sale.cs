using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale record in the system, following the External Identity pattern
/// to reference Customer, Branch and Products with denormalized descriptions.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the unique sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets the external identifier of the customer.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets the denormalized customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the external identifier of the branch where the sale was made.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets the denormalized branch name.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale (sum of all non-cancelled items' total amounts).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the items included in this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation of the sale entity using the SaleValidator rules.
    /// </summary>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    /// <summary>
    /// Adds an item to the sale, enforcing quantity/discount business rules,
    /// and recalculates the sale's total amount.
    /// </summary>
    public SaleItem AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
            throw new DomainException("Cannot add items to a cancelled sale.");

        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId && !i.IsCancelled);
        if (existingItem != null)
            throw new DomainException($"Product {productName} is already present in this sale. Update the existing item instead.");

        var item = new SaleItem(productId, productName, quantity, unitPrice);
        Items.Add(item);
        RecalculateTotalAmount();
        return item;
    }

    /// <summary>
    /// Recalculates the total sale amount based on non-cancelled items.
    /// </summary>
    public void RecalculateTotalAmount()
    {
        TotalAmount = Items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
    }

    /// <summary>
    /// Cancels the entire sale and all of its items.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        foreach (var item in Items)
            item.IsCancelled = true;

        RecalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels a single item within the sale by its identifier.
    /// </summary>
    public SaleItem CancelItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException("Sale item not found.");

        item.Cancel();
        RecalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
        return item;
    }
}
