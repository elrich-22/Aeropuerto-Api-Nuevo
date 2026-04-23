using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/asientos-avion")]
public sealed class AsientosAvionController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ASIENTO_AVION",
        "ASA_ID_ASIENTO",
        ["ASA_ID_AVION", "ASA_CODIGO", "ASA_CLASE", "ASA_ESTADO"],
        ["ASA_ID_AVION", "ASA_CODIGO", "ASA_CLASE", "ASA_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearAsientoAvionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAsientoAvionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AsientoAvionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AsientoAvionDto(
            row.ToInt("ASA_ID_ASIENTO"),
            row.ToInt("ASA_ID_AVION"),
            row.ToStringValue("ASA_CODIGO"),
            row.ToStringValue("ASA_CLASE"),
            row.ToStringValue("ASA_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAsientoAvionDto dto) => new Dictionary<string, object?>
    {
        ["ASA_ID_AVION"] = dto.AvionId,
        ["ASA_CODIGO"] = dto.Codigo,
        ["ASA_CLASE"] = dto.Clase,
        ["ASA_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAsientoAvionDto dto) => new Dictionary<string, object?>
    {
        ["ASA_ID_AVION"] = dto.AvionId,
        ["ASA_CODIGO"] = dto.Codigo,
        ["ASA_CLASE"] = dto.Clase,
        ["ASA_ESTADO"] = dto.Estado
    };
}
