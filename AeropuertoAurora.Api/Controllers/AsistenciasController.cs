using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/asistencias")]
public sealed class AsistenciasController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_ASISTENCIA",
        "ASI_ID_ASISTENCIA",
        ["ASI_ID_EMPLEADO", "ASI_FECHA", "ASI_HORA_ENTRADA", "ASI_HORA_SALIDA", "ASI_HORAS_TRABAJADAS", "ASI_TIPO", "ASI_ESTADO"],
        ["ASI_ID_EMPLEADO", "ASI_FECHA", "ASI_HORA_ENTRADA", "ASI_HORA_SALIDA", "ASI_HORAS_TRABAJADAS", "ASI_TIPO", "ASI_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearAsistenciaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarAsistenciaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static AsistenciaDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new AsistenciaDto(
            row.ToInt("ASI_ID_ASISTENCIA"),
            row.ToInt("ASI_ID_EMPLEADO"),
            row.ToDateTimeValue("ASI_FECHA"),
            row.ToNullableDateTime("ASI_HORA_ENTRADA"),
            row.ToNullableDateTime("ASI_HORA_SALIDA"),
            row.ToNullableDecimal("ASI_HORAS_TRABAJADAS"),
            row.ToStringValue("ASI_TIPO"),
            row.ToStringValue("ASI_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearAsistenciaDto dto) => new Dictionary<string, object?>
    {
        ["ASI_ID_EMPLEADO"] = dto.EmpleadoId,
        ["ASI_FECHA"] = dto.Fecha,
        ["ASI_HORA_ENTRADA"] = dto.HoraEntrada,
        ["ASI_HORA_SALIDA"] = dto.HoraSalida,
        ["ASI_HORAS_TRABAJADAS"] = dto.HorasTrabajadas,
        ["ASI_TIPO"] = dto.Tipo,
        ["ASI_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarAsistenciaDto dto) => new Dictionary<string, object?>
    {
        ["ASI_ID_EMPLEADO"] = dto.EmpleadoId,
        ["ASI_FECHA"] = dto.Fecha,
        ["ASI_HORA_ENTRADA"] = dto.HoraEntrada,
        ["ASI_HORA_SALIDA"] = dto.HoraSalida,
        ["ASI_HORAS_TRABAJADAS"] = dto.HorasTrabajadas,
        ["ASI_TIPO"] = dto.Tipo,
        ["ASI_ESTADO"] = dto.Estado
    };
}
