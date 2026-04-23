namespace AeropuertoAurora.Api.DTOs;

public sealed record ModeloAvionDto(
    int Id,
    string NombreModelo,
    string? Fabricante,
    int CapacidadPasajeros,
    int? CapacidadCarga,
    int? AlcanceKm,
    int? VelocidadCrucero,
    int? AnioIntroduccion,
    string? TipoMotor);

public sealed record CrearModeloAvionDto(
    string NombreModelo,
    string? Fabricante,
    int CapacidadPasajeros,
    int? CapacidadCarga,
    int? AlcanceKm,
    int? VelocidadCrucero,
    int? AnioIntroduccion,
    string? TipoMotor);

public sealed record ActualizarModeloAvionDto(
    string NombreModelo,
    string? Fabricante,
    int CapacidadPasajeros,
    int? CapacidadCarga,
    int? AlcanceKm,
    int? VelocidadCrucero,
    int? AnioIntroduccion,
    string? TipoMotor);
