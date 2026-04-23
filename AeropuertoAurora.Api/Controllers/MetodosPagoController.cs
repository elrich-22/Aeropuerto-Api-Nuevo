using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/metodos-pago")]
public sealed class MetodosPagoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_METODOPAGO",
        "MET_ID_METODO_PAGO",
        ["MET_NOMBRE", "MET_TIPO", "MET_ESTADO", "MET_COMISION_PORCENTAJE"],
        ["MET_NOMBRE", "MET_TIPO", "MET_ESTADO", "MET_COMISION_PORCENTAJE"]);

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
    public async Task<IActionResult> Create(CrearMetodoPagoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarMetodoPagoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static MetodoPagoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new MetodoPagoDto(
            row.ToInt("MET_ID_METODO_PAGO"),
            row.ToStringValue("MET_NOMBRE"),
            row.ToStringValue("MET_TIPO"),
            row.ToStringValue("MET_ESTADO"),
            row.ToNullableDecimal("MET_COMISION_PORCENTAJE") ?? 0m);
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearMetodoPagoDto dto) => new Dictionary<string, object?>
    {
        ["MET_NOMBRE"] = dto.Nombre,
        ["MET_TIPO"] = dto.Tipo,
        ["MET_ESTADO"] = dto.Estado,
        ["MET_COMISION_PORCENTAJE"] = dto.ComisionPorcentaje
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarMetodoPagoDto dto) => new Dictionary<string, object?>
    {
        ["MET_NOMBRE"] = dto.Nombre,
        ["MET_TIPO"] = dto.Tipo,
        ["MET_ESTADO"] = dto.Estado,
        ["MET_COMISION_PORCENTAJE"] = dto.ComisionPorcentaje
    };
}
