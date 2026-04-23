using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/detalles-venta-boleto")]
public sealed class DetallesVentaBoletoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_DETALLEVENTABOLETO",
        "DEV_ID_DETALLE_VENTA",
        ["DEV_ID_VENTA", "DEV_ID_RESERVA", "DEV_PRECIO_BASE", "DEV_CARGOS_ADICIONALES"],
        ["DEV_ID_VENTA", "DEV_ID_RESERVA", "DEV_PRECIO_BASE", "DEV_CARGOS_ADICIONALES"]);

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
    public async Task<IActionResult> Create(CrearDetalleVentaBoletoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarDetalleVentaBoletoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static DetalleVentaBoletoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new DetalleVentaBoletoDto(
            row.ToInt("DEV_ID_DETALLE_VENTA"),
            row.ToInt("DEV_ID_VENTA"),
            row.ToInt("DEV_ID_RESERVA"),
            row.ToNullableDecimal("DEV_PRECIO_BASE") ?? 0m,
            row.ToNullableDecimal("DEV_CARGOS_ADICIONALES"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearDetalleVentaBoletoDto dto) => new Dictionary<string, object?>
    {
        ["DEV_ID_VENTA"] = dto.VentaId,
        ["DEV_ID_RESERVA"] = dto.ReservaId,
        ["DEV_PRECIO_BASE"] = dto.PrecioBase,
        ["DEV_CARGOS_ADICIONALES"] = dto.CargosAdicionales
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarDetalleVentaBoletoDto dto) => new Dictionary<string, object?>
    {
        ["DEV_ID_VENTA"] = dto.VentaId,
        ["DEV_ID_RESERVA"] = dto.ReservaId,
        ["DEV_PRECIO_BASE"] = dto.PrecioBase,
        ["DEV_CARGOS_ADICIONALES"] = dto.CargosAdicionales
    };
}
