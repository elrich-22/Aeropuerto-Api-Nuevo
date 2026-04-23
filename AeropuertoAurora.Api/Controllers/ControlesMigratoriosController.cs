using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/controles-migratorios")]
public sealed class ControlesMigratoriosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CONTROL_MIGRATORIO",
        "CMI_ID_CONTROL",
        ["CMI_ID_PASAJERO", "CMI_ID_VUELO", "CMI_ID_EMPLEADO", "CMI_TIPO", "CMI_RESULTADO", "CMI_FECHA_HORA", "CMI_OBSERVACION"],
        ["CMI_ID_PASAJERO", "CMI_ID_VUELO", "CMI_ID_EMPLEADO", "CMI_TIPO", "CMI_RESULTADO", "CMI_FECHA_HORA", "CMI_OBSERVACION"]);

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
    public async Task<IActionResult> Create(CrearControlMigratorioDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarControlMigratorioDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ControlMigratorioDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new ControlMigratorioDto(
            row.ToInt("CMI_ID_CONTROL"),
            row.ToInt("CMI_ID_PASAJERO"),
            row.ToInt("CMI_ID_VUELO"),
            row.ToNullableInt("CMI_ID_EMPLEADO"),
            row.ToStringValue("CMI_TIPO"),
            row.ToStringValue("CMI_RESULTADO"),
            row.ToNullableDateTime("CMI_FECHA_HORA"),
            row.ToNullableString("CMI_OBSERVACION"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearControlMigratorioDto dto) => new Dictionary<string, object?>
    {
        ["CMI_ID_PASAJERO"] = dto.PasajeroId,
        ["CMI_ID_VUELO"] = dto.VueloId,
        ["CMI_ID_EMPLEADO"] = dto.EmpleadoId,
        ["CMI_TIPO"] = dto.Tipo,
        ["CMI_RESULTADO"] = dto.Resultado,
        ["CMI_FECHA_HORA"] = dto.FechaHora,
        ["CMI_OBSERVACION"] = dto.Observacion
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarControlMigratorioDto dto) => new Dictionary<string, object?>
    {
        ["CMI_ID_PASAJERO"] = dto.PasajeroId,
        ["CMI_ID_VUELO"] = dto.VueloId,
        ["CMI_ID_EMPLEADO"] = dto.EmpleadoId,
        ["CMI_TIPO"] = dto.Tipo,
        ["CMI_RESULTADO"] = dto.Resultado,
        ["CMI_FECHA_HORA"] = dto.FechaHora,
        ["CMI_OBSERVACION"] = dto.Observacion
    };
}
