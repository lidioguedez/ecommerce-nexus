using FluentAssertions;
using OrderService.Application.Orders.Queries.GetOrder;
using Xunit;

namespace OrderService.Application.Tests.Orders.Queries;

/// <summary>
/// Unit tests for GetOrderQueryHandler.
/// </summary>
public class GetOrderQueryHandlerTests
{
    private readonly GetOrderQueryHandler _handler;

    public GetOrderQueryHandlerTests()
    {
        _handler = new GetOrderQueryHandler();
    }

    [Fact]
    public async Task Handle_WithValidOrderId_ShouldReturnSuccessResult()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderQuery(orderId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(orderId);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderWithItems()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderQuery(orderId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().NotBeEmpty();
        result.Value.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderWithCorrectFormat()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderQuery(orderId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Should().BeGreaterThan(0);
        result.Value.Currency.Should().Be("USD");
        result.Value.Status.Should().Be("Pending");
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }
}