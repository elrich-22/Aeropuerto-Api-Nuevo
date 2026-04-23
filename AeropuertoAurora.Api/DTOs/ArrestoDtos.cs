namespace AeropuertoAurora.Api.DTOs;

public sealed record ArrestoDto(
    int Id,
    int PasajeroId,
    int? VueloId,
    int AeropuertoId,
    DateTime? FechaHoraArresto,
    string Motivo,
    string? AutoridadCargo,
    string? DescripcionIncidente,
    string? UbicacionArresto,
    string EstadoCaso,
    string? NumeroExpediente);

public sealed record CrearArrestoDto(
    int PasajeroId,
    int? VueloId,
    int AeropuertoId,
    DateTime? FechaHoraArresto,
    string Motivo,
    string? AutoridadCargo,
    string? DescripcionIncidente,
    string? UbicacionArresto,
    string EstadoCaso,
    string? NumeroExpediente);

public sealed record ActualizarArrestoDto(
    int PasajeroId,
    int? VueloId,
    int AeropuertoId,
    DateTime? FechaHoraArresto,
    string Motivo,
    string? AutoridadCargo,
    string? DescripcionIncidente,
    string? UbicacionArresto,
    string EstadoCaso,
    string? NumeroExpediente);
