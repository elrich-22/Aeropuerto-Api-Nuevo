using System.ComponentModel.DataAnnotations;

namespace AeropuertoAurora.Api.DTOs;

public sealed record LoginRequestDto(
    [StringLength(100, MinimumLength = 1)] string UsuarioOEmail,
    [StringLength(200, MinimumLength = 1)] string Contrasena);

public sealed record RegisterRequestDto(
    [StringLength(50, MinimumLength = 1)] string Usuario,
    [StringLength(254, MinimumLength = 1)] string Email,
    [StringLength(200, MinimumLength = 8)] string Contrasena,
    [StringLength(30, MinimumLength = 1)] string NumeroDocumento,
    [StringLength(20, MinimumLength = 1)] string TipoDocumento,
    [StringLength(50, MinimumLength = 1)] string PrimerNombre,
    [StringLength(50)] string? SegundoNombre,
    [StringLength(50, MinimumLength = 1)] string PrimerApellido,
    [StringLength(50)] string? SegundoApellido,
    DateTime? FechaNacimiento,
    [StringLength(50)] string? Nacionalidad,
    [StringLength(1)] string? Sexo,
    [StringLength(20)] string? Telefono);

public sealed record UsuarioSesionDto(
    int UsuarioId,
    int PasajeroId,
    string Usuario,
    string Email,
    string NombreCompleto,
    string? NumeroDocumento,
    string? TipoDocumento,
    string? Telefono,
    string? Token = null);

public sealed record PerfilUsuarioDto(
    int UsuarioId,
    int PasajeroId,
    string Usuario,
    string Email);

public sealed record PasajeroAdicionalDto(
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

public sealed record ReservaAdicionalInfo(
    int PasajeroId,
    int ReservaId,
    string CodigoReserva);

public sealed record CompraVueloRequestDto(
    int UsuarioId,
    int PasajeroId,
    int VueloId,
    string Clase,
    int NumeroPasajeros,
    int EquipajeFacturado,
    decimal? PesoEquipaje,
    decimal TarifaPagada,
    int MetodoPagoId,
    string? EmailConfirmacion,
    bool? EnviarCorreoConfirmacion,
    IReadOnlyList<PasajeroAdicionalDto>? PasajerosAdicionales = null);

public sealed record CompraVueloResponseDto(
    int ReservaId,
    int VentaId,
    int DetalleVentaId,
    string CodigoReserva,
    string NumeroVenta,
    decimal Total,
    int NumeroPasajeros,
    int PlazasOcupadas,
    int PlazasDisponibles,
    bool CorreoConfirmacionEnviado,
    string? CorreoConfirmacionDestino,
    IReadOnlyList<ReservaAdicionalInfo> ReservasAdicionales);

public sealed record CompraConfirmacionEmailRequestDto(
    string EmailConfirmacion,
    string? PasajeroNombre,
    decimal Total,
    IReadOnlyList<CompraConfirmacionReservaDto> Reservas);

public sealed record CompraConfirmacionReservaDto(
    string CodigoReserva,
    string NumeroVenta,
    string NumeroVuelo,
    string Aerolinea,
    string Origen,
    string Destino,
    DateTime FechaVuelo,
    string Clase,
    decimal Total);
