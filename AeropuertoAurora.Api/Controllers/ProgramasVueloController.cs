using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/programas-vuelo")]
public sealed class ProgramasVueloController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PROGRAMAVUELO",
        "PRV_ID",
        ["PRV_NUMERO_VUELO", "PRV_ID_AEROLINEA", "PRV_ID_AEROPUERTO_ORIGEN", "PRV_ID_AEROPUERTO_DESTINO", "PRV_HORA_SALIDA_PROGRAMADA", "PRV_HORA_LLEGADA_PROGRAMADA", "PRV_DURACION_ESTIMADA", "PRV_TIPO_VUELO", "PRV_ESTADO"],
        ["PRV_NUMERO_VUELO", "PRV_ID_AEROLINEA", "PRV_ID_AEROPUERTO_ORIGEN", "PRV_ID_AEROPUERTO_DESTINO", "PRV_HORA_SALIDA_PROGRAMADA", "PRV_HORA_LLEGADA_PROGRAMADA", "PRV_DURACION_ESTIMADA", "PRV_TIPO_VUELO", "PRV_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearProgramaVueloDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarProgramaVueloDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ProgramaVueloDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new ProgramaVueloDto(
            row.ToInt("PRV_ID"),
            row.ToStringValue("PRV_NUMERO_VUELO"),
            row.ToInt("PRV_ID_AEROLINEA"),
            row.ToInt("PRV_ID_AEROPUERTO_ORIGEN"),
            row.ToInt("PRV_ID_AEROPUERTO_DESTINO"),
            row.ToStringValue("PRV_HORA_SALIDA_PROGRAMADA"),
            row.ToStringValue("PRV_HORA_LLEGADA_PROGRAMADA"),
            row.ToNullableInt("PRV_DURACION_ESTIMADA"),
            row.ToNullableString("PRV_TIPO_VUELO"),
            row.ToStringValue("PRV_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearProgramaVueloDto dto) => new Dictionary<string, object?>
    {
        ["PRV_NUMERO_VUELO"] = dto.NumeroVuelo,
        ["PRV_ID_AEROLINEA"] = dto.AerolineaId,
        ["PRV_ID_AEROPUERTO_ORIGEN"] = dto.AeropuertoOrigenId,
        ["PRV_ID_AEROPUERTO_DESTINO"] = dto.AeropuertoDestinoId,
        ["PRV_HORA_SALIDA_PROGRAMADA"] = dto.HoraSalidaProgramada,
        ["PRV_HORA_LLEGADA_PROGRAMADA"] = dto.HoraLlegadaProgramada,
        ["PRV_DURACION_ESTIMADA"] = dto.DuracionEstimada,
        ["PRV_TIPO_VUELO"] = dto.TipoVuelo,
        ["PRV_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarProgramaVueloDto dto) => new Dictionary<string, object?>
    {
        ["PRV_NUMERO_VUELO"] = dto.NumeroVuelo,
        ["PRV_ID_AEROLINEA"] = dto.AerolineaId,
        ["PRV_ID_AEROPUERTO_ORIGEN"] = dto.AeropuertoOrigenId,
        ["PRV_ID_AEROPUERTO_DESTINO"] = dto.AeropuertoDestinoId,
        ["PRV_HORA_SALIDA_PROGRAMADA"] = dto.HoraSalidaProgramada,
        ["PRV_HORA_LLEGADA_PROGRAMADA"] = dto.HoraLlegadaProgramada,
        ["PRV_DURACION_ESTIMADA"] = dto.DuracionEstimada,
        ["PRV_TIPO_VUELO"] = dto.TipoVuelo,
        ["PRV_ESTADO"] = dto.Estado
    };
}
