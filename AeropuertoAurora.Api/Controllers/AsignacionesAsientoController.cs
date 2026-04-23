using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/asignaciones-asiento")]
public sealed class AsignacionesAsientoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ASIGNACION_ASIENTO",
        "AAS_ID_ASIGNACION",
        ["AAS_ID_VUELO", "AAS_ID_PASAJERO", "AAS_ID_ASIENTO", "AAS_FECHA_ASIGNACION", "AAS_ESTADO"],
        ["AAS_ID_VUELO", "AAS_ID_PASAJERO", "AAS_ID_ASIENTO", "AAS_FECHA_ASIGNACION", "AAS_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearAsignacionAsientoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAsignacionAsientoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AsignacionAsientoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AsignacionAsientoDto(
            row.ToInt("AAS_ID_ASIGNACION"),
            row.ToInt("AAS_ID_VUELO"),
            row.ToInt("AAS_ID_PASAJERO"),
            row.ToInt("AAS_ID_ASIENTO"),
            row.ToNullableDateTime("AAS_FECHA_ASIGNACION"),
            row.ToStringValue("AAS_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAsignacionAsientoDto dto) => new Dictionary<string, object?>
    {
        ["AAS_ID_VUELO"] = dto.VueloId,
        ["AAS_ID_PASAJERO"] = dto.PasajeroId,
        ["AAS_ID_ASIENTO"] = dto.AsientoId,
        ["AAS_FECHA_ASIGNACION"] = dto.FechaAsignacion,
        ["AAS_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAsignacionAsientoDto dto) => new Dictionary<string, object?>
    {
        ["AAS_ID_VUELO"] = dto.VueloId,
        ["AAS_ID_PASAJERO"] = dto.PasajeroId,
        ["AAS_ID_ASIENTO"] = dto.AsientoId,
        ["AAS_FECHA_ASIGNACION"] = dto.FechaAsignacion,
        ["AAS_ESTADO"] = dto.Estado
    };
}
