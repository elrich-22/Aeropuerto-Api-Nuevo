using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/asignaciones-hangar")]
public sealed class AsignacionesHangarController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ASIGNACIONHANGAR",
        "ASH_ID_ASIGNACION",
        ["ASH_ID_HANGAR", "ASH_ID_AVION", "ASH_FECHA_ENTRADA", "ASH_FECHA_SALIDA_PROGRAMADA", "ASH_FECHA_SALIDA_REAL", "ASH_MOTIVO", "ASH_COSTO_HORA", "ASH_COSTO_TOTAL", "ASH_ESTADO"],
        ["ASH_ID_HANGAR", "ASH_ID_AVION", "ASH_FECHA_ENTRADA", "ASH_FECHA_SALIDA_PROGRAMADA", "ASH_FECHA_SALIDA_REAL", "ASH_MOTIVO", "ASH_COSTO_HORA", "ASH_COSTO_TOTAL", "ASH_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearAsignacionHangarDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAsignacionHangarDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AsignacionHangarDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("ASH_ID_ASIGNACION"),
        row.ToInt("ASH_ID_HANGAR"),
        row.ToInt("ASH_ID_AVION"),
        row.ToDateTimeValue("ASH_FECHA_ENTRADA"),
        row.ToNullableDateTime("ASH_FECHA_SALIDA_PROGRAMADA"),
        row.ToNullableDateTime("ASH_FECHA_SALIDA_REAL"),
        row.ToNullableString("ASH_MOTIVO"),
        row.ToNullableDecimal("ASH_COSTO_HORA"),
        row.ToNullableDecimal("ASH_COSTO_TOTAL"),
        row.ToStringValue("ASH_ESTADO"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAsignacionHangarDto dto) => new Dictionary<string, object?>
    {
        ["ASH_ID_HANGAR"] = dto.HangarId,
        ["ASH_ID_AVION"] = dto.AvionId,
        ["ASH_FECHA_ENTRADA"] = dto.FechaEntrada,
        ["ASH_FECHA_SALIDA_PROGRAMADA"] = dto.FechaSalidaProgramada,
        ["ASH_FECHA_SALIDA_REAL"] = dto.FechaSalidaReal,
        ["ASH_MOTIVO"] = dto.Motivo,
        ["ASH_COSTO_HORA"] = dto.CostoHora,
        ["ASH_COSTO_TOTAL"] = dto.CostoTotal,
        ["ASH_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAsignacionHangarDto dto) => ToValues(new CrearAsignacionHangarDto(
        dto.HangarId, dto.AvionId, dto.FechaEntrada, dto.FechaSalidaProgramada, dto.FechaSalidaReal, dto.Motivo, dto.CostoHora, dto.CostoTotal, dto.Estado));
}
