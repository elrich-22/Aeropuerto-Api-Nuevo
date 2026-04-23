using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/tarjetas-embarque")]
public sealed class TarjetasEmbarqueController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_TARJETA_EMBARQUE",
        "TAE_ID_TARJETA",
        ["TAE_ID_CHECKIN", "TAE_CODIGO_QR", "TAE_GRUPO_ABORDAJE", "TAE_ZONA", "TAE_FECHA_EMISION"],
        ["TAE_ID_CHECKIN", "TAE_CODIGO_QR", "TAE_GRUPO_ABORDAJE", "TAE_ZONA", "TAE_FECHA_EMISION"]);

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
    public async Task<IActionResult> Create(CrearTarjetaEmbarqueDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarTarjetaEmbarqueDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static TarjetaEmbarqueDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new TarjetaEmbarqueDto(
            row.ToInt("TAE_ID_TARJETA"),
            row.ToInt("TAE_ID_CHECKIN"),
            row.ToStringValue("TAE_CODIGO_QR"),
            row.ToNullableString("TAE_GRUPO_ABORDAJE"),
            row.ToNullableString("TAE_ZONA"),
            row.ToNullableDateTime("TAE_FECHA_EMISION"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearTarjetaEmbarqueDto dto) => new Dictionary<string, object?>
    {
        ["TAE_ID_CHECKIN"] = dto.CheckInId,
        ["TAE_CODIGO_QR"] = dto.CodigoQr,
        ["TAE_GRUPO_ABORDAJE"] = dto.GrupoAbordaje,
        ["TAE_ZONA"] = dto.Zona,
        ["TAE_FECHA_EMISION"] = dto.FechaEmision
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarTarjetaEmbarqueDto dto) => new Dictionary<string, object?>
    {
        ["TAE_ID_CHECKIN"] = dto.CheckInId,
        ["TAE_CODIGO_QR"] = dto.CodigoQr,
        ["TAE_GRUPO_ABORDAJE"] = dto.GrupoAbordaje,
        ["TAE_ZONA"] = dto.Zona,
        ["TAE_FECHA_EMISION"] = dto.FechaEmision
    };
}
