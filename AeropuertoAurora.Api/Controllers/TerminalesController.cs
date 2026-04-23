using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/terminales")]
public sealed class TerminalesController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_TERMINAL",
        "TER_ID_TERMINAL",
        ["TER_ID_AEROPUERTO", "TER_NOMBRE", "TER_TIPO", "TER_CAPACIDAD_PASAJEROS", "TER_ESTADO"],
        ["TER_ID_AEROPUERTO", "TER_NOMBRE", "TER_TIPO", "TER_CAPACIDAD_PASAJEROS", "TER_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearTerminalDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarTerminalDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static TerminalDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new TerminalDto(
            row.ToInt("TER_ID_TERMINAL"),
            row.ToInt("TER_ID_AEROPUERTO"),
            row.ToStringValue("TER_NOMBRE"),
            row.ToStringValue("TER_TIPO"),
            row.ToNullableInt("TER_CAPACIDAD_PASAJEROS"),
            row.ToStringValue("TER_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearTerminalDto dto) => new Dictionary<string, object?>
    {
        ["TER_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["TER_NOMBRE"] = dto.Nombre,
        ["TER_TIPO"] = dto.Tipo,
        ["TER_CAPACIDAD_PASAJEROS"] = dto.CapacidadPasajeros,
        ["TER_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarTerminalDto dto) => new Dictionary<string, object?>
    {
        ["TER_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["TER_NOMBRE"] = dto.Nombre,
        ["TER_TIPO"] = dto.Tipo,
        ["TER_CAPACIDAD_PASAJEROS"] = dto.CapacidadPasajeros,
        ["TER_ESTADO"] = dto.Estado
    };
}
