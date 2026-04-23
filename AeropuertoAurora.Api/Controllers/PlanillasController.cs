using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/planillas")]
public sealed class PlanillasController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PLANILLA",
        "PLA_ID_PLANILLA",
        ["PLA_ID_EMPLEADO", "PLA_PERIODO_INICIO", "PLA_PERIODO_FIN", "PLA_SALARIO_BASE", "PLA_BONIFICACIONES", "PLA_HORAS_EXTRA", "PLA_DEDUCCIONES", "PLA_SALARIO_NETO", "PLA_FECHA_PAGO", "PLA_ESTADO"],
        ["PLA_ID_EMPLEADO", "PLA_PERIODO_INICIO", "PLA_PERIODO_FIN", "PLA_SALARIO_BASE", "PLA_BONIFICACIONES", "PLA_HORAS_EXTRA", "PLA_DEDUCCIONES", "PLA_SALARIO_NETO", "PLA_FECHA_PAGO", "PLA_ESTADO"]);

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
    public async Task<IActionResult> Create(CrearPlanillaDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarPlanillaDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static PlanillaDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new PlanillaDto(
            row.ToInt("PLA_ID_PLANILLA"),
            row.ToInt("PLA_ID_EMPLEADO"),
            row.ToDateTimeValue("PLA_PERIODO_INICIO"),
            row.ToDateTimeValue("PLA_PERIODO_FIN"),
            row.ToNullableDecimal("PLA_SALARIO_BASE") ?? 0m,
            row.ToNullableDecimal("PLA_BONIFICACIONES"),
            row.ToNullableDecimal("PLA_HORAS_EXTRA"),
            row.ToNullableDecimal("PLA_DEDUCCIONES"),
            row.ToNullableDecimal("PLA_SALARIO_NETO"),
            row.ToNullableDateTime("PLA_FECHA_PAGO"),
            row.ToStringValue("PLA_ESTADO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearPlanillaDto dto) => new Dictionary<string, object?>
    {
        ["PLA_ID_EMPLEADO"] = dto.EmpleadoId,
        ["PLA_PERIODO_INICIO"] = dto.PeriodoInicio,
        ["PLA_PERIODO_FIN"] = dto.PeriodoFin,
        ["PLA_SALARIO_BASE"] = dto.SalarioBase,
        ["PLA_BONIFICACIONES"] = dto.Bonificaciones,
        ["PLA_HORAS_EXTRA"] = dto.HorasExtra,
        ["PLA_DEDUCCIONES"] = dto.Deducciones,
        ["PLA_SALARIO_NETO"] = dto.SalarioNeto,
        ["PLA_FECHA_PAGO"] = dto.FechaPago,
        ["PLA_ESTADO"] = dto.Estado
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarPlanillaDto dto) => new Dictionary<string, object?>
    {
        ["PLA_ID_EMPLEADO"] = dto.EmpleadoId,
        ["PLA_PERIODO_INICIO"] = dto.PeriodoInicio,
        ["PLA_PERIODO_FIN"] = dto.PeriodoFin,
        ["PLA_SALARIO_BASE"] = dto.SalarioBase,
        ["PLA_BONIFICACIONES"] = dto.Bonificaciones,
        ["PLA_HORAS_EXTRA"] = dto.HorasExtra,
        ["PLA_DEDUCCIONES"] = dto.Deducciones,
        ["PLA_SALARIO_NETO"] = dto.SalarioNeto,
        ["PLA_FECHA_PAGO"] = dto.FechaPago,
        ["PLA_ESTADO"] = dto.Estado
    };
}
