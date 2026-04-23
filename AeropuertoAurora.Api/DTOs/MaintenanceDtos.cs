namespace AeropuertoAurora.Api.DTOs;

public sealed record HangarDto(
    int Id,
    string Codigo,
    string Nombre,
    int AeropuertoId,
    int? CapacidadAviones,
    decimal? AreaM2,
    decimal? AlturaMaxima,
    string? Tipo,
    string Estado);

public sealed record CrearHangarDto(
    string Codigo,
    string Nombre,
    int AeropuertoId,
    int? CapacidadAviones,
    decimal? AreaM2,
    decimal? AlturaMaxima,
    string? Tipo,
    string Estado);

public sealed record ActualizarHangarDto(
    string Codigo,
    string Nombre,
    int AeropuertoId,
    int? CapacidadAviones,
    decimal? AreaM2,
    decimal? AlturaMaxima,
    string? Tipo,
    string Estado);

public sealed record AsignacionHangarDto(
    int Id,
    int HangarId,
    int AvionId,
    DateTime FechaEntrada,
    DateTime? FechaSalidaProgramada,
    DateTime? FechaSalidaReal,
    string? Motivo,
    decimal? CostoHora,
    decimal? CostoTotal,
    string Estado);

public sealed record CrearAsignacionHangarDto(
    int HangarId,
    int AvionId,
    DateTime FechaEntrada,
    DateTime? FechaSalidaProgramada,
    DateTime? FechaSalidaReal,
    string? Motivo,
    decimal? CostoHora,
    decimal? CostoTotal,
    string Estado);

public sealed record ActualizarAsignacionHangarDto(
    int HangarId,
    int AvionId,
    DateTime FechaEntrada,
    DateTime? FechaSalidaProgramada,
    DateTime? FechaSalidaReal,
    string? Motivo,
    decimal? CostoHora,
    decimal? CostoTotal,
    string Estado);

public sealed record CategoriaRepuestoDto(
    int Id,
    string Nombre,
    string? Descripcion);

public sealed record CrearCategoriaRepuestoDto(
    string Nombre,
    string? Descripcion);

public sealed record ActualizarCategoriaRepuestoDto(
    string Nombre,
    string? Descripcion);

public sealed record RepuestoDto(
    int Id,
    string Codigo,
    string Nombre,
    string? Descripcion,
    int CategoriaId,
    int? ModeloAvionId,
    string? NumeroParteFabricante,
    int? StockMinimo,
    int? StockActual,
    int? StockMaximo,
    decimal PrecioUnitario,
    string? UbicacionBodega,
    string Estado);

public sealed record CrearRepuestoDto(
    string Codigo,
    string Nombre,
    string? Descripcion,
    int CategoriaId,
    int? ModeloAvionId,
    string? NumeroParteFabricante,
    int? StockMinimo,
    int? StockActual,
    int? StockMaximo,
    decimal PrecioUnitario,
    string? UbicacionBodega,
    string Estado);

public sealed record ActualizarRepuestoDto(
    string Codigo,
    string Nombre,
    string? Descripcion,
    int CategoriaId,
    int? ModeloAvionId,
    string? NumeroParteFabricante,
    int? StockMinimo,
    int? StockActual,
    int? StockMaximo,
    decimal PrecioUnitario,
    string? UbicacionBodega,
    string Estado);

public sealed record ProveedorDto(
    int Id,
    string NombreEmpresa,
    string Nit,
    string? DireccionCalle,
    string? DireccionZona,
    string? DireccionMunicipio,
    string? DireccionDepartamento,
    string? Pais,
    string? Telefono,
    string? Email,
    string? ContactoNombre,
    string? ContactoTelefono,
    string? ContactoEmail,
    string Estado,
    decimal? Calificacion);

public sealed record CrearProveedorDto(
    string NombreEmpresa,
    string Nit,
    string? DireccionCalle,
    string? DireccionZona,
    string? DireccionMunicipio,
    string? DireccionDepartamento,
    string? Pais,
    string? Telefono,
    string? Email,
    string? ContactoNombre,
    string? ContactoTelefono,
    string? ContactoEmail,
    string Estado,
    decimal? Calificacion);

