using AeropuertoAurora.Api.Configuration;
using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

    [HttpGet("{tabla}/metadata")]
    public async Task<IActionResult> GetMetadata(string tabla, CancellationToken cancellationToken)
    {
        if (!AirportTableRegistry.TryResolve(tabla, out var tableName))
        {
            return NotFound(new ErrorApiDto("La tabla solicitada no existe o no esta habilitada para administracion."));
        }

        return Ok(await repository.GetMetadataAsync(tableName, cancellationToken));
    }

    [HttpPost("{tabla}")]
    public async Task<IActionResult> CreateRow(
        string tabla,
        [FromBody] Dictionary<string, JsonElement> body,
        CancellationToken cancellationToken)
    {
        if (!AirportTableRegistry.TryResolve(tabla, out var tableName))
        {
            return NotFound(new ErrorApiDto("La tabla solicitada no existe o no esta habilitada para administracion."));
        }

        var row = await repository.CreateRowAsync(tableName, ToValues(body), cancellationToken);
        return row is null
            ? BadRequest(new ErrorApiDto("No se recibieron columnas editables para crear el registro."))
            : CreatedAtAction(nameof(GetRows), new { tabla }, row);
    }

    [HttpPut("{tabla}/{id}")]
    public async Task<IActionResult> UpdateRow(
        string tabla,
        string id,
        [FromBody] Dictionary<string, JsonElement> body,
        CancellationToken cancellationToken)
    {
        if (!AirportTableRegistry.TryResolve(tabla, out var tableName))
        {
            return NotFound(new ErrorApiDto("La tabla solicitada no existe o no esta habilitada para administracion."));
        }

        return await repository.UpdateRowAsync(tableName, id, ToValues(body), cancellationToken)
            ? NoContent()
            : NotFound(new ErrorApiDto("No se encontro el registro o no hubo columnas editables para actualizar."));
    }

    [HttpDelete("{tabla}/{id}")]
    public async Task<IActionResult> DeleteRow(string tabla, string id, CancellationToken cancellationToken)
    {
        if (!AirportTableRegistry.TryResolve(tabla, out var tableName))
        {
            return NotFound(new ErrorApiDto("La tabla solicitada no existe o no esta habilitada para administracion."));
        }

        return await repository.DeleteRowAsync(tableName, id, cancellationToken)
            ? NoContent()
            : NotFound(new ErrorApiDto("No se encontro el registro solicitado."));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(Dictionary<string, JsonElement> body)
    {
        return body.ToDictionary(pair => pair.Key, pair => ToValue(pair.Value), StringComparer.OrdinalIgnoreCase);
    }

    private static object? ToValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number when value.TryGetInt64(out var integer) => integer,
            JsonValueKind.Number when value.TryGetDecimal(out var number) => number,
            JsonValueKind.String => value.GetString(),
            _ => value.ToString()
        };
    }
}
