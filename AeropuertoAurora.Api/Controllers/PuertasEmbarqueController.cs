using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/puertas-embarque")]
public sealed class PuertasEmbarqueController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PUERTA_EMBARQUE",
        "PUE_ID_PUERTA",
        ["PUE_ID_TERMINAL", "PUE_CODIGO", "PUE_ESTADO", "PUE_TIPO"],
        ["PUE_ID_TERMINAL", "PUE_CODIGO", "PUE_ESTADO", "PUE_TIPO"]);

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
    public async Task<IActionResult> Create(CrearPuertaEmbarqueDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarPuertaEmbarqueDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static PuertaEmbarqueDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new PuertaEmbarqueDto(
            row.ToInt("PUE_ID_PUERTA"),
            row.ToInt("PUE_ID_TERMINAL"),
            row.ToStringValue("PUE_CODIGO"),
            row.ToStringValue("PUE_ESTADO"),
            row.ToStringValue("PUE_TIPO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearPuertaEmbarqueDto dto) => new Dictionary<string, object?>
    {
        ["PUE_ID_TERMINAL"] = dto.TerminalId,
        ["PUE_CODIGO"] = dto.Codigo,
        ["PUE_ESTADO"] = dto.Estado,
        ["PUE_TIPO"] = dto.Tipo
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarPuertaEmbarqueDto dto) => new Dictionary<string, object?>
    {
        ["PUE_ID_TERMINAL"] = dto.TerminalId,
        ["PUE_CODIGO"] = dto.Codigo,
        ["PUE_ESTADO"] = dto.Estado,
        ["PUE_TIPO"] = dto.Tipo
    };
}
