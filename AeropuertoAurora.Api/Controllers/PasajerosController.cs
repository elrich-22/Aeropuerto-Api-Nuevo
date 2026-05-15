using System.Security.Claims;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/pasajeros")]
public sealed class PasajerosController(IAeropuertoQueryService service) : ControllerBase
{
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetPassengersAsync(limit, cancellationToken));
    }

    [HttpGet("{id:int}/reservas")]
    public async Task<IActionResult> GetReservations(int id, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        if (!User.IsInRole("ADMIN"))
        {
            var claimValue = User.FindFirstValue("pasajeroId");
            if (!int.TryParse(claimValue, out var myPasajeroId) || myPasajeroId != id)
                return Forbid();
        }

        return Ok(await service.GetReservationsAsync(id, limit, cancellationToken));
    }
}
