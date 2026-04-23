using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "Aeropuerto Aurora API",
            utc = DateTimeOffset.UtcNow
        });
    }
}
