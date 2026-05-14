using AeropuertoAurora.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[Authorize(Roles = "ADMIN")]
[ApiController]
[Route("api/reportes")]
public sealed class ReportesController(IAeropuertoQueryService service) : ControllerBase
{
    [HttpGet("ventas-por-fecha")]
    public async Task<IActionResult> GetSalesByDate(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetSalesReportAsync(fechaInicio, fechaFin, cancellationToken));
    }

    [HttpGet("destinos-mas-buscados")]
    public async Task<IActionResult> GetTopDestinations(
        [FromQuery] int limit = 10,
        CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetTopDestinationsReportAsync(limit, cancellationToken));
    }

    [HttpGet("incidentes-por-severidad")]
    public async Task<IActionResult> GetIncidentsBySeverity(CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetIncidentsBySeverityReportAsync(cancellationToken));
    }

    [HttpGet("ocupacion-vuelos")]
    public async Task<IActionResult> GetFlightOccupancy(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] int limit = 100,
        CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetFlightOccupancyReportAsync(fechaInicio, fechaFin, limit, cancellationToken));
    }

    [HttpGet("metodos-pago")]
    public async Task<IActionResult> GetPaymentMethods(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        CancellationToken cancellationToken = default)
    {
        return Ok(await service.GetPaymentMethodsReportAsync(fechaInicio, fechaFin, cancellationToken));
    }
}
