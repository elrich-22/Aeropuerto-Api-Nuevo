namespace AeropuertoAurora.Api.DTOs;

public sealed record BusquedaVueloDto(
    int Id,
    int? SesionId,
    int AeropuertoOrigenId,
    int AeropuertoDestinoId,
    DateTime FechaIda,
    DateTime? FechaVuelta,
    int NumeroPasajeros,
    string? Clase,
    DateTime? FechaBusqueda,
    string SeConvirtioCompra);

public sealed record CrearBusquedaVueloDto(
    int? SesionId,
    int AeropuertoOrigenId,
    int AeropuertoDestinoId,
    DateTime FechaIda,
    DateTime? FechaVuelta,
    int NumeroPasajeros,
    string? Clase,
    DateTime? FechaBusqueda,
    string SeConvirtioCompra);

public sealed record ActualizarBusquedaVueloDto(
    int? SesionId,
    int AeropuertoOrigenId,
    int AeropuertoDestinoId,
    DateTime FechaIda,
    DateTime? FechaVuelta,
    int NumeroPasajeros,
    string? Clase,
    DateTime? FechaBusqueda,
    string SeConvirtioCompra);

public sealed record ClickDestinoDto(
    int Id,
    int? SesionId,
    int AeropuertoDestinoId,
    DateTime? FechaClick,
    string? OrigenBusqueda,
    DateTime? FechaViajeBuscada,
    int? NumeroPasajeros,
    string? ClaseBuscada);

public sealed record CrearClickDestinoDto(
    int? SesionId,
    int AeropuertoDestinoId,
    DateTime? FechaClick,
    string? OrigenBusqueda,
    DateTime? FechaViajeBuscada,
    int? NumeroPasajeros,
    string? ClaseBuscada);

public sealed record ActualizarClickDestinoDto(
    int? SesionId,
    int AeropuertoDestinoId,
    DateTime? FechaClick,
    string? OrigenBusqueda,
    DateTime? FechaViajeBuscada,
    int? NumeroPasajeros,
    string? ClaseBuscada);

public sealed record PuntoVentaDto(
    int Id,
    string Codigo,
    string Nombre,
    int? AeropuertoId,
    string? Ubicacion,
    string Estado);

public sealed record CrearPuntoVentaDto(
    string Codigo,
    string Nombre,
    int? AeropuertoId,
    string? Ubicacion,
    string Estado);

public sealed record ActualizarPuntoVentaDto(
    string Codigo,
    string Nombre,
    int? AeropuertoId,
    string? Ubicacion,
    string Estado);
