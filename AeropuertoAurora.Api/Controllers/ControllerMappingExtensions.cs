namespace AeropuertoAurora.Api.Controllers;

internal static class ControllerMappingExtensions
{
    public static int ToInt(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return Convert.ToInt32(row[column]);
    }

    public static int? ToNullableInt(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return row[column] is null ? null : Convert.ToInt32(row[column]);
    }

    public static decimal? ToNullableDecimal(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return row[column] is null ? null : Convert.ToDecimal(row[column]);
    }

    public static string? ToNullableString(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return row[column]?.ToString();
    }

    public static string ToStringValue(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return row[column]?.ToString() ?? string.Empty;
    }

    public static DateTime? ToNullableDateTime(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return row[column] is null ? null : Convert.ToDateTime(row[column]);
    }

    public static DateTime ToDateTimeValue(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return Convert.ToDateTime(row[column]);
    }

    public static byte[]? ToNullableBytes(this IReadOnlyDictionary<string, object?> row, string column)
    {
        return row[column] as byte[];
    }
}
