namespace AeropuertoAurora.Api.Repositories;

public sealed record CrudTableDefinition(
    string TableName,
    string IdColumn,
    IReadOnlyList<string> InsertColumns,
    IReadOnlyList<string> UpdateColumns);
