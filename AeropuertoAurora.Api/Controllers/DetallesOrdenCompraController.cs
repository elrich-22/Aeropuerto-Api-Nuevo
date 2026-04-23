using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/detalles-orden-compra")]
public sealed class DetallesOrdenCompraController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_DETALLEORDENCOMPRA",
        "DET_ID_DETALLE",
        ["DET_ID_ORDEN_COMPRA", "DET_ID_REPUESTO", "DET_CANTIDAD", "DET_PRECIO_UNITARIO", "DET_SUBTOTAL"],
        ["DET_ID_ORDEN_COMPRA", "DET_ID_REPUESTO", "DET_CANTIDAD", "DET_PRECIO_UNITARIO", "DET_SUBTOTAL"]);

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
    public async Task<IActionResult> Create(CrearDetalleOrdenCompraDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarDetalleOrdenCompraDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static DetalleOrdenCompraDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("DET_ID_DETALLE"),
        row.ToInt("DET_ID_ORDEN_COMPRA"),
        row.ToInt("DET_ID_REPUESTO"),
        row.ToInt("DET_CANTIDAD"),
        row.ToNullableDecimal("DET_PRECIO_UNITARIO") ?? 0m,
        row.ToNullableDecimal("DET_SUBTOTAL") ?? 0m);

    private static IReadOnlyDictionary<string, object?> ToValues(CrearDetalleOrdenCompraDto dto) => new Dictionary<string, object?>
    {
        ["DET_ID_ORDEN_COMPRA"] = dto.OrdenCompraId,
        ["DET_ID_REPUESTO"] = dto.RepuestoId,
        ["DET_CANTIDAD"] = dto.Cantidad,
        ["DET_PRECIO_UNITARIO"] = dto.PrecioUnitario,
        ["DET_SUBTOTAL"] = dto.Subtotal
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarDetalleOrdenCompraDto dto) => ToValues(new CrearDetalleOrdenCompraDto(
        dto.OrdenCompraId, dto.RepuestoId, dto.Cantidad, dto.PrecioUnitario, dto.Subtotal));
}
