namespace AeropuertoAurora.Api.DTOs;

public sealed record AuditoriaDto(
    int Id,
    string TablaAfectada,
    string Operacion,
    string Usuario,
    DateTime? FechaHora,
    string? IpAddress,
    string? DatosAnteriores,
    string? DatosNuevos);

public sealed record CrearAuditoriaDto(
    string TablaAfectada,
    string Operacion,
    string Usuario,
    DateTime? FechaHora,
    string? IpAddress,
    string? DatosAnteriores,
    string? DatosNuevos);

public sealed record ActualizarAuditoriaDto(
    string TablaAfectada,
    string Operacion,
    string Usuario,
    DateTime? FechaHora,
    string? IpAddress,
    string? DatosAnteriores,
    string? DatosNuevos);
