using Microsoft.AspNetCore.Mvc;
using MediatR;
using OrderService.Application.Orders.Commands.CreateOrder;
using OrderService.Application.Orders.Queries.GetOrder;

namespace OrderService.API.Controllers;

/// <summary>
/// Orders controller for managing order operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the OrdersController class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get an order by its identifier.
    /// </summary>
    /// <param name="id">The order identifier.</param>
    /// <returns>The order information.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var query = new GetOrderQuery(id);
        var result = await _mediator.Send(query);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <param name="command">The create order command.</param>
    /// <returns>The created order identifier.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return CreatedAtAction(nameof(GetOrder), new { id = result.Value }, new { OrderId = result.Value });
    }

    /// <summary>
    /// Get sample order data (for demonstration purposes).
    /// </summary>
    /// <returns>Sample order information.</returns>
    [HttpGet("sample")]
    public IActionResult GetSample()
    {
        return Ok(new { 
            OrderId = Guid.NewGuid(),
            Amount = "99.99 USD",
            Currency = "USD",
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        });
    }
}