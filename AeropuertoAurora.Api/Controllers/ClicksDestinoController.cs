using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/clicks-destino")]
public sealed class ClicksDestinoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CLICKDESTINO",
        "CLI_ID_CLICK",
        ["CLI_ID_SESION", "CLI_ID_AEROPUERTO_DESTINO", "CLI_FECHA_CLICK", "CLI_ORIGEN_BUSQUEDA", "CLI_FECHA_VIAJE_BUSCADA", "CLI_NUMERO_PASAJEROS", "CLI_CLASE_BUSCADA"],
        ["CLI_ID_SESION", "CLI_ID_AEROPUERTO_DESTINO", "CLI_FECHA_CLICK", "CLI_ORIGEN_BUSQUEDA", "CLI_FECHA_VIAJE_BUSCADA", "CLI_NUMERO_PASAJEROS", "CLI_CLASE_BUSCADA"]);

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
    public async Task<IActionResult> Create(CrearClickDestinoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarClickDestinoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ClickDestinoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("CLI_ID_CLICK"),
        row.ToNullableInt("CLI_ID_SESION"),
        row.ToInt("CLI_ID_AEROPUERTO_DESTINO"),
        row.ToNullableDateTime("CLI_FECHA_CLICK"),
        row.ToNullableString("CLI_ORIGEN_BUSQUEDA"),
        row.ToNullableDateTime("CLI_FECHA_VIAJE_BUSCADA"),
        row.ToNullableInt("CLI_NUMERO_PASAJEROS"),
        row.ToNullableString("CLI_CLASE_BUSCADA"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearClickDestinoDto dto) => new Dictionary<string, object?>
    {
        ["CLI_ID_SESION"] = dto.SesionId,
        ["CLI_ID_AEROPUERTO_DESTINO"] = dto.AeropuertoDestinoId,
        ["CLI_FECHA_CLICK"] = dto.FechaClick,
        ["CLI_ORIGEN_BUSQUEDA"] = dto.OrigenBusqueda,
        ["CLI_FECHA_VIAJE_BUSCADA"] = dto.FechaViajeBuscada,
        ["CLI_NUMERO_PASAJEROS"] = dto.NumeroPasajeros,
        ["CLI_CLASE_BUSCADA"] = dto.ClaseBuscada
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarClickDestinoDto dto) => ToValues(new CrearClickDestinoDto(
        dto.SesionId, dto.AeropuertoDestinoId, dto.FechaClick, dto.OrigenBusqueda, dto.FechaViajeBuscada, dto.NumeroPasajeros, dto.ClaseBuscada));
}
