using System.Security.Claims;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reservas")]
public sealed class ReservasController(IAeropuertoQueryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? pasajeroId, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        if (!User.IsInRole("ADMIN"))
        {
            var claimValue = User.FindFirstValue("pasajeroId");
            if (!int.TryParse(claimValue, out var myPasajeroId))
                return Forbid();

            if (pasajeroId.HasValue && pasajeroId.Value != myPasajeroId)
                return Forbid();

            pasajeroId = myPasajeroId;
        }

        return Ok(await service.GetReservationsAsync(pasajeroId, limit, cancellationToken));
    }
}
