using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Data;

public interface IOracleConnectionFactory
{
    Task<OracleConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken);
}
