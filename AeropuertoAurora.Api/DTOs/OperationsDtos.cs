namespace AeropuertoAurora.Api.DTOs;

public sealed record EquipajeDto(
    int Id,
    string CodigoBarras,
    string Pasajero,
    int VueloId,
    string NumeroVuelo,
    decimal PesoKg,
    string Estado);

public sealed record MantenimientoDto(
    int Id,
    string MatriculaAvion,
    string TipoMantenimiento,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    decimal Costo,
    string Estado,
    string? Descripcion);

public sealed record ControlSeguridadDto(
    int Id,
    string Pasajero,
    int VueloId,
    string NumeroVuelo,
    string Empleado,
    string Resultado,
    string? Observacion);

public sealed record IncidenteOperacionDto(
    int Id,
    string TipoIncidente,
    DateTime? FechaIncidente,
    string Ubicacion,
    string Severidad,
    string Estado,
    string? Descripcion);
