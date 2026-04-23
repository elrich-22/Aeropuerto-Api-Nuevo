using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/proveedores")]
public sealed class ProveedoresController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PROVEEDOR",
        "PRV_ID_PROVEEDOR",
        ["PRV_NOMBRE_EMPRESA", "PRV_NIT", "PRV_DIR_CALLE", "PRV_DIR_ZONA", "PRV_DIR_MUNICIPIO", "PRV_DIR_DEPARTAMENTO", "PRV_PAIS", "PRV_TELEFONO", "PRV_EMAIL", "PRV_CONTACTO_NOMBRE", "PRV_CONTACTO_TELEFONO", "PRV_CONTACTO_EMAIL", "PRV_ESTADO", "PRV_CALIFICACION"],
        ["PRV_NOMBRE_EMPRESA", "PRV_NIT", "PRV_DIR_CALLE", "PRV_DIR_ZONA", "PRV_DIR_MUNICIPIO", "PRV_DIR_DEPARTAMENTO", "PRV_PAIS", "PRV_TELEFONO", "PRV_EMAIL", "PRV_CONTACTO_NOMBRE", "PRV_CONTACTO_TELEFONO", "PRV_CONTACTO_EMAIL", "PRV_ESTADO", "PRV_CALIFICACION"]);

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
    public async Task<IActionResult> Create(CrearProveedorDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarProveedorDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ProveedorDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("PRV_ID_PROVEEDOR"),
        row.ToStringValue("PRV_NOMBRE_EMPRESA"),
        row.ToStringValue("PRV_NIT"),
        row.ToNullableString("PRV_DIR_CALLE"),
        row.ToNullableString("PRV_DIR_ZONA"),
        row.ToNullableString("PRV_DIR_MUNICIPIO"),
        row.ToNullableString("PRV_DIR_DEPARTAMENTO"),
        row.ToNullableString("PRV_PAIS"),
        row.ToNullableString("PRV_TELEFONO"),
        row.ToNullableString("PRV_EMAIL"),
        row.ToNullableString("PRV_CONTACTO_NOMBRE"),
        row.ToNullableString("PRV_CONTACTO_TELEFONO"),
        row.ToNullableString("PRV_CONTACTO_EMAIL"),
        row.ToStringValue("PRV_ESTADO"),
        row.ToNullableDecimal("PRV_CALIFICACION"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearProveedorDto dto) => new Dictionary<string, object?>
    {
        ["PRV_NOMBRE_EMPRESA"] = dto.NombreEmpresa,
        ["PRV_NIT"] = dto.Nit,
        ["PRV_DIR_CALLE"] = dto.DireccionCalle,
        ["PRV_DIR_ZONA"] = dto.DireccionZona,
        ["PRV_DIR_MUNICIPIO"] = dto.DireccionMunicipio,
        ["PRV_DIR_DEPARTAMENTO"] = dto.DireccionDepartamento,
        ["PRV_PAIS"] = dto.Pais,
        ["PRV_TELEFONO"] = dto.Telefono,
        ["PRV_EMAIL"] = dto.Email,
        ["PRV_CONTACTO_NOMBRE"] = dto.ContactoNombre,
        ["PRV_CONTACTO_TELEFONO"] = dto.ContactoTelefono,
        ["PRV_CONTACTO_EMAIL"] = dto.ContactoEmail,
        ["PRV_ESTADO"] = dto.Estado,
        ["PRV_CALIFICACION"] = dto.Calificacion
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarProveedorDto dto) => ToValues(new CrearProveedorDto(
        dto.NombreEmpresa, dto.Nit, dto.DireccionCalle, dto.DireccionZona, dto.DireccionMunicipio, dto.DireccionDepartamento, dto.Pais, dto.Telefono, dto.Email, dto.ContactoNombre, dto.ContactoTelefono, dto.ContactoEmail, dto.Estado, dto.Calificacion));
}
