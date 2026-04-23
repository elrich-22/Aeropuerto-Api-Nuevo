using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/vuelos")]
public sealed class VuelosController(IAeropuertoQueryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? fecha, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetFlightsAsync(fecha, limit, cancellationToken));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var flight = await service.GetFlightByIdAsync(id, cancellationToken);
        return flight is null ? NotFound() : Ok(flight);
    }

    [HttpGet("programas")]
    public async Task<IActionResult> GetPrograms([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetFlightProgramsAsync(limit, cancellationToken));
    }
}
