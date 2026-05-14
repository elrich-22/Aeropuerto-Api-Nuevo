using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IOracleCrudRepository repository, IAeropuertoQueryService service) : ControllerBase
{
    private static readonly CrudTableDefinition PassengersTable = new(
        "AER_PASAJERO",
        "PAS_ID_PASAJERO",
        ["PAS_NUMERO_DOCUMENTO", "PAS_TIPO_DOCUMENTO", "PAS_PRIMER_NOMBRE", "PAS_SEGUNDO_NOMBRE", "PAS_PRIMER_APELLIDO", "PAS_SEGUNDO_APELLIDO", "PAS_FECHA_NACIMIENTO", "PAS_NACIONALIDAD", "PAS_SEXO", "PAS_TELEFONO", "PAS_EMAIL", "PAS_FECHA_REGISTRO"],
        ["PAS_NUMERO_DOCUMENTO", "PAS_TIPO_DOCUMENTO", "PAS_PRIMER_NOMBRE", "PAS_SEGUNDO_NOMBRE", "PAS_PRIMER_APELLIDO", "PAS_SEGUNDO_APELLIDO", "PAS_FECHA_NACIMIENTO", "PAS_NACIONALIDAD", "PAS_SEXO", "PAS_TELEFONO", "PAS_EMAIL", "PAS_FECHA_REGISTRO"]);

    private static readonly CrudTableDefinition UsersTable = new(
        "AER_USUARIO_LOGIN",
        "USL_ID_USUARIO",
        ["USL_ID_PASAJERO", "USL_USUARIO", "USL_EMAIL", "USL_CONTRASENA_HASH", "USL_SAL", "USL_ESTADO", "USL_EMAIL_VERIFICADO", "USL_TOKEN_VERIFICACION", "USL_FECHA_REGISTRO", "USL_ULTIMO_ACCESO", "USL_INTENTOS_FALLIDOS", "USL_BLOQUEADO_HASTA", "USL_TOKEN_RECUPERACION", "USL_VENCIMIENTO_TOKEN"],
        ["USL_ID_PASAJERO", "USL_USUARIO", "USL_EMAIL", "USL_CONTRASENA_HASH", "USL_SAL", "USL_ESTADO", "USL_EMAIL_VERIFICADO", "USL_TOKEN_VERIFICACION", "USL_FECHA_REGISTRO", "USL_ULTIMO_ACCESO", "USL_INTENTOS_FALLIDOS", "USL_BLOQUEADO_HASTA", "USL_TOKEN_RECUPERACION", "USL_VENCIMIENTO_TOKEN"]);

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.UsuarioOEmail) || string.IsNullOrWhiteSpace(dto.Contrasena))
        {
            return BadRequest(new { message = "Usuario y contrasena son obligatorios." });
        }

        var users = await repository.GetAllAsync(UsersTable, 500, cancellationToken);
        var user = users.FirstOrDefault(row =>
            string.Equals(row.ToStringValue("USL_USUARIO"), dto.UsuarioOEmail, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(row.ToStringValue("USL_EMAIL"), dto.UsuarioOEmail, StringComparison.OrdinalIgnoreCase));

        if (user is null || !string.Equals(user.ToStringValue("USL_ESTADO"), "ACTIVO", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new { message = "Usuario o contrasena incorrectos." });
        }

        if (!PasswordMatches(dto.Contrasena, user.ToStringValue("USL_CONTRASENA_HASH"), user.ToStringValue("USL_SAL")))
        {
            return Unauthorized(new { message = "Usuario o contrasena incorrectos." });
        }

        var passengerId = user.ToInt("USL_ID_PASAJERO");
        var passenger = (await service.GetPassengersAsync(500, cancellationToken)).FirstOrDefault(item => item.Id == passengerId);
        var fullName = passenger is null
            ? user.ToStringValue("USL_USUARIO")
            : string.Join(" ", new[] { passenger.PrimerNombre, passenger.SegundoNombre, passenger.PrimerApellido, passenger.SegundoApellido }.Where(part => !string.IsNullOrWhiteSpace(part)));

        return Ok(new UsuarioSesionDto(
            user.ToInt("USL_ID_USUARIO"),
            passengerId,
            user.ToStringValue("USL_USUARIO"),
            user.ToStringValue("USL_EMAIL"),
            fullName,
            passenger?.NumeroDocumento,
            passenger?.TipoDocumento,
            passenger?.Telefono));
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.Usuario) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Contrasena) ||
            string.IsNullOrWhiteSpace(dto.NumeroDocumento) ||
            string.IsNullOrWhiteSpace(dto.TipoDocumento) ||
            string.IsNullOrWhiteSpace(dto.PrimerNombre) ||
            string.IsNullOrWhiteSpace(dto.SegundoNombre) ||
            string.IsNullOrWhiteSpace(dto.PrimerApellido) ||
            string.IsNullOrWhiteSpace(dto.SegundoApellido) ||
            dto.FechaNacimiento is null ||
            string.IsNullOrWhiteSpace(dto.Nacionalidad) ||
            string.IsNullOrWhiteSpace(dto.Sexo) ||
            string.IsNullOrWhiteSpace(dto.Telefono))
        {
            return BadRequest(new { message = "Completa todos los campos del registro." });
        }

        if (!dto.Email.Contains('@', StringComparison.Ordinal) || !dto.Email.Contains('.', StringComparison.Ordinal))
        {
            return BadRequest(new { message = "Ingresa un email valido." });
        }

        var users = await repository.GetAllAsync(UsersTable, 1000, cancellationToken);
        if (users.Any(row =>
            string.Equals(row.ToStringValue("USL_USUARIO"), dto.Usuario, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(row.ToStringValue("USL_EMAIL"), dto.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(new { message = "Ya existe un usuario con ese nombre o email." });
        }

        var passengerId = await repository.CreateAsync(PassengersTable, new Dictionary<string, object?>
        {
            ["PAS_NUMERO_DOCUMENTO"] = dto.NumeroDocumento,
            ["PAS_TIPO_DOCUMENTO"] = dto.TipoDocumento,
            ["PAS_PRIMER_NOMBRE"] = dto.PrimerNombre,
            ["PAS_SEGUNDO_NOMBRE"] = dto.SegundoNombre,
            ["PAS_PRIMER_APELLIDO"] = dto.PrimerApellido,
            ["PAS_SEGUNDO_APELLIDO"] = dto.SegundoApellido,
            ["PAS_FECHA_NACIMIENTO"] = dto.FechaNacimiento,
            ["PAS_NACIONALIDAD"] = dto.Nacionalidad,
            ["PAS_SEXO"] = string.IsNullOrWhiteSpace(dto.Sexo) ? null : dto.Sexo.ToUpperInvariant()[..1],
            ["PAS_TELEFONO"] = dto.Telefono,
            ["PAS_EMAIL"] = dto.Email,
            ["PAS_FECHA_REGISTRO"] = DateTime.Now
        }, cancellationToken);

        var salt = Guid.NewGuid().ToString("N")[..12];
        var userId = await repository.CreateAsync(UsersTable, new Dictionary<string, object?>
        {
            ["USL_ID_PASAJERO"] = passengerId,
            ["USL_USUARIO"] = dto.Usuario,
            ["USL_EMAIL"] = dto.Email,
            ["USL_CONTRASENA_HASH"] = $"{dto.Contrasena}:{salt}",
            ["USL_SAL"] = salt,
            ["USL_ESTADO"] = "ACTIVO",
            ["USL_EMAIL_VERIFICADO"] = "S",
            ["USL_TOKEN_VERIFICACION"] = null,
            ["USL_FECHA_REGISTRO"] = DateTime.Now,
            ["USL_ULTIMO_ACCESO"] = DateTime.Now,
            ["USL_INTENTOS_FALLIDOS"] = 0,
            ["USL_BLOQUEADO_HASTA"] = null,
            ["USL_TOKEN_RECUPERACION"] = null,
            ["USL_VENCIMIENTO_TOKEN"] = null
        }, cancellationToken);

        var fullName = string.Join(" ", new[] { dto.PrimerNombre, dto.SegundoNombre, dto.PrimerApellido, dto.SegundoApellido }.Where(part => !string.IsNullOrWhiteSpace(part)));

        return CreatedAtAction(nameof(Login), new UsuarioSesionDto(
            userId,
            passengerId,
            dto.Usuario,
            dto.Email,
            fullName,
            dto.NumeroDocumento,
            dto.TipoDocumento,
            dto.Telefono));
    }

    private static bool PasswordMatches(string password, string storedHash, string salt)
    {
        if (string.Equals(password, storedHash, StringComparison.Ordinal))
        {
            return true;
        }

        if (storedHash.StartsWith("$2a$10$demoHash", StringComparison.Ordinal) &&
            string.Equals(password, "demo", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return string.Equals($"{password}:{salt}", storedHash, StringComparison.Ordinal);
    }
}
