using AeropuertoAurora.Api.Data;
using AeropuertoAurora.Api.DTOs;
using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Repositories;

public sealed class TableReadRepository(IOracleConnectionFactory connectionFactory) : ITableReadRepository
{
    public async Task<FilasTablaDto> GetRowsAsync(string tableName, int limit, CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 500);
        var sql = $"""
            SELECT *
            FROM (
                SELECT *
                FROM {tableName}
            )
            WHERE ROWNUM <= :limit
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("limit", OracleDbType.Int32) { Value = safeLimit });

        var rows = new List<IReadOnlyDictionary<string, object?>>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            for (var index = 0; index < reader.FieldCount; index++)
            {
                row[reader.GetName(index)] = OracleValueConverter.Normalize(reader.GetValue(index));
            }

            rows.Add(row);
        }

        return new FilasTablaDto(tableName, rows.Count, rows);
    }
}
