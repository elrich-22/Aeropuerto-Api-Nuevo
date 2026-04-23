namespace AeropuertoAurora.Api.DTOs;

public sealed record ControlMigratorioDto(
    int Id,
    int PasajeroId,
    int VueloId,
    int? EmpleadoId,
    string Tipo,
    string Resultado,
    DateTime? FechaHora,
    string? Observacion);

public sealed record CrearControlMigratorioDto(
    int PasajeroId,
    int VueloId,
    int? EmpleadoId,
    string Tipo,
    string Resultado,
    DateTime? FechaHora,
    string? Observacion);

public sealed record ActualizarControlMigratorioDto(
    int PasajeroId,
    int VueloId,
    int? EmpleadoId,
    string Tipo,
    string Resultado,
    DateTime? FechaHora,
    string? Observacion);
