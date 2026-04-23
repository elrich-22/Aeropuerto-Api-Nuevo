namespace AeropuertoAurora.Api.DTOs;

public sealed record ControlSeguridadOperacionDto(
    int Id,
    int PasajeroId,
    int VueloId,
    int? EmpleadoId,
    string Resultado,
    DateTime? FechaHora,
    string? Observacion);

public sealed record CrearControlSeguridadOperacionDto(
    int PasajeroId,
    int VueloId,
    int? EmpleadoId,
    string Resultado,
    DateTime? FechaHora,
    string? Observacion);

public sealed record ActualizarControlSeguridadOperacionDto(
    int PasajeroId,
    int VueloId,
    int? EmpleadoId,
    string Resultado,
    DateTime? FechaHora,
    string? Observacion);
