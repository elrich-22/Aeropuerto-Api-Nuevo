using AeropuertoAurora.Api.Configuration;
using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/tablas")]
public sealed class TablasController(ITableReadRepository repository) : ControllerBase
{
    [HttpGet]
    public IActionResult GetTables()
    {
        var tables = AirportTableRegistry.GetTableNames()
            .Select(table => new TablaInfoDto(table, table.Replace("AER_", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant()))
            .ToList();

        return Ok(tables);
    }

    [HttpGet("{tabla}")]
    public async Task<IActionResult> GetRows(string tabla, [FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        if (!AirportTableRegistry.TryResolve(tabla, out var tableName))
        {
            return NotFound(new ErrorApiDto("La tabla solicitada no existe o no esta habilitada para consulta."));
        }

        return Ok(await repository.GetRowsAsync(tableName, limit, cancellationToken));
    }
}
