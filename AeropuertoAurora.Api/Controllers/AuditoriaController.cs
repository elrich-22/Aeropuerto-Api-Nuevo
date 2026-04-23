using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/auditoria")]
public sealed class AuditoriaController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_AUDITORIA",
        "AUD_ID_AUDITORIA",
        ["AUD_TABLA_AFECTADA", "AUD_OPERACION", "AUD_USUARIO", "AUD_FECHA_HORA", "AUD_IP_ADDRESS", "AUD_DATOS_ANTERIORES", "AUD_DATOS_NUEVOS"],
        ["AUD_TABLA_AFECTADA", "AUD_OPERACION", "AUD_USUARIO", "AUD_FECHA_HORA", "AUD_IP_ADDRESS", "AUD_DATOS_ANTERIORES", "AUD_DATOS_NUEVOS"]);

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
    public async Task<IActionResult> Create(CrearAuditoriaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAuditoriaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AuditoriaDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("AUD_ID_AUDITORIA"),
        row.ToStringValue("AUD_TABLA_AFECTADA"),
        row.ToStringValue("AUD_OPERACION"),
        row.ToStringValue("AUD_USUARIO"),
        row.ToNullableDateTime("AUD_FECHA_HORA"),
        row.ToNullableString("AUD_IP_ADDRESS"),
        row.ToNullableString("AUD_DATOS_ANTERIORES"),
        row.ToNullableString("AUD_DATOS_NUEVOS"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAuditoriaDto dto) => new Dictionary<string, object?>
    {
        ["AUD_TABLA_AFECTADA"] = dto.TablaAfectada,
        ["AUD_OPERACION"] = dto.Operacion,
        ["AUD_USUARIO"] = dto.Usuario,
        ["AUD_FECHA_HORA"] = dto.FechaHora,
        ["AUD_IP_ADDRESS"] = dto.IpAddress,
        ["AUD_DATOS_ANTERIORES"] = dto.DatosAnteriores,
        ["AUD_DATOS_NUEVOS"] = dto.DatosNuevos
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAuditoriaDto dto) => ToValues(new CrearAuditoriaDto(
        dto.TablaAfectada, dto.Operacion, dto.Usuario, dto.FechaHora, dto.IpAddress, dto.DatosAnteriores, dto.DatosNuevos));
}
