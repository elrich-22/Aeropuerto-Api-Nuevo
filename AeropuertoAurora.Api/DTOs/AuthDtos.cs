namespace AeropuertoAurora.Api.DTOs;

public sealed record LoginRequestDto(
    string UsuarioOEmail,
    string Contrasena);

public sealed record RegisterRequestDto(
    string Usuario,
    string Email,
    string Contrasena,
    string NumeroDocumento,
    string TipoDocumento,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    DateTime? FechaNacimiento,
    string? Nacionalidad,
    string? Sexo,
    string? Telefono);

public sealed record UsuarioSesionDto(
    int UsuarioId,
    int PasajeroId,
    string Usuario,
    string Email,
    string NombreCompleto,
    string? NumeroDocumento,
    string? TipoDocumento,
    string? Telefono);

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
    bool? EnviarCorreoConfirmacion);

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
    string? CorreoConfirmacionDestino);

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
