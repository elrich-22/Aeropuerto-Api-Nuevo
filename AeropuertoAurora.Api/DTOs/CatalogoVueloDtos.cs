namespace AeropuertoAurora.Api.DTOs;

public sealed record DiaVueloDto(
    int Id,
    int ProgramaVueloId,
    int DiaSemana);

public sealed record CrearDiaVueloDto(
    int ProgramaVueloId,
    int DiaSemana);

public sealed record ActualizarDiaVueloDto(
    int ProgramaVueloId,
    int DiaSemana);

public sealed record EscalaTecnicaDto(
    int Id,
    int ProgramaVueloId,
    int AeropuertoId,
    int NumeroOrden,
    string HoraLlegadaEstimada,
    string HoraSalidaEstimada,
    int? DuracionEscala);

public sealed record CrearEscalaTecnicaDto(
    int ProgramaVueloId,
    int AeropuertoId,
    int NumeroOrden,
    string HoraLlegadaEstimada,
    string HoraSalidaEstimada,
    int? DuracionEscala);

public sealed record ActualizarEscalaTecnicaDto(
    int ProgramaVueloId,
    int AeropuertoId,
    int NumeroOrden,
    string HoraLlegadaEstimada,
    string HoraSalidaEstimada,
    int? DuracionEscala);

public sealed record MetodoPagoDto(
    int Id,
    string Nombre,
    string Tipo,
    string Estado,
    decimal ComisionPorcentaje);

public sealed record CrearMetodoPagoDto(
    string Nombre,
    string Tipo,
    string Estado,
    decimal ComisionPorcentaje);

public sealed record ActualizarMetodoPagoDto(
    string Nombre,
    string Tipo,
    string Estado,
    decimal ComisionPorcentaje);

public sealed record PromocionDto(
    int Id,
    string Codigo,
    string? Descripcion,
    string TipoDescuento,
    decimal ValorDescuento,
    DateTime FechaInicio,
    DateTime FechaFin,
    int? UsosMaximos,
    int UsosActuales,
    string Estado);

public sealed record CrearPromocionDto(
    string Codigo,
    string? Descripcion,
    string TipoDescuento,
    decimal ValorDescuento,
    DateTime FechaInicio,
    DateTime FechaFin,
    int? UsosMaximos,
    int UsosActuales,
    string Estado);

public sealed record ActualizarPromocionDto(
    string Codigo,
    string? Descripcion,
    string TipoDescuento,
    decimal ValorDescuento,
    DateTime FechaInicio,
    DateTime FechaFin,
    int? UsosMaximos,
    int UsosActuales,
    string Estado);
