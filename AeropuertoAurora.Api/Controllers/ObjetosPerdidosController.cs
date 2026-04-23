using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/objetos-perdidos")]
public sealed class ObjetosPerdidosController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_OBJETOPERDIDO",
        "OBJ_ID_OBJETO",
        ["OBJ_ID_VUELO", "OBJ_ID_AEROPUERTO", "OBJ_DESCRIPCION", "OBJ_FECHA_REPORTE", "OBJ_UBICACION_ENCONTRADO", "OBJ_ESTADO", "OBJ_REP_PRIMER_NOMBRE", "OBJ_REP_SEGUNDO_NOMBRE", "OBJ_REP_PRIMER_APELLIDO", "OBJ_REP_SEGUNDO_APELLIDO", "OBJ_CONTACTO_REPORTANTE", "OBJ_FECHA_ENTREGA", "OBJ_REC_PRIMER_NOMBRE", "OBJ_REC_SEGUNDO_NOMBRE", "OBJ_REC_PRIMER_APELLIDO", "OBJ_REC_SEGUNDO_APELLIDO"],
        ["OBJ_ID_VUELO", "OBJ_ID_AEROPUERTO", "OBJ_DESCRIPCION", "OBJ_FECHA_REPORTE", "OBJ_UBICACION_ENCONTRADO", "OBJ_ESTADO", "OBJ_REP_PRIMER_NOMBRE", "OBJ_REP_SEGUNDO_NOMBRE", "OBJ_REP_PRIMER_APELLIDO", "OBJ_REP_SEGUNDO_APELLIDO", "OBJ_CONTACTO_REPORTANTE", "OBJ_FECHA_ENTREGA", "OBJ_REC_PRIMER_NOMBRE", "OBJ_REC_SEGUNDO_NOMBRE", "OBJ_REC_PRIMER_APELLIDO", "OBJ_REC_SEGUNDO_APELLIDO"]);

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
    public async Task<IActionResult> Create(CrearObjetoPerdidoDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarObjetoPerdidoDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ObjetoPerdidoDto Map(IReadOnlyDictionary<string, object?> row) => new(
        row.ToInt("OBJ_ID_OBJETO"),
        row.ToNullableInt("OBJ_ID_VUELO"),
        row.ToInt("OBJ_ID_AEROPUERTO"),
        row.ToStringValue("OBJ_DESCRIPCION"),
        row.ToNullableDateTime("OBJ_FECHA_REPORTE"),
        row.ToNullableString("OBJ_UBICACION_ENCONTRADO"),
        row.ToStringValue("OBJ_ESTADO"),
        row.ToNullableString("OBJ_REP_PRIMER_NOMBRE"),
        row.ToNullableString("OBJ_REP_SEGUNDO_NOMBRE"),
        row.ToNullableString("OBJ_REP_PRIMER_APELLIDO"),
        row.ToNullableString("OBJ_REP_SEGUNDO_APELLIDO"),
        row.ToNullableString("OBJ_CONTACTO_REPORTANTE"),
        row.ToNullableDateTime("OBJ_FECHA_ENTREGA"),
        row.ToNullableString("OBJ_REC_PRIMER_NOMBRE"),
        row.ToNullableString("OBJ_REC_SEGUNDO_NOMBRE"),
        row.ToNullableString("OBJ_REC_PRIMER_APELLIDO"),
        row.ToNullableString("OBJ_REC_SEGUNDO_APELLIDO"));

    private static IReadOnlyDictionary<string, object?> ToValues(CrearObjetoPerdidoDto dto) => new Dictionary<string, object?>
    {
        ["OBJ_ID_VUELO"] = dto.VueloId,
        ["OBJ_ID_AEROPUERTO"] = dto.AeropuertoId,
        ["OBJ_DESCRIPCION"] = dto.Descripcion,
        ["OBJ_FECHA_REPORTE"] = dto.FechaReporte,
        ["OBJ_UBICACION_ENCONTRADO"] = dto.UbicacionEncontrado,
        ["OBJ_ESTADO"] = dto.Estado,
        ["OBJ_REP_PRIMER_NOMBRE"] = dto.ReportantePrimerNombre,
        ["OBJ_REP_SEGUNDO_NOMBRE"] = dto.ReportanteSegundoNombre,
        ["OBJ_REP_PRIMER_APELLIDO"] = dto.ReportantePrimerApellido,
        ["OBJ_REP_SEGUNDO_APELLIDO"] = dto.ReportanteSegundoApellido,
        ["OBJ_CONTACTO_REPORTANTE"] = dto.ContactoReportante,
        ["OBJ_FECHA_ENTREGA"] = dto.FechaEntrega,
        ["OBJ_REC_PRIMER_NOMBRE"] = dto.ReclamantePrimerNombre,
        ["OBJ_REC_SEGUNDO_NOMBRE"] = dto.ReclamanteSegundoNombre,
        ["OBJ_REC_PRIMER_APELLIDO"] = dto.ReclamantePrimerApellido,
        ["OBJ_REC_SEGUNDO_APELLIDO"] = dto.ReclamanteSegundoApellido
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarObjetoPerdidoDto dto) => ToValues(new CrearObjetoPerdidoDto(
        dto.VueloId, dto.AeropuertoId, dto.Descripcion, dto.FechaReporte, dto.UbicacionEncontrado, dto.Estado, dto.ReportantePrimerNombre, dto.ReportanteSegundoNombre, dto.ReportantePrimerApellido, dto.ReportanteSegundoApellido, dto.ContactoReportante, dto.FechaEntrega, dto.ReclamantePrimerNombre, dto.ReclamanteSegundoNombre, dto.ReclamantePrimerApellido, dto.ReclamanteSegundoApellido));
}
