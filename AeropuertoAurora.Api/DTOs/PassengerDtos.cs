namespace AeropuertoAurora.Api.DTOs;

public sealed record PasajeroDto(
    int Id,
    string NumeroDocumento,
    string TipoDocumento,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    DateTime? FechaNacimiento,
    string? Nacionalidad,
    string? Sexo,
    string? Telefono,
    string? Email);

public sealed record UsuarioLoginDto(
    int Id,
    int PasajeroId,
    string Usuario,
    string Email,
    string Estado,
    string EmailVerificado,
    DateTime? FechaRegistro,
    DateTime? UltimoAcceso,
    int IntentosFallidos,
    DateTime? BloqueadoHasta,
    string Rol);

public sealed record CrearUsuarioLoginDto(
    int PasajeroId,
    string Usuario,
    string Email,
    string ContrasenaHash,
    string Sal,
    string Estado,
    string EmailVerificado,
    string? TokenVerificacion,
    DateTime? FechaRegistro,
    DateTime? UltimoAcceso,
    int IntentosFallidos,
    DateTime? BloqueadoHasta,
    string? TokenRecuperacion,
    DateTime? VencimientoToken);

public sealed record ActualizarUsuarioLoginDto(
    int PasajeroId,
    string Usuario,
    string Email,
    string ContrasenaHash,
    string Sal,
    string Estado,
    string EmailVerificado,
    string? TokenVerificacion,
    DateTime? FechaRegistro,
    DateTime? UltimoAcceso,
    int IntentosFallidos,
    DateTime? BloqueadoHasta,
    string? TokenRecuperacion,
    DateTime? VencimientoToken);

public sealed record SesionUsuarioDto(
    int Id,
    string SesionId,
    int? UsuarioId,
    int? PasajeroId,
    string? IpAddress,
    string? Navegador,
    string? SistemaOperativo,
    string? Dispositivo,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    int? DuracionSegundos);

public sealed record CrearSesionUsuarioDto(
    string SesionId,
    int? UsuarioId,
    int? PasajeroId,
    string? IpAddress,
    string? Navegador,
    string? SistemaOperativo,
    string? Dispositivo,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    int? DuracionSegundos);

public sealed record ActualizarSesionUsuarioDto(
    string SesionId,
    int? UsuarioId,
    int? PasajeroId,
    string? IpAddress,
    string? Navegador,
    string? SistemaOperativo,
    string? Dispositivo,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    int? DuracionSegundos);

public sealed record PreferenciaClienteDto(
    int Id,
    int PasajeroId,
    string TipoPreferencia,
    string? ValorPreferencia,
    DateTime? FechaRegistro);

public sealed record CrearPreferenciaClienteDto(
    int PasajeroId,
    string TipoPreferencia,
    string? ValorPreferencia,
    DateTime? FechaRegistro);

public sealed record ActualizarPreferenciaClienteDto(
    int PasajeroId,
    string TipoPreferencia,
    string? ValorPreferencia,
    DateTime? FechaRegistro);
