using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/equipajes")]
public sealed class EquipajesController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_EQUIPAJE",
        "EQU_ID_EQUIPAJE",
        ["EQU_ID_PASAJERO", "EQU_ID_VUELO", "EQU_CODIGO_BARRAS", "EQU_PESO_KG", "EQU_ESTADO", "EQU_FECHA_REGISTRO"],
        ["EQU_ID_PASAJERO", "EQU_ID_VUELO", "EQU_CODIGO_BARRAS", "EQU_PESO_KG", "EQU_ESTADO", "EQU_FECHA_REGISTRO"]);

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
    public async Task<IActionResult> Create(CrearEquipajeOperacionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarEquipajeOperacionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static EquipajeOperacionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new EquipajeOperacionDto(
            row.ToInt("EQU_ID_EQUIPAJE"),
            row.ToInt("EQU_ID_PASAJERO"),
            row.ToInt("EQU_ID_VUELO"),
            row.ToStringValue("EQU_CODIGO_BARRAS"),
            row.ToNullableDecimal("EQU_PESO_KG") ?? 0m,
            row.ToStringValue("EQU_ESTADO"),
            row.ToNullableDateTime("EQU_FECHA_REGISTRO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearEquipajeOperacionDto dto) => new Dictionary<string, object?>
    {
        ["EQU_ID_PASAJERO"] = dto.PasajeroId,
        ["EQU_ID_VUELO"] = dto.VueloId,
        ["EQU_CODIGO_BARRAS"] = dto.CodigoBarras,
        ["EQU_PESO_KG"] = dto.PesoKg,
        ["EQU_ESTADO"] = dto.Estado,
        ["EQU_FECHA_REGISTRO"] = dto.FechaRegistro
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarEquipajeOperacionDto dto) => new Dictionary<string, object?>
    {
        ["EQU_ID_PASAJERO"] = dto.PasajeroId,
        ["EQU_ID_VUELO"] = dto.VueloId,
        ["EQU_CODIGO_BARRAS"] = dto.CodigoBarras,
        ["EQU_PESO_KG"] = dto.PesoKg,
        ["EQU_ESTADO"] = dto.Estado,
        ["EQU_FECHA_REGISTRO"] = dto.FechaRegistro
    };
}
