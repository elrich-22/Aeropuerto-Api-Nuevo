using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/operaciones")]
public sealed class OperacionesController(IAeropuertoQueryService service) : ControllerBase
{
    [HttpGet("equipaje")]
    public async Task<IActionResult> GetBaggage([FromQuery] int? vueloId, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetBaggageAsync(vueloId, limit, cancellationToken));
    }

    [HttpGet("mantenimientos")]
    public async Task<IActionResult> GetMaintenance([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetMaintenanceAsync(limit, cancellationToken));
    }

    [HttpGet("controles-seguridad")]
    public async Task<IActionResult> GetSecurityControls([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetSecurityControlsAsync(limit, cancellationToken));
    }

    [HttpGet("incidentes")]
    public async Task<IActionResult> GetIncidents([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetIncidentsAsync(limit, cancellationToken));
    }
}
