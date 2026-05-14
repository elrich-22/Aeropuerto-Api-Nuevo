namespace AeropuertoAurora.Api.DTOs;

public sealed record ProgramaVueloDto(
    int Id,
    string NumeroVuelo,
    int AerolineaId,
    int AeropuertoOrigenId,
    int AeropuertoDestinoId,
    string HoraSalidaProgramada,
    string HoraLlegadaProgramada,
    int? DuracionEstimada,
    string? TipoVuelo,
    string Estado);

public sealed record CrearProgramaVueloDto(
    string NumeroVuelo,
    int AerolineaId,
    int AeropuertoOrigenId,
    int AeropuertoDestinoId,
    string HoraSalidaProgramada,
    string HoraLlegadaProgramada,
    int? DuracionEstimada,
    string? TipoVuelo,
    string Estado);

public sealed record ActualizarProgramaVueloDto(
    string NumeroVuelo,
    int AerolineaId,
    int AeropuertoOrigenId,
    int AeropuertoDestinoId,
    string HoraSalidaProgramada,
    string HoraLlegadaProgramada,
    int? DuracionEstimada,
    string? TipoVuelo,
    string Estado);

public sealed record VueloDto(
    int Id,
    string NumeroVuelo,
    string Aerolinea,
    string Origen,
    string Destino,
    DateTime FechaVuelo,
    DateTime? SalidaReal,
    DateTime? LlegadaReal,
    int PlazasOcupadas,
    int PlazasDisponibles,
    string Estado,
    int RetrasoMinutos,
    string MatriculaAvion);

public sealed record TripulacionDto(
    int Id,
    int VueloId,
    int EmpleadoId,
    string Rol,
    decimal? HorasVuelo);

public sealed record CrearTripulacionDto(
    int VueloId,
    int EmpleadoId,
    string Rol,
    decimal? HorasVuelo);

public sealed record ActualizarTripulacionDto(
    int VueloId,
    int EmpleadoId,
    string Rol,
    decimal? HorasVuelo);

public sealed record AsignacionPuertaDto(
    int Id,
    int VueloId,
    int PuertaId,
    DateTime FechaHoraInicio,
    DateTime? FechaHoraFin,
    string Estado);

public sealed record CrearAsignacionPuertaDto(
    int VueloId,
    int PuertaId,
    DateTime FechaHoraInicio,
    DateTime? FechaHoraFin,
    string Estado);

public sealed record ActualizarAsignacionPuertaDto(
    int VueloId,
    int PuertaId,
    DateTime FechaHoraInicio,
    DateTime? FechaHoraFin,
    string Estado);

public sealed record AsignacionAsientoDto(
    int Id,
    int VueloId,
    int PasajeroId,
    int AsientoId,
    DateTime? FechaAsignacion,
    string Estado);

public sealed record CrearAsignacionAsientoDto(
    int VueloId,
    int PasajeroId,
    int AsientoId,
    DateTime? FechaAsignacion,
    string Estado);

public sealed record ActualizarAsignacionAsientoDto(
    int VueloId,
    int PasajeroId,
    int AsientoId,
    DateTime? FechaAsignacion,
    string Estado);

public sealed record ActualizarVueloEstadoDto(
    string Estado,
    DateTime? FechaVuelo);
