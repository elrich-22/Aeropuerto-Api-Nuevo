using AeropuertoAurora.Api.DTOs;

namespace AeropuertoAurora.Api.Repositories;

public interface ITableReadRepository
{
    Task<FilasTablaDto> GetRowsAsync(string tableName, int limit, CancellationToken cancellationToken);

    Task<MetadataTablaDto> GetMetadataAsync(string tableName, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, object?>?> CreateRowAsync(
        string tableName,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken);

    Task<bool> UpdateRowAsync(
        string tableName,
        string keyValue,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken);

    Task<bool> DeleteRowAsync(string tableName, string keyValue, CancellationToken cancellationToken);
}
