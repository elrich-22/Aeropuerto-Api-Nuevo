using AeropuertoAurora.Api.DTOs;
using AeropuertoAurora.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AeropuertoAurora.Api.Controllers;

[ApiController]
[Route("api/modelos-avion")]
public sealed class ModelosAvionController(IOracleCrudRepository repository) : ControllerBase
{
    private static readonly CrudTableDefinition Table = new(
        "AER_MODELOAVION",
        "MOD_ID_MODELO",
        ["MOD_NOMBRE_MODELO", "MOD_FABRICANTE", "MOD_CAPACIDAD_PASAJEROS", "MOD_CAPACIDAD_CARGA", "MOD_ALCANCE_KM", "MOD_VELOCIDAD_CRUCERO", "MOD_ANIO_INTRODUCCION", "MOD_TIPO_MOTOR"],
        ["MOD_NOMBRE_MODELO", "MOD_FABRICANTE", "MOD_CAPACIDAD_PASAJEROS", "MOD_CAPACIDAD_CARGA", "MOD_ALCANCE_KM", "MOD_VELOCIDAD_CRUCERO", "MOD_ANIO_INTRODUCCION", "MOD_TIPO_MOTOR"]);

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
    public async Task<IActionResult> Create(CrearModeloAvionDto dto, CancellationToken cancellationToken)
    {
        var id = await repository.CreateAsync(Table, ToValues(dto), cancellationToken);
        var row = await repository.GetByIdAsync(Table, id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, Map(row!));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ActualizarModeloAvionDto dto, CancellationToken cancellationToken)
    {
        return await repository.UpdateAsync(Table, id, ToValues(dto), cancellationToken) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        return await repository.DeleteAsync(Table, id, cancellationToken) ? NoContent() : NotFound();
    }

    private static ModeloAvionDto Map(IReadOnlyDictionary<string, object?> row)
    {
        return new ModeloAvionDto(
            row.ToInt("MOD_ID_MODELO"),
            row.ToStringValue("MOD_NOMBRE_MODELO"),
            row.ToNullableString("MOD_FABRICANTE"),
            row.ToInt("MOD_CAPACIDAD_PASAJEROS"),
            row.ToNullableInt("MOD_CAPACIDAD_CARGA"),
            row.ToNullableInt("MOD_ALCANCE_KM"),
            row.ToNullableInt("MOD_VELOCIDAD_CRUCERO"),
            row.ToNullableInt("MOD_ANIO_INTRODUCCION"),
            row.ToNullableString("MOD_TIPO_MOTOR"));
    }

    private static IReadOnlyDictionary<string, object?> ToValues(CrearModeloAvionDto dto) => new Dictionary<string, object?>
    {
        ["MOD_NOMBRE_MODELO"] = dto.NombreModelo,
        ["MOD_FABRICANTE"] = dto.Fabricante,
        ["MOD_CAPACIDAD_PASAJEROS"] = dto.CapacidadPasajeros,
        ["MOD_CAPACIDAD_CARGA"] = dto.CapacidadCarga,
        ["MOD_ALCANCE_KM"] = dto.AlcanceKm,
        ["MOD_VELOCIDAD_CRUCERO"] = dto.VelocidadCrucero,
        ["MOD_ANIO_INTRODUCCION"] = dto.AnioIntroduccion,
        ["MOD_TIPO_MOTOR"] = dto.TipoMotor
    };

    private static IReadOnlyDictionary<string, object?> ToValues(ActualizarModeloAvionDto dto) => new Dictionary<string, object?>
    {
        ["MOD_NOMBRE_MODELO"] = dto.NombreModelo,
        ["MOD_FABRICANTE"] = dto.Fabricante,
        ["MOD_CAPACIDAD_PASAJEROS"] = dto.CapacidadPasajeros,
        ["MOD_CAPACIDAD_CARGA"] = dto.CapacidadCarga,
        ["MOD_ALCANCE_KM"] = dto.AlcanceKm,
        ["MOD_VELOCIDAD_CRUCERO"] = dto.VelocidadCrucero,
        ["MOD_ANIO_INTRODUCCION"] = dto.AnioIntroduccion,
        ["MOD_TIPO_MOTOR"] = dto.TipoMotor
    };
}
