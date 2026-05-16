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
    public async Task<IActionResult> Update(int id, ActualizarEstadoObjetoPerdidoDto dto, CancellationToken cancellationToken)
    {
        var currentRow = await repository.GetByIdAsync(Table, id, cancellationToken);
        if (currentRow is null)
        {
            return NotFound();
        }

        var currentStatus = currentRow.ToStringValue("OBJ_ESTADO");
        if (string.Equals(currentStatus, "ENTREGADO", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Este objeto ya fue marcado como ENTREGADO y su estado no puede cambiar nuevamente." });
        }

        if (string.IsNullOrWhiteSpace(dto.Estado))
        {
            return BadRequest(new { message = "El estado es obligatorio." });
        }

        var estado = dto.Estado.Trim();
        var isDelivered = string.Equals(estado, "ENTREGADO", StringComparison.OrdinalIgnoreCase);

        if (isDelivered && string.IsNullOrWhiteSpace(dto.ReclamantePrimerNombre))
        {
            return BadRequest(new { message = "Debes indicar el nombre de la persona a quien se entrego el objeto." });
        }

        if (isDelivered && !dto.FechaEntrega.HasValue)
        {
            return BadRequest(new { message = "Debes indicar la fecha de entrega cuando el estado es ENTREGADO." });
        }

        var values = new Dictionary<string, object?>
        {
            ["OBJ_ESTADO"] = estado,
            ["OBJ_FECHA_ENTREGA"] = isDelivered ? dto.FechaEntrega : DBNull.Value,
            ["OBJ_REC_PRIMER_NOMBRE"] = isDelivered ? dto.ReclamantePrimerNombre?.Trim() : DBNull.Value,
            ["OBJ_REC_SEGUNDO_NOMBRE"] = isDelivered ? NullIfWhiteSpace(dto.ReclamanteSegundoNombre) : DBNull.Value,
            ["OBJ_REC_PRIMER_APELLIDO"] = isDelivered ? NullIfWhiteSpace(dto.ReclamantePrimerApellido) : DBNull.Value,
            ["OBJ_REC_SEGUNDO_APELLIDO"] = isDelivered ? NullIfWhiteSpace(dto.ReclamanteSegundoApellido) : DBNull.Value
        };

        return await repository.UpdatePartialAsync(Table, id, values, cancellationToken)
            ? NoContent()
            : NotFound();
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
        ["OBJ_FECHA_REPORTE"] = DateTime.Today,
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

    private static object? NullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();
}
