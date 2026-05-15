namespace AeropuertoAurora.Api.Data;

internal static class DateColumnDefaults
{
    private static readonly string[] CreationDateSuffixes =
    [
        "_FECHA_CREACION",
        "_FECHA_REGISTRO"
    ];

    private static readonly HashSet<string> CreationDateColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "VEN_FECHA_VENTA",
        "RES_FECHA_RESERVA",
        "BUS_FECHA_BUSQUEDA",
        "CLI_FECHA_CLICK",
        "SES_FECHA_INICIO",
        "OBJ_FECHA_REPORTE"
    };

    public static bool ShouldUseCurrentTimestamp(string columnName)
    {
        if (CreationDateColumns.Contains(columnName))
        {
            return true;
        }

        return CreationDateSuffixes.Any(suffix => columnName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }
}
