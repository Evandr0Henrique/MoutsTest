using System;
using System.Linq;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover item management, total calculation and cancellation behavior.
/// </summary>
public class SaleTests
{
    private static Sale CreateValidSale()
    {
        return new Sale
        {
            SaleNumber = "SALE-0001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "John Doe",
            BranchId = Guid.NewGuid(),
            BranchName = "Main Branch"
        };
    }

    [Fact(DisplayName = "Adding an item should recalculate the sale total amount")]
    public void Given_Sale_When_ItemAdded_Then_TotalAmountIsRecalculated()
    {
        // Arrange
        var sale = CreateValidSale();

        // Act
        sale.AddItem(Guid.NewGuid(), "Product A", 4, 10m); // 10% discount -> 36
        sale.AddItem(Guid.NewGuid(), "Product B", 1, 20m); // no discount -> 20

        // Assert
        Assert.Equal(56m, sale.TotalAmount);
    }

    [Fact(DisplayName = "Adding a duplicate product should throw a domain exception")]
    public void Given_Sale_When_DuplicateProductAdded_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = CreateValidSale();
        var productId = Guid.NewGuid();
        sale.AddItem(productId, "Product A", 2, 10m);

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(productId, "Product A", 3, 10m));
    }

    [Fact(DisplayName = "Cancelling a sale should cancel the sale and all of its items")]
    public void Given_Sale_When_Cancelled_Then_SaleAndItemsAreCancelled()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(Guid.NewGuid(), "Product A", 2, 10m);
        sale.AddItem(Guid.NewGuid(), "Product B", 5, 10m);

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
        Assert.All(sale.Items, item => Assert.True(item.IsCancelled));
        Assert.Equal(0m, sale.TotalAmount);
    }

    [Fact(DisplayName = "Cancelling a single item should exclude it from the total amount")]
    public void Given_Sale_When_ItemCancelled_Then_TotalAmountExcludesIt()
    {
        // Arrange
        var sale = CreateValidSale();
        var item = sale.AddItem(Guid.NewGuid(), "Product A", 2, 10m); // 20
        sale.AddItem(Guid.NewGuid(), "Product B", 1, 30m); // 30

        // Act
        sale.CancelItem(item.Id);

        // Assert
        Assert.True(sale.Items.First(i => i.Id == item.Id).IsCancelled);
        Assert.Equal(30m, sale.TotalAmount);
    }

    [Fact(DisplayName = "Adding an item to a cancelled sale should throw a domain exception")]
    public void Given_CancelledSale_When_ItemAdded_Then_ThrowsDomainException()
    {
        // Arrange
        var sale = CreateValidSale();
        sale.AddItem(Guid.NewGuid(), "Product A", 2, 10m);
        sale.Cancel();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(Guid.NewGuid(), "Product B", 1, 10m));
    }
}
