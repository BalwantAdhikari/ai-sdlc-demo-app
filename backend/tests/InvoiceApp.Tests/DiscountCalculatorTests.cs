using InvoiceApp.Domain;
using Xunit;

namespace InvoiceApp.Tests;

public class DiscountCalculatorTests
{
    [Fact]
    public void QuantityDiscount_ReturnsExpectedTierForLargeOrders()
    {
        var calculator = new DiscountCalculator();

        Assert.Equal(0.15m, calculator.QuantityDiscount(100));
        Assert.Equal(0.10m, calculator.QuantityDiscount(75));
        Assert.Equal(0.05m, calculator.QuantityDiscount(10));
        Assert.Equal(0m, calculator.QuantityDiscount(9));
    }

    [Fact]
    public void LoyaltyDiscount_CapsAtTenPercent()
    {
        var calculator = new DiscountCalculator();

        Assert.Equal(0.01m, calculator.LoyaltyDiscount(1));
        Assert.Equal(0.10m, calculator.LoyaltyDiscount(20));
        Assert.Equal(0.05m, calculator.LoyaltyDiscount(5));
    }

    [Fact]
    public void CombinedDiscount_AddsQuantityAndLoyaltyDiscounts()
    {
        var calculator = new DiscountCalculator();

        Assert.Equal(0.20m, calculator.CombinedDiscount(100, 5, null));
        Assert.Equal(0.30m, calculator.CombinedDiscount(100, 15, "Nonprofit"));
    }

    [Fact]
    public void ApplyDiscount_RoundsToTwoDecimalPlaces()
    {
        var calculator = new DiscountCalculator();

        Assert.Equal(90.00m, calculator.ApplyDiscount(100m, 0.10m));
        Assert.Equal(75.00m, calculator.ApplyDiscount(100m, 0.25m));
    }
}
