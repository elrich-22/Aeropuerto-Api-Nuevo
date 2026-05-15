using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;

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
        ["PAS_NUMERO_DOCUMENTO", "PAS_TIPO_DOCUMENTO", "PAS_PRIMER_NOMBRE", "PAS_SEGUNDO_NOMBRE", "PAS_PRIMER_APELLIDO", "PAS_SEGUNDO_APELLIDO", "PAS_FECHA_NACIMIENTO", "PAS_NACIONALIDAD", "PAS_SEXO", "PAS_TELEFONO", "PAS_EMAIL", "PAS_FECHA_REGISTRO"],
        ["PAS_NUMERO_DOCUMENTO", "PAS_TIPO_DOCUMENTO", "PAS_PRIMER_NOMBRE", "PAS_SEGUNDO_NOMBRE", "PAS_PRIMER_APELLIDO", "PAS_SEGUNDO_APELLIDO", "PAS_FECHA_NACIMIENTO", "PAS_NACIONALIDAD", "PAS_SEXO", "PAS_TELEFONO", "PAS_EMAIL", "PAS_FECHA_REGISTRO"]);

    private static readonly CrudTableDefinition PassengerDocumentsTable = new(
        "AER_DOCUMENTO_PASAJERO",
        "DCP_ID_DOCUMENTO",
        ["DCP_ID_PASAJERO", "DCP_TIPO_DOCUMENTO", "DCP_NUMERO_DOCUMENTO", "DCP_PAIS_EMISOR", "DCP_FECHA_VENCIMIENTO", "DCP_ESTADO_VALIDACION", "DCP_FECHA_REGISTRO", "DCP_FECHA_VALIDACION"],
        ["DCP_ID_PASAJERO", "DCP_TIPO_DOCUMENTO", "DCP_NUMERO_DOCUMENTO", "DCP_PAIS_EMISOR", "DCP_FECHA_VENCIMIENTO", "DCP_ESTADO_VALIDACION", "DCP_FECHA_REGISTRO", "DCP_FECHA_VALIDACION"]);

    private static readonly CrudTableDefinition ReservasTable = new(
        "AER_RESERVA",
        "RES_ID_RESERVA",
        ["RES_ID_VUELO", "RES_ID_PASAJERO", "RES_CLASE", "RES_FECHA_RESERVA", "RES_ESTADO", "RES_EQUIPAJE_FACTURADO", "RES_PESO_EQUIPAJE", "RES_TARIFA_PAGADA", "RES_CODIGO_RESERVA"],
        ["RES_ID_VUELO", "RES_ID_PASAJERO", "RES_CLASE", "RES_FECHA_RESERVA", "RES_ESTADO", "RES_EQUIPAJE_FACTURADO", "RES_PESO_EQUIPAJE", "RES_TARIFA_PAGADA", "RES_CODIGO_RESERVA"]);

    private static readonly CrudTableDefinition VentasTable = new(
        "AER_VENTABOLETO",
        "VEN_ID_VENTA",
        ["VEN_NUMERO_VENTA", "VEN_ID_PUNTO_VENTA", "VEN_ID_EMPLEADO_VENDEDOR", "VEN_ID_PASAJERO", "VEN_FECHA_VENTA", "VEN_MONTO_SUBTOTAL", "VEN_IMPUESTOS", "VEN_DESCUENTOS", "VEN_MONTO_TOTAL", "VEN_ID_METODO_PAGO", "VEN_CANTIDAD_BOLETOS", "VEN_CANAL_VENTA", "VEN_ESTADO"],
        ["VEN_NUMERO_VENTA", "VEN_ID_PUNTO_VENTA", "VEN_ID_EMPLEADO_VENDEDOR", "VEN_ID_PASAJERO", "VEN_FECHA_VENTA", "VEN_MONTO_SUBTOTAL", "VEN_IMPUESTOS", "VEN_DESCUENTOS", "VEN_MONTO_TOTAL", "VEN_ID_METODO_PAGO", "VEN_CANTIDAD_BOLETOS", "VEN_CANAL_VENTA", "VEN_ESTADO"]);

    private static readonly CrudTableDefinition DetallesTable = new(
        "AER_DETALLEVENTABOLETO",
        "DEV_ID_DETALLE_VENTA",
        ["DEV_ID_VENTA", "DEV_ID_RESERVA", "DEV_PRECIO_BASE", "DEV_CARGOS_ADICIONALES", "DEV_NOMBRE_PASAJERO", "DEV_TIPO_DOCUMENTO", "DEV_NUMERO_DOCUMENTO"],
        ["DEV_ID_VENTA", "DEV_ID_RESERVA", "DEV_PRECIO_BASE", "DEV_CARGOS_ADICIONALES", "DEV_NOMBRE_PASAJERO", "DEV_TIPO_DOCUMENTO", "DEV_NUMERO_DOCUMENTO"]);

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
        var additionalCount = dto.PasajerosAdicionales?.Count ?? 0;
        var seatsToBook = additionalCount > 0 ? 1 + additionalCount : Math.Max(1, dto.NumeroPasajeros);
        var fare = dto.TarifaPagada > 0 ? dto.TarifaPagada : 1250m;
        var taxes = Math.Round(fare * 0.12m, 2);
        var total = fare + taxes;
        var now = DateTime.Now;
        var saleNumber = await GenerateUniqueSaleNumberAsync(dto.UsuarioId, dto.VueloId, cancellationToken);

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

        var buyerPassenger = await repository.GetByIdAsync(PasajerosTable, dto.PasajeroId, cancellationToken);
        if (buyerPassenger is null)
        {
            return BadRequest(new { message = "El pasajero seleccionado no existe." });
        }

        var mainPassengerPayload = dto.PasajeroPrincipal ?? ToPassengerPayload(buyerPassenger);
        var mainPassengerValidationError = ValidatePassengerProfile(mainPassengerPayload, "el pasajero principal");
        if (mainPassengerValidationError is not null)
        {
            return BadRequest(new { message = mainPassengerValidationError });
        }

        var mainPassportValidationError = ValidatePassengerDocument(mainPassengerPayload, flight.FechaVuelo);
        if (mainPassportValidationError is not null)
        {
            return BadRequest(new { message = mainPassportValidationError });
        }

        int mainPassengerId;
        try
        {
            mainPassengerId = dto.PasajeroPrincipal is null
                ? dto.PasajeroId
                : await EnsurePassengerExistsAsync(dto.PasajeroPrincipal, now, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        var mainPassenger = await repository.GetByIdAsync(PasajerosTable, mainPassengerId, cancellationToken) ?? buyerPassenger;

        var reservationCode = await GenerateUniqueReservationCodeAsync(mainPassengerId, dto.VueloId, cancellationToken);
        await TryUpsertPassengerDocumentAsync(mainPassengerId, mainPassengerPayload, now, cancellationToken);

        var fareAllocation = AllocateAmount(fare, seatsToBook);
        var taxAllocation = AllocateAmount(taxes, seatsToBook);

        var reservationId = await repository.CreateAsync(ReservasTable, new Dictionary<string, object?>
        {
            ["RES_ID_VUELO"] = dto.VueloId,
            ["RES_ID_PASAJERO"] = mainPassengerId,
            ["RES_CLASE"] = normalizedClass,
            ["RES_FECHA_RESERVA"] = now,
            ["RES_ESTADO"] = "CONFIRMADA",
            ["RES_EQUIPAJE_FACTURADO"] = dto.EquipajeFacturado,
            ["RES_PESO_EQUIPAJE"] = dto.PesoEquipaje,
            ["RES_TARIFA_PAGADA"] = fareAllocation[0],
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
            ["VEN_CANTIDAD_BOLETOS"] = seatsToBook,
            ["VEN_CANAL_VENTA"] = "web",
            ["VEN_ESTADO"] = "COMPLETADA"
        }, cancellationToken);

        var detailId = await repository.CreateAsync(DetallesTable, new Dictionary<string, object?>
        {
            ["DEV_ID_VENTA"] = saleId,
            ["DEV_ID_RESERVA"] = reservationId,
            ["DEV_PRECIO_BASE"] = fareAllocation[0],
            ["DEV_CARGOS_ADICIONALES"] = taxAllocation[0],
            ["DEV_NOMBRE_PASAJERO"] = BuildPassengerName(mainPassenger),
            ["DEV_TIPO_DOCUMENTO"] = mainPassenger.ToNullableString("PAS_TIPO_DOCUMENTO"),
            ["DEV_NUMERO_DOCUMENTO"] = mainPassenger.ToNullableString("PAS_NUMERO_DOCUMENTO")
        }, cancellationToken);

        var additionalReservations = new List<ReservaAdicionalInfo>();

        if (dto.PasajerosAdicionales is { Count: > 0 })
        {
            for (var index = 0; index < dto.PasajerosAdicionales.Count; index++)
            {
                var extra = dto.PasajerosAdicionales[index];
                var passengerLabel = $"el pasajero {index + 2}";
                var extraPassengerValidationError = ValidatePassengerProfile(extra, passengerLabel);
                if (extraPassengerValidationError is not null)
                {
                    return BadRequest(new { message = extraPassengerValidationError });
                }

                var extraPassportValidationError = ValidatePassengerDocument(extra, flight.FechaVuelo);
                if (extraPassportValidationError is not null)
                {
                    return BadRequest(new { message = extraPassportValidationError });
                }

                int extraPassengerId;
                try
                {
                    extraPassengerId = await EnsurePassengerExistsAsync(extra, now, cancellationToken);
                }
                catch (InvalidOperationException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
                await TryUpsertPassengerDocumentAsync(extraPassengerId, extra, now, cancellationToken);
                var extraPassenger = await repository.GetByIdAsync(PasajerosTable, extraPassengerId, cancellationToken);
                var extraCode = await GenerateUniqueReservationCodeAsync(extraPassengerId, dto.VueloId, cancellationToken);
                var extraReservationId = await repository.CreateAsync(ReservasTable, new Dictionary<string, object?>
                {
                    ["RES_ID_VUELO"] = dto.VueloId,
                    ["RES_ID_PASAJERO"] = extraPassengerId,
                    ["RES_CLASE"] = normalizedClass,
                    ["RES_FECHA_RESERVA"] = now,
                    ["RES_ESTADO"] = "CONFIRMADA",
                    ["RES_EQUIPAJE_FACTURADO"] = dto.EquipajeFacturado,
                    ["RES_PESO_EQUIPAJE"] = dto.PesoEquipaje,
                    ["RES_TARIFA_PAGADA"] = fareAllocation[index + 1],
                    ["RES_CODIGO_RESERVA"] = extraCode
                }, cancellationToken);

                await repository.CreateAsync(DetallesTable, new Dictionary<string, object?>
                {
                    ["DEV_ID_VENTA"] = saleId,
                    ["DEV_ID_RESERVA"] = extraReservationId,
                    ["DEV_PRECIO_BASE"] = fareAllocation[index + 1],
                    ["DEV_CARGOS_ADICIONALES"] = taxAllocation[index + 1],
                    ["DEV_NOMBRE_PASAJERO"] = extraPassenger is null ? BuildPassengerName(extra) : BuildPassengerName(extraPassenger),
                    ["DEV_TIPO_DOCUMENTO"] = extraPassenger?.ToNullableString("PAS_TIPO_DOCUMENTO") ?? extra.TipoDocumento,
                    ["DEV_NUMERO_DOCUMENTO"] = extraPassenger?.ToNullableString("PAS_NUMERO_DOCUMENTO") ?? extra.NumeroDocumento
                }, cancellationToken);

                additionalReservations.Add(new ReservaAdicionalInfo(extraPassengerId, extraReservationId, extraCode));
            }
        }

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
            : buyerPassenger.ToNullableString("PAS_EMAIL");
        var confirmationEmailSent = false;

        if (dto.EnviarCorreoConfirmacion != false && !string.IsNullOrWhiteSpace(confirmationEmail))
        {
            try
            {
                confirmationEmailSent = await emailService.SendReservationConfirmationAsync(
                    confirmationEmail,
                    BuildPassengerName(mainPassenger),
                    reservationCode,
                    saleNumber,
                    updatedFlight ?? flight,
                    total,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning("No se pudo enviar el correo de confirmacion para la reserva {ReservationCode}. La compra ya fue registrada. Detalle: {Error}", reservationCode, ex.Message);
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
            confirmationEmail,
            additionalReservations));
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
            logger.LogWarning("No se pudo enviar el correo de resumen de compra a {Email}. La compra ya fue registrada. Detalle: {Error}", dto.EmailConfirmacion, ex.Message);
        }

        return Ok(new
        {
            correoConfirmacionEnviado = sent,
            correoConfirmacionDestino = dto.EmailConfirmacion
        });
    }

    private async Task<int> EnsurePassengerExistsAsync(PasajeroAdicionalDto dto, DateTime registeredAt, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByColumnAsync(PasajerosTable, "PAS_NUMERO_DOCUMENTO", dto.NumeroDocumento, cancellationToken);
        var matchedPassenger = existing.FirstOrDefault(row =>
            string.Equals(row.ToNullableString("PAS_TIPO_DOCUMENTO"), dto.TipoDocumento, StringComparison.OrdinalIgnoreCase));

        if (matchedPassenger is not null)
        {
            if (!PassengerIdentityMatches(matchedPassenger, dto))
            {
                throw new InvalidOperationException(
                    $"Ya existe un pasajero con el documento {dto.TipoDocumento} {dto.NumeroDocumento}, pero el nombre no coincide con el registro existente.");
            }

            return matchedPassenger.ToInt("PAS_ID_PASAJERO");
        }

        return await repository.CreateAsync(PasajerosTable, new Dictionary<string, object?>
        {
            ["PAS_NUMERO_DOCUMENTO"] = dto.NumeroDocumento,
            ["PAS_TIPO_DOCUMENTO"] = dto.TipoDocumento,
            ["PAS_PRIMER_NOMBRE"] = dto.PrimerNombre,
            ["PAS_SEGUNDO_NOMBRE"] = dto.SegundoNombre,
            ["PAS_PRIMER_APELLIDO"] = dto.PrimerApellido,
            ["PAS_SEGUNDO_APELLIDO"] = dto.SegundoApellido,
            ["PAS_FECHA_NACIMIENTO"] = dto.FechaNacimiento,
            ["PAS_NACIONALIDAD"] = dto.Nacionalidad,
            ["PAS_SEXO"] = dto.Sexo,
            ["PAS_TELEFONO"] = dto.Telefono,
            ["PAS_EMAIL"] = dto.Email,
            ["PAS_FECHA_REGISTRO"] = registeredAt
        }, cancellationToken);
    }

    private static bool PassengerIdentityMatches(IReadOnlyDictionary<string, object?> passenger, PasajeroAdicionalDto dto)
    {
        return string.Equals(NormalizeIdentityValue(passenger.ToNullableString("PAS_PRIMER_NOMBRE")), NormalizeIdentityValue(dto.PrimerNombre), StringComparison.Ordinal) &&
            string.Equals(NormalizeIdentityValue(passenger.ToNullableString("PAS_SEGUNDO_NOMBRE")), NormalizeIdentityValue(dto.SegundoNombre), StringComparison.Ordinal) &&
            string.Equals(NormalizeIdentityValue(passenger.ToNullableString("PAS_PRIMER_APELLIDO")), NormalizeIdentityValue(dto.PrimerApellido), StringComparison.Ordinal) &&
            string.Equals(NormalizeIdentityValue(passenger.ToNullableString("PAS_SEGUNDO_APELLIDO")), NormalizeIdentityValue(dto.SegundoApellido), StringComparison.Ordinal);
    }

    private static string NormalizeIdentityValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToUpperInvariant();
    }

    private async Task UpsertPassengerDocumentAsync(
        int passengerId,
        PasajeroAdicionalDto dto,
        DateTime registeredAt,
        CancellationToken cancellationToken)
    {
        var normalizedType = NormalizeDocumentType(dto.TipoDocumento);
        var existingByPassenger = await repository.GetByColumnAsync(PassengerDocumentsTable, "DCP_ID_PASAJERO", passengerId, cancellationToken);
        var existingDocument = existingByPassenger.FirstOrDefault(row =>
            string.Equals(row.ToNullableString("DCP_TIPO_DOCUMENTO"), normalizedType, StringComparison.OrdinalIgnoreCase));
        var validationStatus = normalizedType == "PASAPORTE" ? "VALIDADO" : "NO_REQUIERE";

        if (existingDocument is not null)
        {
            await repository.UpdateAsync(PassengerDocumentsTable, existingDocument.ToInt("DCP_ID_DOCUMENTO"), new Dictionary<string, object?>
            {
                ["DCP_ID_PASAJERO"] = passengerId,
                ["DCP_TIPO_DOCUMENTO"] = normalizedType,
                ["DCP_NUMERO_DOCUMENTO"] = dto.NumeroDocumento.Trim(),
                ["DCP_PAIS_EMISOR"] = string.IsNullOrWhiteSpace(dto.PaisEmisorDocumento) ? null : dto.PaisEmisorDocumento.Trim(),
                ["DCP_FECHA_VENCIMIENTO"] = dto.FechaVencimientoDocumento,
                ["DCP_ESTADO_VALIDACION"] = validationStatus,
                ["DCP_FECHA_REGISTRO"] = existingDocument.ToNullableDateTime("DCP_FECHA_REGISTRO") ?? registeredAt,
                ["DCP_FECHA_VALIDACION"] = normalizedType == "PASAPORTE" ? registeredAt : null
            }, cancellationToken);
            return;
        }

        await repository.CreateAsync(PassengerDocumentsTable, new Dictionary<string, object?>
        {
            ["DCP_ID_PASAJERO"] = passengerId,
            ["DCP_TIPO_DOCUMENTO"] = normalizedType,
            ["DCP_NUMERO_DOCUMENTO"] = dto.NumeroDocumento.Trim(),
            ["DCP_PAIS_EMISOR"] = string.IsNullOrWhiteSpace(dto.PaisEmisorDocumento) ? null : dto.PaisEmisorDocumento.Trim(),
            ["DCP_FECHA_VENCIMIENTO"] = dto.FechaVencimientoDocumento,
            ["DCP_ESTADO_VALIDACION"] = validationStatus,
            ["DCP_FECHA_REGISTRO"] = registeredAt,
            ["DCP_FECHA_VALIDACION"] = normalizedType == "PASAPORTE" ? registeredAt : null
        }, cancellationToken);
    }

    private async Task TryUpsertPassengerDocumentAsync(
        int passengerId,
        PasajeroAdicionalDto dto,
        DateTime registeredAt,
        CancellationToken cancellationToken)
    {
        try
        {
            await UpsertPassengerDocumentAsync(passengerId, dto, registeredAt, cancellationToken);
        }
        catch (OracleException ex) when (IsMissingPassengerDocumentsTable(ex))
        {
            logger.LogWarning(
                "Se omitio el guardado en AER_DOCUMENTO_PASAJERO para el pasajero {PassengerId} porque la tabla aun no existe. Ejecuta la migracion alter_add_documento_pasajero.sql. Detalle: {Error}",
                passengerId,
                ex.Message);
        }
    }

    private static string NormalizeClass(string? value)
    {
        var normalized = value?.Trim().ToLowerInvariant();
        return normalized is "ejecutiva" or "primera" ? normalized : "economica";
    }

    private static string NormalizeDocumentType(string? value)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        return normalized switch
        {
            "PASAPORTE" or "PASSPORT" => "PASAPORTE",
            "DPI" => "DPI",
            "LICENCIA" => "LICENCIA",
            _ => normalized ?? string.Empty
        };
    }


    private static bool IsMissingPassengerDocumentsTable(OracleException exception)
    {
        return exception.Number == 942 ||
            exception.Message.Contains("ORA-00942", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string> GenerateUniqueReservationCodeAsync(int passengerId, int flightId, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var code = $"R{DateTime.UtcNow:MMddHHmmssfff}{passengerId % 100:D2}{flightId % 100:D2}{Random.Shared.Next(10, 99):D2}";
            var existing = await repository.GetByColumnAsync(ReservasTable, "RES_CODIGO_RESERVA", code, cancellationToken);
            if (existing.Count == 0)
            {
                return code;
            }
        }

        return $"R-{Guid.NewGuid():N}"[..20];
    }

    private async Task<string> GenerateUniqueSaleNumberAsync(int userId, int flightId, CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var number = $"WEB-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{userId:D4}-{flightId:D4}-{Random.Shared.Next(1000, 9999)}";
            var existing = await repository.GetByColumnAsync(VentasTable, "VEN_NUMERO_VENTA", number, cancellationToken);
            if (existing.Count == 0)
            {
                return number;
            }
        }

        return $"WEB-{Guid.NewGuid():N}"[..32];
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

    private static string BuildPassengerName(PasajeroAdicionalDto passenger)
    {
        return string.Join(" ", new[]
        {
            passenger.PrimerNombre,
            passenger.SegundoNombre,
            passenger.PrimerApellido,
            passenger.SegundoApellido
        }.Where(value => !string.IsNullOrWhiteSpace(value)));
    }

    private static PasajeroAdicionalDto ToPassengerPayload(IReadOnlyDictionary<string, object?> passenger)
    {
        return new PasajeroAdicionalDto(
            passenger.ToNullableString("PAS_NUMERO_DOCUMENTO") ?? string.Empty,
            passenger.ToNullableString("PAS_TIPO_DOCUMENTO") ?? "DPI",
            passenger.ToNullableString("PAS_PRIMER_NOMBRE") ?? string.Empty,
            passenger.ToNullableString("PAS_SEGUNDO_NOMBRE"),
            passenger.ToNullableString("PAS_PRIMER_APELLIDO") ?? string.Empty,
            passenger.ToNullableString("PAS_SEGUNDO_APELLIDO"),
            passenger.ToNullableDateTime("PAS_FECHA_NACIMIENTO"),
            passenger.ToNullableString("PAS_NACIONALIDAD"),
            passenger.ToNullableString("PAS_SEXO"),
            passenger.ToNullableString("PAS_TELEFONO"),
            passenger.ToNullableString("PAS_EMAIL"),
            null,
            null);
    }

    private static bool IsValidEmail(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
            value.Contains('@', StringComparison.Ordinal) &&
            value.Contains('.', StringComparison.Ordinal);
    }

    private static string? ValidatePassengerProfile(PasajeroAdicionalDto dto, string passengerLabel)
    {
        if (string.IsNullOrWhiteSpace(dto.TipoDocumento))
        {
            return $"Debes indicar el tipo de documento de {passengerLabel}.";
        }

        if (string.IsNullOrWhiteSpace(dto.NumeroDocumento))
        {
            return $"Debes indicar el numero de documento de {passengerLabel}.";
        }

        if (string.IsNullOrWhiteSpace(dto.PrimerNombre))
        {
            return $"Debes indicar el primer nombre de {passengerLabel}.";
        }

        if (string.IsNullOrWhiteSpace(dto.PrimerApellido))
        {
            return $"Debes indicar el primer apellido de {passengerLabel}.";
        }

        return null;
    }

    private static string? ValidatePassengerDocument(PasajeroAdicionalDto dto, DateTime flightDate)
    {
        if (!string.Equals(NormalizeDocumentType(dto.TipoDocumento), "PASAPORTE", StringComparison.Ordinal))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(dto.NumeroDocumento) || dto.NumeroDocumento.Trim().Length < 6)
        {
            return "El numero de pasaporte no tiene un formato valido.";
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(dto.NumeroDocumento.Trim(), "^[A-Za-z0-9-]{6,20}$"))
        {
            return "El numero de pasaporte solo puede contener letras, numeros o guiones.";
        }

        if (string.IsNullOrWhiteSpace(dto.PaisEmisorDocumento))
        {
            return $"Debes indicar el pais emisor del pasaporte de {BuildPassengerName(dto)}.";
        }

        if (!dto.FechaVencimientoDocumento.HasValue)
        {
            return $"Debes indicar la fecha de vencimiento del pasaporte de {BuildPassengerName(dto)}.";
        }

        if (dto.FechaVencimientoDocumento.Value.Date <= DateTime.Today)
        {
            return $"El pasaporte de {BuildPassengerName(dto)} ya esta vencido o vence hoy.";
        }

        if (dto.FechaVencimientoDocumento.Value.Date < flightDate.Date)
        {
            return $"El pasaporte de {BuildPassengerName(dto)} vence antes de la fecha del vuelo.";
        }

        return null;
    }

    private static string? Coalesce(string? preferred, string? fallback)
    {
        return string.IsNullOrWhiteSpace(preferred) ? fallback : preferred.Trim();
    }

    private static List<decimal> AllocateAmount(decimal total, int parts)
    {
        if (parts <= 0)
        {
            return [];
        }

        var totalCents = (int)Math.Round(total * 100m, MidpointRounding.AwayFromZero);
        var baseCents = totalCents / parts;
        var remainder = totalCents % parts;
        var amounts = new List<decimal>(parts);

        for (var index = 0; index < parts; index++)
        {
            var cents = baseCents + (index < remainder ? 1 : 0);
            amounts.Add(cents / 100m);
        }

        return amounts;
    }
}
