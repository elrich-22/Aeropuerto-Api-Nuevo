using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/incidentes")]
public sealed class IncidentesController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_INCIDENTE",
        "INC_ID_INCIDENTE",
        ["INC_TIPO", "INC_DESCRIPCION", "INC_FECHA_HORA", "INC_SEVERIDAD", "INC_ESTADO", "INC_ID_VUELO", "INC_ID_EMPLEADO_REPORTA"],
        ["INC_TIPO", "INC_DESCRIPCION", "INC_FECHA_HORA", "INC_SEVERIDAD", "INC_ESTADO", "INC_ID_VUELO", "INC_ID_EMPLEADO_REPORTA"]);

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
    public async Task<IActionResult> Create(CrearIncidenteDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarIncidenteDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static IncidenteDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("INC_ID_INCIDENTE"),
        row.ToStringValue("INC_TIPO"),
        row.ToStringValue("INC_DESCRIPCION"),
        row.ToNullableDateTime("INC_FECHA_HORA"),
        row.ToStringValue("INC_SEVERIDAD"),
        row.ToStringValue("INC_ESTADO"),
        row.ToNullableInt("INC_ID_VUELO"),
        row.ToNullableInt("INC_ID_EMPLEADO_REPORTA"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearIncidenteDto dto) => new Dictionary<string, object?>
    {
        ["INC_TIPO"] = dto.Tipo,
        ["INC_DESCRIPCION"] = dto.Descripcion,
        ["INC_FECHA_HORA"] = dto.FechaHora,
        ["INC_SEVERIDAD"] = dto.Severidad,
        ["INC_ESTADO"] = dto.Estado,
        ["INC_ID_VUELO"] = dto.VueloId,
        ["INC_ID_EMPLEADO_REPORTA"] = dto.EmpleadoReportaId
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarIncidenteDto dto) => ToValues(new CrearIncidenteDto(
        dto.Tipo, dto.Descripcion, dto.FechaHora, dto.Severidad, dto.Estado, dto.VueloId, dto.EmpleadoReportaId));
}
