namespace AeropuertoAurora.Api.DTOs;

public sealed record ReservaDto(
    int Id,
    string Codigo,
    int VueloId,
    string NumeroVuelo,
    string Pasajero,
    string Clase,
    DateTime FechaReserva,
    string Estado,
    int EquipajeFacturado,
    decimal? PesoEquipaje,
    decimal TarifaPagada);

public sealed record CarritoCompraDto(
    int Id,
    int PasajeroId,
    string? SesionId,
    DateTime? FechaCreacion,
    DateTime? FechaExpiracion,
    string Estado);

public sealed record CrearCarritoCompraDto(
    int PasajeroId,
    string? SesionId,
    DateTime? FechaCreacion,
    DateTime? FechaExpiracion,
    string Estado);

public sealed record ActualizarCarritoCompraDto(
    int PasajeroId,
    string? SesionId,
    DateTime? FechaCreacion,
    DateTime? FechaExpiracion,
    string Estado);

public sealed record ItemCarritoDto(
    int Id,
    int CarritoId,
    int VueloId,
    string? NumeroAsiento,
    string? Clase,
    decimal PrecioUnitario,
    int Cantidad);

public sealed record CrearItemCarritoDto(
    int CarritoId,
    int VueloId,
    string? NumeroAsiento,
    string? Clase,
    decimal PrecioUnitario,
    int Cantidad);

public sealed record ActualizarItemCarritoDto(
    int CarritoId,
    int VueloId,
    string? NumeroAsiento,
    string? Clase,
    decimal PrecioUnitario,
    int Cantidad);

public sealed record AgregarItemCarritoUsuarioDto(
    int VueloId,
    string? Clase,
    decimal PrecioUnitario,
    int Cantidad);

public sealed record ItemCarritoUsuarioDto(
    int Id,
    int CarritoId,
    int VueloId,
    string NumeroVuelo,
    string Aerolinea,
    string Origen,
    string Destino,
    DateTime FechaVuelo,
    DateTime? SalidaReal,
    DateTime? LlegadaReal,
    int PlazasOcupadas,
    int PlazasDisponibles,
    string Estado,
    int RetrasoMinutos,
    string MatriculaAvion,
    string? SelectedClass,
    int PassengerCount,
    decimal PrecioUnitario);

public sealed record VentaBoletoDto(
    int Id,
    string NumeroVenta,
    int? PuntoVentaId,
    int? EmpleadoVendedorId,
    int PasajeroId,
    DateTime? FechaVenta,
    decimal MontoSubtotal,
    decimal? Impuestos,
    decimal? Descuentos,
    decimal MontoTotal,
    int MetodoPagoId,
    string CanalVenta,
    string Estado);

public sealed record CrearVentaBoletoDto(
    string NumeroVenta,
    int? PuntoVentaId,
    int? EmpleadoVendedorId,
    int PasajeroId,
    DateTime? FechaVenta,
    decimal MontoSubtotal,
    decimal? Impuestos,
    decimal? Descuentos,
    decimal MontoTotal,
    int MetodoPagoId,
    string CanalVenta,
    string Estado);

public sealed record ActualizarVentaBoletoDto(
    string NumeroVenta,
    int? PuntoVentaId,
    int? EmpleadoVendedorId,
    int PasajeroId,
    DateTime? FechaVenta,
    decimal MontoSubtotal,
    decimal? Impuestos,
    decimal? Descuentos,
    decimal MontoTotal,
    int MetodoPagoId,
    string CanalVenta,
    string Estado);

public sealed record DetalleVentaBoletoDto(
    int Id,
    int VentaId,
    int ReservaId,
    decimal PrecioBase,
    decimal? CargosAdicionales);

public sealed record CrearDetalleVentaBoletoDto(
    int VentaId,
    int ReservaId,
    decimal PrecioBase,
    decimal? CargosAdicionales);

public sealed record ActualizarDetalleVentaBoletoDto(
    int VentaId,
    int ReservaId,
    decimal PrecioBase,
    decimal? CargosAdicionales);

public sealed record TransaccionPagoDto(
    int Id,
    int ReservaId,
    int MetodoPagoId,
    decimal MontoTotal,
    string Moneda,
    DateTime? FechaTransaccion,
    string Estado,
    string? NumeroAutorizacion,
    string? ReferenciaExterna,
    string? IpCliente,
    string? DetallesTarjeta);

public sealed record CrearTransaccionPagoDto(
    int ReservaId,
    int MetodoPagoId,
    decimal MontoTotal,
    string Moneda,
    DateTime? FechaTransaccion,
    string Estado,
    string? NumeroAutorizacion,
    string? ReferenciaExterna,
    string? IpCliente,
    string? DetallesTarjeta);

public sealed record ActualizarTransaccionPagoDto(
    int ReservaId,
    int MetodoPagoId,
    decimal MontoTotal,
    string Moneda,
    DateTime? FechaTransaccion,
    string Estado,
    string? NumeroAutorizacion,
    string? ReferenciaExterna,
    string? IpCliente,
    string? DetallesTarjeta);

public sealed record UsoPromocionDto(
    int Id,
    int PromocionId,
    int ReservaId,
    DateTime? FechaUso,
    decimal MontoDescuento);

public sealed record CrearUsoPromocionDto(
    int PromocionId,
    int ReservaId,
    DateTime? FechaUso,
    decimal MontoDescuento);

public sealed record ActualizarUsoPromocionDto(
    int PromocionId,
    int ReservaId,
    DateTime? FechaUso,
    decimal MontoDescuento);
