using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/pasajeros")]
public sealed class PasajerosController(IAeropuertoQueryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetPassengersAsync(limit, cancellationToken));
    }

    [HttpGet("{id:int}/reservas")]
    public async Task<IActionResult> GetReservations(int id, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetReservationsAsync(id, limit, cancellationToken));
    }
}
