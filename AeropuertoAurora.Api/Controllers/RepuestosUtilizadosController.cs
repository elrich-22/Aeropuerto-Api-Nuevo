using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/repuestos-utilizados")]
public sealed class RepuestosUtilizadosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_REPUESTOUTILIZADO",
        "RUT_ID_REPUESTO_UTILIZADO",
        ["RUT_ID_MANTENIMIENTO", "RUT_ID_REPUESTO", "RUT_CANTIDAD"],
        ["RUT_ID_MANTENIMIENTO", "RUT_ID_REPUESTO", "RUT_CANTIDAD"]);

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
    public async Task<IActionResult> Create(CrearRepuestoUtilizadoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarRepuestoUtilizadoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static RepuestoUtilizadoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("RUT_ID_REPUESTO_UTILIZADO"),
        row.ToInt("RUT_ID_MANTENIMIENTO"),
        row.ToInt("RUT_ID_REPUESTO"),
        row.ToInt("RUT_CANTIDAD"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearRepuestoUtilizadoDto dto) => new Dictionary<string, object?>
    {
        ["RUT_ID_MANTENIMIENTO"] = dto.MantenimientoId,
        ["RUT_ID_REPUESTO"] = dto.RepuestoId,
        ["RUT_CANTIDAD"] = dto.Cantidad
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarRepuestoUtilizadoDto dto) => ToValues(new CrearRepuestoUtilizadoDto(
        dto.MantenimientoId, dto.RepuestoId, dto.Cantidad));
}
