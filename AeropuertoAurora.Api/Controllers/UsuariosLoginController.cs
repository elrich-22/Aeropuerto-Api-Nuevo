using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/usuarios-login")]
public sealed class UsuariosLoginController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_USUARIO_LOGIN",
        "USL_ID_USUARIO",
        ["USL_ID_PASAJERO", "USL_USUARIO", "USL_EMAIL", "USL_CONTRASENA_HASH", "USL_SAL", "USL_ESTADO", "USL_EMAIL_VERIFICADO", "USL_TOKEN_VERIFICACION", "USL_FECHA_REGISTRO", "USL_ULTIMO_ACCESO", "USL_INTENTOS_FALLIDOS", "USL_BLOQUEADO_HASTA", "USL_TOKEN_RECUPERACION", "USL_VENCIMIENTO_TOKEN", "USL_ROL"],
        ["USL_ID_PASAJERO", "USL_USUARIO", "USL_EMAIL", "USL_CONTRASENA_HASH", "USL_SAL", "USL_ESTADO", "USL_EMAIL_VERIFICADO", "USL_TOKEN_VERIFICACION", "USL_FECHA_REGISTRO", "USL_ULTIMO_ACCESO", "USL_INTENTOS_FALLIDOS", "USL_BLOQUEADO_HASTA", "USL_TOKEN_RECUPERACION", "USL_VENCIMIENTO_TOKEN"]);

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
    public async Task<IActionResult> Create(CrearUsuarioLoginDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarUsuarioLoginDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static UsuarioLoginDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new UsuarioLoginDto(
            row.ToInt("USL_ID_USUARIO"),
            row.ToInt("USL_ID_PASAJERO"),
            row.ToStringValue("USL_USUARIO"),
            row.ToStringValue("USL_EMAIL"),
            row.ToStringValue("USL_ESTADO"),
            row.ToStringValue("USL_EMAIL_VERIFICADO"),
            row.ToNullableDateTime("USL_FECHA_REGISTRO"),
            row.ToNullableDateTime("USL_ULTIMO_ACCESO"),
            row.ToNullableInt("USL_INTENTOS_FALLIDOS") ?? 0,
            row.ToNullableDateTime("USL_BLOQUEADO_HASTA"),
            row.ToStringValue("USL_ROL") ?? "PASAJERO");
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearUsuarioLoginDto dto) => new Dictionary<string, object?>
    {
        ["USL_ID_PASAJERO"] = dto.PasajeroId,
        ["USL_USUARIO"] = dto.Usuario,
        ["USL_EMAIL"] = dto.Email,
        ["USL_CONTRASENA_HASH"] = dto.ContrasenaHash,
        ["USL_SAL"] = dto.Sal,
        ["USL_ESTADO"] = dto.Estado,
        ["USL_EMAIL_VERIFICADO"] = dto.EmailVerificado,
        ["USL_TOKEN_VERIFICACION"] = dto.TokenVerificacion,
        ["USL_FECHA_REGISTRO"] = dto.FechaRegistro,
        ["USL_ULTIMO_ACCESO"] = dto.UltimoAcceso,
        ["USL_INTENTOS_FALLIDOS"] = dto.IntentosFallidos,
        ["USL_BLOQUEADO_HASTA"] = dto.BloqueadoHasta,
        ["USL_TOKEN_RECUPERACION"] = dto.TokenRecuperacion,
        ["USL_VENCIMIENTO_TOKEN"] = dto.VencimientoToken
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarUsuarioLoginDto dto) => new Dictionary<string, object?>
    {
        ["USL_ID_PASAJERO"] = dto.PasajeroId,
        ["USL_USUARIO"] = dto.Usuario,
        ["USL_EMAIL"] = dto.Email,
        ["USL_CONTRASENA_HASH"] = dto.ContrasenaHash,
        ["USL_SAL"] = dto.Sal,
        ["USL_ESTADO"] = dto.Estado,
        ["USL_EMAIL_VERIFICADO"] = dto.EmailVerificado,
        ["USL_TOKEN_VERIFICACION"] = dto.TokenVerificacion,
        ["USL_FECHA_REGISTRO"] = dto.FechaRegistro,
        ["USL_ULTIMO_ACCESO"] = dto.UltimoAcceso,
        ["USL_INTENTOS_FALLIDOS"] = dto.IntentosFallidos,
        ["USL_BLOQUEADO_HASTA"] = dto.BloqueadoHasta,
        ["USL_TOKEN_RECUPERACION"] = dto.TokenRecuperacion,
        ["USL_VENCIMIENTO_TOKEN"] = dto.VencimientoToken
    };
}
