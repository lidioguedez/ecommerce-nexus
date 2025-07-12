using FluentAssertions;
using OrderService.Application.Orders.Commands.CreateOrder;
using Xunit;

namespace OrderService.Application.Tests.Orders.Commands;

/// <summary>
/// Unit tests for CreateOrderCommandHandler.
/// </summary>
public class CreateOrderCommandHandlerTests
{
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _handler = new CreateOrderCommandHandler();
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldReturnSuccessResult()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    UnitPrice = 10.50m,
                    Currency = "USD",
                    Quantity = 2
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WithMultipleItems_ShouldReturnSuccessResult()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<CreateOrderItemDto>
            {
                new()
                {
                    ProductId = Guid.NewGuid(),
                    UnitPrice = 10.50m,
                    Currency = "USD",
                    Quantity = 2
                },
                new()
                {
                    ProductId = Guid.NewGuid(),
                    UnitPrice = 25.00m,
                    Currency = "USD",
                    Quantity = 1
                }
            }
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}