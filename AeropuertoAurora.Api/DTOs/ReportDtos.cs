namespace AeropuertoAurora.Api.DTOs;

public sealed record ReporteVentasPorFechaDto(
    DateTime Fecha,
    int TotalVentas,
    decimal Subtotal,
    decimal Impuestos,
    decimal Descuentos,
    decimal MontoTotal);

public sealed record ReporteDestinoBuscadoDto(
    int AeropuertoId,
    string Aeropuerto,
    int TotalBusquedas,
    int TotalClicks,
    int TotalPasajeros);

public sealed record ReporteIncidenteSeveridadDto(
    string Severidad,
    int TotalIncidentes,
    int Abiertos,
    int Cerrados);

public sealed record ReporteOcupacionVueloDto(
    int VueloId,
    string NumeroVuelo,
    DateTime FechaVuelo,
    int PlazasOcupadas,
    int PlazasDisponibles,
    decimal PorcentajeOcupacion,
    string Estado);

public sealed record ReporteMetodoPagoDto(
    int MetodoPagoId,
    string MetodoPago,
    int TotalTransacciones,
    decimal MontoTotal,
    decimal MontoPromedio,
    string EstadoPrincipal);
