using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/tripulacion")]
public sealed class TripulacionController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_TRIPULACION",
        "TRI_ID_TRIPULACION",
        ["TRI_ID_VUELO", "TRI_ID_EMPLEADO", "TRI_ROL", "TRI_HORAS_VUELO"],
        ["TRI_ID_VUELO", "TRI_ID_EMPLEADO", "TRI_ROL", "TRI_HORAS_VUELO"]);

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
    public async Task<IActionResult> Create(CrearTripulacionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarTripulacionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static TripulacionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new TripulacionDto(
            row.ToInt("TRI_ID_TRIPULACION"),
            row.ToInt("TRI_ID_VUELO"),
            row.ToInt("TRI_ID_EMPLEADO"),
            row.ToStringValue("TRI_ROL"),
            row.ToNullableDecimal("TRI_HORAS_VUELO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearTripulacionDto dto) => new Dictionary<string, object?>
    {
        ["TRI_ID_VUELO"] = dto.VueloId,
        ["TRI_ID_EMPLEADO"] = dto.EmpleadoId,
        ["TRI_ROL"] = dto.Rol,
        ["TRI_HORAS_VUELO"] = dto.HorasVuelo
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarTripulacionDto dto) => new Dictionary<string, object?>
    {
        ["TRI_ID_VUELO"] = dto.VueloId,
        ["TRI_ID_EMPLEADO"] = dto.EmpleadoId,
        ["TRI_ROL"] = dto.Rol,
        ["TRI_HORAS_VUELO"] = dto.HorasVuelo
    };
}
