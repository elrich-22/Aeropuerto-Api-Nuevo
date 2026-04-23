using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/reservas")]
public sealed class ReservasController(IAeropuertoQueryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? pasajeroId, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetReservationsAsync(pasajeroId, limit, cancellationToken));
    }
}
