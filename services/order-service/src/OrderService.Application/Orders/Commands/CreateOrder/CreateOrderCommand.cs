using FluentValidation;
using OrderService.Application.Common.Models;
using OrderService.Application.Common.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Orders.Commands.CreateOrder;

/// <summary>
/// Command to create a new order.
/// </summary>
public record CreateOrderCommand : Command<Result<Guid>>
{
    /// <summary>
    /// Gets or sets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Gets or sets the collection of order items.
    /// </summary>
    public ICollection<CreateOrderItemDto> Items { get; init; } = new List<CreateOrderItemDto>();
}

/// <summary>
/// Data transfer object for creating an order item.
/// </summary>
public record CreateOrderItemDto
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Gets or sets the currency.
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; init; }
}

/// <summary>
/// Validator for the CreateOrderCommand.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    /// <summary>
    /// Initializes a new instance of the CreateOrderCommandValidator class.
    /// </summary>
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
            .SetValidator(new CreateOrderItemDtoValidator());
    }
}

/// <summary>
/// Validator for the CreateOrderItemDto.
/// </summary>
public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    /// <summary>
    /// Initializes a new instance of the CreateOrderItemDtoValidator class.
    /// </summary>
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .WithMessage("Currency must be a valid 3-letter code.");
    }
}

/// <summary>
/// Handler for the CreateOrderCommand.
/// </summary>
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Result<Guid>>
{
    // Note: In a real implementation, this would use a repository or database context
    // For now, this is a placeholder implementation

    /// <summary>
    /// Handles the CreateOrderCommand.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result containing the order identifier.</returns>
    public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create a new order
            var order = Order.Create(request.CustomerId);

            // Add items to the order
            foreach (var itemDto in request.Items)
            {
                var unitPrice = new Money(itemDto.UnitPrice, itemDto.Currency);
                order.AddItem(itemDto.ProductId, unitPrice, itemDto.Quantity);
            }

            // In a real implementation, you would save the order to the database here
            // await _orderRepository.AddAsync(order, cancellationToken);
            // await _unitOfWork.SaveChangesAsync(cancellationToken);

            await Task.CompletedTask; // Placeholder

            return Result<Guid>.Success(order.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create order: {ex.Message}");
        }
    }
}