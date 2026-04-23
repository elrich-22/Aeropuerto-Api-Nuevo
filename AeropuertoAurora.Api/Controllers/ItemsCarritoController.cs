using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/items-carrito")]
public sealed class ItemsCarritoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ITEMCARRITO",
        "ITE_ID_ITEM_CARRITO",
        ["ITE_ID_CARRITO", "ITE_ID_VUELO", "ITE_NUMERO_ASIENTO", "ITE_CLASE", "ITE_PRECIO_UNITARIO", "ITE_CANTIDAD"],
        ["ITE_ID_CARRITO", "ITE_ID_VUELO", "ITE_NUMERO_ASIENTO", "ITE_CLASE", "ITE_PRECIO_UNITARIO", "ITE_CANTIDAD"]);

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
    public async Task<IActionResult> Create(CrearItemCarritoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarItemCarritoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ItemCarritoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new ItemCarritoDto(
            row.ToInt("ITE_ID_ITEM_CARRITO"),
            row.ToInt("ITE_ID_CARRITO"),
            row.ToInt("ITE_ID_VUELO"),
            row.ToNullableString("ITE_NUMERO_ASIENTO"),
            row.ToNullableString("ITE_CLASE"),
            row.ToNullableDecimal("ITE_PRECIO_UNITARIO") ?? 0m,
            row.ToInt("ITE_CANTIDAD"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearItemCarritoDto dto) => new Dictionary<string, object?>
    {
        ["ITE_ID_CARRITO"] = dto.CarritoId,
        ["ITE_ID_VUELO"] = dto.VueloId,
        ["ITE_NUMERO_ASIENTO"] = dto.NumeroAsiento,
        ["ITE_CLASE"] = dto.Clase,
        ["ITE_PRECIO_UNITARIO"] = dto.PrecioUnitario,
        ["ITE_CANTIDAD"] = dto.Cantidad
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarItemCarritoDto dto) => new Dictionary<string, object?>
    {
        ["ITE_ID_CARRITO"] = dto.CarritoId,
        ["ITE_ID_VUELO"] = dto.VueloId,
        ["ITE_NUMERO_ASIENTO"] = dto.NumeroAsiento,
        ["ITE_CLASE"] = dto.Clase,
        ["ITE_PRECIO_UNITARIO"] = dto.PrecioUnitario,
        ["ITE_CANTIDAD"] = dto.Cantidad
    };
}
