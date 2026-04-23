using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/escalas-tecnicas")]
public sealed class EscalasTecnicasController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ESCALATECNICA",
        "ESC_ID_ESCALA",
        ["ESC_ID_PROGRAMA_VUELO", "ESC_ID_AEROPUERTO", "ESC_NUMERO_ORDEN", "ESC_HORA_LLEGADA_ESTIMADA", "ESC_HORA_SALIDA_ESTIMADA", "ESC_DURACION_ESCALA"],
        ["ESC_ID_PROGRAMA_VUELO", "ESC_ID_AEROPUERTO", "ESC_NUMERO_ORDEN", "ESC_HORA_LLEGADA_ESTIMADA", "ESC_HORA_SALIDA_ESTIMADA", "ESC_DURACION_ESCALA"]);

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
    public async Task<IActionResult> Create(CrearEscalaTecnicaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarEscalaTecnicaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static EscalaTecnicaDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new EscalaTecnicaDto(
            row.ToInt("ESC_ID_ESCALA"),
            row.ToInt("ESC_ID_PROGRAMA_VUELO"),
            row.ToInt("ESC_ID_AEROPUERTO"),
            row.ToInt("ESC_NUMERO_ORDEN"),
            row.ToStringValue("ESC_HORA_LLEGADA_ESTIMADA"),
            row.ToStringValue("ESC_HORA_SALIDA_ESTIMADA"),
            row.ToNullableInt("ESC_DURACION_ESCALA"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearEscalaTecnicaDto dto) => new Dictionary<string, object?>
    {
        ["ESC_ID_PROGRAMA_VUELO"] = dto.ProgramaVueloId,
        ["ESC_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["ESC_NUMERO_ORDEN"] = dto.NumeroOrden,
        ["ESC_HORA_LLEGADA_ESTIMADA"] = dto.HoraLlegadaEstimada,
        ["ESC_HORA_SALIDA_ESTIMADA"] = dto.HoraSalidaEstimada,
        ["ESC_DURACION_ESCALA"] = dto.DuracionEscala
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarEscalaTecnicaDto dto) => new Dictionary<string, object?>
    {
        ["ESC_ID_PROGRAMA_VUELO"] = dto.ProgramaVueloId,
        ["ESC_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["ESC_NUMERO_ORDEN"] = dto.NumeroOrden,
        ["ESC_HORA_LLEGADA_ESTIMADA"] = dto.HoraLlegadaEstimada,
        ["ESC_HORA_SALIDA_ESTIMADA"] = dto.HoraSalidaEstimada,
        ["ESC_DURACION_ESCALA"] = dto.DuracionEscala
    };
}
