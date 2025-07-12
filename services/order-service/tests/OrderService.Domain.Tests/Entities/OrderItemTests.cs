using FluentAssertions;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using Xunit;

namespace OrderService.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the OrderItem entity.
/// </summary>
public class OrderItemTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateOrderItem()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        var quantity = 2;

        // Act
        var orderItem = OrderItem.Create(productId, unitPrice, quantity);

        // Assert
        orderItem.Should().NotBeNull();
        orderItem.ProductId.Should().Be(productId);
        orderItem.UnitPrice.Should().Be(unitPrice);
        orderItem.Quantity.Should().Be(quantity);
        orderItem.TotalPrice.Amount.Should().Be(21.00m);
        orderItem.TotalPrice.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidQuantity_ShouldThrowException(int quantity)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");

        // Act & Assert
        var act = () => OrderItem.Create(productId, unitPrice, quantity);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Quantity must be greater than zero.*");
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdateQuantityAndTotal()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        var orderItem = OrderItem.Create(productId, unitPrice, 2);

        // Act
        orderItem.UpdateQuantity(5);

        // Assert
        orderItem.Quantity.Should().Be(5);
        orderItem.TotalPrice.Amount.Should().Be(52.50m);
        orderItem.TotalPrice.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UpdateQuantity_WithInvalidQuantity_ShouldThrowException(int newQuantity)
    {
        // Arrange
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        var orderItem = OrderItem.Create(productId, unitPrice, 2);

        // Act & Assert
        var act = () => orderItem.UpdateQuantity(newQuantity);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Quantity must be greater than zero.*");
    }

    [Fact]
    public void UpdateQuantity_ShouldUpdateTimestamp()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        var orderItem = OrderItem.Create(productId, unitPrice, 2);
        var originalTimestamp = orderItem.UpdatedAt;

        // Wait a small amount to ensure timestamp difference
        Thread.Sleep(10);

        // Act
        orderItem.UpdateQuantity(3);

        // Assert
        orderItem.UpdatedAt.Should().BeAfter(originalTimestamp);
    }
}