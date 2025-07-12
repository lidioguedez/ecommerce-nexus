using FluentAssertions;
using OrderService.Application.Orders.Commands.CreateOrder;
using Xunit;

namespace OrderService.Application.Tests.Orders.Commands;

/// <summary>
/// Unit tests for CreateOrderCommand validation.
/// </summary>
public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
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
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCustomerId_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.Empty,
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
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.CustomerId));
    }

    [Fact]
    public void Validate_WithEmptyItems_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand
        {
            CustomerId = Guid.NewGuid(),
            Items = new List<CreateOrderItemDto>()
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateOrderCommand.Items));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidQuantity_ShouldFail(int quantity)
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
                    Quantity = quantity
                }
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidUnitPrice_ShouldFail(decimal unitPrice)
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
                    UnitPrice = unitPrice,
                    Currency = "USD",
                    Quantity = 1
                }
            }
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("UnitPrice"));
    }
}