using OrderService.Domain.Common;
using OrderService.Domain.ValueObjects;
using OrderService.Domain.Events;

namespace OrderService.Domain.Entities;

/// <summary>
/// Represents an order in the domain.
/// </summary>
public class Order : AggregateRoot
{
    /// <summary>
    /// Gets the customer identifier associated with this order.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the total amount of the order.
    /// </summary>
    public Money TotalAmount { get; private set; }

    /// <summary>
    /// Gets the current status of the order.
    /// </summary>
    public OrderStatus Status { get; private set; }

    /// <summary>
    /// Gets the collection of order items.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private readonly List<OrderItem> _items = new();

    /// <summary>
    /// Initializes a new instance of the Order class.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    private Order(Guid customerId) : base()
    {
        CustomerId = customerId;
        TotalAmount = Money.Zero();
        Status = OrderStatus.Pending;
    }

    /// <summary>
    /// Creates a new order for the specified customer.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <returns>A new order instance.</returns>
    public static Order Create(Guid customerId)
    {
        var order = new Order(customerId);
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerId));
        return order;
    }

    /// <summary>
    /// Adds an item to the order.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="unitPrice">The unit price of the product.</param>
    /// <param name="quantity">The quantity of the product.</param>
    public void AddItem(Guid productId, Money unitPrice, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            var newItem = OrderItem.Create(productId, unitPrice, quantity);
            _items.Add(newItem);
        }

        RecalculateTotal();
        UpdateTimestamp();
    }

    /// <summary>
    /// Confirms the order and changes its status to confirmed.
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed.");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot confirm an order without items.");

        Status = OrderStatus.Confirmed;
        UpdateTimestamp();
        AddDomainEvent(new OrderConfirmedEvent(Id, CustomerId, TotalAmount));
    }

    /// <summary>
    /// Cancels the order.
    /// </summary>
    public void Cancel()
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel an order with status {Status}.");

        Status = OrderStatus.Cancelled;
        UpdateTimestamp();
        AddDomainEvent(new OrderCancelledEvent(Id, CustomerId));
    }

    private void RecalculateTotal()
    {
        if (!_items.Any())
        {
            TotalAmount = Money.Zero();
            return;
        }

        var currency = _items.First().TotalPrice.Currency;
        var totalAmount = _items.Sum(item => item.TotalPrice.Amount);
        TotalAmount = new Money(totalAmount, currency);
    }
}

/// <summary>
/// Represents the status of an order.
/// </summary>
public enum OrderStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled
}