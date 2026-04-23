using AeropuertoAurora.Api.Configuration;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Data;

public sealed class OracleConnectionFactory(IOptions<DatabaseOptions> options) : IOracleConnectionFactory
{
    private readonly DatabaseOptions _options = options.Value;

    public async Task<OracleConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("Database:ConnectionString no esta configurada.");
        }

        var connection = new OracleConnection(_options.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.Schema))
        {
            await using var command = connection.CreateCommand();
            command.CommandText = $"ALTER SESSION SET CURRENT_SCHEMA = {_options.Schema}";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        return connection;
    }
}
