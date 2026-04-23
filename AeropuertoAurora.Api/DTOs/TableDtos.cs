namespace AeropuertoAurora.Api.DTOs;

public sealed record TablaInfoDto(string Nombre, string Alias);

public sealed record FilasTablaDto(
    string Tabla,
    int Cantidad,
    IReadOnlyList<IReadOnlyDictionary<string, object?>> Filas);
