using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/hangares")]
public sealed class HangaresController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_HANGAR",
        "HAN_ID_HANGAR",
        ["HAN_CODIGO_HANGAR", "HAN_NOMBRE", "HAN_ID_AEROPUERTO", "HAN_CAPACIDAD_AVIONES", "HAN_AREA_M2", "HAN_ALTURA_MAXIMA", "HAN_TIPO", "HAN_ESTADO"],
        ["HAN_CODIGO_HANGAR", "HAN_NOMBRE", "HAN_ID_AEROPUERTO", "HAN_CAPACIDAD_AVIONES", "HAN_AREA_M2", "HAN_ALTURA_MAXIMA", "HAN_TIPO", "HAN_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearHangarDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarHangarDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static HangarDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("HAN_ID_HANGAR"),
        row.ToStringValue("HAN_CODIGO_HANGAR"),
        row.ToStringValue("HAN_NOMBRE"),
        row.ToInt("HAN_ID_AEROPUERTO"),
        row.ToNullableInt("HAN_CAPACIDAD_AVIONES"),
        row.ToNullableDecimal("HAN_AREA_M2"),
        row.ToNullableDecimal("HAN_ALTURA_MAXIMA"),
        row.ToNullableString("HAN_TIPO"),
        row.ToStringValue("HAN_ESTADO"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearHangarDto dto) => new Dictionary<string, object?>
    {
        ["HAN_CODIGO_HANGAR"] = dto.Codigo,
        ["HAN_NOMBRE"] = dto.Nombre,
        ["HAN_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["HAN_CAPACIDAD_AVIONES"] = dto.CapacidadAviones,
        ["HAN_AREA_M2"] = dto.AreaM2,
        ["HAN_ALTURA_MAXIMA"] = dto.AlturaMaxima,
        ["HAN_TIPO"] = dto.Tipo,
        ["HAN_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarHangarDto dto) => ToValues(new CrearHangarDto(
        dto.Codigo, dto.Nombre, dto.AeropuertoId, dto.CapacidadAviones, dto.AreaM2, dto.AlturaMaxima, dto.Tipo, dto.Estado));
}
