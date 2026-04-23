using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Data;

public static class OracleDataReaderExtensions
{
    public static int GetInt32OrDefault(this OracleDataReader reader, string column)
    {
        var value = reader[column];
        return value is DBNull ? 0 : Convert.ToInt32(value);
    }

    public static decimal GetDecimalOrDefault(this OracleDataReader reader, string column)
    {
        var value = reader[column];
        return value is DBNull ? 0m : Convert.ToDecimal(value);
    }

    public static string? GetStringOrNull(this OracleDataReader reader, string column)
    {
        var value = reader[column];
        return value is DBNull ? null : Convert.ToString(value);
    }

    public static DateTime? GetDateTimeOrNull(this OracleDataReader reader, string column)
    {
        var value = reader[column];
        return value is DBNull ? null : Convert.ToDateTime(value);
    }
}
