using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/mantenimientos-avion")]
public sealed class MantenimientosAvionController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_MANTENIMIENTOAVION",
        "MAN_ID_MANTENIMIENTO",
        ["MAN_ID_AVION", "MAN_TIPO_MANTENIMIENTO", "MAN_FECHA_INICIO", "MAN_FECHA_FIN_ESTIMADA", "MAN_FECHA_FIN_REAL", "MAN_DESCRIPCION_TRABAJO", "MAN_ID_EMPLEADO_RESPONSABLE", "MAN_COSTO_MANO_OBRA", "MAN_COSTO_REPUESTOS", "MAN_COSTO_TOTAL", "MAN_ESTADO"],
        ["MAN_ID_AVION", "MAN_TIPO_MANTENIMIENTO", "MAN_FECHA_INICIO", "MAN_FECHA_FIN_ESTIMADA", "MAN_FECHA_FIN_REAL", "MAN_DESCRIPCION_TRABAJO", "MAN_ID_EMPLEADO_RESPONSABLE", "MAN_COSTO_MANO_OBRA", "MAN_COSTO_REPUESTOS", "MAN_COSTO_TOTAL", "MAN_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearMantenimientoAvionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarMantenimientoAvionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static MantenimientoAvionDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("MAN_ID_MANTENIMIENTO"),
        row.ToInt("MAN_ID_AVION"),
        row.ToStringValue("MAN_TIPO_MANTENIMIENTO"),
        row.ToDateTimeValue("MAN_FECHA_INICIO"),
        row.ToNullableDateTime("MAN_FECHA_FIN_ESTIMADA"),
        row.ToNullableDateTime("MAN_FECHA_FIN_REAL"),
        row.ToNullableString("MAN_DESCRIPCION_TRABAJO"),
        row.ToNullableInt("MAN_ID_EMPLEADO_RESPONSABLE"),
        row.ToNullableDecimal("MAN_COSTO_MANO_OBRA"),
        row.ToNullableDecimal("MAN_COSTO_REPUESTOS"),
        row.ToNullableDecimal("MAN_COSTO_TOTAL"),
        row.ToStringValue("MAN_ESTADO"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearMantenimientoAvionDto dto) => new Dictionary<string, object?>
    {
        ["MAN_ID_AVION"] = dto.AvionId,
        ["MAN_TIPO_MANTENIMIENTO"] = dto.TipoMantenimiento,
        ["MAN_FECHA_INICIO"] = dto.FechaInicio,
        ["MAN_FECHA_FIN_ESTIMADA"] = dto.FechaFinEstimada,
        ["MAN_FECHA_FIN_REAL"] = dto.FechaFinReal,
        ["MAN_DESCRIPCION_TRABAJO"] = dto.DescripcionTrabajo,
        ["MAN_ID_EMPLEADO_RESPONSABLE"] = dto.EmpleadoResponsableId,
        ["MAN_COSTO_MANO_OBRA"] = dto.CostoManoObra,
        ["MAN_COSTO_REPUESTOS"] = dto.CostoRepuestos,
        ["MAN_COSTO_TOTAL"] = dto.CostoTotal,
        ["MAN_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarMantenimientoAvionDto dto) => ToValues(new CrearMantenimientoAvionDto(
        dto.AvionId, dto.TipoMantenimiento, dto.FechaInicio, dto.FechaFinEstimada, dto.FechaFinReal, dto.DescripcionTrabajo, dto.EmpleadoResponsableId, dto.CostoManoObra, dto.CostoRepuestos, dto.CostoTotal, dto.Estado));
}
