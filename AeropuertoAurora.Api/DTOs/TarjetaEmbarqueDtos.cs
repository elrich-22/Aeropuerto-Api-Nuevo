namespace AeropuertoAurora.Api.DTOs;

public sealed record TarjetaEmbarqueDto(
    int Id,
    int CheckInId,
    string CodigoQr,
    string? GrupoAbordaje,
    string? Zona,
    DateTime? FechaEmision);

public sealed record CrearTarjetaEmbarqueDto(
    int CheckInId,
    string CodigoQr,
    string? GrupoAbordaje,
    string? Zona,
    DateTime? FechaEmision);

public sealed record ActualizarTarjetaEmbarqueDto(
    int CheckInId,
    string CodigoQr,
    string? GrupoAbordaje,
    string? Zona,
    DateTime? FechaEmision);
