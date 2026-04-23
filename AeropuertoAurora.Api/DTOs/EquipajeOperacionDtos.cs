namespace AeropuertoAurora.Api.DTOs;

public sealed record EquipajeOperacionDto(
    int Id,
    int PasajeroId,
    int VueloId,
    string CodigoBarras,
    decimal PesoKg,
    string Estado,
    DateTime? FechaRegistro);

public sealed record CrearEquipajeOperacionDto(
    int PasajeroId,
    int VueloId,
    string CodigoBarras,
    decimal PesoKg,
    string Estado,
    DateTime? FechaRegistro);

public sealed record ActualizarEquipajeOperacionDto(
    int PasajeroId,
    int VueloId,
    string CodigoBarras,
    decimal PesoKg,
    string Estado,
    DateTime? FechaRegistro);
