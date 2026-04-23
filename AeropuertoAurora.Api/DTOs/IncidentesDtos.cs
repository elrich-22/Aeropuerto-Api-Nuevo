namespace AeropuertoAurora.Api.DTOs;

public sealed record IncidenteDto(
    int Id,
    string Tipo,
    string Descripcion,
    DateTime? FechaHora,
    string Severidad,
    string Estado,
    int? VueloId,
    int? EmpleadoReportaId);

public sealed record CrearIncidenteDto(
    string Tipo,
    string Descripcion,
    DateTime? FechaHora,
    string Severidad,
    string Estado,
    int? VueloId,
    int? EmpleadoReportaId);

public sealed record ActualizarIncidenteDto(
    string Tipo,
    string Descripcion,
    DateTime? FechaHora,
    string Severidad,
    string Estado,
    int? VueloId,
    int? EmpleadoReportaId);
