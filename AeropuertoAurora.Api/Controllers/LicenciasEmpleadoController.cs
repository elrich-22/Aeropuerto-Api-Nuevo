using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/licencias-empleado")]
public sealed class LicenciasEmpleadoController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_LICENCIAEMPLEADO",
        "LIC_ID_LICENCIA",
        ["LIC_ID_EMPLEADO", "LIC_TIPO_LICENCIA", "LIC_NUMERO_LICENCIA", "LIC_FECHA_EMISION", "LIC_FECHA_VENCIMIENTO", "LIC_AUTORIDAD_EMISORA", "LIC_ESTADO"],
        ["LIC_ID_EMPLEADO", "LIC_TIPO_LICENCIA", "LIC_NUMERO_LICENCIA", "LIC_FECHA_EMISION", "LIC_FECHA_VENCIMIENTO", "LIC_AUTORIDAD_EMISORA", "LIC_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearLicenciaEmpleadoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarLicenciaEmpleadoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static LicenciaEmpleadoDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new LicenciaEmpleadoDto(
            row.ToInt("LIC_ID_LICENCIA"),
            row.ToInt("LIC_ID_EMPLEADO"),
            row.ToStringValue("LIC_TIPO_LICENCIA"),
            row.ToStringValue("LIC_NUMERO_LICENCIA"),
            row.ToDateTimeValue("LIC_FECHA_EMISION"),
            row.ToDateTimeValue("LIC_FECHA_VENCIMIENTO"),
            row.ToNullableString("LIC_AUTORIDAD_EMISORA"),
            row.ToStringValue("LIC_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearLicenciaEmpleadoDto dto) => new Dictionary<string, object?>
    {
        ["LIC_ID_EMPLEADO"] = dto.EmpleadoId,
        ["LIC_TIPO_LICENCIA"] = dto.TipoLicencia,
        ["LIC_NUMERO_LICENCIA"] = dto.NumeroLicencia,
        ["LIC_FECHA_EMISION"] = dto.FechaEmision,
        ["LIC_FECHA_VENCIMIENTO"] = dto.FechaVencimiento,
        ["LIC_AUTORIDAD_EMISORA"] = dto.AutoridadEmisora,
        ["LIC_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarLicenciaEmpleadoDto dto) => new Dictionary<string, object?>
    {
        ["LIC_ID_EMPLEADO"] = dto.EmpleadoId,
        ["LIC_TIPO_LICENCIA"] = dto.TipoLicencia,
        ["LIC_NUMERO_LICENCIA"] = dto.NumeroLicencia,
        ["LIC_FECHA_EMISION"] = dto.FechaEmision,
        ["LIC_FECHA_VENCIMIENTO"] = dto.FechaVencimiento,
        ["LIC_AUTORIDAD_EMISORA"] = dto.AutoridadEmisora,
        ["LIC_ESTADO"] = dto.Estado
    };
}
