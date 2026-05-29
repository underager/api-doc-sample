using APIDocumentationSample.Interfaces;
using APIDocumentationSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace APIDocumentationSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarBusinessLogic _carBusinessLogic;
    private readonly ILogger<CarsController> _logger;

    public CarsController(ICarBusinessLogic carBusinessLogic, ILogger<CarsController> logger)
    {
        _carBusinessLogic = carBusinessLogic;
        _logger = logger;
    }

    /// <summary>
    /// Returns a simple ping response to indicate the API is running.
    /// </summary>
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Ping()
    {
        var message = $"Cars API is running. Timestamp: {DateTimeOffset.UtcNow:O}";
        _logger.LogInformation("Ping request received from CarsController");
        return Ok(message);
    }

    /// <summary>
    /// Retrieves a list of cars from cache or database.
    /// </summary>
    [HttpGet("details")]
    [ProducesResponseType(typeof(List<Car>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<Car>>> GetCarDetails()
    {
        try
        {
            _logger.LogInformation("GetCarDetails endpoint called");
            var cars = await _carBusinessLogic.GetCarDetailsAsync();
            return Ok(cars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCarDetails endpoint");
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving car details", error = ex.Message });
        }
    }
}
