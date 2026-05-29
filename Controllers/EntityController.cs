using Microsoft.AspNetCore.Mvc;

namespace APIDocumentationSample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EntityController : ControllerBase
{
    private readonly ILogger<EntityController> _logger;

    public EntityController(ILogger<EntityController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns a simple ping response to indicate the API is running.
    /// </summary>
    [HttpGet("ping")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Ping()
    {
        var message = $"API is running. Timestamp: {DateTimeOffset.UtcNow:O}";

        _logger.LogInformation("Ping request received. Message: {Message}", message);

        return Ok(message);
    }
}
