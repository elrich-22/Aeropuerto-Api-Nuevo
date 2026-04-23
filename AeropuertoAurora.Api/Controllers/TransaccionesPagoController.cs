using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/transacciones-pago")]
public sealed class TransaccionesPagoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_TRANSACCIONPAGO",
        "TRA_ID_TRANSACCION",
        ["TRA_ID_RESERVA", "TRA_ID_METODO_PAGO", "TRA_MONTO_TOTAL", "TRA_MONEDA", "TRA_FECHA_TRANSACCION", "TRA_ESTADO", "TRA_NUMERO_AUTORIZACION", "TRA_REFERENCIA_EXTERNA", "TRA_IP_CLIENTE", "TRA_DETALLES_TARJETA"],
        ["TRA_ID_RESERVA", "TRA_ID_METODO_PAGO", "TRA_MONTO_TOTAL", "TRA_MONEDA", "TRA_FECHA_TRANSACCION", "TRA_ESTADO", "TRA_NUMERO_AUTORIZACION", "TRA_REFERENCIA_EXTERNA", "TRA_IP_CLIENTE", "TRA_DETALLES_TARJETA"]);

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
    public async Task<IActionResult> Create(CrearTransaccionPagoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarTransaccionPagoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static TransaccionPagoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new TransaccionPagoDto(
            row.ToInt("TRA_ID_TRANSACCION"),
            row.ToInt("TRA_ID_RESERVA"),
            row.ToInt("TRA_ID_METODO_PAGO"),
            row.ToNullableDecimal("TRA_MONTO_TOTAL") ?? 0m,
            row.ToStringValue("TRA_MONEDA"),
            row.ToNullableDateTime("TRA_FECHA_TRANSACCION"),
            row.ToStringValue("TRA_ESTADO"),
            row.ToNullableString("TRA_NUMERO_AUTORIZACION"),
            row.ToNullableString("TRA_REFERENCIA_EXTERNA"),
            row.ToNullableString("TRA_IP_CLIENTE"),
            row.ToNullableString("TRA_DETALLES_TARJETA"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearTransaccionPagoDto dto) => new Dictionary<string, object?>
    {
        ["TRA_ID_RESERVA"] = dto.ReservaId,
        ["TRA_ID_METODO_PAGO"] = dto.MetodoPagoId,
        ["TRA_MONTO_TOTAL"] = dto.MontoTotal,
        ["TRA_MONEDA"] = dto.Moneda,
        ["TRA_FECHA_TRANSACCION"] = dto.FechaTransaccion,
        ["TRA_ESTADO"] = dto.Estado,
        ["TRA_NUMERO_AUTORIZACION"] = dto.NumeroAutorizacion,
        ["TRA_REFERENCIA_EXTERNA"] = dto.ReferenciaExterna,
        ["TRA_IP_CLIENTE"] = dto.IpCliente,
        ["TRA_DETALLES_TARJETA"] = dto.DetallesTarjeta
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarTransaccionPagoDto dto) => new Dictionary<string, object?>
    {
        ["TRA_ID_RESERVA"] = dto.ReservaId,
        ["TRA_ID_METODO_PAGO"] = dto.MetodoPagoId,
        ["TRA_MONTO_TOTAL"] = dto.MontoTotal,
        ["TRA_MONEDA"] = dto.Moneda,
        ["TRA_FECHA_TRANSACCION"] = dto.FechaTransaccion,
        ["TRA_ESTADO"] = dto.Estado,
        ["TRA_NUMERO_AUTORIZACION"] = dto.NumeroAutorizacion,
        ["TRA_REFERENCIA_EXTERNA"] = dto.ReferenciaExterna,
        ["TRA_IP_CLIENTE"] = dto.IpCliente,
        ["TRA_DETALLES_TARJETA"] = dto.DetallesTarjeta
    };
}
