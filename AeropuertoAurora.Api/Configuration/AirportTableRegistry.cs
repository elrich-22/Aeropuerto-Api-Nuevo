namespace AeropuertoAurora.Api.Configuration;

public static class AirportTableRegistry
{
    private static readonly string[] TableNames =
    [
        "AER_AEROPUERTO",
        "AER_TERMINAL",
        "AER_PUERTA_EMBARQUE",
        "AER_AEROLINEA",
        "AER_MODELOAVION",
        "AER_AVION",
        "AER_ASIENTO_AVION",
        "AER_PROGRAMAVUELO",
        "AER_DIASVUELO",
        "AER_ESCALATECNICA",
        "AER_METODOPAGO",
        "AER_PROMOCION",
        "AER_DEPARTAMENTO",
        "AER_PUESTO",
        "AER_EMPLEADO",
        "AER_LICENCIAEMPLEADO",
        "AER_ASISTENCIA",
        "AER_PLANILLA",
        "AER_PASAJERO",
        "AER_DOCUMENTO_PASAJERO",
        "AER_USUARIO_LOGIN",
        "AER_SESIONUSUARIO",
        "AER_PREFERENCIACLIENTE",
        "AER_VUELO",
        "AER_TRIPULACION",
        "AER_ASIGNACION_PUERTA",
        "AER_ASIGNACION_ASIENTO",
        "AER_RESERVA",
        "AER_CARRITOCOMPRA",
        "AER_ITEMCARRITO",
        "AER_BUSQUEDAVUELO",
        "AER_CLICKDESTINO",
        "AER_PUNTOVENTA",
        "AER_VENTABOLETO",
        "AER_DETALLEVENTABOLETO",
        "AER_TRANSACCIONPAGO",
        "AER_USOPROMOCION",
        "AER_CHECKIN",
        "AER_TARJETA_EMBARQUE",
        "AER_EQUIPAJE",
        "AER_MOVIMIENTO_EQUIPAJE",
        "AER_CONTROL_SEGURIDAD",
        "AER_CONTROL_MIGRATORIO",
        "AER_HANGAR",
        "AER_ASIGNACIONHANGAR",
        "AER_CATEGORIAREPUESTO",
        "AER_REPUESTO",
        "AER_PROVEEDOR",
        "AER_ORDENCOMPRAREPUESTO",
        "AER_DETALLEORDENCOMPRA",
        "AER_MOVIMIENTOREPUESTO",
        "AER_MANTENIMIENTOAVION",
        "AER_REPUESTOUTILIZADO",
        "AER_INCIDENTE",
        "AER_OBJETOPERDIDO",
        "AER_ARRESTO",
        "AER_AUDITORIA"
    ];

    private static readonly IReadOnlyDictionary<string, string> TablesByAlias = BuildAliases();

    public static IReadOnlyList<string> GetTableNames()
    {
        return TableNames;
    }

    public static bool TryResolve(string tableOrAlias, out string tableName)
    {
        return TablesByAlias.TryGetValue(Normalize(tableOrAlias), out tableName!);
    }

    private static IReadOnlyDictionary<string, string> BuildAliases()
    {
        var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var table in TableNames)
        {
            aliases[Normalize(table)] = table;
            aliases[Normalize(table.Replace("AER_", string.Empty, StringComparison.OrdinalIgnoreCase))] = table;
        }

        return aliases;
    }

    private static string Normalize(string value)
    {
        return value.Trim().Replace("-", "_", StringComparison.Ordinal).ToUpperInvariant();
    }
}
