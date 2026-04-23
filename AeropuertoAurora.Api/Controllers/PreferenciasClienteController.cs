using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/preferencias-cliente")]
public sealed class PreferenciasClienteController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_PREFERENCIACLIENTE",
        "PRF_ID_PREFERENCIA",
        ["PRF_ID_PASAJERO", "PRF_TIPO_PREFERENCIA", "PRF_VALOR_PREFERENCIA", "PRF_FECHA_REGISTRO"],
        ["PRF_ID_PASAJERO", "PRF_TIPO_PREFERENCIA", "PRF_VALOR_PREFERENCIA", "PRF_FECHA_REGISTRO"]);

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int limit = 100, CancellationToken cancellationToken = default)
    {
        var rows = await repository.GetAllAsync(Table, limit, cancellationToken);
        return Ok(rows.Select(Map));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return row is null ? NotFound() : Ok(Map(row));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrearPreferenciaClienteDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarPreferenciaClienteDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static PreferenciaClienteDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new PreferenciaClienteDto(
            row.ToInt("PRF_ID_PREFERENCIA"),
            row.ToInt("PRF_ID_PASAJERO"),
            row.ToStringValue("PRF_TIPO_PREFERENCIA"),
            row.ToNullableString("PRF_VALOR_PREFERENCIA"),
            row.ToNullableDateTime("PRF_FECHA_REGISTRO"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearPreferenciaClienteDto dto) => new Dictionary<string, object?>
    {
        ["PRF_ID_PASAJERO"] = dto.PasajeroId,
        ["PRF_TIPO_PREFERENCIA"] = dto.TipoPreferencia,
        ["PRF_VALOR_PREFERENCIA"] = dto.ValorPreferencia,
        ["PRF_FECHA_REGISTRO"] = dto.FechaRegistro
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarPreferenciaClienteDto dto) => new Dictionary<string, object?>
    {
        ["PRF_ID_PASAJERO"] = dto.PasajeroId,
        ["PRF_TIPO_PREFERENCIA"] = dto.TipoPreferencia,
        ["PRF_VALOR_PREFERENCIA"] = dto.ValorPreferencia,
        ["PRF_FECHA_REGISTRO"] = dto.FechaRegistro
    };
}
