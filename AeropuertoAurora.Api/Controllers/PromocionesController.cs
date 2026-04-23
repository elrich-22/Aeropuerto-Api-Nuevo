using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/promociones")]
public sealed class PromocionesController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PROMOCION",
        "PRO_ID_PROMOCION",
        ["PRO_CODIGO_PROMOCION", "PRO_DESCRIPCION", "PRO_TIPO_DESCUENTO", "PRO_VALOR_DESCUENTO", "PRO_FECHA_INICIO", "PRO_FECHA_FIN", "PRO_USOS_MAXIMOS", "PRO_USOS_ACTUALES", "PRO_ESTADO"],
        ["PRO_CODIGO_PROMOCION", "PRO_DESCRIPCION", "PRO_TIPO_DESCUENTO", "PRO_VALOR_DESCUENTO", "PRO_FECHA_INICIO", "PRO_FECHA_FIN", "PRO_USOS_MAXIMOS", "PRO_USOS_ACTUALES", "PRO_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearPromocionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarPromocionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static PromocionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new PromocionDto(
            row.ToInt("PRO_ID_PROMOCION"),
            row.ToStringValue("PRO_CODIGO_PROMOCION"),
            row.ToNullableString("PRO_DESCRIPCION"),
            row.ToStringValue("PRO_TIPO_DESCUENTO"),
            row.ToNullableDecimal("PRO_VALOR_DESCUENTO") ?? 0m,
            row.ToDateTimeValue("PRO_FECHA_INICIO"),
            row.ToDateTimeValue("PRO_FECHA_FIN"),
            row.ToNullableInt("PRO_USOS_MAXIMOS"),
            row.ToNullableInt("PRO_USOS_ACTUALES") ?? 0,
            row.ToStringValue("PRO_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearPromocionDto dto) => new Dictionary<string, object?>
    {
        ["PRO_CODIGO_PROMOCION"] = dto.Codigo,
        ["PRO_DESCRIPCION"] = dto.Descripcion,
        ["PRO_TIPO_DESCUENTO"] = dto.TipoDescuento,
        ["PRO_VALOR_DESCUENTO"] = dto.ValorDescuento,
        ["PRO_FECHA_INICIO"] = dto.FechaInicio,
        ["PRO_FECHA_FIN"] = dto.FechaFin,
        ["PRO_USOS_MAXIMOS"] = dto.UsosMaximos,
        ["PRO_USOS_ACTUALES"] = dto.UsosActuales,
        ["PRO_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarPromocionDto dto) => new Dictionary<string, object?>
    {
        ["PRO_CODIGO_PROMOCION"] = dto.Codigo,
        ["PRO_DESCRIPCION"] = dto.Descripcion,
        ["PRO_TIPO_DESCUENTO"] = dto.TipoDescuento,
        ["PRO_VALOR_DESCUENTO"] = dto.ValorDescuento,
        ["PRO_FECHA_INICIO"] = dto.FechaInicio,
        ["PRO_FECHA_FIN"] = dto.FechaFin,
        ["PRO_USOS_MAXIMOS"] = dto.UsosMaximos,
        ["PRO_USOS_ACTUALES"] = dto.UsosActuales,
        ["PRO_ESTADO"] = dto.Estado
    };
}
