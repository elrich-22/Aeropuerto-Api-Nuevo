using System.Security.Claims;
using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IOracleCrudRepository repository,
    IAeropuertoQueryService service,
    IJwtService jwtService,
    ILogger<AuthController> logger) : ControllerBase
{
    private const int MaxFailedAttempts = 5;
    private const int LockoutMinutes = 30;

    private static readonly CrudTableDefinition PassengersTable = new(
        "AER_PASAJERO",
        "PAS_ID_PASAJERO",
        ["PAS_NUMERO_DOCUMENTO", "PAS_TIPO_DOCUMENTO", "PAS_PRIMER_NOMBRE", "PAS_SEGUNDO_NOMBRE", "PAS_PRIMER_APELLIDO", "PAS_SEGUNDO_APELLIDO", "PAS_FECHA_NACIMIENTO", "PAS_NACIONALIDAD", "PAS_SEXO", "PAS_TELEFONO", "PAS_EMAIL", "PAS_FECHA_REGISTRO"],
        ["PAS_NUMERO_DOCUMENTO", "PAS_TIPO_DOCUMENTO", "PAS_PRIMER_NOMBRE", "PAS_SEGUNDO_NOMBRE", "PAS_PRIMER_APELLIDO", "PAS_SEGUNDO_APELLIDO", "PAS_FECHA_NACIMIENTO", "PAS_NACIONALIDAD", "PAS_SEXO", "PAS_TELEFONO", "PAS_EMAIL", "PAS_FECHA_REGISTRO"]);

    private static readonly CrudTableDefinition PassengerDocumentsTable = new(
        "AER_DOCUMENTO_PASAJERO",
        "DCP_ID_DOCUMENTO",
        ["DCP_ID_PASAJERO", "DCP_TIPO_DOCUMENTO", "DCP_NUMERO_DOCUMENTO", "DCP_PAIS_EMISOR", "DCP_FECHA_VENCIMIENTO", "DCP_ESTADO_VALIDACION", "DCP_FECHA_REGISTRO", "DCP_FECHA_VALIDACION"],
        ["DCP_ID_PASAJERO", "DCP_TIPO_DOCUMENTO", "DCP_NUMERO_DOCUMENTO", "DCP_PAIS_EMISOR", "DCP_FECHA_VENCIMIENTO", "DCP_ESTADO_VALIDACION", "DCP_FECHA_REGISTRO", "DCP_FECHA_VALIDACION"]);

    private static readonly CrudTableDefinition UsersTable = new(
        "AER_USUARIO_LOGIN",
        "USL_ID_USUARIO",
        ["USL_ID_PASAJERO", "USL_USUARIO", "USL_EMAIL", "USL_CONTRASENA_HASH", "USL_SAL", "USL_ESTADO", "USL_EMAIL_VERIFICADO", "USL_TOKEN_VERIFICACION", "USL_FECHA_REGISTRO", "USL_ULTIMO_ACCESO", "USL_INTENTOS_FALLIDOS", "USL_BLOQUEADO_HASTA", "USL_TOKEN_RECUPERACION", "USL_VENCIMIENTO_TOKEN", "USL_ROL"],
        ["USL_ID_PASAJERO", "USL_USUARIO", "USL_EMAIL", "USL_CONTRASENA_HASH", "USL_SAL", "USL_ESTADO", "USL_EMAIL_VERIFICADO", "USL_TOKEN_VERIFICACION", "USL_FECHA_REGISTRO", "USL_ULTIMO_ACCESO", "USL_INTENTOS_FALLIDOS", "USL_BLOQUEADO_HASTA", "USL_TOKEN_RECUPERACION", "USL_VENCIMIENTO_TOKEN"]);

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.UsuarioOEmail) || string.IsNullOrWhiteSpace(dto.Contrasena))
        {
            return BadRequest(new { message = "Usuario y contrasena son obligatorios." });
        }

        var user = await repository.GetByLoginIdentifierAsync(
            UsersTable, "USL_USUARIO", "USL_EMAIL", dto.UsuarioOEmail, cancellationToken);

        if (user is null)
        {
            return Unauthorized(new { message = "Usuario o contrasena incorrectos." });
        }

        var userId = user.ToInt("USL_ID_USUARIO");
        var estado = user.ToStringValue("USL_ESTADO");

        if (string.Equals(estado, "INACTIVO", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(new { message = "La cuenta esta inactiva. Contacta al administrador." });
        }

        var bloqueadoHasta = user.ToNullableDateTime("USL_BLOQUEADO_HASTA");
        if (bloqueadoHasta.HasValue && bloqueadoHasta.Value > DateTime.Now)
        {
            var minutosRestantes = (int)Math.Ceiling((bloqueadoHasta.Value - DateTime.Now).TotalMinutes);
            return StatusCode(StatusCodes.Status423Locked, new
            {
                message = $"Cuenta bloqueada temporalmente. Intenta de nuevo en {minutosRestantes} minuto(s)."
            });
        }

        if (!PasswordMatches(dto.Contrasena, user.ToStringValue("USL_CONTRASENA_HASH"), user.ToStringValue("USL_SAL")))
        {
            await RegisterFailedAttemptAsync(userId, user, cancellationToken);
            return Unauthorized(new { message = "Usuario o contrasena incorrectos." });
        }

        await RegisterSuccessfulLoginAsync(userId, cancellationToken);

        var passengerId = user.ToInt("USL_ID_PASAJERO");
        var passenger = (await service.GetPassengersAsync(500, cancellationToken)).FirstOrDefault(p => p.Id == passengerId);
        var fullName = passenger is null
            ? user.ToStringValue("USL_USUARIO")
            : string.Join(" ", new[] { passenger.PrimerNombre, passenger.SegundoNombre, passenger.PrimerApellido, passenger.SegundoApellido }
                .Where(part => !string.IsNullOrWhiteSpace(part)));

        var usuarioStr = user.ToStringValue("USL_USUARIO");
        var emailStr = user.ToStringValue("USL_EMAIL");
        var rol = user.ToStringValue("USL_ROL") ?? "PASAJERO";
        var token = jwtService.GenerateToken(userId, passengerId, usuarioStr, emailStr, rol);

        return Ok(new UsuarioSesionDto(
            userId,
            passengerId,
            usuarioStr,
            emailStr,
            fullName,
            passenger?.NumeroDocumento,
            passenger?.TipoDocumento,
            passenger?.Telefono,
            rol,
            token));
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
            string.IsNullOrWhiteSpace(dto.PrimerApellido) ||
            dto.FechaNacimiento is null)
        {
            return BadRequest(new { message = "Completa todos los campos obligatorios del registro." });
        }

        try { _ = new System.Net.Mail.MailAddress(dto.Email); }
        catch { return BadRequest(new { message = "Ingresa un email valido." }); }

        var existing = await repository.GetByLoginIdentifierAsync(
            UsersTable, "USL_USUARIO", "USL_EMAIL", dto.Usuario, cancellationToken);
        if (existing is not null)
        {
            return Conflict(new { message = "Ya existe un usuario con ese nombre." });
        }

        var existingEmail = await repository.GetByLoginIdentifierAsync(
            UsersTable, "USL_USUARIO", "USL_EMAIL", dto.Email, cancellationToken);
        if (existingEmail is not null)
        {
            return Conflict(new { message = "Ya existe un usuario con ese email." });
        }

        var passportValidationError = ValidatePassportDocument(dto.TipoDocumento, dto.NumeroDocumento, dto.PaisEmisorDocumento, dto.FechaVencimientoDocumento);
        if (passportValidationError is not null)
        {
            return BadRequest(new { message = passportValidationError });
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

        await TryUpsertPassengerDocumentAsync(
            passengerId,
            dto.TipoDocumento,
            dto.NumeroDocumento,
            dto.PaisEmisorDocumento,
            dto.FechaVencimientoDocumento,
            cancellationToken);

        var passwordData = PasswordHasher.HashPassword(dto.Contrasena);
        var userId = await repository.CreateAsync(UsersTable, new Dictionary<string, object?>
        {
            ["USL_ID_PASAJERO"] = passengerId,
            ["USL_USUARIO"] = dto.Usuario,
            ["USL_EMAIL"] = dto.Email,
            ["USL_CONTRASENA_HASH"] = passwordData.Hash,
            ["USL_SAL"] = passwordData.Salt,
            ["USL_ESTADO"] = "ACTIVO",
            ["USL_EMAIL_VERIFICADO"] = "S",
            ["USL_TOKEN_VERIFICACION"] = null,
            ["USL_FECHA_REGISTRO"] = DateTime.Now,
            ["USL_ULTIMO_ACCESO"] = DateTime.Now,
            ["USL_INTENTOS_FALLIDOS"] = 0,
            ["USL_BLOQUEADO_HASTA"] = null,
            ["USL_TOKEN_RECUPERACION"] = null,
            ["USL_VENCIMIENTO_TOKEN"] = null,
            ["USL_ROL"] = "PASAJERO"
        }, cancellationToken);

        const string nuevaRol = "PASAJERO";
        var fullName = string.Join(" ", new[] { dto.PrimerNombre, dto.SegundoNombre, dto.PrimerApellido, dto.SegundoApellido }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
        var token = jwtService.GenerateToken(userId, passengerId, dto.Usuario, dto.Email, nuevaRol);

        return CreatedAtAction(nameof(Login), new UsuarioSesionDto(
            userId,
            passengerId,
            dto.Usuario,
            dto.Email,
            fullName,
            dto.NumeroDocumento,
            dto.TipoDocumento,
            dto.Telefono,
            nuevaRol,
            token));
    }

    [Authorize]
    [HttpGet("perfil")]
    public IActionResult Perfil()
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : 0;
        var pasajeroId = int.TryParse(User.FindFirstValue("pasajeroId"), out var pid) ? pid : 0;
        var usuario = User.FindFirstValue("usuario") ?? string.Empty;
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email") ?? string.Empty;
        var rol = User.FindFirstValue(ClaimTypes.Role) ?? "PASAJERO";

        return Ok(new PerfilUsuarioDto(userId, pasajeroId, usuario, email, rol));
    }

    private async Task RegisterFailedAttemptAsync(
        int userId,
        IReadOnlyDictionary<string, object?> user,
        CancellationToken cancellationToken)
    {
        var intentos = (user.ToNullableInt("USL_INTENTOS_FALLIDOS") ?? 0) + 1;
        DateTime? bloqueadoHasta = intentos >= MaxFailedAttempts
            ? DateTime.Now.AddMinutes(LockoutMinutes)
            : null;

        await repository.UpdatePartialAsync(UsersTable, userId, new Dictionary<string, object?>
        {
            ["USL_INTENTOS_FALLIDOS"] = intentos,
            ["USL_BLOQUEADO_HASTA"] = (object?)bloqueadoHasta ?? DBNull.Value
        }, cancellationToken);
    }

    private async Task RegisterSuccessfulLoginAsync(int userId, CancellationToken cancellationToken)
    {
        await repository.UpdatePartialAsync(UsersTable, userId, new Dictionary<string, object?>
        {
            ["USL_INTENTOS_FALLIDOS"] = 0,
            ["USL_BLOQUEADO_HASTA"] = DBNull.Value,
            ["USL_ULTIMO_ACCESO"] = DateTime.Now
        }, cancellationToken);
    }

    private static bool PasswordMatches(string password, string storedHash, string salt) =>
        PasswordHasher.VerifyPassword(password, storedHash, salt);

    private async Task UpsertPassengerDocumentAsync(
        int passengerId,
        string tipoDocumento,
        string numeroDocumento,
        string? paisEmisor,
        DateTime? fechaVencimiento,
        CancellationToken cancellationToken)
    {
        var existingByPassenger = await repository.GetByColumnAsync(PassengerDocumentsTable, "DCP_ID_PASAJERO", passengerId, cancellationToken);
        var normalizedType = NormalizeDocumentType(tipoDocumento);
        var now = DateTime.Now;
        var validationStatus = normalizedType == "PASAPORTE" ? "VALIDADO" : "NO_REQUIERE";

        var existingDocument = existingByPassenger.FirstOrDefault(row =>
            string.Equals(row.ToNullableString("DCP_TIPO_DOCUMENTO"), normalizedType, StringComparison.OrdinalIgnoreCase));

        if (existingDocument is not null)
        {
            await repository.UpdateAsync(PassengerDocumentsTable, existingDocument.ToInt("DCP_ID_DOCUMENTO"), new Dictionary<string, object?>
            {
                ["DCP_ID_PASAJERO"] = passengerId,
                ["DCP_TIPO_DOCUMENTO"] = normalizedType,
                ["DCP_NUMERO_DOCUMENTO"] = numeroDocumento.Trim(),
                ["DCP_PAIS_EMISOR"] = string.IsNullOrWhiteSpace(paisEmisor) ? null : paisEmisor.Trim(),
                ["DCP_FECHA_VENCIMIENTO"] = fechaVencimiento,
                ["DCP_ESTADO_VALIDACION"] = validationStatus,
                ["DCP_FECHA_REGISTRO"] = existingDocument.ToNullableDateTime("DCP_FECHA_REGISTRO") ?? now,
                ["DCP_FECHA_VALIDACION"] = normalizedType == "PASAPORTE" ? now : null
            }, cancellationToken);

            return;
        }

        await repository.CreateAsync(PassengerDocumentsTable, new Dictionary<string, object?>
        {
            ["DCP_ID_PASAJERO"] = passengerId,
            ["DCP_TIPO_DOCUMENTO"] = normalizedType,
            ["DCP_NUMERO_DOCUMENTO"] = numeroDocumento.Trim(),
            ["DCP_PAIS_EMISOR"] = string.IsNullOrWhiteSpace(paisEmisor) ? null : paisEmisor.Trim(),
            ["DCP_FECHA_VENCIMIENTO"] = fechaVencimiento,
            ["DCP_ESTADO_VALIDACION"] = validationStatus,
            ["DCP_FECHA_REGISTRO"] = now,
            ["DCP_FECHA_VALIDACION"] = normalizedType == "PASAPORTE" ? now : null
        }, cancellationToken);
    }

    private async Task TryUpsertPassengerDocumentAsync(
        int passengerId,
        string tipoDocumento,
        string numeroDocumento,
        string? paisEmisor,
        DateTime? fechaVencimiento,
        CancellationToken cancellationToken)
    {
        try
        {
            await UpsertPassengerDocumentAsync(
                passengerId,
                tipoDocumento,
                numeroDocumento,
                paisEmisor,
                fechaVencimiento,
                cancellationToken);
        }
        catch (OracleException ex) when (IsMissingPassengerDocumentsTable(ex))
        {
            logger.LogWarning(
                "Se omitio el guardado en AER_DOCUMENTO_PASAJERO para el pasajero {PassengerId} porque la tabla aun no existe. Ejecuta alter_add_documento_pasajero.sql. Detalle: {Error}",
                passengerId,
                ex.Message);
        }
    }

    private static string? ValidatePassportDocument(
        string? tipoDocumento,
        string? numeroDocumento,
        string? paisEmisor,
        DateTime? fechaVencimiento)
    {
        if (!string.Equals(NormalizeDocumentType(tipoDocumento), "PASAPORTE", StringComparison.Ordinal))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(numeroDocumento) || numeroDocumento.Trim().Length < 6)
        {
            return "El numero de pasaporte no tiene un formato valido.";
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(numeroDocumento.Trim(), "^[A-Za-z0-9-]{6,20}$"))
        {
            return "El numero de pasaporte solo puede contener letras, numeros o guiones.";
        }

        if (string.IsNullOrWhiteSpace(paisEmisor))
        {
            return "Debes indicar el pais emisor del pasaporte.";
        }

        if (!fechaVencimiento.HasValue)
        {
            return "Debes indicar la fecha de vencimiento del pasaporte.";
        }

        if (fechaVencimiento.Value.Date <= DateTime.Today)
        {
            return "El pasaporte ya esta vencido o vence hoy.";
        }

        return null;
    }

    private static string NormalizeDocumentType(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "PASAPORTE" or "PASSPORT" => "PASAPORTE",
            "DPI" => "DPI",
            "LICENCIA" => "LICENCIA",
            _ => normalized ?? string.Empty
        };
    }

    private static bool IsMissingPassengerDocumentsTable(OracleException exception)
    {
        return exception.Number == 942 ||
            exception.Message.Contains("ORA-00942", StringComparison.OrdinalIgnoreCase);
    }
}
