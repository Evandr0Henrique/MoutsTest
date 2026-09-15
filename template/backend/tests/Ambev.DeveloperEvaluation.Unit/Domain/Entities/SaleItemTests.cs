using System;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover the quantity-based discount business rules.
/// </summary>
public class SaleItemTests
{
    [Theory(DisplayName = "Quantity below 4 should not have any discount")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Given_QuantityBelowFour_When_Calculated_Then_NoDiscountApplied(int quantity)
    {
        // Act
        var discount = SaleItem.CalculateDiscountPercentage(quantity);

        // Assert
        Assert.Equal(0m, discount);
    }

    [Theory(DisplayName = "Quantity between 4 and 9 should have 10% discount")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void Given_QuantityBetweenFourAndNine_When_Calculated_Then_TenPercentDiscountApplied(int quantity)
    {
        // Act
        var discount = SaleItem.CalculateDiscountPercentage(quantity);

        // Assert
        Assert.Equal(0.10m, discount);
    }

    [Theory(DisplayName = "Quantity between 10 and 20 should have 20% discount")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetweenTenAndTwenty_When_Calculated_Then_TwentyPercentDiscountApplied(int quantity)
    {
        // Act
        var discount = SaleItem.CalculateDiscountPercentage(quantity);

        // Assert
        Assert.Equal(0.20m, discount);
    }

    [Fact(DisplayName = "Quantity above 20 should not be allowed")]
    public void Given_QuantityAboveTwenty_When_Created_Then_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new SaleItem(Guid.NewGuid(), "Product", 21, 10m));
    }

    [Fact(DisplayName = "Total amount should reflect quantity, unit price and discount")]
    public void Given_ValidItem_When_Created_Then_TotalAmountIsCalculatedCorrectly()
    {
        // Arrange
        const int quantity = 10;
        const decimal unitPrice = 100m;

        // Act
        var item = new SaleItem(Guid.NewGuid(), "Product", quantity, unitPrice);

        // Assert
        Assert.Equal(0.20m, item.DiscountPercentage);
        Assert.Equal(800m, item.TotalAmount); // 10 * 100 * (1 - 0.20)
    }

    [Fact(DisplayName = "Cancelling an item should mark it as cancelled")]
    public void Given_Item_When_Cancelled_Then_IsCancelledIsTrue()
    {
        // Arrange
        var item = new SaleItem(Guid.NewGuid(), "Product", 5, 10m);

        // Act
        item.Cancel();

        // Assert
        Assert.True(item.IsCancelled);
    }
}
