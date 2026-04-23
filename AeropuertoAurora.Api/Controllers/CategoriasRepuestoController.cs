using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/categorias-repuesto")]
public sealed class CategoriasRepuestoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_CATEGORIAREPUESTO",
        "CAT_ID_CATEGORIA",
        ["CAT_NOMBRE", "CAT_DESCRIPCION"],
        ["CAT_NOMBRE", "CAT_DESCRIPCION"]);

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
    public async Task<IActionResult> Create(CrearCategoriaRepuestoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarCategoriaRepuestoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static CategoriaRepuestoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("CAT_ID_CATEGORIA"),
        row.ToStringValue("CAT_NOMBRE"),
        row.ToNullableString("CAT_DESCRIPCION"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearCategoriaRepuestoDto dto) => new Dictionary<string, object?>
    {
        ["CAT_NOMBRE"] = dto.Nombre,
        ["CAT_DESCRIPCION"] = dto.Descripcion
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarCategoriaRepuestoDto dto) => ToValues(new CrearCategoriaRepuestoDto(dto.Nombre, dto.Descripcion));
}
