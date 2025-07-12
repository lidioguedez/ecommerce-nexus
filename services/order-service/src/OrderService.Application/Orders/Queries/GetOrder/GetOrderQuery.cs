using OrderService.Application.Common.Models;
using OrderService.Application.Common.Interfaces;
using OrderService.Domain.Entities;

namespace OrderService.Application.Orders.Queries.GetOrder;

/// <summary>
/// Query to get an order by its identifier.
/// </summary>
/// <param name="OrderId">The order identifier.</param>
public record GetOrderQuery(Guid OrderId) : Query<Result<OrderDto>>;

/// <summary>
/// Data transfer object for order information.
/// </summary>
public record OrderDto
{
    /// <summary>
    /// Gets or sets the order identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    public string Currency { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the order status.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the collection of order items.
    /// </summary>
    public ICollection<OrderItemDto> Items { get; init; } = new List<OrderItemDto>();
}

/// <summary>
/// Data transfer object for order item information.
/// </summary>
public record OrderItemDto
{
    /// <summary>
    /// Gets or sets the item identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Gets or sets the total price.
    /// </summary>
    public decimal TotalPrice { get; init; }
}

/// <summary>
/// Handler for the GetOrderQuery.
/// </summary>
public class GetOrderQueryHandler : IQueryHandler<GetOrderQuery, Result<OrderDto>>
{
    // Note: In a real implementation, this would use a repository or database context
    // For now, this is a placeholder implementation

    /// <summary>
    /// Handles the GetOrderQuery.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The order result.</returns>
    public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Placeholder implementation - in real scenario, this would query the database
        await Task.CompletedTask;

        // For demo purposes, return a sample order
        var orderDto = new OrderDto
        {
            Id = request.OrderId,
            CustomerId = Guid.NewGuid(),
            TotalAmount = 99.99m,
            Currency = "USD",
            Status = OrderStatus.Pending.ToString(),
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItemDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    UnitPrice = 99.99m,
                    Quantity = 1,
                    TotalPrice = 99.99m
                }
            }
        };

        return Result<OrderDto>.Success(orderDto);
    }
}