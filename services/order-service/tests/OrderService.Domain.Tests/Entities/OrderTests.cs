using FluentAssertions;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;
using OrderService.Domain.Events;
using Xunit;

namespace OrderService.Domain.Tests.Entities;

/// <summary>
/// Unit tests for the Order entity.
/// </summary>
public class OrderTests
{
    [Fact]
    public void Create_WithValidCustomerId_ShouldCreateOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        // Act
        var order = Order.Create(customerId);

        // Assert
        order.Should().NotBeNull();
        order.CustomerId.Should().Be(customerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.TotalAmount.Should().Be(Money.Zero());
        order.Items.Should().BeEmpty();
        order.DomainEvents.Should().HaveCount(1);
        order.DomainEvents.First().Should().BeOfType<OrderCreatedEvent>();
    }

    [Fact]
    public void AddItem_WithValidItem_ShouldAddItemAndUpdateTotal()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        var quantity = 2;

        // Act
        order.AddItem(productId, unitPrice, quantity);

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items.First().ProductId.Should().Be(productId);
        order.Items.First().UnitPrice.Should().Be(unitPrice);
        order.Items.First().Quantity.Should().Be(quantity);
        order.TotalAmount.Amount.Should().Be(21.00m);
        order.TotalAmount.Currency.Should().Be("USD");
    }

    [Fact]
    public void AddItem_WithSameProduct_ShouldUpdateQuantity()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");

        // Act
        order.AddItem(productId, unitPrice, 2);
        order.AddItem(productId, unitPrice, 3);

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items.First().Quantity.Should().Be(5);
        order.TotalAmount.Amount.Should().Be(52.50m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AddItem_WithInvalidQuantity_ShouldThrowException(int quantity)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");

        // Act & Assert
        var act = () => order.AddItem(productId, unitPrice, quantity);
        act.Should().Throw<ArgumentException>()
           .WithMessage("Quantity must be greater than zero.*");
    }

    [Fact]
    public void Confirm_WithPendingOrderAndItems_ShouldConfirmOrder()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        order.AddItem(productId, unitPrice, 2);

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
        order.DomainEvents.Should().Contain(e => e is OrderConfirmedEvent);
    }

    [Fact]
    public void Confirm_WithoutItems_ShouldThrowException()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);

        // Act & Assert
        var act = () => order.Confirm();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot confirm an order without items.");
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void Confirm_WithNonPendingStatus_ShouldThrowException(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);
        var productId = Guid.NewGuid();
        var unitPrice = new Money(10.50m, "USD");
        order.AddItem(productId, unitPrice, 2);

        // Use reflection to set the status for testing
        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty?.SetValue(order, status);

        // Act & Assert
        var act = () => order.Confirm();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Only pending orders can be confirmed.");
    }

    [Theory]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Confirmed)]
    public void Cancel_WithValidStatus_ShouldCancelOrder(OrderStatus initialStatus)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);

        // Use reflection to set the status for testing
        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty?.SetValue(order, initialStatus);

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().Contain(e => e is OrderCancelledEvent);
    }

    [Theory]
    [InlineData(OrderStatus.Completed)]
    [InlineData(OrderStatus.Cancelled)]
    public void Cancel_WithInvalidStatus_ShouldThrowException(OrderStatus status)
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var order = Order.Create(customerId);

        // Use reflection to set the status for testing
        var statusProperty = typeof(Order).GetProperty("Status");
        statusProperty?.SetValue(order, status);

        // Act & Assert
        var act = () => order.Cancel();
        act.Should().Throw<InvalidOperationException>()
           .WithMessage($"Cannot cancel an order with status {status}.");
    }
}