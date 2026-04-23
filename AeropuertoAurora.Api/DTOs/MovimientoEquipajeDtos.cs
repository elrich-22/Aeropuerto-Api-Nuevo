namespace AeropuertoAurora.Api.DTOs;

public sealed record MovimientoEquipajeDto(
    int Id,
    int EquipajeId,
    string Ubicacion,
    string Estado,
    DateTime? FechaHora,
    string? Observacion);

public sealed record CrearMovimientoEquipajeDto(
    int EquipajeId,
    string Ubicacion,
    string Estado,
    DateTime? FechaHora,
    string? Observacion);

public sealed record ActualizarMovimientoEquipajeDto(
    int EquipajeId,
    string Ubicacion,
    string Estado,
    DateTime? FechaHora,
    string? Observacion);
