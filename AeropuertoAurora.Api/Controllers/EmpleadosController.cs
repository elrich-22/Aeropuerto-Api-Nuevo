using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/empleados")]
public sealed class EmpleadosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_EMPLEADO",
        "EMP_ID_EMPLEADO",
        ["EMP_NUMERO_EMPLEADO", "EMP_PRIMER_NOMBRE", "EMP_SEGUNDO_NOMBRE", "EMP_PRIMER_APELLIDO", "EMP_SEGUNDO_APELLIDO", "EMP_FECHA_NACIMIENTO", "EMP_DPI", "EMP_NIT", "EMP_DIR_CALLE", "EMP_DIR_ZONA", "EMP_DIR_MUNICIPIO", "EMP_DIR_DEPARTAMENTO", "EMP_DIR_PAIS", "EMP_TELEFONO", "EMP_EMAIL", "EMP_FECHA_CONTRATACION", "EMP_ID_PUESTO", "EMP_ID_DEPARTAMENTO", "EMP_SALARIO_ACTUAL", "EMP_TIPO_CONTRATO", "EMP_ESTADO", "EMP_FOTO"],
        ["EMP_NUMERO_EMPLEADO", "EMP_PRIMER_NOMBRE", "EMP_SEGUNDO_NOMBRE", "EMP_PRIMER_APELLIDO", "EMP_SEGUNDO_APELLIDO", "EMP_FECHA_NACIMIENTO", "EMP_DPI", "EMP_NIT", "EMP_DIR_CALLE", "EMP_DIR_ZONA", "EMP_DIR_MUNICIPIO", "EMP_DIR_DEPARTAMENTO", "EMP_DIR_PAIS", "EMP_TELEFONO", "EMP_EMAIL", "EMP_FECHA_CONTRATACION", "EMP_ID_PUESTO", "EMP_ID_DEPARTAMENTO", "EMP_SALARIO_ACTUAL", "EMP_TIPO_CONTRATO", "EMP_ESTADO", "EMP_FOTO"]);

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
    public async Task<IActionResult> Create(CrearEmpleadoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarEmpleadoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static EmpleadoDetalleDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new EmpleadoDetalleDto(
            row.ToInt("EMP_ID_EMPLEADO"),
            row.ToStringValue("EMP_NUMERO_EMPLEADO"),
            row.ToStringValue("EMP_PRIMER_NOMBRE"),
            row.ToNullableString("EMP_SEGUNDO_NOMBRE"),
            row.ToStringValue("EMP_PRIMER_APELLIDO"),
            row.ToNullableString("EMP_SEGUNDO_APELLIDO"),
            row.ToNullableDateTime("EMP_FECHA_NACIMIENTO"),
            row.ToNullableString("EMP_DPI"),
            row.ToNullableString("EMP_NIT"),
            row.ToNullableString("EMP_DIR_CALLE"),
            row.ToNullableString("EMP_DIR_ZONA"),
            row.ToNullableString("EMP_DIR_MUNICIPIO"),
            row.ToNullableString("EMP_DIR_DEPARTAMENTO"),
            row.ToNullableString("EMP_DIR_PAIS"),
            row.ToNullableString("EMP_TELEFONO"),
            row.ToStringValue("EMP_EMAIL"),
            row.ToDateTimeValue("EMP_FECHA_CONTRATACION"),
            row.ToInt("EMP_ID_PUESTO"),
            row.ToInt("EMP_ID_DEPARTAMENTO"),
            row.ToNullableDecimal("EMP_SALARIO_ACTUAL"),
            row.ToNullableString("EMP_TIPO_CONTRATO"),
            row.ToStringValue("EMP_ESTADO"),
            row.ToNullableBytes("EMP_FOTO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearEmpleadoDto dto) => new Dictionary<string, object?>
    {
        ["EMP_NUMERO_EMPLEADO"] = dto.NumeroEmpleado,
        ["EMP_PRIMER_NOMBRE"] = dto.PrimerNombre,
        ["EMP_SEGUNDO_NOMBRE"] = dto.SegundoNombre,
        ["EMP_PRIMER_APELLIDO"] = dto.PrimerApellido,
        ["EMP_SEGUNDO_APELLIDO"] = dto.SegundoApellido,
        ["EMP_FECHA_NACIMIENTO"] = dto.FechaNacimiento,
        ["EMP_DPI"] = dto.Dpi,
        ["EMP_NIT"] = dto.Nit,
        ["EMP_DIR_CALLE"] = dto.DireccionCalle,
        ["EMP_DIR_ZONA"] = dto.DireccionZona,
        ["EMP_DIR_MUNICIPIO"] = dto.DireccionMunicipio,
        ["EMP_DIR_DEPARTAMENTO"] = dto.DireccionDepartamento,
        ["EMP_DIR_PAIS"] = dto.DireccionPais,
        ["EMP_TELEFONO"] = dto.Telefono,
        ["EMP_EMAIL"] = dto.Email,
        ["EMP_FECHA_CONTRATACION"] = dto.FechaContratacion,
        ["EMP_ID_PUESTO"] = dto.PuestoId,
        ["EMP_ID_DEPARTAMENTO"] = dto.DepartamentoId,
        ["EMP_SALARIO_ACTUAL"] = dto.SalarioActual,
        ["EMP_TIPO_CONTRATO"] = dto.TipoContrato,
        ["EMP_ESTADO"] = dto.Estado,
        ["EMP_FOTO"] = dto.Foto
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarEmpleadoDto dto) => new Dictionary<string, object?>
    {
        ["EMP_NUMERO_EMPLEADO"] = dto.NumeroEmpleado,
        ["EMP_PRIMER_NOMBRE"] = dto.PrimerNombre,
        ["EMP_SEGUNDO_NOMBRE"] = dto.SegundoNombre,
        ["EMP_PRIMER_APELLIDO"] = dto.PrimerApellido,
        ["EMP_SEGUNDO_APELLIDO"] = dto.SegundoApellido,
        ["EMP_FECHA_NACIMIENTO"] = dto.FechaNacimiento,
        ["EMP_DPI"] = dto.Dpi,
        ["EMP_NIT"] = dto.Nit,
        ["EMP_DIR_CALLE"] = dto.DireccionCalle,
        ["EMP_DIR_ZONA"] = dto.DireccionZona,
        ["EMP_DIR_MUNICIPIO"] = dto.DireccionMunicipio,
        ["EMP_DIR_DEPARTAMENTO"] = dto.DireccionDepartamento,
        ["EMP_DIR_PAIS"] = dto.DireccionPais,
        ["EMP_TELEFONO"] = dto.Telefono,
        ["EMP_EMAIL"] = dto.Email,
        ["EMP_FECHA_CONTRATACION"] = dto.FechaContratacion,
        ["EMP_ID_PUESTO"] = dto.PuestoId,
        ["EMP_ID_DEPARTAMENTO"] = dto.DepartamentoId,
        ["EMP_SALARIO_ACTUAL"] = dto.SalarioActual,
        ["EMP_TIPO_CONTRATO"] = dto.TipoContrato,
        ["EMP_ESTADO"] = dto.Estado,
        ["EMP_FOTO"] = dto.Foto
    };
}
