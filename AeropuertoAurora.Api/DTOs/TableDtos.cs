namespace AeropuertoAurora.Api.DTOs;

public sealed record TablaInfoDto(string Nombre, string Alias);

public sealed record ColumnaTablaDto(
    string Nombre,
    string TipoDato,
    bool EsNullable,
    bool EsIdentidad,
    bool EsLlavePrimaria,
    int? Longitud,
    int? Precision,
    int? Escala);

public sealed record MetadataTablaDto(
    string Tabla,
    string? LlavePrimaria,
    IReadOnlyList<ColumnaTablaDto> Columnas);

public sealed record FilasTablaDto(
    string Tabla,
    int Cantidad,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Filas);
