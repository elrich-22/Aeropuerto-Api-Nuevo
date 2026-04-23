using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/aviones")]
public sealed class AvionesController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_AVION",
        "AVI_ID",
        ["AVI_MATRICULA", "AVI_ID_MODELO", "AVI_ID_AEROLINEA", "AVI_ANIO_FABRICACION", "AVI_ESTADO", "AVI_ULTIMA_REVISION", "AVI_PROXIMA_REVISION", "AVI_HORAS_VUELO"],
        ["AVI_MATRICULA", "AVI_ID_MODELO", "AVI_ID_AEROLINEA", "AVI_ANIO_FABRICACION", "AVI_ESTADO", "AVI_ULTIMA_REVISION", "AVI_PROXIMA_REVISION", "AVI_HORAS_VUELO"]);

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
    public async Task<IActionResult> Create(CrearAvionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAvionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AvionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AvionDto(
            row.ToInt("AVI_ID"),
            row.ToStringValue("AVI_MATRICULA"),
            row.ToInt("AVI_ID_MODELO"),
            row.ToInt("AVI_ID_AEROLINEA"),
            row.ToNullableInt("AVI_ANIO_FABRICACION"),
            row.ToStringValue("AVI_ESTADO"),
            row.ToNullableDateTime("AVI_ULTIMA_REVISION"),
            row.ToNullableDateTime("AVI_PROXIMA_REVISION"),
            row.ToInt("AVI_HORAS_VUELO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAvionDto dto) => new Dictionary<string, object?>
    {
        ["AVI_MATRICULA"] = dto.Matricula,
        ["AVI_ID_MODELO"] = dto.ModeloId,
        ["AVI_ID_AEROLINEA"] = dto.AerolineaId,
        ["AVI_ANIO_FABRICACION"] = dto.AnioFabricacion,
        ["AVI_ESTADO"] = dto.Estado,
        ["AVI_ULTIMA_REVISION"] = dto.UltimaRevision,
        ["AVI_PROXIMA_REVISION"] = dto.ProximaRevision,
        ["AVI_HORAS_VUELO"] = dto.HorasVuelo
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAvionDto dto) => new Dictionary<string, object?>
    {
        ["AVI_MATRICULA"] = dto.Matricula,
        ["AVI_ID_MODELO"] = dto.ModeloId,
        ["AVI_ID_AEROLINEA"] = dto.AerolineaId,
        ["AVI_ANIO_FABRICACION"] = dto.AnioFabricacion,
        ["AVI_ESTADO"] = dto.Estado,
        ["AVI_ULTIMA_REVISION"] = dto.UltimaRevision,
        ["AVI_PROXIMA_REVISION"] = dto.ProximaRevision,
        ["AVI_HORAS_VUELO"] = dto.HorasVuelo
    };
}