public sealed record ActualizarProveedorDto(
    string NombreEmpresa,
    string Nit,
    string? DireccionCalle,
    string? DireccionZona,
    string? DireccionMunicipio,
    string? DireccionDepartamento,
    string? Pais,
    string? Telefono,
    string? Email,
    string? ContactoNombre,
    string? ContactoTelefono,
    string? ContactoEmail,
    string Estado,
    decimal? Calificacion);

public sealed record OrdenCompraRepuestoDto(
    int Id,
    string NumeroOrden,
    int ProveedorId,
    DateTime? FechaOrden,
    DateTime? FechaEntregaEsperada,
    DateTime? FechaEntregaReal,
    decimal? MontoTotal,
    string Estado,
    int? EmpleadoSolicitaId,
    string? Observaciones);

public sealed record CrearOrdenCompraRepuestoDto(
    string NumeroOrden,
    int ProveedorId,
    DateTime? FechaOrden,
    DateTime? FechaEntregaEsperada,
    DateTime? FechaEntregaReal,
    decimal? MontoTotal,
    string Estado,
    int? EmpleadoSolicitaId,
    string? Observaciones);

public sealed record ActualizarOrdenCompraRepuestoDto(
    string NumeroOrden,
    int ProveedorId,
    DateTime? FechaOrden,
    DateTime? FechaEntregaEsperada,
    DateTime? FechaEntregaReal,
    decimal? MontoTotal,
    string Estado,
    int? EmpleadoSolicitaId,
    string? Observaciones);

public sealed record DetalleOrdenCompraDto(
    int Id,
    int OrdenCompraId,
    int RepuestoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public sealed record CrearDetalleOrdenCompraDto(
    int OrdenCompraId,
    int RepuestoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public sealed record ActualizarDetalleOrdenCompraDto(
    int OrdenCompraId,
    int RepuestoId,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public sealed record MovimientoRepuestoDto(
    int Id,
    int RepuestoId,
    string TipoMovimiento,
    int Cantidad,
    DateTime? FechaMovimiento,
    int? EmpleadoId,
    string? Motivo,
    string? Referencia);

public sealed record CrearMovimientoRepuestoDto(
    int RepuestoId,
    string TipoMovimiento,
    int Cantidad,
    DateTime? FechaMovimiento,
    int? EmpleadoId,
    string? Motivo,
    string? Referencia);

public sealed record ActualizarMovimientoRepuestoDto(
    int RepuestoId,
    string TipoMovimiento,
    int Cantidad,
    DateTime? FechaMovimiento,
    int? EmpleadoId,
    string? Motivo,
    string? Referencia);

public sealed record MantenimientoAvionDto(
    int Id,
    int AvionId,
    string TipoMantenimiento,
    DateTime FechaInicio,
    DateTime? FechaFinEstimada,
    DateTime? FechaFinReal,
    string? DescripcionTrabajo,
    int? EmpleadoResponsableId,
    decimal? CostoManoObra,
    decimal? CostoRepuestos,
    decimal? CostoTotal,
    string Estado);

public sealed record CrearMantenimientoAvionDto(
    int AvionId,
    string TipoMantenimiento,
    DateTime FechaInicio,
    DateTime? FechaFinEstimada,
    DateTime? FechaFinReal,
    string? DescripcionTrabajo,
    int? EmpleadoResponsableId,
    decimal? CostoManoObra,
    decimal? CostoRepuestos,
    decimal? CostoTotal,
    string Estado);

public sealed record ActualizarMantenimientoAvionDto(
    int AvionId,
    string TipoMantenimiento,
    DateTime FechaInicio,
    DateTime? FechaFinEstimada,
    DateTime? FechaFinReal,
    string? DescripcionTrabajo,
    int? EmpleadoResponsableId,
    decimal? CostoManoObra,
    decimal? CostoRepuestos,
    decimal? CostoTotal,
    string Estado);

public sealed record RepuestoUtilizadoDto(
    int Id,
    int MantenimientoId,
    int RepuestoId,
    int Cantidad);

public sealed record CrearRepuestoUtilizadoDto(
    int MantenimientoId,
    int RepuestoId,
    int Cantidad);

public sealed record ActualizarRepuestoUtilizadoDto(
    int MantenimientoId,
    int RepuestoId,
    int Cantidad);
