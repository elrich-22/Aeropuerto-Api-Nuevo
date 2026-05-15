using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;

namespace AeropuertoAurora.Api.Services;

public sealed class AeropuertoQueryService(IAeropuertoReadRepository repository) : IAeropuertoQueryService
{
    public Task<IReadOnlyList<AeropuertoDto>> GetAirportsAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetAirportsAsync(limit, cancellationToken);

    public Task<AeropuertoDto?> GetAirportByIdAsync(int id, CancellationToken cancellationToken) =>
        repository.GetAirportByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<TerminalDto>> GetTerminalsAsync(int? airportId, int limit, CancellationToken cancellationToken) =>
        repository.GetTerminalsAsync(airportId, limit, cancellationToken);

    public Task<IReadOnlyList<PuertaEmbarqueDto>> GetGatesAsync(int? terminalId, int limit, CancellationToken cancellationToken) =>
        repository.GetGatesAsync(terminalId, limit, cancellationToken);

    public Task<IReadOnlyList<AerolineaDto>> GetAirlinesAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetAirlinesAsync(limit, cancellationToken);

    public Task<IReadOnlyList<AvionDto>> GetAircraftAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetAircraftAsync(limit, cancellationToken);

    public Task<IReadOnlyList<AsientoAvionDto>> GetAircraftSeatsAsync(int aircraftId, CancellationToken cancellationToken) =>
        repository.GetAircraftSeatsAsync(aircraftId, cancellationToken);

    public Task<IReadOnlyList<ProgramaVueloDto>> GetFlightProgramsAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetFlightProgramsAsync(limit, cancellationToken);

    public Task<IReadOnlyList<VueloDto>> GetFlightsAsync(DateTime? date, string? origen, string? destino, int limit, CancellationToken cancellationToken) =>
        repository.GetFlightsAsync(date, origen, destino, limit, cancellationToken);

    public Task<VueloDto?> GetFlightByIdAsync(int id, CancellationToken cancellationToken) =>
        repository.GetFlightByIdAsync(id, cancellationToken);

    public Task<IReadOnlyList<PasajeroDto>> GetPassengersAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetPassengersAsync(limit, cancellationToken);

    public Task<IReadOnlyList<ReservaDto>> GetReservationsAsync(int? passengerId, int limit, CancellationToken cancellationToken) =>
        repository.GetReservationsAsync(passengerId, limit, cancellationToken);

    public Task<IReadOnlyList<EmpleadoDto>> GetEmployeesAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetEmployeesAsync(limit, cancellationToken);

    public Task<IReadOnlyList<EquipajeDto>> GetBaggageAsync(int? flightId, int limit, CancellationToken cancellationToken) =>
        repository.GetBaggageAsync(flightId, limit, cancellationToken);

    public Task<IReadOnlyList<MantenimientoDto>> GetMaintenanceAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetMaintenanceAsync(limit, cancellationToken);

    public Task<IReadOnlyList<ControlSeguridadDto>> GetSecurityControlsAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetSecurityControlsAsync(limit, cancellationToken);

    public Task<IReadOnlyList<IncidenteOperacionDto>> GetIncidentsAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetIncidentsAsync(limit, cancellationToken);

    public Task<IReadOnlyList<ReporteVentasPorFechaDto>> GetSalesReportAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken) =>
        repository.GetSalesReportAsync(fechaInicio, fechaFin, cancellationToken);

    public Task<IReadOnlyList<ReporteDestinoBuscadoDto>> GetTopDestinationsReportAsync(int limit, CancellationToken cancellationToken) =>
        repository.GetTopDestinationsReportAsync(limit, cancellationToken);

    public Task<IReadOnlyList<ReporteIncidenteSeveridadDto>> GetIncidentsBySeverityReportAsync(CancellationToken cancellationToken) =>
        repository.GetIncidentsBySeverityReportAsync(cancellationToken);

    public Task<IReadOnlyList<ReporteOcupacionVueloDto>> GetFlightOccupancyReportAsync(DateTime? fechaInicio, DateTime? fechaFin, int limit, CancellationToken cancellationToken) =>
        repository.GetFlightOccupancyReportAsync(fechaInicio, fechaFin, limit, cancellationToken);

    public Task<IReadOnlyList<ReporteMetodoPagoDto>> GetPaymentMethodsReportAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken) =>
        repository.GetPaymentMethodsReportAsync(fechaInicio, fechaFin, cancellationToken);
}
