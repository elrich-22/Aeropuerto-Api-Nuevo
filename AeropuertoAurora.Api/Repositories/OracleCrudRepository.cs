using AeropuertoAurora.Api.Data;
using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Repositories;

public sealed class OracleCrudRepository(IOracleConnectionFactory connectionFactory) : IOracleCrudRepository
{
    public async Task<IReadOnlyList<IReadOnlyDictionary<string, object?>>> GetAllAsync(
        CrudTableDefinition table,
        int limit,
        CancellationToken cancellationToken)
    {
        var safeLimit = Math.Clamp(limit, 1, 500);
        var sql = $"""
            SELECT *
            FROM (
                SELECT *
                FROM {table.TableName}
                ORDER BY {table.IdColumn}
            )
            WHERE ROWNUM <= :limit
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("limit", safeLimit));

        var rows = new List<IReadOnlyDictionary<string, object?>>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            rows.Add(ReadRow(reader));
        }

        return rows;
    }

    public async Task<IReadOnlyDictionary<string, object?>?> GetByIdAsync(
        CrudTableDefinition table,
        int id,
        CancellationToken cancellationToken)
    {
        var sql = $"SELECT * FROM {table.TableName} WHERE {table.IdColumn} = :id";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("id", id));

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadRow(reader) : null;
    }

    public async Task<int> CreateAsync(
        CrudTableDefinition table,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        var safeValues = FilterValues(table.InsertColumns, values);
        var columns = string.Join(", ", safeValues.Keys);
        var parameters = string.Join(", ", safeValues.Keys.Select(column => $":{column}"));
        var sql = $"""
            INSERT INTO {table.TableName} ({columns})
            VALUES ({parameters})
            RETURNING {table.IdColumn} INTO :newId
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;

        foreach (var value in safeValues)
        {
            command.Parameters.Add(new OracleParameter(value.Key, value.Value ?? DBNull.Value));
        }

        var idParameter = new OracleParameter("newId", Oracle.ManagedDataAccess.Client.OracleDbType.Int32)
        {
            Direction = System.Data.ParameterDirection.Output
        };
        command.Parameters.Add(idParameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
        return Convert.ToInt32(idParameter.Value.ToString());
    }

    public async Task<bool> UpdateAsync(
        CrudTableDefinition table,
        int id,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        var safeValues = FilterValues(table.UpdateColumns, values);
        var assignments = string.Join(", ", safeValues.Keys.Select(column => $"{column} = :{column}"));
        var sql = $"UPDATE {table.TableName} SET {assignments} WHERE {table.IdColumn} = :id";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;

        foreach (var value in safeValues)
        {
            command.Parameters.Add(new OracleParameter(value.Key, value.Value ?? DBNull.Value));
        }

        command.Parameters.Add(new OracleParameter("id", id));
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(
        CrudTableDefinition table,
        int id,
        CancellationToken cancellationToken)
    {
        var sql = $"DELETE FROM {table.TableName} WHERE {table.IdColumn} = :id";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("id", id));

        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private static Dictionary<string, object?> FilterValues(
        IReadOnlyList<string> allowedColumns,
        IReadOnlyDictionary<string, object?> values)
    {
        return allowedColumns.ToDictionary(column => column, column => values[column], StringComparer.OrdinalIgnoreCase);
    }

    private static IReadOnlyDictionary<string, object?> ReadRow(OracleDataReader reader)
    {
        var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        for (var index = 0; index < reader.FieldCount; index++)
        {
            row[reader.GetName(index)] = OracleValueConverter.Normalize(reader.GetValue(index));
        }

        return row;
    }
}
