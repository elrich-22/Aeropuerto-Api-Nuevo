using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/controles-seguridad")]
public sealed class ControlesSeguridadController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CONTROL_SEGURIDAD",
        "CSE_ID_CONTROL",
        ["CSE_ID_PASAJERO", "CSE_ID_VUELO", "CSE_ID_EMPLEADO", "CSE_RESULTADO", "CSE_FECHA_HORA", "CSE_OBSERVACION"],
        ["CSE_ID_PASAJERO", "CSE_ID_VUELO", "CSE_ID_EMPLEADO", "CSE_RESULTADO", "CSE_FECHA_HORA", "CSE_OBSERVACION"]);

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
    public async Task<IActionResult> Create(CrearControlSeguridadOperacionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarControlSeguridadOperacionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ControlSeguridadOperacionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new ControlSeguridadOperacionDto(
            row.ToInt("CSE_ID_CONTROL"),
            row.ToInt("CSE_ID_PASAJERO"),
            row.ToInt("CSE_ID_VUELO"),
            row.ToNullableInt("CSE_ID_EMPLEADO"),
            row.ToStringValue("CSE_RESULTADO"),
            row.ToNullableDateTime("CSE_FECHA_HORA"),
            row.ToNullableString("CSE_OBSERVACION"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearControlSeguridadOperacionDto dto) => new Dictionary<string, object?>
    {
        ["CSE_ID_PASAJERO"] = dto.PasajeroId,
        ["CSE_ID_VUELO"] = dto.VueloId,
        ["CSE_ID_EMPLEADO"] = dto.EmpleadoId,
        ["CSE_RESULTADO"] = dto.Resultado,
        ["CSE_FECHA_HORA"] = dto.FechaHora,
        ["CSE_OBSERVACION"] = dto.Observacion
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarControlSeguridadOperacionDto dto) => new Dictionary<string, object?>
    {
        ["CSE_ID_PASAJERO"] = dto.PasajeroId,
        ["CSE_ID_VUELO"] = dto.VueloId,
        ["CSE_ID_EMPLEADO"] = dto.EmpleadoId,
        ["CSE_RESULTADO"] = dto.Resultado,
        ["CSE_FECHA_HORA"] = dto.FechaHora,
        ["CSE_OBSERVACION"] = dto.Observacion
    };
}
