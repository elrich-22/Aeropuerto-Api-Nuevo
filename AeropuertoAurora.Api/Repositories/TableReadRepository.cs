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

    public async Task<MetadataTablaDto> GetMetadataAsync(string tableName, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.NULLABLE,
                c.IDENTITY_COLUMN,
                c.DATA_LENGTH,
                c.DATA_PRECISION,
                c.DATA_SCALE,
                CASE WHEN pk.COLUMN_NAME IS NULL THEN 'NO' ELSE 'YES' END AS IS_PRIMARY_KEY
            FROM USER_TAB_COLUMNS c
            LEFT JOIN (
                SELECT cols.TABLE_NAME, cols.COLUMN_NAME
                FROM USER_CONSTRAINTS cons
                INNER JOIN USER_CONS_COLUMNS cols
                    ON cons.CONSTRAINT_NAME = cols.CONSTRAINT_NAME
                WHERE cons.CONSTRAINT_TYPE = 'P'
            ) pk
                ON pk.TABLE_NAME = c.TABLE_NAME
                AND pk.COLUMN_NAME = c.COLUMN_NAME
            WHERE c.TABLE_NAME = :tableName
            ORDER BY c.COLUMN_ID
            """;

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("tableName", tableName));

        var columns = new List<ColumnaTablaDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            columns.Add(new ColumnaTablaDto(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2).Equals("Y", StringComparison.OrdinalIgnoreCase),
                reader.GetString(3).Equals("YES", StringComparison.OrdinalIgnoreCase),
                reader.GetString(7).Equals("YES", StringComparison.OrdinalIgnoreCase),
                reader.IsDBNull(4) ? null : reader.GetInt32(4),
                reader.IsDBNull(5) ? null : reader.GetInt32(5),
                reader.IsDBNull(6) ? null : reader.GetInt32(6)));
        }

        return new MetadataTablaDto(tableName, columns.FirstOrDefault(column => column.EsLlavePrimaria)?.Nombre, columns);
    }

    public async Task<IReadOnlyDictionary<string, object?>?> CreateRowAsync(
        string tableName,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        var metadata = await GetMetadataAsync(tableName, cancellationToken);
        var safeValues = FilterValues(metadata, values, includePrimaryKey: true);

        if (safeValues.Count == 0)
        {
            return null;
        }

        var columns = string.Join(", ", safeValues.Keys);
        var parameters = string.Join(", ", safeValues.Keys.Select(column => $":{column}"));
        var sql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;

        foreach (var value in safeValues)
        {
            command.Parameters.Add(new OracleParameter(value.Key, value.Value ?? DBNull.Value));
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
        return await GetLastRowAsync(connection, tableName, metadata, cancellationToken);
    }

    public async Task<bool> UpdateRowAsync(
        string tableName,
        string keyValue,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        var metadata = await GetMetadataAsync(tableName, cancellationToken);
        var keyColumn = RequirePrimaryKey(metadata);
        var safeValues = FilterValues(metadata, values, includePrimaryKey: false);

        if (safeValues.Count == 0)
        {
            return false;
        }

        var assignments = string.Join(", ", safeValues.Keys.Select(column => $"{column} = :{column}"));
        var sql = $"UPDATE {tableName} SET {assignments} WHERE {keyColumn.Nombre} = :keyValue";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;

        foreach (var value in safeValues)
        {
            command.Parameters.Add(new OracleParameter(value.Key, value.Value ?? DBNull.Value));
        }

        command.Parameters.Add(new OracleParameter("keyValue", ConvertValue(keyColumn, keyValue)));
        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteRowAsync(string tableName, string keyValue, CancellationToken cancellationToken)
    {
        var metadata = await GetMetadataAsync(tableName, cancellationToken);
        var keyColumn = RequirePrimaryKey(metadata);
        var sql = $"DELETE FROM {tableName} WHERE {keyColumn.Nombre} = :keyValue";

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.Add(new OracleParameter("keyValue", ConvertValue(keyColumn, keyValue)));

        return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
    }

    private static ColumnaTablaDto RequirePrimaryKey(MetadataTablaDto metadata)
    {
        return metadata.Columnas.FirstOrDefault(column => column.EsLlavePrimaria)
            ?? throw new InvalidOperationException($"La tabla {metadata.Tabla} no tiene llave primaria configurada.");
    }

    private static Dictionary<string, object?> FilterValues(
        MetadataTablaDto metadata,
        IReadOnlyDictionary<string, object?> values,
        bool includePrimaryKey)
    {
        var allowedColumns = metadata.Columnas
            .Where(column => !column.EsIdentidad && (includePrimaryKey || !column.EsLlavePrimaria))
            .ToDictionary(column => column.Nombre, StringComparer.OrdinalIgnoreCase);

        var filtered = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var value in values)
        {
            if (!allowedColumns.TryGetValue(value.Key, out var column))
            {
                continue;
            }

            filtered[column.Nombre] = ConvertValue(column, value.Value);
        }

        return filtered;
    }

    private static object? ConvertValue(ColumnaTablaDto column, object? value)
    {
        if (value is null)
        {
            return null;
        }

        var text = value.ToString();

        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var dataType = column.TipoDato.ToUpperInvariant();

        if (dataType.Contains("DATE", StringComparison.Ordinal) ||
            dataType.Contains("TIMESTAMP", StringComparison.Ordinal))
        {
            return DateTime.TryParse(text, out var date) ? date : value;
        }

        if (dataType.Contains("NUMBER", StringComparison.Ordinal) ||
            dataType.Contains("FLOAT", StringComparison.Ordinal) ||
            dataType.Contains("INTEGER", StringComparison.Ordinal))
        {
            return decimal.TryParse(text, out var number) ? number : value;
        }

        return value;
    }

    private static async Task<IReadOnlyDictionary<string, object?>?> GetLastRowAsync(
        OracleConnection connection,
        string tableName,
        MetadataTablaDto metadata,
        CancellationToken cancellationToken)
    {
        var keyColumn = metadata.LlavePrimaria;
        var orderColumn = keyColumn ?? metadata.Columnas.First().Nombre;
        var sql = $"""
            SELECT *
            FROM (
                SELECT *
                FROM {tableName}
                ORDER BY {orderColumn} DESC
            )
            WHERE ROWNUM = 1
            """;

        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken) ? ReadRow(reader) : null;
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


