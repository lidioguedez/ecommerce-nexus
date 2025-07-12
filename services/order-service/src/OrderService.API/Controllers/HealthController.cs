using Microsoft.AspNetCore.Mvc;

namespace OrderService.API.Controllers;

/// <summary>
/// Health check controller for the Order Service API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Get the health status of the service
    /// </summary>
    /// <returns>Health status response</returns>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { 
            Status = "Healthy", 
            Service = "OrderService.API",
            Timestamp = DateTime.UtcNow 
        });
    }
}