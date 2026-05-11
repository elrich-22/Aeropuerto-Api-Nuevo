using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/compras")]
public sealed class ComprasController(
    IOracleCrudRepository repository,
    IAeropuertoQueryService service,
    IEmailService emailService,
    ILogger<ComprasController> logger) : ControllerBase
{
    private static readonly CrudTableDefinition PasajerosTable = new(
        "AER_PASAJERO",
        "PAS_ID_PASAJERO",
        [],
        []);

    private static readonly CrudTableDefinition ReservasTable = new(
        "AER_RESERVA",
        "RES_ID_RESERVA",
        ["RES_ID_VUELO", "RES_ID_PASAJERO", "RES_CLASE", "RES_FECHA_RESERVA", "RES_ESTADO", "RES_EQUIPAJE_FACTURADO", "RES_PESO_EQUIPAJE", "RES_TARIFA_PAGADA", "RES_CODIGO_RESERVA"],
        ["RES_ID_VUELO", "RES_ID_PASAJERO", "RES_CLASE", "RES_FECHA_RESERVA", "RES_ESTADO", "RES_EQUIPAJE_FACTURADO", "RES_PESO_EQUIPAJE", "RES_TARIFA_PAGADA", "RES_CODIGO_RESERVA"]);

    private static readonly CrudTableDefinition VentasTable = new(
        "AER_VENTABOLETO",
        "VEN_ID_VENTA",
        ["VEN_NUMERO_VENTA", "VEN_ID_PUNTO_VENTA", "VEN_ID_EMPLEADO_VENDEDOR", "VEN_ID_PASAJERO", "VEN_FECHA_VENTA", "VEN_MONTO_SUBTOTAL", "VEN_IMPUESTOS", "VEN_DESCUENTOS", "VEN_MONTO_TOTAL", "VEN_ID_METODO_PAGO", "VEN_CANAL_VENTA", "VEN_ESTADO"],
        ["VEN_NUMERO_VENTA", "VEN_ID_PUNTO_VENTA", "VEN_ID_EMPLEADO_VENDEDOR", "VEN_ID_PASAJERO", "VEN_FECHA_VENTA", "VEN_MONTO_SUBTOTAL", "VEN_IMPUESTOS", "VEN_DESCUENTOS", "VEN_MONTO_TOTAL", "VEN_ID_METODO_PAGO", "VEN_CANAL_VENTA", "VEN_ESTADO"]);

    private static readonly CrudTableDefinition DetallesTable = new(
        "AER_DETALLEVENTABOLETO",
        "DEV_ID_DETALLE_VENTA",
        ["DEV_ID_VENTA", "DEV_ID_RESERVA", "DEV_PRECIO_BASE", "DEV_CARGOS_ADICIONALES"],
        ["DEV_ID_VENTA", "DEV_ID_RESERVA", "DEV_PRECIO_BASE", "DEV_CARGOS_ADICIONALES"]);

    private static readonly CrudTableDefinition VuelosDisponibilidadTable = new(
        "AER_VUELO",
        "VUE_ID_VUELO",
        [],
        ["VUE_PLAZAS_OCUPADAS", "VUE_PLAZAS_VACIAS"]);

    [HttpPost("vuelos")]
    public async Task<IActionResult> ComprarVuelo(CompraVueloRequestDto dto, CancellationToken cancellationToken)
    {
        if (dto.UsuarioId <= 0 || dto.PasajeroId <= 0 || dto.VueloId <= 0)
        {
            return BadRequest(new { message = "Usuario, pasajero y vuelo son obligatorios." });
        }

        var normalizedClass = NormalizeClass(dto.Clase);
        var seatsToBook = Math.Max(1, dto.NumeroPasajeros);
        var fare = dto.TarifaPagada > 0 ? dto.TarifaPagada : 1250m;
        var taxes = Math.Round(fare * 0.12m, 2);
        var total = fare + taxes;
        var now = DateTime.UtcNow;
        var reservationCode = $"R{now:MMddHHmmssfff}{dto.PasajeroId % 100:D2}{dto.VueloId % 100:D2}";
        var saleNumber = $"WEB-{now:yyMMddHHmmssfff}-{dto.UsuarioId}-{dto.VueloId}";

        var flight = await service.GetFlightByIdAsync(dto.VueloId, cancellationToken);
        if (flight is null)
        {
            return NotFound(new { message = "El vuelo seleccionado no existe." });
        }

        if (!string.Equals(flight.Estado, "PROGRAMADO", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Solo se pueden comprar vuelos programados." });
        }

        if (flight.PlazasDisponibles < seatsToBook)
        {
            return BadRequest(new { message = $"Solo quedan {flight.PlazasDisponibles} plazas disponibles para este vuelo." });
        }

        var passenger = await repository.GetByIdAsync(PasajerosTable, dto.PasajeroId, cancellationToken);
        if (passenger is null)
        {
            return BadRequest(new { message = "El pasajero seleccionado no existe." });
        }

        var reservationId = await repository.CreateAsync(ReservasTable, new Dictionary<string, object?>
        {
            ["RES_ID_VUELO"] = dto.VueloId,
            ["RES_ID_PASAJERO"] = dto.PasajeroId,
            ["RES_CLASE"] = normalizedClass,
            ["RES_FECHA_RESERVA"] = now,
            ["RES_ESTADO"] = "CONFIRMADA",
            ["RES_EQUIPAJE_FACTURADO"] = dto.EquipajeFacturado,
            ["RES_PESO_EQUIPAJE"] = dto.PesoEquipaje,
            ["RES_TARIFA_PAGADA"] = fare,
            ["RES_CODIGO_RESERVA"] = reservationCode
        }, cancellationToken);

        var saleId = await repository.CreateAsync(VentasTable, new Dictionary<string, object?>
        {
            ["VEN_NUMERO_VENTA"] = saleNumber,
            ["VEN_ID_PUNTO_VENTA"] = null,
            ["VEN_ID_EMPLEADO_VENDEDOR"] = null,
            ["VEN_ID_PASAJERO"] = dto.PasajeroId,
            ["VEN_FECHA_VENTA"] = now,
            ["VEN_MONTO_SUBTOTAL"] = fare,
            ["VEN_IMPUESTOS"] = taxes,
            ["VEN_DESCUENTOS"] = 0m,
            ["VEN_MONTO_TOTAL"] = total,
            ["VEN_ID_METODO_PAGO"] = dto.MetodoPagoId <= 0 ? 1 : dto.MetodoPagoId,
            ["VEN_CANAL_VENTA"] = "web",
            ["VEN_ESTADO"] = "COMPLETADA"
        }, cancellationToken);

        var detailId = await repository.CreateAsync(DetallesTable, new Dictionary<string, object?>
        {
            ["DEV_ID_VENTA"] = saleId,
            ["DEV_ID_RESERVA"] = reservationId,
            ["DEV_PRECIO_BASE"] = fare,
            ["DEV_CARGOS_ADICIONALES"] = taxes
        }, cancellationToken);

        var occupiedSeats = flight.PlazasOcupadas + seatsToBook;
        var availableSeats = Math.Max(0, flight.PlazasDisponibles - seatsToBook);
        await repository.UpdateAsync(VuelosDisponibilidadTable, dto.VueloId, new Dictionary<string, object?>
        {
            ["VUE_PLAZAS_OCUPADAS"] = occupiedSeats,
            ["VUE_PLAZAS_VACIAS"] = availableSeats
        }, cancellationToken);

        var updatedFlight = await service.GetFlightByIdAsync(dto.VueloId, cancellationToken);
        var requestedConfirmationEmail = dto.EmailConfirmacion?.Trim();
        var confirmationEmail = IsValidEmail(requestedConfirmationEmail)
            ? requestedConfirmationEmail
            : passenger.ToNullableString("PAS_EMAIL");
        var confirmationEmailSent = false;

        if (dto.EnviarCorreoConfirmacion != false && !string.IsNullOrWhiteSpace(confirmationEmail))
        {
            try
            {
                confirmationEmailSent = await emailService.SendReservationConfirmationAsync(
                    confirmationEmail,
                    BuildPassengerName(passenger),
                    reservationCode,
                    saleNumber,
                    updatedFlight ?? flight,
                    total,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "No se pudo enviar el correo de confirmacion para la reserva {ReservationCode}.", reservationCode);
            }
        }
        else if (dto.EnviarCorreoConfirmacion == false)
        {
            logger.LogInformation("Correo individual omitido para la reserva {ReservationCode} porque se enviara un resumen de compra.", reservationCode);
        }
        else
        {
            logger.LogInformation("Reserva {ReservationCode} creada sin correo de confirmacion porque el pasajero no tiene email.", reservationCode);
        }

        return Created(string.Empty, new CompraVueloResponseDto(
            reservationId,
            saleId,
            detailId,
            reservationCode,
            saleNumber,
            total,
            seatsToBook,
            updatedFlight?.PlazasOcupadas ?? occupiedSeats,
            updatedFlight?.PlazasDisponibles ?? availableSeats,
            confirmationEmailSent,
            confirmationEmail));
    }

    [HttpPost("confirmacion-correo")]
    public async Task<IActionResult> EnviarConfirmacionCorreo(CompraConfirmacionEmailRequestDto dto, CancellationToken cancellationToken)
    {
        if (!IsValidEmail(dto.EmailConfirmacion))
        {
            return BadRequest(new { message = "Ingresa un email valido para enviar la confirmacion." });
        }

        if (dto.Reservas.Count == 0)
        {
            return BadRequest(new { message = "Debe incluir al menos una reserva para enviar la confirmacion." });
        }

        var sent = false;
        try
        {
            sent = await emailService.SendPurchaseSummaryAsync(
                dto.EmailConfirmacion.Trim(),
                dto.PasajeroNombre ?? string.Empty,
                dto.Reservas,
                dto.Total,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo enviar el correo de resumen de compra a {Email}.", dto.EmailConfirmacion);
        }

        return Ok(new
        {
            correoConfirmacionEnviado = sent,
            correoConfirmacionDestino = dto.EmailConfirmacion
        });
    }

    private static string NormalizeClass(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return normalized is "ejecutiva" or "primera" ? normalized : "economica";
    }

    private static string BuildPassengerName(IReadOnlyDictionary<string, object?> passenger)
    {
        return string.Join(" ", new[]
        {
            passenger.ToNullableString("PAS_PRIMER_NOMBRE"),
            passenger.ToNullableString("PAS_SEGUNDO_NOMBRE"),
            passenger.ToNullableString("PAS_PRIMER_APELLIDO"),
            passenger.ToNullableString("PAS_SEGUNDO_APELLIDO")
        }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static bool IsValidEmail(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            value.Contains('@', StringComparison.Ordinal) &&
            value.Contains('.', StringComparison.Ordinal);
    }
}
