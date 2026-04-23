using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/departamentos")]
public sealed class DepartamentosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_DEPARTAMENTO",
        "DEP_ID_DEPARTAMENTO",
        ["DEP_NOMBRE", "DEP_DESCRIPCION", "DEP_ID_AEROPUERTO", "DEP_ESTADO"],
        ["DEP_NOMBRE", "DEP_DESCRIPCION", "DEP_ID_AEROPUERTO", "DEP_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearDepartamentoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarDepartamentoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static DepartamentoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new DepartamentoDto(
            row.ToInt("DEP_ID_DEPARTAMENTO"),
            row.ToStringValue("DEP_NOMBRE"),
            row.ToNullableString("DEP_DESCRIPCION"),
            row.ToInt("DEP_ID_AEROPUERTO"),
            row.ToStringValue("DEP_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearDepartamentoDto dto) => new Dictionary<string, object?>
    {
        ["DEP_NOMBRE"] = dto.Nombre,
        ["DEP_DESCRIPCION"] = dto.Descripcion,
        ["DEP_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["DEP_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarDepartamentoDto dto) => new Dictionary<string, object?>
    {
        ["DEP_NOMBRE"] = dto.Nombre,
        ["DEP_DESCRIPCION"] = dto.Descripcion,
        ["DEP_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["DEP_ESTADO"] = dto.Estado
    };
}
