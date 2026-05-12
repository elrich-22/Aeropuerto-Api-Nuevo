using AeropuertoAurora.Api.Data;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;
using System.Text.Json;

namespace AeropuertoAurora.Api.Repositories;

public sealed class OracleCrudRepository(
    IOracleConnectionFactory connectionFactory,
    IHttpContextAccessor httpContextAccessor) : IOracleCrudRepository
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
        if (safeValues.Count == 0)
        {
            return 0;
        }

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
        var newId = Convert.ToInt32(idParameter.Value.ToString());
        await WriteAuditAsync(connection, table.TableName, "INSERT", null, WithKey(table, newId, safeValues), cancellationToken);
        return newId;
    }

    public async Task<bool> UpdateAsync(
        CrudTableDefinition table,
        int id,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        var safeValues = FilterValues(table.UpdateColumns, values);
        if (safeValues.Count == 0)
        {
            return false;
        }

        var previous = await GetByIdAsync(table, id, cancellationToken);
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
        var updated = await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        if (updated)
        {
            await WriteAuditAsync(connection, table.TableName, "UPDATE", previous, WithKey(table, id, safeValues), cancellationToken);
        }

        return updated;
    }

    public async Task<bool> DeleteAsync(
        CrudTableDefinition table,
        int id,
        CancellationToken cancellationToken)
    {
        var previous = await GetByIdAsync(table, id, cancellationToken);
        var sql = $"DELETE FROM {table.TableName} WHERE {table.IdColumn} = :id";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("id", id));

        var deleted = await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        if (deleted)
        {
            await WriteAuditAsync(connection, table.TableName, "DELETE", previous, null, cancellationToken);
        }

        return deleted;
    }

    private async Task WriteAuditAsync(
        OracleConnection connection,
        string tableName,
        string operation,
        IReadOnlyDictionary<string, object?>? previous,
        IReadOnlyDictionary<string, object?>? next,
        CancellationToken cancellationToken)
    {
        if (tableName.Equals("AER_AUDITORIA", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        const string sql = """
            INSERT INTO AER_AUDITORIA (
                AUD_TABLA_AFECTADA,
                AUD_OPERACION,
                AUD_USUARIO,
                AUD_FECHA_HORA,
                AUD_IP_ADDRESS,
                AUD_DATOS_ANTERIORES,
                AUD_DATOS_NUEVOS
            )
            VALUES (
                :tableName,
                :operation,
                :userName,
                SYSTIMESTAMP,
                :ipAddress,
                :previousData,
                :nextData
            )
            """;

        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("tableName", tableName));
        command.Parameters.Add(new OracleParameter("operation", operation));
        command.Parameters.Add(new OracleParameter("userName", CurrentUser()));
        command.Parameters.Add(new OracleParameter("ipAddress", CurrentIpAddress()));
        command.Parameters.Add(new OracleParameter("previousData", SerializeAuditData(previous)));
        command.Parameters.Add(new OracleParameter("nextData", SerializeAuditData(next)));

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static IReadOnlyDictionary<string, object?> WithKey(
        CrudTableDefinition table,
        int id,
        IReadOnlyDictionary<string, object?> values)
    {
        var result = new Dictionary<string, object?>(values, StringComparer.OrdinalIgnoreCase)
        {
            [table.IdColumn] = id
        };

        return result;
    }

    private string CurrentUser()
    {
        var request = httpContextAccessor.HttpContext?.Request;
        var value =
            request?.Headers["X-User"].FirstOrDefault() ??
            request?.Headers["X-Usuario"].FirstOrDefault() ??
            request?.Headers["X-User-Email"].FirstOrDefault();

        return string.IsNullOrWhiteSpace(value) ? "sistema" : value[..Math.Min(value.Length, 50)];
    }

    private string? CurrentIpAddress()
    {
        var context = httpContextAccessor.HttpContext;
        var forwarded = context?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var value = string.IsNullOrWhiteSpace(forwarded)
            ? context?.Connection.RemoteIpAddress?.ToString()
            : forwarded.Split(',')[0].Trim();

        return string.IsNullOrWhiteSpace(value) ? null : value[..Math.Min(value.Length, 50)];
    }

    private static string? SerializeAuditData(IReadOnlyDictionary<string, object?>? values)
    {
        if (values is null)
        {
            return null;
        }

        var masked = values.ToDictionary(
            pair => pair.Key,
            pair => IsSensitiveColumn(pair.Key) ? "***" : pair.Value,
            StringComparer.OrdinalIgnoreCase);

        return JsonSerializer.Serialize(masked);
    }

    private static bool IsSensitiveColumn(string column)
    {
        return column.Contains("CONTRASENA", StringComparison.OrdinalIgnoreCase) ||
            column.Contains("PASSWORD", StringComparison.OrdinalIgnoreCase) ||
            column.Contains("HASH", StringComparison.OrdinalIgnoreCase) ||
            column.Equals("USL_SAL", StringComparison.OrdinalIgnoreCase) ||
            column.Contains("SALT", StringComparison.OrdinalIgnoreCase) ||
            column.Contains("TOKEN", StringComparison.OrdinalIgnoreCase) ||
            column.Contains("TARJETA", StringComparison.OrdinalIgnoreCase);
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
