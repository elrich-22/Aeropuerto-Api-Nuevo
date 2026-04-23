using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/movimientos-repuesto")]
public sealed class MovimientosRepuestoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_MOVIMIENTOREPUESTO",
        "MOV_ID_MOVIMIENTO",
        ["MOV_ID_REPUESTO", "MOV_TIPO_MOVIMIENTO", "MOV_CANTIDAD", "MOV_FECHA_MOVIMIENTO", "MOV_ID_EMPLEADO", "MOV_MOTIVO", "MOV_REFERENCIA"],
        ["MOV_ID_REPUESTO", "MOV_TIPO_MOVIMIENTO", "MOV_CANTIDAD", "MOV_FECHA_MOVIMIENTO", "MOV_ID_EMPLEADO", "MOV_MOTIVO", "MOV_REFERENCIA"]);

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
    public async Task<IActionResult> Create(CrearMovimientoRepuestoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarMovimientoRepuestoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static MovimientoRepuestoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("MOV_ID_MOVIMIENTO"),
        row.ToInt("MOV_ID_REPUESTO"),
        row.ToStringValue("MOV_TIPO_MOVIMIENTO"),
        row.ToInt("MOV_CANTIDAD"),
        row.ToNullableDateTime("MOV_FECHA_MOVIMIENTO"),
        row.ToNullableInt("MOV_ID_EMPLEADO"),
        row.ToNullableString("MOV_MOTIVO"),
        row.ToNullableString("MOV_REFERENCIA"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearMovimientoRepuestoDto dto) => new Dictionary<string, object?>
    {
        ["MOV_ID_REPUESTO"] = dto.RepuestoId,
        ["MOV_TIPO_MOVIMIENTO"] = dto.TipoMovimiento,
        ["MOV_CANTIDAD"] = dto.Cantidad,
        ["MOV_FECHA_MOVIMIENTO"] = dto.FechaMovimiento,
        ["MOV_ID_EMPLEADO"] = dto.EmpleadoId,
        ["MOV_MOTIVO"] = dto.Motivo,
        ["MOV_REFERENCIA"] = dto.Referencia
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarMovimientoRepuestoDto dto) => ToValues(new CrearMovimientoRepuestoDto(
        dto.RepuestoId, dto.TipoMovimiento, dto.Cantidad, dto.FechaMovimiento, dto.EmpleadoId, dto.Motivo, dto.Referencia));
}
