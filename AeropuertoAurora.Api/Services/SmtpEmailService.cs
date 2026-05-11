using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Text;
using AeropuertoAurora.Api.Configuration;
using AeropuertoAurora.Api.DTOs;
using Microsoft.Extensions.Options;

namespace AeropuertoAurora.Api.Services;

public sealed class SmtpEmailService(IOptions<EmailOptions> options, ILogger<SmtpEmailService> logger) : IEmailService
{
    private readonly EmailOptions options = options.Value;

    public async Task<bool> SendReservationConfirmationAsync(
        string to,
        string passengerName,
        string reservationCode,
        string saleNumber,
        VueloDto flight,
        decimal total,
        CancellationToken cancellationToken)
    {
        if (!CanSend(to))
        {
            logger.LogInformation("Correo de confirmacion omitido porque SMTP no esta configurado o el destinatario esta vacio.");
            return false;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(options.FromEmail, options.FromName, Encoding.UTF8),
            Subject = $"Confirmacion de reserva {reservationCode}",
            Body = BuildBody(passengerName, reservationCode, saleNumber, flight, total),
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };
        message.To.Add(new MailAddress(to));

        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = options.UseSsl,
            Credentials = new NetworkCredential(options.User, options.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
        return true;
    }

    public async Task<bool> SendPurchaseSummaryAsync(
        string to,
        string passengerName,
        IReadOnlyList<CompraConfirmacionReservaDto> reservations,
        decimal total,
        CancellationToken cancellationToken)
    {
        if (!CanSend(to))
        {
            logger.LogInformation("Correo de resumen de compra omitido porque SMTP no esta configurado o el destinatario esta vacio.");
            return false;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(options.FromEmail, options.FromName, Encoding.UTF8),
            Subject = reservations.Count > 1
                ? $"Confirmacion de reservas {string.Join(", ", reservations.Select(item => item.CodigoReserva))}"
                : $"Confirmacion de reserva {reservations[0].CodigoReserva}",
            Body = BuildSummaryBody(passengerName, reservations, total),
            IsBodyHtml = true,
            BodyEncoding = Encoding.UTF8,
            SubjectEncoding = Encoding.UTF8
        };
        message.To.Add(new MailAddress(to));

        using var client = new SmtpClient(options.Host, options.Port)
        {
            EnableSsl = options.UseSsl,
            Credentials = new NetworkCredential(options.User, options.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
        return true;
    }

    private bool CanSend(string to)
    {
        return options.Enabled &&
            !string.IsNullOrWhiteSpace(options.Host) &&
            !string.IsNullOrWhiteSpace(options.User) &&
            !string.IsNullOrWhiteSpace(options.Password) &&
            !string.IsNullOrWhiteSpace(options.FromEmail) &&
            !string.IsNullOrWhiteSpace(to);
    }

    private static string BuildBody(string passengerName, string reservationCode, string saleNumber, VueloDto flight, decimal total)
    {
        var safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(passengerName) ? "pasajero" : passengerName);
        var subtotal = CalculateSubtotal(total);
        var taxes = total - subtotal;
        var card = BuildReservationCard(
            "Reserva",
            reservationCode,
            saleNumber,
            flight.NumeroVuelo,
            flight.Aerolinea,
            flight.Origen,
            flight.Destino,
            flight.FechaVuelo,
            "Clase seleccionada",
            total);

        return $"""
            <!doctype html>
            <html lang="es">
            <body style="margin: 0; padding: 24px; font-family: Arial, sans-serif; color: #111827; background: #f3f4f6;">
                <div style="max-width: 960px; margin: 0 auto; background: #ffffff; padding: 24px; border-radius: 8px;">
                    <h2 style="margin: 0 0 12px; color: #0f766e;">Compra confirmada</h2>
                    <p style="margin: 0 0 20px;">Hola {safeName}, tu compra fue completada correctamente.</p>
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td style="vertical-align: top; padding-right: 16px;">
                                {card}
                            </td>
                            <td style="vertical-align: top; width: 240px;">
                                {BuildPaymentSummary(subtotal, taxes, total)}
                            </td>
                        </tr>
                    </table>
                    <p style="text-align: center; margin: 24px 0 0;">Gracias por comprar con Aeropuerto Aurora.</p>
                </div>
            </body>
            </html>
            """;
    }

    private static string BuildSummaryBody(string passengerName, IReadOnlyList<CompraConfirmacionReservaDto> reservations, decimal total)
    {
        var safeName = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(passengerName) ? "pasajero" : passengerName);
        var subtotal = CalculateSubtotal(total);
        var taxes = total - subtotal;
        var cards = string.Join(Environment.NewLine, reservations.Select((item, index) => BuildReservationCard(
            index == 0 ? "Ida" : index == 1 ? "Vuelta" : $"Vuelo {index + 1}",
            item.CodigoReserva,
            item.NumeroVenta,
            item.NumeroVuelo,
            item.Aerolinea,
            item.Origen,
            item.Destino,
            item.FechaVuelo,
            item.Clase,
            item.Total)));

        return $"""
            <!doctype html>
            <html lang="es">
            <body style="margin: 0; padding: 24px; font-family: Arial, sans-serif; color: #111827; background: #f3f4f6;">
                <div style="max-width: 960px; margin: 0 auto; background: #ffffff; padding: 24px; border-radius: 8px;">
                    <h2 style="margin: 0 0 12px; color: #0f766e;">Compra confirmada</h2>
                    <p style="margin: 0 0 20px;">Hola {safeName}, tu compra fue completada correctamente.</p>
                    <table style="border-collapse: collapse; width: 100%;">
                        <tr>
                            <td style="vertical-align: top; padding-right: 16px;">
                                {cards}
                            </td>
                            <td style="vertical-align: top; width: 240px;">
                                {BuildPaymentSummary(subtotal, taxes, total)}
                            </td>
                        </tr>
                    </table>
                    <p style="text-align: center; margin: 24px 0 0;">Gracias por comprar con Aeropuerto Aurora.</p>
                </div>
            </body>
            </html>
            """;
    }

    private static string BuildReservationCard(
        string label,
        string reservationCode,
        string saleNumber,
        string flightNumber,
        string airline,
        string origin,
        string destination,
        DateTime flightDate,
        string className,
        decimal total)
    {
        var safeLabel = WebUtility.HtmlEncode(label);
        var safeReservationCode = WebUtility.HtmlEncode(reservationCode);
        var safeSaleNumber = WebUtility.HtmlEncode(saleNumber);
        var safeFlightNumber = WebUtility.HtmlEncode(flightNumber);
        var safeAirline = WebUtility.HtmlEncode(airline);
        var safeOrigin = WebUtility.HtmlEncode(origin);
        var safeDestination = WebUtility.HtmlEncode(destination);
        var safeOriginCode = WebUtility.HtmlEncode(AirportCode(origin));
        var safeDestinationCode = WebUtility.HtmlEncode(AirportCode(destination));
        var safeClass = WebUtility.HtmlEncode(className);
        var safeTotal = FormatMoney(total);
        var safeDate = flightDate.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

        return $"""
            <div style="border: 1px solid #9ca3af; border-radius: 8px; padding: 12px; margin-bottom: 12px; background: #ffffff;">
                <table style="border-collapse: collapse; width: 100%;">
                    <tr>
                        <td style="vertical-align: middle; width: 190px;">
                            <strong style="display: block; font-size: 15px;">{safeLabel}</strong>
                            <strong style="display: block; margin-top: 4px;">{safeReservationCode}</strong>
                            <small style="color: #4b5563;">{safeSaleNumber}</small>
                        </td>
                        <td style="vertical-align: middle; text-align: center;">
                            <strong style="font-size: 30px; letter-spacing: 1px;">{safeOriginCode}</strong><br>
                            <small>{safeOrigin}</small>
                        </td>
                        <td style="vertical-align: middle; text-align: center; width: 80px;">
                            <span style="display: inline-block; border: 1px solid #6b7280; border-radius: 999px; padding: 6px 10px; font-weight: bold;">AV</span>
                        </td>
                        <td style="vertical-align: middle; text-align: center;">
                            <strong style="font-size: 30px; letter-spacing: 1px;">{safeDestinationCode}</strong><br>
                            <small>{safeDestination}</small>
                        </td>
                        <td style="vertical-align: middle; text-align: right; width: 150px;">
                            <strong>{safeClass}</strong><br>
                            <small>{safeFlightNumber} - {safeAirline}</small><br>
                            <small>{safeDate}</small><br>
                            <small>{safeTotal}</small>
                        </td>
                    </tr>
                </table>
            </div>
            """;
    }

    private static string BuildPaymentSummary(decimal subtotal, decimal taxes, decimal total)
    {
        return $"""
            <div style="border: 1px solid #9ca3af; border-radius: 8px; padding: 14px; background: #f9fafb;">
                <strong style="display: block; margin-bottom: 12px;">Resumen de pago</strong>
                <table style="border-collapse: collapse; width: 100%;">
                    <tr><td style="padding: 4px 0;">Subtotal</td><td style="text-align: right; padding: 4px 0;">{FormatMoney(subtotal)}</td></tr>
                    <tr><td style="padding: 4px 0;">Impuestos</td><td style="text-align: right; padding: 4px 0;">{FormatMoney(taxes)}</td></tr>
                    <tr><td style="padding: 8px 0; border-top: 1px solid #d1d5db;"><strong>Total</strong></td><td style="text-align: right; padding: 8px 0; border-top: 1px solid #d1d5db;"><strong>{FormatMoney(total)}</strong></td></tr>
                </table>
                <p style="font-size: 20px; font-weight: bold; margin: 16px 0 0;">Total pagado: {FormatMoney(total)}</p>
            </div>
            """;
    }

    private static decimal CalculateSubtotal(decimal total)
    {
        return Math.Round(total / 1.12m, 2);
    }

    private static string FormatMoney(decimal value)
    {
        return $"Q{value.ToString("N2", CultureInfo.InvariantCulture)}";
    }

    private static string AirportCode(string value)
    {
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized.Contains("AURORA", StringComparison.Ordinal)) return "GUA";
        if (normalized.Contains("BENITO", StringComparison.Ordinal) || normalized.Contains("MEXICO", StringComparison.Ordinal)) return "MEX";
        if (normalized.Contains("DORADO", StringComparison.Ordinal) || normalized.Contains("BOGOTA", StringComparison.Ordinal)) return "BOG";
        if (normalized.Contains("MIAMI", StringComparison.Ordinal)) return "MIA";
        if (normalized.Contains("SALVADOR", StringComparison.Ordinal)) return "SAL";

        var letters = new string(normalized.Where(char.IsLetter).Take(3).ToArray());
        return letters.Length == 3 ? letters : "---";
    }
}
