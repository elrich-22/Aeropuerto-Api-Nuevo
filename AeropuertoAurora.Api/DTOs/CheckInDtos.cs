namespace AeropuertoAurora.Api.DTOs;

public sealed record CheckInDto(
    int Id,
    int ReservaId,
    int PasajeroId,
    int VueloId,
    DateTime? FechaHora,
    string Metodo,
    string Estado);

public sealed record CrearCheckInDto(
    int ReservaId,
    int PasajeroId,
    int VueloId,
    DateTime? FechaHora,
    string Metodo,
    string Estado);

public sealed record ActualizarCheckInDto(
    int ReservaId,
    int PasajeroId,
    int VueloId,
    DateTime? FechaHora,
    string Metodo,
    string Estado);
