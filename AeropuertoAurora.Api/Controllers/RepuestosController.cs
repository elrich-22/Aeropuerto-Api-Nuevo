using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/repuestos")]
public sealed class RepuestosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_REPUESTO",
        "REP_ID_REPUESTO",
        ["REP_CODIGO_REPUESTO", "REP_NOMBRE", "REP_DESCRIPCION", "REP_ID_CATEGORIA", "REP_ID_MODELO_AVION", "REP_NUMERO_PARTE_FABRICANTE", "REP_STOCK_MINIMO", "REP_STOCK_ACTUAL", "REP_STOCK_MAXIMO", "REP_PRECIO_UNITARIO", "REP_UBICACION_BODEGA", "REP_ESTADO"],
        ["REP_CODIGO_REPUESTO", "REP_NOMBRE", "REP_DESCRIPCION", "REP_ID_CATEGORIA", "REP_ID_MODELO_AVION", "REP_NUMERO_PARTE_FABRICANTE", "REP_STOCK_MINIMO", "REP_STOCK_ACTUAL", "REP_STOCK_MAXIMO", "REP_PRECIO_UNITARIO", "REP_UBICACION_BODEGA", "REP_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearRepuestoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarRepuestoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static RepuestoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("REP_ID_REPUESTO"),
        row.ToStringValue("REP_CODIGO_REPUESTO"),
        row.ToStringValue("REP_NOMBRE"),
        row.ToNullableString("REP_DESCRIPCION"),
        row.ToInt("REP_ID_CATEGORIA"),
        row.ToNullableInt("REP_ID_MODELO_AVION"),
        row.ToNullableString("REP_NUMERO_PARTE_FABRICANTE"),
        row.ToNullableInt("REP_STOCK_MINIMO"),
        row.ToNullableInt("REP_STOCK_ACTUAL"),
        row.ToNullableInt("REP_STOCK_MAXIMO"),
        row.ToNullableDecimal("REP_PRECIO_UNITARIO") ?? 0m,
        row.ToNullableString("REP_UBICACION_BODEGA"),
        row.ToStringValue("REP_ESTADO"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearRepuestoDto dto) => new Dictionary<string, object?>
    {
        ["REP_CODIGO_REPUESTO"] = dto.Codigo,
        ["REP_NOMBRE"] = dto.Nombre,
        ["REP_DESCRIPCION"] = dto.Descripcion,
        ["REP_ID_CATEGORIA"] = dto.CategoriaId,
        ["REP_ID_MODELO_AVION"] = dto.ModeloAvionId,
        ["REP_NUMERO_PARTE_FABRICANTE"] = dto.NumeroParteFabricante,
        ["REP_STOCK_MINIMO"] = dto.StockMinimo,
        ["REP_STOCK_ACTUAL"] = dto.StockActual,
        ["REP_STOCK_MAXIMO"] = dto.StockMaximo,
        ["REP_PRECIO_UNITARIO"] = dto.PrecioUnitario,
        ["REP_UBICACION_BODEGA"] = dto.UbicacionBodega,
        ["REP_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarRepuestoDto dto) => ToValues(new CrearRepuestoDto(
        dto.Codigo, dto.Nombre, dto.Descripcion, dto.CategoriaId, dto.ModeloAvionId, dto.NumeroParteFabricante, dto.StockMinimo, dto.StockActual, dto.StockMaximo, dto.PrecioUnitario, dto.UbicacionBodega, dto.Estado));
}
