using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/puestos")]
public sealed class PuestosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PUESTO",
        "PUE_ID_PUESTO",
        ["PUE_NOMBRE", "PUE_ID_DEPARTAMENTO", "PUE_DESCRIPCION", "PUE_SALARIO_MINIMO", "PUE_SALARIO_MAXIMO", "PUE_REQUIERE_LICENCIA"],
        ["PUE_NOMBRE", "PUE_ID_DEPARTAMENTO", "PUE_DESCRIPCION", "PUE_SALARIO_MINIMO", "PUE_SALARIO_MAXIMO", "PUE_REQUIERE_LICENCIA"]);

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
    public async Task<IActionResult> Create(CrearPuestoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarPuestoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static PuestoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new PuestoDto(
            row.ToInt("PUE_ID_PUESTO"),
            row.ToStringValue("PUE_NOMBRE"),
            row.ToInt("PUE_ID_DEPARTAMENTO"),
            row.ToNullableString("PUE_DESCRIPCION"),
            row.ToNullableDecimal("PUE_SALARIO_MINIMO"),
            row.ToNullableDecimal("PUE_SALARIO_MAXIMO"),
            row.ToStringValue("PUE_REQUIERE_LICENCIA"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearPuestoDto dto) => new Dictionary<string, object?>
    {
        ["PUE_NOMBRE"] = dto.Nombre,
        ["PUE_ID_DEPARTAMENTO"] = dto.DepartamentoId,
        ["PUE_DESCRIPCION"] = dto.Descripcion,
        ["PUE_SALARIO_MINIMO"] = dto.SalarioMinimo,
        ["PUE_SALARIO_MAXIMO"] = dto.SalarioMaximo,
        ["PUE_REQUIERE_LICENCIA"] = dto.RequiereLicencia
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarPuestoDto dto) => new Dictionary<string, object?>
    {
        ["PUE_NOMBRE"] = dto.Nombre,
        ["PUE_ID_DEPARTAMENTO"] = dto.DepartamentoId,
        ["PUE_DESCRIPCION"] = dto.Descripcion,
        ["PUE_SALARIO_MINIMO"] = dto.SalarioMinimo,
        ["PUE_SALARIO_MAXIMO"] = dto.SalarioMaximo,
        ["PUE_REQUIERE_LICENCIA"] = dto.RequiereLicencia
    };
}
