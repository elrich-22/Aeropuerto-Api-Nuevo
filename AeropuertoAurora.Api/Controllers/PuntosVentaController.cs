using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/puntos-venta")]
public sealed class PuntosVentaController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PUNTOVENTA",
        "PUV_ID_PUNTO_VENTA",
        ["PUV_CODIGO_PUNTO", "PUV_NOMBRE", "PUV_ID_AEROPUERTO", "PUV_UBICACION", "PUV_ESTADO"],
        ["PUV_CODIGO_PUNTO", "PUV_NOMBRE", "PUV_ID_AEROPUERTO", "PUV_UBICACION", "PUV_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearPuntoVentaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarPuntoVentaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static PuntoVentaDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("PUV_ID_PUNTO_VENTA"),
        row.ToStringValue("PUV_CODIGO_PUNTO"),
        row.ToStringValue("PUV_NOMBRE"),
        row.ToNullableInt("PUV_ID_AEROPUERTO"),
        row.ToNullableString("PUV_UBICACION"),
        row.ToStringValue("PUV_ESTADO"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearPuntoVentaDto dto) => new Dictionary<string, object?>
    {
        ["PUV_CODIGO_PUNTO"] = dto.Codigo,
        ["PUV_NOMBRE"] = dto.Nombre,
        ["PUV_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["PUV_UBICACION"] = dto.Ubicacion,
        ["PUV_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarPuntoVentaDto dto) => ToValues(new CrearPuntoVentaDto(
        dto.Codigo, dto.Nombre, dto.AeropuertoId, dto.Ubicacion, dto.Estado));
}
