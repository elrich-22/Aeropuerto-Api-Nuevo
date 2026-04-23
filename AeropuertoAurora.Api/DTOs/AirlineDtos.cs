namespace AeropuertoAurora.Api.DTOs;

public sealed record AerolineaDto(
    int Id,
    string? Codigo,
    string Nombre,
    string? Pais,
    string? CodigoIata,
    string? CodigoIcao,
    string Estado,
    string? Telefono,
    string? Email,
    string? SitioWeb);

public sealed record CrearAerolineaDto(
    string? Codigo,
    string Nombre,
    string? Pais,
    string? CodigoIata,
    string? CodigoIcao,
    string Estado,
    string? Telefono,
    string? Email,
    string? SitioWeb);

public sealed record ActualizarAerolineaDto(
    string? Codigo,
    string Nombre,
    string? Pais,
    string? CodigoIata,
    string? CodigoIcao,
    string Estado,
    string? Telefono,
    string? Email,
    string? SitioWeb);
