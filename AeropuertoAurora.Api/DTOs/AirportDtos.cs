namespace AeropuertoAurora.Api.DTOs;

public sealed record AeropuertoDto(
    int Id,
    string Codigo,
    string Nombre,
    string Ciudad,
    string Pais,
    string? ZonaHoraria,
    string Estado,
    string? Tipo,
    decimal? Latitud,
    decimal? Longitud,
    string? CodigoIata,
    string? CodigoIcao);

public sealed record CrearAeropuertoDto(
    string Codigo,
    string Nombre,
    string Ciudad,
    string Pais,
    string? ZonaHoraria,
    string Estado,
    string? Tipo,
    decimal? Latitud,
    decimal? Longitud,
    string? CodigoIata,
    string? CodigoIcao);

public sealed record ActualizarAeropuertoDto(
    string Codigo,
    string Nombre,
    string Ciudad,
    string Pais,
    string? ZonaHoraria,
    string Estado,
    string? Tipo,
    decimal? Latitud,
    decimal? Longitud,
    string? CodigoIata,
    string? CodigoIcao);

public sealed record TerminalDto(
    int Id,
    int AeropuertoId,
    string Nombre,
    string Tipo,
    int? CapacidadPasajeros,
    string Estado);

public sealed record CrearTerminalDto(
    int AeropuertoId,
    string Nombre,
    string Tipo,
    int? CapacidadPasajeros,
    string Estado);

public sealed record ActualizarTerminalDto(
    int AeropuertoId,
    string Nombre,
    string Tipo,
    int? CapacidadPasajeros,
    string Estado);

public sealed record PuertaEmbarqueDto(
    int Id,
    int TerminalId,
    string Codigo,
    string Estado,
    string Tipo);

public sealed record CrearPuertaEmbarqueDto(
    int TerminalId,
    string Codigo,
    string Estado,
    string Tipo);

public sealed record ActualizarPuertaEmbarqueDto(
    int TerminalId,
    string Codigo,
    string Estado,
    string Tipo);
