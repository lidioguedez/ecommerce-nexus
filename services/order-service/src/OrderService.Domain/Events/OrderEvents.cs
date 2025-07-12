using OrderService.Domain.Common;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Events;

/// <summary>
/// Domain event raised when an order is created.
/// </summary>
public class OrderCreatedEvent : IDomainEvent
{
    /// <inheritdoc />
    public string EventType => "order.created";

    /// <inheritdoc />
    public Guid AggregateId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; }

    /// <inheritdoc />
    public Dictionary<string, object> EventData => new()
    {
        { "orderId", AggregateId },
        { "customerId", CustomerId },
        { "occurredOn", OccurredOn }
    };

    /// <summary>
    /// Initializes a new instance of the OrderCreatedEvent class.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    public OrderCreatedEvent(Guid orderId, Guid customerId)
    {
        AggregateId = orderId;
        CustomerId = customerId;
        OccurredOn = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when an order is confirmed.
/// </summary>
public class OrderConfirmedEvent : IDomainEvent
{
    /// <inheritdoc />
    public string EventType => "order.confirmed";

    /// <inheritdoc />
    public Guid AggregateId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; }

    /// <summary>
    /// Gets the total amount of the order.
    /// </summary>
    public Money TotalAmount { get; }

    /// <inheritdoc />
    public Dictionary<string, object> EventData => new()
    {
        { "orderId", AggregateId },
        { "customerId", CustomerId },
        { "totalAmount", TotalAmount.Amount },
        { "currency", TotalAmount.Currency },
        { "occurredOn", OccurredOn }
    };

    /// <summary>
    /// Initializes a new instance of the OrderConfirmedEvent class.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="totalAmount">The total amount of the order.</param>
    public OrderConfirmedEvent(Guid orderId, Guid customerId, Money totalAmount)
    {
        AggregateId = orderId;
        CustomerId = customerId;
        TotalAmount = totalAmount;
        OccurredOn = DateTime.UtcNow;
    }
}

/// <summary>
/// Domain event raised when an order is cancelled.
/// </summary>
public class OrderCancelledEvent : IDomainEvent
{
    /// <inheritdoc />
    public string EventType => "order.cancelled";

    /// <inheritdoc />
    public Guid AggregateId { get; }

    /// <inheritdoc />
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; }

    /// <inheritdoc />
    public Dictionary<string, object> EventData => new()
    {
        { "orderId", AggregateId },
        { "customerId", CustomerId },
        { "occurredOn", OccurredOn }
    };

    /// <summary>
    /// Initializes a new instance of the OrderCancelledEvent class.
    /// </summary>
    /// <param name="orderId">The order identifier.</param>
    /// <param name="customerId">The customer identifier.</param>
    public OrderCancelledEvent(Guid orderId, Guid customerId)
    {
        AggregateId = orderId;
        CustomerId = customerId;
        OccurredOn = DateTime.UtcNow;
    }
}