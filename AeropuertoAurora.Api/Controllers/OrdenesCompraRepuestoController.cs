using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/ordenes-compra-repuesto")]
public sealed class OrdenesCompraRepuestoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ORDENCOMPRAREPUESTO",
        "ORC_ID_ORDEN_COMPRA",
        ["ORC_NUMERO_ORDEN", "ORC_ID_PROVEEDOR", "ORC_FECHA_ORDEN", "ORC_FECHA_ENTREGA_ESPERADA", "ORC_FECHA_ENTREGA_REAL", "ORC_MONTO_TOTAL", "ORC_ESTADO", "ORC_ID_EMPLEADO_SOLICITA", "ORC_OBSERVACIONES"],
        ["ORC_NUMERO_ORDEN", "ORC_ID_PROVEEDOR", "ORC_FECHA_ORDEN", "ORC_FECHA_ENTREGA_ESPERADA", "ORC_FECHA_ENTREGA_REAL", "ORC_MONTO_TOTAL", "ORC_ESTADO", "ORC_ID_EMPLEADO_SOLICITA", "ORC_OBSERVACIONES"]);

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
    public async Task<IActionResult> Create(CrearOrdenCompraRepuestoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarOrdenCompraRepuestoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static OrdenCompraRepuestoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("ORC_ID_ORDEN_COMPRA"),
        row.ToStringValue("ORC_NUMERO_ORDEN"),
        row.ToInt("ORC_ID_PROVEEDOR"),
        row.ToNullableDateTime("ORC_FECHA_ORDEN"),
        row.ToNullableDateTime("ORC_FECHA_ENTREGA_ESPERADA"),
        row.ToNullableDateTime("ORC_FECHA_ENTREGA_REAL"),
        row.ToNullableDecimal("ORC_MONTO_TOTAL"),
        row.ToStringValue("ORC_ESTADO"),
        row.ToNullableInt("ORC_ID_EMPLEADO_SOLICITA"),
        row.ToNullableString("ORC_OBSERVACIONES"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearOrdenCompraRepuestoDto dto) => new Dictionary<string, object?>
    {
        ["ORC_NUMERO_ORDEN"] = dto.NumeroOrden,
        ["ORC_ID_PROVEEDOR"] = dto.ProveedorId,
        ["ORC_FECHA_ORDEN"] = dto.FechaOrden,
        ["ORC_FECHA_ENTREGA_ESPERADA"] = dto.FechaEntregaEsperada,
        ["ORC_FECHA_ENTREGA_REAL"] = dto.FechaEntregaReal,
        ["ORC_MONTO_TOTAL"] = dto.MontoTotal,
        ["ORC_ESTADO"] = dto.Estado,
        ["ORC_ID_EMPLEADO_SOLICITA"] = dto.EmpleadoSolicitaId,
        ["ORC_OBSERVACIONES"] = dto.Observaciones
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarOrdenCompraRepuestoDto dto) => ToValues(new CrearOrdenCompraRepuestoDto(
        dto.NumeroOrden, dto.ProveedorId, dto.FechaOrden, dto.FechaEntregaEsperada, dto.FechaEntregaReal, dto.MontoTotal, dto.Estado, dto.EmpleadoSolicitaId, dto.Observaciones));
}
