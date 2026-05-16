using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/arrestos")]
public sealed class ArrestosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ARRESTO",
        "ARR_ID_ARRESTO",
        ["ARR_ID_PASAJERO", "ARR_ID_VUELO", "ARR_ID_AEROPUERTO", "ARR_FECHA_HORA_ARRESTO", "ARR_MOTIVO", "ARR_AUTORIDAD_CARGO", "ARR_DESCRIPCION_INCIDENTE", "ARR_UBICACION_ARRESTO", "ARR_ESTADO_CASO", "ARR_NUMERO_EXPEDIENTE"],
        ["ARR_ID_PASAJERO", "ARR_ID_VUELO", "ARR_ID_AEROPUERTO", "ARR_FECHA_HORA_ARRESTO", "ARR_MOTIVO", "ARR_AUTORIDAD_CARGO", "ARR_DESCRIPCION_INCIDENTE", "ARR_UBICACION_ARRESTO", "ARR_ESTADO_CASO", "ARR_NUMERO_EXPEDIENTE"]);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        var rows = await repository.GetAllAsync(Table, limit, cancellationToken);
        return Ok(rows.Select(Map));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return row is null ? NotFound() : Ok(Map(row));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearArrestoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarEstadoArrestoDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.EstadoCaso))
        {
            return BadRequest(new { message = "El estado del caso es obligatorio." });
        }

        var currentRow = await repository.GetByIdAsync(Table, id, cancellationToken);
        if (currentRow is null)
        {
            return NotFound();
        }

        var currentStatus = currentRow.ToNullableString("ARR_ESTADO_CASO");
        if (string.Equals(currentStatus, "CERRADO", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "El caso ya se encuentra CERRADO y no puede actualizarse nuevamente." });
        }

        var allowedStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ABIERTO", "EN_PROCESO", "CERRADO"
        };
        var status = dto.EstadoCaso.Trim().ToUpperInvariant();
        if (!allowedStatuses.Contains(status))
        {
            return BadRequest(new { message = "Estado invalido. Valores permitidos: ABIERTO, EN_PROCESO, CERRADO." });
        }

        return await repository.UpdatePartialAsync(Table, id, new Dictionary<string, object?>
        {
            ["ARR_ESTADO_CASO"] = status
        }, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ArrestoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("ARR_ID_ARRESTO"),
        row.ToInt("ARR_ID_PASAJERO"),
        row.ToNullableInt("ARR_ID_VUELO"),
        row.ToInt("ARR_ID_AEROPUERTO"),
        row.ToNullableDateTime("ARR_FECHA_HORA_ARRESTO"),
        row.ToStringValue("ARR_MOTIVO"),
        row.ToNullableString("ARR_AUTORIDAD_CARGO"),
        row.ToNullableString("ARR_DESCRIPCION_INCIDENTE"),
        row.ToNullableString("ARR_UBICACION_ARRESTO"),
        row.ToStringValue("ARR_ESTADO_CASO"),
        row.ToNullableString("ARR_NUMERO_EXPEDIENTE"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearArrestoDto dto) => new Dictionary<string, object?>
    {
        ["ARR_ID_PASAJERO"] = dto.PasajeroId,
        ["ARR_ID_VUELO"] = dto.VueloId,
        ["ARR_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["ARR_FECHA_HORA_ARRESTO"] = dto.FechaHoraArresto,
        ["ARR_MOTIVO"] = dto.Motivo,
        ["ARR_AUTORIDAD_CARGO"] = dto.AutoridadCargo,
        ["ARR_DESCRIPCION_INCIDENTE"] = dto.DescripcionIncidente,
        ["ARR_UBICACION_ARRESTO"] = dto.UbicacionArresto,
        ["ARR_ESTADO_CASO"] = "ABIERTO",
        ["ARR_NUMERO_EXPEDIENTE"] = dto.NumeroExpediente
    };
}
