using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/checkin")]
public sealed class CheckInController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CHECKIN",
        "CHK_ID_CHECKIN",
        ["CHK_ID_RESERVA", "CHK_ID_PASAJERO", "CHK_ID_VUELO", "CHK_FECHA_HORA", "CHK_METODO", "CHK_ESTADO"],
        ["CHK_ID_RESERVA", "CHK_ID_PASAJERO", "CHK_ID_VUELO", "CHK_FECHA_HORA", "CHK_METODO", "CHK_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearCheckInDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarCheckInDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static CheckInDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new CheckInDto(
            row.ToInt("CHK_ID_CHECKIN"),
            row.ToInt("CHK_ID_RESERVA"),
            row.ToInt("CHK_ID_PASAJERO"),
            row.ToInt("CHK_ID_VUELO"),
            row.ToNullableDateTime("CHK_FECHA_HORA"),
            row.ToStringValue("CHK_METODO"),
            row.ToStringValue("CHK_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearCheckInDto dto) => new Dictionary<string, object?>
    {
        ["CHK_ID_RESERVA"] = dto.ReservaId,
        ["CHK_ID_PASAJERO"] = dto.PasajeroId,
        ["CHK_ID_VUELO"] = dto.VueloId,
        ["CHK_FECHA_HORA"] = dto.FechaHora,
        ["CHK_METODO"] = dto.Metodo,
        ["CHK_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarCheckInDto dto) => new Dictionary<string, object?>
    {
        ["CHK_ID_RESERVA"] = dto.ReservaId,
        ["CHK_ID_PASAJERO"] = dto.PasajeroId,
        ["CHK_ID_VUELO"] = dto.VueloId,
        ["CHK_FECHA_HORA"] = dto.FechaHora,
        ["CHK_METODO"] = dto.Metodo,
        ["CHK_ESTADO"] = dto.Estado
    };
}
