using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/busquedas-vuelo")]
public sealed class BusquedasVueloController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_BUSQUEDAVUELO",
        "BUS_ID_BUSQUEDA",
        ["BUS_ID_SESION", "BUS_ID_AEROPUERTO_ORIGEN", "BUS_ID_AEROPUERTO_DESTINO", "BUS_FECHA_IDA", "BUS_FECHA_VUELTA", "BUS_NUMERO_PASAJEROS", "BUS_CLASE", "BUS_FECHA_BUSQUEDA", "BUS_SE_CONVIRTIO_COMPRA"],
        ["BUS_ID_SESION", "BUS_ID_AEROPUERTO_ORIGEN", "BUS_ID_AEROPUERTO_DESTINO", "BUS_FECHA_IDA", "BUS_FECHA_VUELTA", "BUS_NUMERO_PASAJEROS", "BUS_CLASE", "BUS_FECHA_BUSQUEDA", "BUS_SE_CONVIRTIO_COMPRA"]);

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
    public async Task<IActionResult> Create(CrearBusquedaVueloDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarBusquedaVueloDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static BusquedaVueloDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("BUS_ID_BUSQUEDA"),
        row.ToNullableInt("BUS_ID_SESION"),
        row.ToInt("BUS_ID_AEROPUERTO_ORIGEN"),
        row.ToInt("BUS_ID_AEROPUERTO_DESTINO"),
        row.ToDateTimeValue("BUS_FECHA_IDA"),
        row.ToNullableDateTime("BUS_FECHA_VUELTA"),
        row.ToInt("BUS_NUMERO_PASAJEROS"),
        row.ToNullableString("BUS_CLASE"),
        row.ToNullableDateTime("BUS_FECHA_BUSQUEDA"),
        row.ToStringValue("BUS_SE_CONVIRTIO_COMPRA"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearBusquedaVueloDto dto) => new Dictionary<string, object?>
    {
        ["BUS_ID_SESION"] = dto.SesionId,
        ["BUS_ID_AEROPUERTO_ORIGEN"] = dto.AeropuertoOrigenId,
        ["BUS_ID_AEROPUERTO_DESTINO"] = dto.AeropuertoDestinoId,
        ["BUS_FECHA_IDA"] = dto.FechaIda,
        ["BUS_FECHA_VUELTA"] = dto.FechaVuelta,
        ["BUS_NUMERO_PASAJEROS"] = dto.NumeroPasajeros,
        ["BUS_CLASE"] = dto.Clase,
        ["BUS_FECHA_BUSQUEDA"] = dto.FechaBusqueda,
        ["BUS_SE_CONVIRTIO_COMPRA"] = dto.SeConvirtioCompra
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarBusquedaVueloDto dto) => ToValues(new CrearBusquedaVueloDto(
        dto.SesionId, dto.AeropuertoOrigenId, dto.AeropuertoDestinoId, dto.FechaIda, dto.FechaVuelta, dto.NumeroPasajeros, dto.Clase, dto.FechaBusqueda, dto.SeConvirtioCompra));
}
