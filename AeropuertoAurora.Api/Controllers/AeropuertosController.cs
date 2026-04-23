using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/aeropuertos")]
public sealed class AeropuertosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_AEROPUERTO",
        "AER_ID",
        ["AER_CODIGO_AEROPUERTO", "AER_NOMBRE", "AER_CIUDAD", "AER_PAIS", "AER_ZONA_HORARIA", "AER_ESTADO", "AER_TIPO", "AER_LATITUD", "AER_LONGITUD", "AER_CODIGO_IATA", "AER_CODIGO_ICAO"],
        ["AER_CODIGO_AEROPUERTO", "AER_NOMBRE", "AER_CIUDAD", "AER_PAIS", "AER_ZONA_HORARIA", "AER_ESTADO", "AER_TIPO", "AER_LATITUD", "AER_LONGITUD", "AER_CODIGO_IATA", "AER_CODIGO_ICAO"]);

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
    public async Task<IActionResult> Create(CrearAeropuertoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAeropuertoDto dto, CancellationToken cancellationToken)
    {
        var updated = await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await repository.DeleteAsync(Table, id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static AeropuertoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AeropuertoDto(
            row.ToInt("AER_ID"),
            row.ToStringValue("AER_CODIGO_AEROPUERTO"),
            row.ToStringValue("AER_NOMBRE"),
            row.ToStringValue("AER_CIUDAD"),
            row.ToStringValue("AER_PAIS"),
            row.ToNullableString("AER_ZONA_HORARIA"),
            row.ToStringValue("AER_ESTADO"),
            row.ToNullableString("AER_TIPO"),
            row.ToNullableDecimal("AER_LATITUD"),
            row.ToNullableDecimal("AER_LONGITUD"),
            row.ToNullableString("AER_CODIGO_IATA"),
            row.ToNullableString("AER_CODIGO_ICAO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAeropuertoDto dto) => new Dictionary<string, object?>
    {
        ["AER_CODIGO_AEROPUERTO"] = dto.Codigo,
        ["AER_NOMBRE"] = dto.Nombre,
        ["AER_CIUDAD"] = dto.Ciudad,
        ["AER_PAIS"] = dto.Pais,
        ["AER_ZONA_HORARIA"] = dto.ZonaHoraria,
        ["AER_ESTADO"] = dto.Estado,
        ["AER_TIPO"] = dto.Tipo,
        ["AER_LATITUD"] = dto.Latitud,
        ["AER_LONGITUD"] = dto.Longitud,
        ["AER_CODIGO_IATA"] = dto.CodigoIata,
        ["AER_CODIGO_ICAO"] = dto.CodigoIcao
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAeropuertoDto dto) => new Dictionary<string, object?>
    {
        ["AER_CODIGO_AEROPUERTO"] = dto.Codigo,
        ["AER_NOMBRE"] = dto.Nombre,
        ["AER_CIUDAD"] = dto.Ciudad,
        ["AER_PAIS"] = dto.Pais,
        ["AER_ZONA_HORARIA"] = dto.ZonaHoraria,
        ["AER_ESTADO"] = dto.Estado,
        ["AER_TIPO"] = dto.Tipo,
        ["AER_LATITUD"] = dto.Latitud,
        ["AER_LONGITUD"] = dto.Longitud,
        ["AER_CODIGO_IATA"] = dto.CodigoIata,
        ["AER_CODIGO_ICAO"] = dto.CodigoIcao
    };
}
