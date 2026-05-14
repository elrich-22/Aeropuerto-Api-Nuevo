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

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> ActualizarVuelo(int id, ActualizarVueloEstadoDto dto, CancellationToken cancellationToken)
    {
        var validStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "PROGRAMADO", "EN_VUELO", "ATERRIZADO", "CANCELADO", "REPROGRAMADO", "RETRASADO" };

        if (string.IsNullOrWhiteSpace(dto.Estado) || !validStatuses.Contains(dto.Estado))
            return BadRequest(new { message = "Estado invalido. Valores permitidos: PROGRAMADO, EN_VUELO, ATERRIZADO, CANCELADO, REPROGRAMADO, RETRASADO." });

        var flight = await service.GetFlightByIdAsync(id, cancellationToken);
        if (flight is null)
            return NotFound(new { message = "El vuelo no existe." });

        if (string.Equals(flight.Estado, "CANCELADO", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "El vuelo ya esta cancelado y no puede modificarse." });

        var values = new Dictionary<string, object?> { ["VUE_ESTADO"] = dto.Estado.ToUpperInvariant() };
        if (dto.FechaVuelo.HasValue)
            values["VUE_FECHA_VUELO"] = dto.FechaVuelo.Value;

        return await repository.UpdateAsync(VuelosTable, id, values, cancellationToken)
            ? Ok(new { message = "Vuelo actualizado correctamente.", id, estado = dto.Estado.ToUpperInvariant() })
            : NotFound(new { message = "No se encontro el vuelo para actualizar." });
    }
}
