using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/dias-vuelo")]
public sealed class DiasVueloController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_DIASVUELO",
        "DIA_ID_DIA_VUELO",
        ["DIA_ID_PROGRAMA_VUELO", "DIA_DIA_SEMANA"],
        ["DIA_ID_PROGRAMA_VUELO", "DIA_DIA_SEMANA"]);

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
    public async Task<IActionResult> Create(CrearDiaVueloDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarDiaVueloDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static DiaVueloDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new DiaVueloDto(row.ToInt("DIA_ID_DIA_VUELO"), row.ToInt("DIA_ID_PROGRAMA_VUELO"), row.ToInt("DIA_DIA_SEMANA"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearDiaVueloDto dto) => new Dictionary<string, object?>
    {
        ["DIA_ID_PROGRAMA_VUELO"] = dto.ProgramaVueloId,
        ["DIA_DIA_SEMANA"] = dto.DiaSemana
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarDiaVueloDto dto) => new Dictionary<string, object?>
    {
        ["DIA_ID_PROGRAMA_VUELO"] = dto.ProgramaVueloId,
        ["DIA_DIA_SEMANA"] = dto.DiaSemana
    };
}
