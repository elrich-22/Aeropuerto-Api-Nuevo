using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/movimientos-equipaje")]
public sealed class MovimientosEquipajeController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_MOVIMIENTO_EQUIPAJE",
        "MEQ_ID_MOVIMIENTO",
        ["MEQ_ID_EQUIPAJE", "MEQ_UBICACION", "MEQ_ESTADO", "MEQ_FECHA_HORA", "MEQ_OBSERVACION"],
        ["MEQ_ID_EQUIPAJE", "MEQ_UBICACION", "MEQ_ESTADO", "MEQ_FECHA_HORA", "MEQ_OBSERVACION"]);

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
    public async Task<IActionResult> Create(CrearMovimientoEquipajeDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarMovimientoEquipajeDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static MovimientoEquipajeDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new MovimientoEquipajeDto(
            row.ToInt("MEQ_ID_MOVIMIENTO"),
            row.ToInt("MEQ_ID_EQUIPAJE"),
            row.ToStringValue("MEQ_UBICACION"),
            row.ToStringValue("MEQ_ESTADO"),
            row.ToNullableDateTime("MEQ_FECHA_HORA"),
            row.ToNullableString("MEQ_OBSERVACION"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearMovimientoEquipajeDto dto) => new Dictionary<string, object?>
    {
        ["MEQ_ID_EQUIPAJE"] = dto.EquipajeId,
        ["MEQ_UBICACION"] = dto.Ubicacion,
        ["MEQ_ESTADO"] = dto.Estado,
        ["MEQ_FECHA_HORA"] = dto.FechaHora,
        ["MEQ_OBSERVACION"] = dto.Observacion
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarMovimientoEquipajeDto dto) => new Dictionary<string, object?>
    {
        ["MEQ_ID_EQUIPAJE"] = dto.EquipajeId,
        ["MEQ_UBICACION"] = dto.Ubicacion,
        ["MEQ_ESTADO"] = dto.Estado,
        ["MEQ_FECHA_HORA"] = dto.FechaHora,
        ["MEQ_OBSERVACION"] = dto.Observacion
    };
}
