using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/usos-promocion")]
public sealed class UsosPromocionController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_USOPROMOCION",
        "USO_ID_USO",
        ["USO_ID_PROMOCION", "USO_ID_RESERVA", "USO_FECHA_USO", "USO_MONTO_DESCUENTO"],
        ["USO_ID_PROMOCION", "USO_ID_RESERVA", "USO_FECHA_USO", "USO_MONTO_DESCUENTO"]);

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
    public async Task<IActionResult> Create(CrearUsoPromocionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarUsoPromocionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static UsoPromocionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new UsoPromocionDto(
            row.ToInt("USO_ID_USO"),
            row.ToInt("USO_ID_PROMOCION"),
            row.ToInt("USO_ID_RESERVA"),
            row.ToNullableDateTime("USO_FECHA_USO"),
            row.ToNullableDecimal("USO_MONTO_DESCUENTO") ?? 0m);
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearUsoPromocionDto dto) => new Dictionary<string, object?>
    {
        ["USO_ID_PROMOCION"] = dto.PromocionId,
        ["USO_ID_RESERVA"] = dto.ReservaId,
        ["USO_FECHA_USO"] = dto.FechaUso,
        ["USO_MONTO_DESCUENTO"] = dto.MontoDescuento
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarUsoPromocionDto dto) => new Dictionary<string, object?>
    {
        ["USO_ID_PROMOCION"] = dto.PromocionId,
        ["USO_ID_RESERVA"] = dto.ReservaId,
        ["USO_FECHA_USO"] = dto.FechaUso,
        ["USO_MONTO_DESCUENTO"] = dto.MontoDescuento
    };
}
