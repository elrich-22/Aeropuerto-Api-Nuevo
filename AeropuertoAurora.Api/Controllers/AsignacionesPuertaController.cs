using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/asignaciones-puerta")]
public sealed class AsignacionesPuertaController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ASIGNACION_PUERTA",
        "ASP_ID_ASIGNACION",
        ["ASP_ID_VUELO", "ASP_ID_PUERTA", "ASP_FECHA_HORA_INICIO", "ASP_FECHA_HORA_FIN", "ASP_ESTADO"],
        ["ASP_ID_VUELO", "ASP_ID_PUERTA", "ASP_FECHA_HORA_INICIO", "ASP_FECHA_HORA_FIN", "ASP_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearAsignacionPuertaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAsignacionPuertaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AsignacionPuertaDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AsignacionPuertaDto(
            row.ToInt("ASP_ID_ASIGNACION"),
            row.ToInt("ASP_ID_VUELO"),
            row.ToInt("ASP_ID_PUERTA"),
            row.ToDateTimeValue("ASP_FECHA_HORA_INICIO"),
            row.ToNullableDateTime("ASP_FECHA_HORA_FIN"),
            row.ToStringValue("ASP_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAsignacionPuertaDto dto) => new Dictionary<string, object?>
    {
        ["ASP_ID_VUELO"] = dto.VueloId,
        ["ASP_ID_PUERTA"] = dto.PuertaId,
        ["ASP_FECHA_HORA_INICIO"] = dto.FechaHoraInicio,
        ["ASP_FECHA_HORA_FIN"] = dto.FechaHoraFin,
        ["ASP_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAsignacionPuertaDto dto) => new Dictionary<string, object?>
    {
        ["ASP_ID_VUELO"] = dto.VueloId,
        ["ASP_ID_PUERTA"] = dto.PuertaId,
        ["ASP_FECHA_HORA_INICIO"] = dto.FechaHoraInicio,
        ["ASP_FECHA_HORA_FIN"] = dto.FechaHoraFin,
        ["ASP_ESTADO"] = dto.Estado
    };
}
