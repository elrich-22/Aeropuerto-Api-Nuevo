namespace AeropuertoAurora.Api.DTOs;

public sealed record AvionDto(
    int Id,
    string Matricula,
    int ModeloId,
    int AerolineaId,
    int? AnioFabricacion,
    string Estado,
    DateTime? UltimaRevision,
    DateTime? ProximaRevision,
    int HorasVuelo);

public sealed record CrearAvionDto(
    string Matricula,
    int ModeloId,
    int AerolineaId,
    int? AnioFabricacion,
    string Estado,
    DateTime? UltimaRevision,
    DateTime? ProximaRevision,
    int HorasVuelo);

public sealed record ActualizarAvionDto(
    string Matricula,
    int ModeloId,
    int AerolineaId,
    int? AnioFabricacion,
    string Estado,
    DateTime? UltimaRevision,
    DateTime? ProximaRevision,
    int HorasVuelo);

public sealed record AsientoAvionDto(
    int Id,
    int AvionId,
    string Codigo,
    string Clase,
    string Estado);

public sealed record CrearAsientoAvionDto(
    int AvionId,
    string Codigo,
    string Clase,
    string Estado);

public sealed record ActualizarAsientoAvionDto(
    int AvionId,
    string Codigo,
    string Clase,
    string Estado);
