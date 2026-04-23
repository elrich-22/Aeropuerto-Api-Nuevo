using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/carritos-compra")]
public sealed class CarritosCompraController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CARRITOCOMPRA",
        "CAR_ID_CARRITO",
        ["CAR_ID_PASAJERO", "CAR_SESION_ID", "CAR_FECHA_CREACION", "CAR_FECHA_EXPIRACION", "CAR_ESTADO"],
        ["CAR_ID_PASAJERO", "CAR_SESION_ID", "CAR_FECHA_CREACION", "CAR_FECHA_EXPIRACION", "CAR_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearCarritoCompraDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarCarritoCompraDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static CarritoCompraDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new CarritoCompraDto(
            row.ToInt("CAR_ID_CARRITO"),
            row.ToInt("CAR_ID_PASAJERO"),
            row.ToNullableString("CAR_SESION_ID"),
            row.ToNullableDateTime("CAR_FECHA_CREACION"),
            row.ToNullableDateTime("CAR_FECHA_EXPIRACION"),
            row.ToStringValue("CAR_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearCarritoCompraDto dto) => new Dictionary<string, object?>
    {
        ["CAR_ID_PASAJERO"] = dto.PasajeroId,
        ["CAR_SESION_ID"] = dto.SesionId,
        ["CAR_FECHA_CREACION"] = dto.FechaCreacion,
        ["CAR_FECHA_EXPIRACION"] = dto.FechaExpiracion,
        ["CAR_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarCarritoCompraDto dto) => new Dictionary<string, object?>
    {
        ["CAR_ID_PASAJERO"] = dto.PasajeroId,
        ["CAR_SESION_ID"] = dto.SesionId,
        ["CAR_FECHA_CREACION"] = dto.FechaCreacion,
        ["CAR_FECHA_EXPIRACION"] = dto.FechaExpiracion,
        ["CAR_ESTADO"] = dto.Estado
    };
}
