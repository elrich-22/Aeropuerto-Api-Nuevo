using AeropuertoAurora.Api.DTOs;

namespace AeropuertoAurora.Api.Repositories;

public interface ITableReadRepository
{
    Task<FilasTablaDto> GetRowsAsync(string tableName, int limit, CancellationToken cancellationToken);
}
