using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/carritos-compra")]
public sealed class CarritosCompraController(IOracleCrudRepository repository, IAeropuertoQueryService service) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CARRITOCOMPRA",
        "CAR_ID_CARRITO",
        ["CAR_ID_PASAJERO", "CAR_SESION_ID", "CAR_FECHA_CREACION", "CAR_FECHA_EXPIRACION", "CAR_ESTADO"],
        ["CAR_ID_PASAJERO", "CAR_SESION_ID", "CAR_FECHA_CREACION", "CAR_FECHA_EXPIRACION", "CAR_ESTADO"]);

    private static readonly CrudTableDefinition ItemsTable = new(
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

    [HttpGet("pasajero/{pasajeroId:int}/items")]
    public async Task<IActionResult> GetItemsByPassenger(int pasajeroId, CancellationToken cancellationToken)
    {
        var cart = await GetActiveCartAsync(pasajeroId, createIfMissing: false, cancellationToken);
        if (cart is null)
        {
            return Ok(Array.Empty<ItemCarritoUsuarioDto>());
        }

        return Ok(await GetCartItemsAsync(cart.Value.Id, cancellationToken));
    }

    [HttpPost("pasajero/{pasajeroId:int}/items")]
    public async Task<IActionResult> AddItemForPassenger(
        int pasajeroId,
        [FromBody] AgregarItemCarritoUsuarioDto dto,
        CancellationToken cancellationToken)
    {
        if (pasajeroId <= 0 || dto.VueloId <= 0)
        {
            return BadRequest(new { message = "Pasajero y vuelo son obligatorios." });
        }

        var cart = await GetActiveCartAsync(pasajeroId, createIfMissing: true, cancellationToken);
        var cartId = cart?.Id ?? throw new InvalidOperationException("No se pudo crear el carrito.");
        var quantity = Math.Max(1, dto.Cantidad);
        var price = dto.PrecioUnitario > 0 ? dto.PrecioUnitario : 1250m;

        var id = await repository.CreateAsync(ItemsTable, new Dictionary<string, object?>
        {
            ["ITE_ID_CARRITO"] = cartId,
            ["ITE_ID_VUELO"] = dto.VueloId,
            ["ITE_NUMERO_ASIENTO"] = null,
            ["ITE_CLASE"] = NormalizeClass(dto.Clase),
            ["ITE_PRECIO_UNITARIO"] = price,
            ["ITE_CANTIDAD"] = quantity
        }, cancellationToken);

        var item = (await GetCartItemsAsync(cartId, cancellationToken)).FirstOrDefault(item => item.Id == id);
        return CreatedAtAction(nameof(GetItemsByPassenger), new { pasajeroId }, item);
    }

    [HttpDelete("pasajero/{pasajeroId:int}/items/{itemId:int}")]
    public async Task<IActionResult> DeletePassengerItem(int pasajeroId, int itemId, CancellationToken cancellationToken)
    {
        var cart = await GetActiveCartAsync(pasajeroId, createIfMissing: false, cancellationToken);
        if (cart is null)
        {
            return NotFound(new { message = "El pasajero no tiene carrito activo." });
        }

        var item = (await repository.GetByIdAsync(ItemsTable, itemId, cancellationToken));
        if (item is null || item.ToInt("ITE_ID_CARRITO") != cart.Value.Id)
        {
            return NotFound(new { message = "El item no existe en el carrito del pasajero." });
        }

        return await repository.DeleteAsync(ItemsTable, itemId, cancellationToken) ? NoContent() : NotFound();
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

    private async Task<(int Id, int PasajeroId)?> GetActiveCartAsync(int pasajeroId, bool createIfMissing, CancellationToken cancellationToken)
    {
        var carts = await repository.GetByColumnAsync(Table, "CAR_ID_PASAJERO", pasajeroId, cancellationToken);
        var active = carts
            .Where(row => string.Equals(row.ToStringValue("CAR_ESTADO"), "ACTIVO", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(row => row.ToInt("CAR_ID_CARRITO"))
            .FirstOrDefault();

        if (active is not null)
        {
            return (active.ToInt("CAR_ID_CARRITO"), pasajeroId);
        }

        if (!createIfMissing)
        {
            return null;
        }

        var now = DateTime.Now;
        var id = await repository.CreateAsync(Table, new Dictionary<string, object?>
        {
            ["CAR_ID_PASAJERO"] = pasajeroId,
            ["CAR_SESION_ID"] = $"shared-{pasajeroId}",
            ["CAR_FECHA_CREACION"] = now,
            ["CAR_FECHA_EXPIRACION"] = now.AddDays(7),
            ["CAR_ESTADO"] = "ACTIVO"
        }, cancellationToken);

        return (id, pasajeroId);
    }

    private async Task<IReadOnlyList<ItemCarritoUsuarioDto>> GetCartItemsAsync(int cartId, CancellationToken cancellationToken)
    {
        var items = (await repository.GetByColumnAsync(ItemsTable, "ITE_ID_CARRITO", cartId, cancellationToken))
            .OrderBy(row => row.ToInt("ITE_ID_ITEM_CARRITO"))
            .ToList();
        var result = new List<ItemCarritoUsuarioDto>();

        foreach (var row in items)
        {
            var flight = await service.GetFlightByIdAsync(row.ToInt("ITE_ID_VUELO"), cancellationToken);
            if (flight is null)
            {
                continue;
            }

            result.Add(new ItemCarritoUsuarioDto(
                row.ToInt("ITE_ID_ITEM_CARRITO"),
                cartId,
                flight.Id,
                flight.NumeroVuelo,
                flight.Aerolinea,
                flight.Origen,
                flight.Destino,
                flight.FechaVuelo,
                flight.SalidaReal,
                flight.LlegadaReal,
                flight.PlazasOcupadas,
                flight.PlazasDisponibles,
                flight.Estado,
                flight.RetrasoMinutos,
                flight.MatriculaAvion,
                row.ToNullableString("ITE_CLASE"),
                row.ToInt("ITE_CANTIDAD"),
                row.ToNullableDecimal("ITE_PRECIO_UNITARIO") ?? 0m));
        }

        return result;
    }

    private static string NormalizeClass(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return normalized is "ejecutiva" or "primera" ? normalized : "economica";
    }
}
