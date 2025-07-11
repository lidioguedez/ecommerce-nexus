namespace OrderService.Domain.Common;

/// <summary>
/// Represents a domain event that occurs within the order management bounded context.
/// Domain events capture important business events that other parts of the system should know about.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the type of the event (e.g., "order.placed", "cart.item.added").
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Gets the unique identifier of the aggregate that raised this event.
    /// </summary>
    Guid AggregateId { get; }

    /// <summary>
    /// Gets the date and time when this event occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the event data as a dictionary for serialization purposes.
    /// </summary>
    Dictionary<string, object> EventData { get; }
}