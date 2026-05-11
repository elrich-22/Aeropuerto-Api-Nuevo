using AeropuertoAurora.Api.DTOs;

namespace AeropuertoAurora.Api.Services;

public interface IEmailService
{
    Task<bool> SendReservationConfirmationAsync(
        string to,
        string passengerName,
        string reservationCode,
        string saleNumber,
        VueloDto flight,
        decimal total,
        CancellationToken cancellationToken);

    Task<bool> SendPurchaseSummaryAsync(
        string to,
        string passengerName,
        IReadOnlyList<CompraConfirmacionReservaDto> reservations,
        decimal total,
        CancellationToken cancellationToken);
}
