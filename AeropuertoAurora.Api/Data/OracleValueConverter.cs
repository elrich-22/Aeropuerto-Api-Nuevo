using Oracle.ManagedDataAccess.Types;

namespace AeropuertoAurora.Api.Data;

public static class OracleValueConverter
{
    public static object? Normalize(object value)
    {
        return value switch
        {
            null => null,
            DBNull => null,
            OracleDecimal oracleDecimal when oracleDecimal.IsNull => null,
            OracleDecimal oracleDecimal => oracleDecimal.Value,
            OracleDate oracleDate when oracleDate.IsNull => null,
            OracleDate oracleDate => oracleDate.Value,
            OracleTimeStamp oracleTimeStamp when oracleTimeStamp.IsNull => null,
            OracleTimeStamp oracleTimeStamp => oracleTimeStamp.Value,
            OracleString oracleString when oracleString.IsNull => null,
            OracleString oracleString => oracleString.Value,
            OracleClob oracleClob when oracleClob.IsNull => null,
            OracleClob oracleClob => oracleClob.Value,
            OracleBlob oracleBlob when oracleBlob.IsNull => null,
            OracleBlob oracleBlob => oracleBlob.Value,
            _ => value
        };
    }
}
