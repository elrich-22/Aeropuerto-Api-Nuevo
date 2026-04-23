namespace AeropuertoAurora.Api.Repositories;

public interface IOracleCrudRepository
{
    Task<IReadOnlyList<IReadOnlyDictionary<string, object?>>> GetAllAsync(
        CrudTableDefinition table,
        int limit,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, object?>?> GetByIdAsync(
        CrudTableDefinition table,
        int id,
        CancellationToken cancellationToken);

    Task<int> CreateAsync(
        CrudTableDefinition table,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        CrudTableDefinition table,
        int id,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        CrudTableDefinition table,
        int id,
        CancellationToken cancellationToken);
}
