using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/ventas-boleto")]
public sealed class VentasBoletoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_VENTABOLETO",
        "VEN_ID_VENTA",
        ["VEN_NUMERO_VENTA", "VEN_ID_PUNTO_VENTA", "VEN_ID_EMPLEADO_VENDEDOR", "VEN_ID_PASAJERO", "VEN_FECHA_VENTA", "VEN_MONTO_SUBTOTAL", "VEN_IMPUESTOS", "VEN_DESCUENTOS", "VEN_MONTO_TOTAL", "VEN_ID_METODO_PAGO", "VEN_CANAL_VENTA", "VEN_ESTADO"],
        ["VEN_NUMERO_VENTA", "VEN_ID_PUNTO_VENTA", "VEN_ID_EMPLEADO_VENDEDOR", "VEN_ID_PASAJERO", "VEN_FECHA_VENTA", "VEN_MONTO_SUBTOTAL", "VEN_IMPUESTOS", "VEN_DESCUENTOS", "VEN_MONTO_TOTAL", "VEN_ID_METODO_PAGO", "VEN_CANAL_VENTA", "VEN_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearVentaBoletoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarVentaBoletoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static VentaBoletoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new VentaBoletoDto(
            row.ToInt("VEN_ID_VENTA"),
            row.ToStringValue("VEN_NUMERO_VENTA"),
            row.ToNullableInt("VEN_ID_PUNTO_VENTA"),
            row.ToNullableInt("VEN_ID_EMPLEADO_VENDEDOR"),
            row.ToInt("VEN_ID_PASAJERO"),
            row.ToNullableDateTime("VEN_FECHA_VENTA"),
            row.ToNullableDecimal("VEN_MONTO_SUBTOTAL") ?? 0m,
            row.ToNullableDecimal("VEN_IMPUESTOS"),
            row.ToNullableDecimal("VEN_DESCUENTOS"),
            row.ToNullableDecimal("VEN_MONTO_TOTAL") ?? 0m,
            row.ToInt("VEN_ID_METODO_PAGO"),
            row.ToStringValue("VEN_CANAL_VENTA"),
            row.ToStringValue("VEN_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearVentaBoletoDto dto) => new Dictionary<string, object?>
    {
        ["VEN_NUMERO_VENTA"] = dto.NumeroVenta,
        ["VEN_ID_PUNTO_VENTA"] = dto.PuntoVentaId,
        ["VEN_ID_EMPLEADO_VENDEDOR"] = dto.EmpleadoVendedorId,
        ["VEN_ID_PASAJERO"] = dto.PasajeroId,
        ["VEN_FECHA_VENTA"] = dto.FechaVenta,
        ["VEN_MONTO_SUBTOTAL"] = dto.MontoSubtotal,
        ["VEN_IMPUESTOS"] = dto.Impuestos,
        ["VEN_DESCUENTOS"] = dto.Descuentos,
        ["VEN_MONTO_TOTAL"] = dto.MontoTotal,
        ["VEN_ID_METODO_PAGO"] = dto.MetodoPagoId,
        ["VEN_CANAL_VENTA"] = dto.CanalVenta,
        ["VEN_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarVentaBoletoDto dto) => new Dictionary<string, object?>
    {
        ["VEN_NUMERO_VENTA"] = dto.NumeroVenta,
        ["VEN_ID_PUNTO_VENTA"] = dto.PuntoVentaId,
        ["VEN_ID_EMPLEADO_VENDEDOR"] = dto.EmpleadoVendedorId,
        ["VEN_ID_PASAJERO"] = dto.PasajeroId,
        ["VEN_FECHA_VENTA"] = dto.FechaVenta,
        ["VEN_MONTO_SUBTOTAL"] = dto.MontoSubtotal,
        ["VEN_IMPUESTOS"] = dto.Impuestos,
        ["VEN_DESCUENTOS"] = dto.Descuentos,
        ["VEN_MONTO_TOTAL"] = dto.MontoTotal,
        ["VEN_ID_METODO_PAGO"] = dto.MetodoPagoId,
        ["VEN_CANAL_VENTA"] = dto.CanalVenta,
        ["VEN_ESTADO"] = dto.Estado
    };
}
