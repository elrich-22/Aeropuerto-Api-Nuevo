using AeropuertoAurora.Api.DTOs;

namespace AeropuertoAurora.Api.Repositories;

public interface IAeropuertoReadRepository
{
    Task<IReadOnlyList<AeropuertoDto>> GetAirportsAsync(int limit, CancellationToken cancellationToken);
    Task<AeropuertoDto?> GetAirportByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerminalDto>> GetTerminalsAsync(int? airportId, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<PuertaEmbarqueDto>> GetGatesAsync(int? terminalId, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<AerolineaDto>> GetAirlinesAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<AvionDto>> GetAircraftAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<AsientoAvionDto>> GetAircraftSeatsAsync(int aircraftId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProgramaVueloDto>> GetFlightProgramsAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<VueloDto>> GetFlightsAsync(DateTime? date, int limit, CancellationToken cancellationToken);
    Task<VueloDto?> GetFlightByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PasajeroDto>> GetPassengersAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReservaDto>> GetReservationsAsync(int? passengerId, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<EmpleadoDto>> GetEmployeesAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<EquipajeDto>> GetBaggageAsync(int? flightId, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<MantenimientoDto>> GetMaintenanceAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<ControlSeguridadDto>> GetSecurityControlsAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<IncidenteOperacionDto>> GetIncidentsAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReporteVentasPorFechaDto>> GetSalesReportAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReporteDestinoBuscadoDto>> GetTopDestinationsReportAsync(int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReporteIncidenteSeveridadDto>> GetIncidentsBySeverityReportAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ReporteOcupacionVueloDto>> GetFlightOccupancyReportAsync(DateTime? fechaInicio, DateTime? fechaFin, int limit, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReporteMetodoPagoDto>> GetPaymentMethodsReportAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken);
}
