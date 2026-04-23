using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/aerolineas")]
public sealed class AerolineasController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_AEROLINEA",
        "ARL_ID",
        ["ARL_CODIGO_AEROLINEA", "ARL_NOMBRE", "ARL_PAIS_ORIGEN", "ARL_CODIGO_IATA", "ARL_CODIGO_ICAO", "ARL_ESTADO", "ARL_TELEFONO", "ARL_EMAIL", "ARL_SITIO_WEB"],
        ["ARL_CODIGO_AEROLINEA", "ARL_NOMBRE", "ARL_PAIS_ORIGEN", "ARL_CODIGO_IATA", "ARL_CODIGO_ICAO", "ARL_ESTADO", "ARL_TELEFONO", "ARL_EMAIL", "ARL_SITIO_WEB"]);

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
    public async Task<IActionResult> Create(CrearAerolineaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAerolineaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AerolineaDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AerolineaDto(
            row.ToInt("ARL_ID"),
            row.ToNullableString("ARL_CODIGO_AEROLINEA"),
            row.ToStringValue("ARL_NOMBRE"),
            row.ToNullableString("ARL_PAIS_ORIGEN"),
            row.ToNullableString("ARL_CODIGO_IATA"),
            row.ToNullableString("ARL_CODIGO_ICAO"),
            row.ToStringValue("ARL_ESTADO"),
            row.ToNullableString("ARL_TELEFONO"),
            row.ToNullableString("ARL_EMAIL"),
            row.ToNullableString("ARL_SITIO_WEB"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAerolineaDto dto) => new Dictionary<string, object?>
    {
        ["ARL_CODIGO_AEROLINEA"] = dto.Codigo,
        ["ARL_NOMBRE"] = dto.Nombre,
        ["ARL_PAIS_ORIGEN"] = dto.Pais,
        ["ARL_CODIGO_IATA"] = dto.CodigoIata,
        ["ARL_CODIGO_ICAO"] = dto.CodigoIcao,
        ["ARL_ESTADO"] = dto.Estado,
        ["ARL_TELEFONO"] = dto.Telefono,
        ["ARL_EMAIL"] = dto.Email,
        ["ARL_SITIO_WEB"] = dto.SitioWeb
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAerolineaDto dto) => new Dictionary<string, object?>
    {
        ["ARL_CODIGO_AEROLINEA"] = dto.Codigo,
        ["ARL_NOMBRE"] = dto.Nombre,
        ["ARL_PAIS_ORIGEN"] = dto.Pais,
        ["ARL_CODIGO_IATA"] = dto.CodigoIata,
        ["ARL_CODIGO_ICAO"] = dto.CodigoIcao,
        ["ARL_ESTADO"] = dto.Estado,
        ["ARL_TELEFONO"] = dto.Telefono,
        ["ARL_EMAIL"] = dto.Email,
        ["ARL_SITIO_WEB"] = dto.SitioWeb
    };
}
