using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/vuelos")]
public sealed class VuelosController(IAeropuertoQueryService service, IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition VuelosTable = new(
        "AER_VUELO",
        "VUE_ID_VUELO",
        [],
        ["VUE_ESTADO", "VUE_FECHA_VUELO"]);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? fecha, [FromQuery] string? origen, [FromQuery] string? destino, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetFlightsAsync(fecha, origen, destino, limit, cancellationToken));
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

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarVuelo(int id, ActualizarVueloEstadoDto dto, CancellationToken cancellationToken)
    {
        var validStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "PROGRAMADO", "EN_VUELO", "ATERRIZADO", "CANCELADO", "REPROGRAMADO", "RETRASADO" };
        var requestedStatus = dto.Estado?.Trim().ToUpperInvariant();
        var persistedStatus = string.Equals(requestedStatus, "REPROGRAMADO", StringComparison.OrdinalIgnoreCase)
            ? "RETRASADO"
            : requestedStatus;

        if (string.IsNullOrWhiteSpace(dto.Estado) || !validStatuses.Contains(dto.Estado))
            return BadRequest(new { message = "Estado invalido. Valores permitidos: PROGRAMADO, EN_VUELO, ATERRIZADO, CANCELADO, REPROGRAMADO, RETRASADO." });

        var flight = await service.GetFlightByIdAsync(id, cancellationToken);
        if (flight is null)
            return NotFound(new { message = "El vuelo no existe." });

        if (string.Equals(flight.Estado, "CANCELADO", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "El vuelo ya esta cancelado y no puede modificarse." });

        if ((string.Equals(requestedStatus, "CANCELADO", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(requestedStatus, "REPROGRAMADO", StringComparison.OrdinalIgnoreCase)) &&
            string.Equals(flight.Estado, "ATERRIZADO", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "No se puede cancelar ni reprogramar un vuelo que ya aterrizo." });
        }

        if (string.Equals(requestedStatus, "REPROGRAMADO", StringComparison.OrdinalIgnoreCase) && !dto.FechaVuelo.HasValue)
        {
            return BadRequest(new { message = "Debes indicar la nueva fecha del vuelo para reprogramarlo." });
        }

        if (dto.FechaVuelo.HasValue)
        {
            var now = DateTime.UtcNow;
            if (dto.FechaVuelo.Value < now)
                return BadRequest(new { message = "La fecha del vuelo no puede ser en el pasado." });
            if (dto.FechaVuelo.Value > now.AddYears(2))
                return BadRequest(new { message = "La fecha del vuelo no puede ser mas de 2 años en el futuro." });
        }

        var values = new Dictionary<string, object?>
        {
            ["VUE_ESTADO"] = persistedStatus,
            ["VUE_FECHA_VUELO"] = dto.FechaVuelo ?? flight.FechaVuelo
        };

        return await repository.UpdateAsync(VuelosTable, id, values, cancellationToken)
            ? Ok(new
            {
                message = string.Equals(requestedStatus, "REPROGRAMADO", StringComparison.OrdinalIgnoreCase)
                    ? "Vuelo reprogramado correctamente."
                    : "Vuelo actualizado correctamente.",
                id,
                estado = persistedStatus
            })
            : NotFound(new { message = "No se encontro el vuelo para actualizar." });
    }
}
