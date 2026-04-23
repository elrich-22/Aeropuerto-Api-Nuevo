using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/sesiones-usuario")]
public sealed class SesionesUsuarioController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_SESIONUSUARIO",
        "SES_ID_SESION",
        ["SES_SESION_ID", "SES_ID_USUARIO", "SES_ID_PASAJERO", "SES_IP_ADDRESS", "SES_NAVEGADOR", "SES_SISTEMA_OPERATIVO", "SES_DISPOSITIVO", "SES_FECHA_INICIO", "SES_FECHA_FIN", "SES_DURACION_SEGUNDOS"],
        ["SES_SESION_ID", "SES_ID_USUARIO", "SES_ID_PASAJERO", "SES_IP_ADDRESS", "SES_NAVEGADOR", "SES_SISTEMA_OPERATIVO", "SES_DISPOSITIVO", "SES_FECHA_INICIO", "SES_FECHA_FIN", "SES_DURACION_SEGUNDOS"]);

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
    public async Task<IActionResult> Create(CrearSesionUsuarioDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarSesionUsuarioDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static SesionUsuarioDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new SesionUsuarioDto(
            row.ToInt("SES_ID_SESION"),
            row.ToStringValue("SES_SESION_ID"),
            row.ToNullableInt("SES_ID_USUARIO"),
            row.ToNullableInt("SES_ID_PASAJERO"),
            row.ToNullableString("SES_IP_ADDRESS"),
            row.ToNullableString("SES_NAVEGADOR"),
            row.ToNullableString("SES_SISTEMA_OPERATIVO"),
            row.ToNullableString("SES_DISPOSITIVO"),
            row.ToNullableDateTime("SES_FECHA_INICIO"),
            row.ToNullableDateTime("SES_FECHA_FIN"),
            row.ToNullableInt("SES_DURACION_SEGUNDOS"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearSesionUsuarioDto dto) => new Dictionary<string, object?>
    {
        ["SES_SESION_ID"] = dto.SesionId,
        ["SES_ID_USUARIO"] = dto.UsuarioId,
        ["SES_ID_PASAJERO"] = dto.PasajeroId,
        ["SES_IP_ADDRESS"] = dto.IpAddress,
        ["SES_NAVEGADOR"] = dto.Navegador,
        ["SES_SISTEMA_OPERATIVO"] = dto.SistemaOperativo,
        ["SES_DISPOSITIVO"] = dto.Dispositivo,
        ["SES_FECHA_INICIO"] = dto.FechaInicio,
        ["SES_FECHA_FIN"] = dto.FechaFin,
        ["SES_DURACION_SEGUNDOS"] = dto.DuracionSegundos
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarSesionUsuarioDto dto) => new Dictionary<string, object?>
    {
        ["SES_SESION_ID"] = dto.SesionId,
        ["SES_ID_USUARIO"] = dto.UsuarioId,
        ["SES_ID_PASAJERO"] = dto.PasajeroId,
        ["SES_IP_ADDRESS"] = dto.IpAddress,
        ["SES_NAVEGADOR"] = dto.Navegador,
        ["SES_SISTEMA_OPERATIVO"] = dto.SistemaOperativo,
        ["SES_DISPOSITIVO"] = dto.Dispositivo,
        ["SES_FECHA_INICIO"] = dto.FechaInicio,
        ["SES_FECHA_FIN"] = dto.FechaFin,
        ["SES_DURACION_SEGUNDOS"] = dto.DuracionSegundos
    };
}
