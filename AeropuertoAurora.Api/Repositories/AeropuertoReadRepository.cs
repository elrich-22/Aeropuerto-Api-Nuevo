using AeropuertoAurora.Api.Data;
using AeropuertoAurora.Api.DTOs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace AeropuertoAurora.Api.Repositories;

public sealed class AeropuertoReadRepository(IOracleConnectionFactory connectionFactory) : IAeropuertoReadRepository
{
    public Task<IReadOnlyList<AeropuertoDto>> GetAirportsAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    AER_ID AS ID,
                    AER_CODIGO_AEROPUERTO AS CODE,
                    AER_NOMBRE AS NAME,
                    AER_CIUDAD AS CITY,
                    AER_PAIS AS COUNTRY,
                    AER_ZONA_HORARIA AS TIME_ZONE,
                    AER_ESTADO AS STATUS,
                    AER_TIPO AS TYPE,
                    AER_LATITUD AS LATITUDE,
                    AER_LONGITUD AS LONGITUDE,
                    AER_CODIGO_IATA AS IATA_CODE,
                    AER_CODIGO_ICAO AS ICAO_CODE
                FROM AER_AEROPUERTO
                ORDER BY AER_NOMBRE
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, MapAirport, cancellationToken, Limit(limit));
    }

    public async Task<AeropuertoDto?> GetAirportByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                AER_ID AS ID,
                AER_CODIGO_AEROPUERTO AS CODE,
                AER_NOMBRE AS NAME,
                AER_CIUDAD AS CITY,
                AER_PAIS AS COUNTRY,
                AER_ZONA_HORARIA AS TIME_ZONE,
                AER_ESTADO AS STATUS,
                AER_TIPO AS TYPE,
                AER_LATITUD AS LATITUDE,
                AER_LONGITUD AS LONGITUDE,
                AER_CODIGO_IATA AS IATA_CODE,
                AER_CODIGO_ICAO AS ICAO_CODE
            FROM AER_AEROPUERTO
            WHERE AER_ID = :id
            """;

        return (await QueryAsync(sql, MapAirport, cancellationToken, Param("id", id))).FirstOrDefault();
    }

    public Task<IReadOnlyList<TerminalDto>> GetTerminalsAsync(int? airportId, int limit, CancellationToken cancellationToken)
    {
        var sql = """
            SELECT * FROM (
                SELECT
                    TER_ID_TERMINAL AS ID,
                    TER_ID_AEROPUERTO AS AIRPORT_ID,
                    TER_NOMBRE AS NAME,
                    TER_TIPO AS TYPE,
                    TER_CAPACIDAD_PASAJEROS AS PASSENGER_CAPACITY,
                    TER_ESTADO AS STATUS
                FROM AER_TERMINAL
                WHERE (:airportId IS NULL OR TER_ID_AEROPUERTO = :airportId)
                ORDER BY TER_NOMBRE
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new TerminalDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetInt32OrDefault("AIRPORT_ID"),
            reader.GetStringOrNull("NAME") ?? string.Empty,
            reader.GetStringOrNull("TYPE") ?? string.Empty,
            ToNullableInt(reader["PASSENGER_CAPACITY"]),
            reader.GetStringOrNull("STATUS") ?? string.Empty), cancellationToken, NullableParam("airportId", airportId), Limit(limit));
    }

    public Task<IReadOnlyList<PuertaEmbarqueDto>> GetGatesAsync(int? terminalId, int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    PUE_ID_PUERTA AS ID,
                    PUE_ID_TERMINAL AS TERMINAL_ID,
                    PUE_CODIGO AS CODE,
                    PUE_ESTADO AS STATUS,
                    PUE_TIPO AS TYPE
                FROM AER_PUERTA_EMBARQUE
                WHERE (:terminalId IS NULL OR PUE_ID_TERMINAL = :terminalId)
                ORDER BY PUE_CODIGO
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new PuertaEmbarqueDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetInt32OrDefault("TERMINAL_ID"),
            reader.GetStringOrNull("CODE") ?? string.Empty,
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetStringOrNull("TYPE") ?? string.Empty), cancellationToken, NullableParam("terminalId", terminalId), Limit(limit));
    }

    public Task<IReadOnlyList<AerolineaDto>> GetAirlinesAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    ARL_ID AS ID,
                    ARL_CODIGO_AEROLINEA AS CODE,
                    ARL_NOMBRE AS NAME,
                    ARL_PAIS_ORIGEN AS COUNTRY,
                    ARL_CODIGO_IATA AS IATA_CODE,
                    ARL_CODIGO_ICAO AS ICAO_CODE,
                    ARL_ESTADO AS STATUS,
                    ARL_TELEFONO AS PHONE,
                    ARL_EMAIL AS EMAIL,
                    ARL_SITIO_WEB AS WEBSITE
                FROM AER_AEROLINEA
                ORDER BY ARL_NOMBRE
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new AerolineaDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("CODE"),
            reader.GetStringOrNull("NAME") ?? string.Empty,
            reader.GetStringOrNull("COUNTRY"),
            reader.GetStringOrNull("IATA_CODE"),
            reader.GetStringOrNull("ICAO_CODE"),
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetStringOrNull("PHONE"),
            reader.GetStringOrNull("EMAIL"),
            reader.GetStringOrNull("WEBSITE")), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<AvionDto>> GetAircraftAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    av.AVI_ID AS ID,
                    av.AVI_MATRICULA AS REGISTRATION,
                    av.AVI_ID_MODELO AS MODEL_ID,
                    av.AVI_ID_AEROLINEA AS AIRLINE_ID,
                    av.AVI_ANIO_FABRICACION AS MANUFACTURING_YEAR,
                    av.AVI_ESTADO AS STATUS,
                    av.AVI_ULTIMA_REVISION AS LAST_REVISION,
                    av.AVI_PROXIMA_REVISION AS NEXT_REVISION,
                    av.AVI_HORAS_VUELO AS FLIGHT_HOURS
                FROM AER_AVION av
                ORDER BY av.AVI_MATRICULA
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, MapAircraft, cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<AsientoAvionDto>> GetAircraftSeatsAsync(int aircraftId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                ASA_ID_ASIENTO AS ID,
                ASA_ID_AVION AS AIRCRAFT_ID,
                ASA_CODIGO AS CODE,
                ASA_CLASE AS CLASS,
                ASA_ESTADO AS STATUS
            FROM AER_ASIENTO_AVION
            WHERE ASA_ID_AVION = :aircraftId
            ORDER BY ASA_CODIGO
            """;

        return QueryAsync(sql, reader => new AsientoAvionDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetInt32OrDefault("AIRCRAFT_ID"),
            reader.GetStringOrNull("CODE") ?? string.Empty,
            reader.GetStringOrNull("CLASS") ?? string.Empty,
            reader.GetStringOrNull("STATUS") ?? string.Empty), cancellationToken, Param("aircraftId", aircraftId));
    }

    public Task<IReadOnlyList<ProgramaVueloDto>> GetFlightProgramsAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    pv.PRV_ID AS ID,
                    pv.PRV_NUMERO_VUELO AS FLIGHT_NUMBER,
                    pv.PRV_ID_AEROLINEA AS AIRLINE_ID,
                    pv.PRV_ID_AEROPUERTO_ORIGEN AS ORIGIN_AIRPORT_ID,
                    pv.PRV_ID_AEROPUERTO_DESTINO AS DESTINATION_AIRPORT_ID,
                    pv.PRV_HORA_SALIDA_PROGRAMADA AS SCHEDULED_DEPARTURE,
                    pv.PRV_HORA_LLEGADA_PROGRAMADA AS SCHEDULED_ARRIVAL,
                    pv.PRV_DURACION_ESTIMADA AS ESTIMATED_DURATION,
                    pv.PRV_TIPO_VUELO AS FLIGHT_TYPE,
                    pv.PRV_ESTADO AS STATUS
                FROM AER_PROGRAMAVUELO pv
                ORDER BY pv.PRV_NUMERO_VUELO
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new ProgramaVueloDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("FLIGHT_NUMBER") ?? string.Empty,
            reader.GetInt32OrDefault("AIRLINE_ID"),
            reader.GetInt32OrDefault("ORIGIN_AIRPORT_ID"),
            reader.GetInt32OrDefault("DESTINATION_AIRPORT_ID"),
            reader.GetStringOrNull("SCHEDULED_DEPARTURE") ?? string.Empty,
            reader.GetStringOrNull("SCHEDULED_ARRIVAL") ?? string.Empty,
            ToNullableInt(reader["ESTIMATED_DURATION"]),
            reader.GetStringOrNull("FLIGHT_TYPE"),
            reader.GetStringOrNull("STATUS") ?? string.Empty), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<VueloDto>> GetFlightsAsync(DateTime? date, int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    vu.VUE_ID_VUELO AS ID,
                    pv.PRV_NUMERO_VUELO AS FLIGHT_NUMBER,
                    ar.ARL_NOMBRE AS AIRLINE,
                    ao.AER_NOMBRE AS ORIGIN,
                    ad.AER_NOMBRE AS DESTINATION,
                    vu.VUE_FECHA_VUELO AS FLIGHT_DATE,
                    vu.VUE_HORA_SALIDA_REAL AS ACTUAL_DEPARTURE,
                    vu.VUE_HORA_LLEGADA_REAL AS ACTUAL_ARRIVAL,
                    vu.VUE_PLAZAS_OCUPADAS AS OCCUPIED_SEATS,
                    vu.VUE_PLAZAS_VACIAS AS AVAILABLE_SEATS,
                    vu.VUE_ESTADO AS STATUS,
                    vu.VUE_RETRASO_MINUTOS AS DELAY_MINUTES,
                    av.AVI_MATRICULA AS AIRCRAFT_REGISTRATION
                FROM AER_VUELO vu
                INNER JOIN AER_PROGRAMAVUELO pv ON pv.PRV_ID = vu.VUE_ID_PROGRAMA_VUELO
                INNER JOIN AER_AEROLINEA ar ON ar.ARL_ID = pv.PRV_ID_AEROLINEA
                INNER JOIN AER_AEROPUERTO ao ON ao.AER_ID = pv.PRV_ID_AEROPUERTO_ORIGEN
                INNER JOIN AER_AEROPUERTO ad ON ad.AER_ID = pv.PRV_ID_AEROPUERTO_DESTINO
                INNER JOIN AER_AVION av ON av.AVI_ID = vu.VUE_ID_AVION
                WHERE (:flightDate IS NULL OR TRUNC(vu.VUE_FECHA_VUELO) = TRUNC(:flightDate))
                ORDER BY vu.VUE_FECHA_VUELO, pv.PRV_NUMERO_VUELO
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, MapFlight, cancellationToken, NullableDateParam("flightDate", date), Limit(limit));
    }

    public async Task<VueloDto?> GetFlightByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                vu.VUE_ID_VUELO AS ID,
                pv.PRV_NUMERO_VUELO AS FLIGHT_NUMBER,
                ar.ARL_NOMBRE AS AIRLINE,
                ao.AER_NOMBRE AS ORIGIN,
                ad.AER_NOMBRE AS DESTINATION,
                vu.VUE_FECHA_VUELO AS FLIGHT_DATE,
                vu.VUE_HORA_SALIDA_REAL AS ACTUAL_DEPARTURE,
                vu.VUE_HORA_LLEGADA_REAL AS ACTUAL_ARRIVAL,
                vu.VUE_PLAZAS_OCUPADAS AS OCCUPIED_SEATS,
                vu.VUE_PLAZAS_VACIAS AS AVAILABLE_SEATS,
                vu.VUE_ESTADO AS STATUS,
                vu.VUE_RETRASO_MINUTOS AS DELAY_MINUTES,
                av.AVI_MATRICULA AS AIRCRAFT_REGISTRATION
            FROM AER_VUELO vu
            INNER JOIN AER_PROGRAMAVUELO pv ON pv.PRV_ID = vu.VUE_ID_PROGRAMA_VUELO
            INNER JOIN AER_AEROLINEA ar ON ar.ARL_ID = pv.PRV_ID_AEROLINEA
            INNER JOIN AER_AEROPUERTO ao ON ao.AER_ID = pv.PRV_ID_AEROPUERTO_ORIGEN
            INNER JOIN AER_AEROPUERTO ad ON ad.AER_ID = pv.PRV_ID_AEROPUERTO_DESTINO
            INNER JOIN AER_AVION av ON av.AVI_ID = vu.VUE_ID_AVION
            WHERE vu.VUE_ID_VUELO = :id
            """;

        return (await QueryAsync(sql, MapFlight, cancellationToken, Param("id", id))).FirstOrDefault();
    }

    public Task<IReadOnlyList<PasajeroDto>> GetPassengersAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    PAS_ID_PASAJERO AS ID,
                    PAS_NUMERO_DOCUMENTO AS DOCUMENT_NUMBER,
                    PAS_TIPO_DOCUMENTO AS DOCUMENT_TYPE,
                    PAS_PRIMER_NOMBRE AS FIRST_NAME,
                    PAS_SEGUNDO_NOMBRE AS MIDDLE_NAME,
                    PAS_PRIMER_APELLIDO AS LAST_NAME,
                    PAS_SEGUNDO_APELLIDO AS SECOND_LAST_NAME,
                    PAS_FECHA_NACIMIENTO AS BIRTH_DATE,
                    PAS_NACIONALIDAD AS NATIONALITY,
                    PAS_SEXO AS GENDER,
                    PAS_TELEFONO AS PHONE,
                    PAS_EMAIL AS EMAIL
                FROM AER_PASAJERO
                ORDER BY PAS_PRIMER_APELLIDO, PAS_PRIMER_NOMBRE
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, MapPassenger, cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<ReservaDto>> GetReservationsAsync(int? passengerId, int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    re.RES_ID_RESERVA AS ID,
                    re.RES_CODIGO_RESERVA AS CODE,
                    re.RES_ID_VUELO AS FLIGHT_ID,
                    pv.PRV_NUMERO_VUELO AS FLIGHT_NUMBER,
                    pa.PAS_PRIMER_NOMBRE || ' ' || pa.PAS_PRIMER_APELLIDO AS PASSENGER,
                    re.RES_CLASE AS CLASS,
                    re.RES_FECHA_RESERVA AS RESERVATION_DATE,
                    re.RES_ESTADO AS STATUS,
                    re.RES_EQUIPAJE_FACTURADO AS CHECKED_BAGS,
                    re.RES_PESO_EQUIPAJE AS BAG_WEIGHT,
                    re.RES_TARIFA_PAGADA AS PAID_FARE
                FROM AER_RESERVA re
                INNER JOIN AER_PASAJERO pa ON pa.PAS_ID_PASAJERO = re.RES_ID_PASAJERO
                INNER JOIN AER_VUELO vu ON vu.VUE_ID_VUELO = re.RES_ID_VUELO
                INNER JOIN AER_PROGRAMAVUELO pv ON pv.PRV_ID = vu.VUE_ID_PROGRAMA_VUELO
                WHERE (:passengerId IS NULL OR re.RES_ID_PASAJERO = :passengerId)
                ORDER BY re.RES_FECHA_RESERVA DESC
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new ReservaDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("CODE") ?? string.Empty,
            reader.GetInt32OrDefault("FLIGHT_ID"),
            reader.GetStringOrNull("FLIGHT_NUMBER") ?? string.Empty,
            reader.GetStringOrNull("PASSENGER") ?? string.Empty,
            reader.GetStringOrNull("CLASS") ?? string.Empty,
            reader.GetDateTimeOrNull("RESERVATION_DATE") ?? DateTime.MinValue,
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetInt32OrDefault("CHECKED_BAGS"),
            ToNullableDecimal(reader["BAG_WEIGHT"]),
            reader.GetDecimalOrDefault("PAID_FARE")), cancellationToken, NullableParam("passengerId", passengerId), Limit(limit));
    }

    public Task<IReadOnlyList<EmpleadoDto>> GetEmployeesAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    em.EMP_ID_EMPLEADO AS ID,
                    em.EMP_NUMERO_EMPLEADO AS EMPLOYEE_NUMBER,
                    em.EMP_PRIMER_NOMBRE || ' ' || em.EMP_PRIMER_APELLIDO AS FULL_NAME,
                    de.DEP_NOMBRE AS DEPARTMENT,
                    pu.PUE_NOMBRE AS POSITION,
                    em.EMP_EMAIL AS EMAIL,
                    em.EMP_TELEFONO AS PHONE,
                    em.EMP_FECHA_CONTRATACION AS HIRE_DATE,
                    em.EMP_SALARIO_ACTUAL AS SALARY,
                    em.EMP_TIPO_CONTRATO AS CONTRACT_TYPE,
                    em.EMP_ESTADO AS STATUS
                FROM AER_EMPLEADO em
                INNER JOIN AER_DEPARTAMENTO de ON de.DEP_ID_DEPARTAMENTO = em.EMP_ID_DEPARTAMENTO
                INNER JOIN AER_PUESTO pu ON pu.PUE_ID_PUESTO = em.EMP_ID_PUESTO
                ORDER BY em.EMP_PRIMER_APELLIDO, em.EMP_PRIMER_NOMBRE
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new EmpleadoDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("EMPLOYEE_NUMBER") ?? string.Empty,
            reader.GetStringOrNull("FULL_NAME") ?? string.Empty,
            reader.GetStringOrNull("DEPARTMENT") ?? string.Empty,
            reader.GetStringOrNull("POSITION") ?? string.Empty,
            reader.GetStringOrNull("EMAIL"),
            reader.GetStringOrNull("PHONE"),
            reader.GetDateTimeOrNull("HIRE_DATE"),
            reader.GetDecimalOrDefault("SALARY"),
            reader.GetStringOrNull("CONTRACT_TYPE") ?? string.Empty,
            reader.GetStringOrNull("STATUS") ?? string.Empty), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<EquipajeDto>> GetBaggageAsync(int? flightId, int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    eq.EQU_ID_EQUIPAJE AS ID,
                    eq.EQU_CODIGO_BARRAS AS BARCODE,
                    pa.PAS_PRIMER_NOMBRE || ' ' || pa.PAS_PRIMER_APELLIDO AS PASSENGER,
                    eq.EQU_ID_VUELO AS FLIGHT_ID,
                    pv.PRV_NUMERO_VUELO AS FLIGHT_NUMBER,
                    eq.EQU_PESO_KG AS WEIGHT_KG,
                    eq.EQU_ESTADO AS STATUS
                FROM AER_EQUIPAJE eq
                INNER JOIN AER_PASAJERO pa ON pa.PAS_ID_PASAJERO = eq.EQU_ID_PASAJERO
                INNER JOIN AER_VUELO vu ON vu.VUE_ID_VUELO = eq.EQU_ID_VUELO
                INNER JOIN AER_PROGRAMAVUELO pv ON pv.PRV_ID = vu.VUE_ID_PROGRAMA_VUELO
                WHERE (:flightId IS NULL OR eq.EQU_ID_VUELO = :flightId)
                ORDER BY eq.EQU_FECHA_REGISTRO DESC
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new EquipajeDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("BARCODE") ?? string.Empty,
            reader.GetStringOrNull("PASSENGER") ?? string.Empty,
            reader.GetInt32OrDefault("FLIGHT_ID"),
            reader.GetStringOrNull("FLIGHT_NUMBER") ?? string.Empty,
            reader.GetDecimalOrDefault("WEIGHT_KG"),
            reader.GetStringOrNull("STATUS") ?? string.Empty), cancellationToken, NullableParam("flightId", flightId), Limit(limit));
    }

    public Task<IReadOnlyList<MantenimientoDto>> GetMaintenanceAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    ma.MAN_ID_MANTENIMIENTO AS ID,
                    av.AVI_MATRICULA AS AIRCRAFT_REGISTRATION,
                    ma.MAN_TIPO_MANTENIMIENTO AS MAINTENANCE_TYPE,
                    ma.MAN_FECHA_INICIO AS START_DATE,
                    NVL(ma.MAN_FECHA_FIN_REAL, ma.MAN_FECHA_FIN_ESTIMADA) AS END_DATE,
                    ma.MAN_COSTO_TOTAL AS COST,
                    ma.MAN_ESTADO AS STATUS,
                    DBMS_LOB.SUBSTR(ma.MAN_DESCRIPCION_TRABAJO, 500, 1) AS DESCRIPTION
                FROM AER_MANTENIMIENTOAVION ma
                INNER JOIN AER_AVION av ON av.AVI_ID = ma.MAN_ID_AVION
                ORDER BY ma.MAN_FECHA_INICIO DESC
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new MantenimientoDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("AIRCRAFT_REGISTRATION") ?? string.Empty,
            reader.GetStringOrNull("MAINTENANCE_TYPE") ?? string.Empty,
            reader.GetDateTimeOrNull("START_DATE"),
            reader.GetDateTimeOrNull("END_DATE"),
            reader.GetDecimalOrDefault("COST"),
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetStringOrNull("DESCRIPTION")), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<ControlSeguridadDto>> GetSecurityControlsAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    cs.CSE_ID_CONTROL AS ID,
                    pa.PAS_PRIMER_NOMBRE || ' ' || pa.PAS_PRIMER_APELLIDO AS PASSENGER,
                    cs.CSE_ID_VUELO AS FLIGHT_ID,
                    pv.PRV_NUMERO_VUELO AS FLIGHT_NUMBER,
                    NVL(em.EMP_PRIMER_NOMBRE || ' ' || em.EMP_PRIMER_APELLIDO, 'Automatico') AS EMPLOYEE,
                    cs.CSE_RESULTADO AS RESULT,
                    cs.CSE_OBSERVACION AS OBSERVATION
                FROM AER_CONTROL_SEGURIDAD cs
                INNER JOIN AER_PASAJERO pa ON pa.PAS_ID_PASAJERO = cs.CSE_ID_PASAJERO
                INNER JOIN AER_VUELO vu ON vu.VUE_ID_VUELO = cs.CSE_ID_VUELO
                INNER JOIN AER_PROGRAMAVUELO pv ON pv.PRV_ID = vu.VUE_ID_PROGRAMA_VUELO
                LEFT JOIN AER_EMPLEADO em ON em.EMP_ID_EMPLEADO = cs.CSE_ID_EMPLEADO
                ORDER BY cs.CSE_FECHA_HORA DESC
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new ControlSeguridadDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("PASSENGER") ?? string.Empty,
            reader.GetInt32OrDefault("FLIGHT_ID"),
            reader.GetStringOrNull("FLIGHT_NUMBER") ?? string.Empty,
            reader.GetStringOrNull("EMPLOYEE") ?? string.Empty,
            reader.GetStringOrNull("RESULT") ?? string.Empty,
            reader.GetStringOrNull("OBSERVATION")), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<IncidenteOperacionDto>> GetIncidentsAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    INC_ID_INCIDENTE AS ID,
                    INC_TIPO AS INCIDENT_TYPE,
                    INC_FECHA_HORA AS INCIDENT_DATE,
                    NVL(TO_CHAR(INC_ID_VUELO), 'Aeropuerto') AS LOCATION,
                    INC_SEVERIDAD AS SEVERITY,
                    INC_ESTADO AS STATUS,
                    INC_DESCRIPCION AS DESCRIPTION
                FROM AER_INCIDENTE
                ORDER BY INC_FECHA_HORA DESC
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new IncidenteOperacionDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("INCIDENT_TYPE") ?? string.Empty,
            reader.GetDateTimeOrNull("INCIDENT_DATE"),
            reader.GetStringOrNull("LOCATION") ?? string.Empty,
            reader.GetStringOrNull("SEVERITY") ?? string.Empty,
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetStringOrNull("DESCRIPTION")), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<ReporteVentasPorFechaDto>> GetSalesReportAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                TRUNC(v.VEN_FECHA_VENTA) AS FECHA,
                COUNT(*) AS TOTAL_VENTAS,
                SUM(v.VEN_MONTO_SUBTOTAL) AS SUBTOTAL,
                SUM(NVL(v.VEN_IMPUESTOS, 0)) AS IMPUESTOS,
                SUM(NVL(v.VEN_DESCUENTOS, 0)) AS DESCUENTOS,
                SUM(v.VEN_MONTO_TOTAL) AS MONTO_TOTAL
            FROM AER_VENTABOLETO v
            WHERE (:fechaInicio IS NULL OR TRUNC(v.VEN_FECHA_VENTA) >= TRUNC(:fechaInicio))
              AND (:fechaFin IS NULL OR TRUNC(v.VEN_FECHA_VENTA) <= TRUNC(:fechaFin))
            GROUP BY TRUNC(v.VEN_FECHA_VENTA)
            ORDER BY FECHA DESC
            """;

        return QueryAsync(sql, reader => new ReporteVentasPorFechaDto(
            reader.GetDateTimeOrNull("FECHA") ?? DateTime.MinValue,
            reader.GetInt32OrDefault("TOTAL_VENTAS"),
            reader.GetDecimalOrDefault("SUBTOTAL"),
            reader.GetDecimalOrDefault("IMPUESTOS"),
            reader.GetDecimalOrDefault("DESCUENTOS"),
            reader.GetDecimalOrDefault("MONTO_TOTAL")), cancellationToken, NullableDateParam("fechaInicio", fechaInicio), NullableDateParam("fechaFin", fechaFin));
    }

    public Task<IReadOnlyList<ReporteDestinoBuscadoDto>> GetTopDestinationsReportAsync(int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    a.AER_ID AS AEROPUERTO_ID,
                    a.AER_NOMBRE AS AEROPUERTO,
                    COUNT(DISTINCT b.BUS_ID_BUSQUEDA) AS TOTAL_BUSQUEDAS,
                    COUNT(DISTINCT c.CLI_ID_CLICK) AS TOTAL_CLICKS,
                    SUM(NVL(b.BUS_NUMERO_PASAJEROS, 0)) AS TOTAL_PASAJEROS
                FROM AER_AEROPUERTO a
                LEFT JOIN AER_BUSQUEDAVUELO b ON b.BUS_ID_AEROPUERTO_DESTINO = a.AER_ID
                LEFT JOIN AER_CLICKDESTINO c ON c.CLI_ID_AEROPUERTO_DESTINO = a.AER_ID
                GROUP BY a.AER_ID, a.AER_NOMBRE
                ORDER BY (COUNT(DISTINCT b.BUS_ID_BUSQUEDA) + COUNT(DISTINCT c.CLI_ID_CLICK)) DESC, a.AER_NOMBRE
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new ReporteDestinoBuscadoDto(
            reader.GetInt32OrDefault("AEROPUERTO_ID"),
            reader.GetStringOrNull("AEROPUERTO") ?? string.Empty,
            reader.GetInt32OrDefault("TOTAL_BUSQUEDAS"),
            reader.GetInt32OrDefault("TOTAL_CLICKS"),
            reader.GetInt32OrDefault("TOTAL_PASAJEROS")), cancellationToken, Limit(limit));
    }

    public Task<IReadOnlyList<ReporteIncidenteSeveridadDto>> GetIncidentsBySeverityReportAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                INC_SEVERIDAD AS SEVERIDAD,
                COUNT(*) AS TOTAL_INCIDENTES,
                SUM(CASE WHEN INC_ESTADO IN ('ABIERTO', 'EN_PROCESO') THEN 1 ELSE 0 END) AS ABIERTOS,
                SUM(CASE WHEN INC_ESTADO = 'CERRADO' THEN 1 ELSE 0 END) AS CERRADOS
            FROM AER_INCIDENTE
            GROUP BY INC_SEVERIDAD
            ORDER BY TOTAL_INCIDENTES DESC, INC_SEVERIDAD
            """;

        return QueryAsync(sql, reader => new ReporteIncidenteSeveridadDto(
            reader.GetStringOrNull("SEVERIDAD") ?? string.Empty,
            reader.GetInt32OrDefault("TOTAL_INCIDENTES"),
            reader.GetInt32OrDefault("ABIERTOS"),
            reader.GetInt32OrDefault("CERRADOS")), cancellationToken);
    }

    public Task<IReadOnlyList<ReporteOcupacionVueloDto>> GetFlightOccupancyReportAsync(DateTime? fechaInicio, DateTime? fechaFin, int limit, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT * FROM (
                SELECT
                    v.VUE_ID_VUELO AS VUELO_ID,
                    p.PRV_NUMERO_VUELO AS NUMERO_VUELO,
                    v.VUE_FECHA_VUELO AS FECHA_VUELO,
                    NVL(v.VUE_PLAZAS_OCUPADAS, 0) AS PLAZAS_OCUPADAS,
                    NVL(v.VUE_PLAZAS_VACIAS, 0) AS PLAZAS_DISPONIBLES,
                    CASE
                        WHEN NVL(v.VUE_PLAZAS_OCUPADAS, 0) + NVL(v.VUE_PLAZAS_VACIAS, 0) = 0 THEN 0
                        ELSE ROUND((NVL(v.VUE_PLAZAS_OCUPADAS, 0) / (NVL(v.VUE_PLAZAS_OCUPADAS, 0) + NVL(v.VUE_PLAZAS_VACIAS, 0))) * 100, 2)
                    END AS PORCENTAJE_OCUPACION,
                    v.VUE_ESTADO AS ESTADO
                FROM AER_VUELO v
                INNER JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
                WHERE (:fechaInicio IS NULL OR TRUNC(v.VUE_FECHA_VUELO) >= TRUNC(:fechaInicio))
                  AND (:fechaFin IS NULL OR TRUNC(v.VUE_FECHA_VUELO) <= TRUNC(:fechaFin))
                ORDER BY v.VUE_FECHA_VUELO DESC, PORCENTAJE_OCUPACION DESC
            ) WHERE ROWNUM <= :limit
            """;

        return QueryAsync(sql, reader => new ReporteOcupacionVueloDto(
            reader.GetInt32OrDefault("VUELO_ID"),
            reader.GetStringOrNull("NUMERO_VUELO") ?? string.Empty,
            reader.GetDateTimeOrNull("FECHA_VUELO") ?? DateTime.MinValue,
            reader.GetInt32OrDefault("PLAZAS_OCUPADAS"),
            reader.GetInt32OrDefault("PLAZAS_DISPONIBLES"),
            reader.GetDecimalOrDefault("PORCENTAJE_OCUPACION"),
            reader.GetStringOrNull("ESTADO") ?? string.Empty), cancellationToken, NullableDateParam("fechaInicio", fechaInicio), NullableDateParam("fechaFin", fechaFin), Limit(limit));
    }

    public Task<IReadOnlyList<ReporteMetodoPagoDto>> GetPaymentMethodsReportAsync(DateTime? fechaInicio, DateTime? fechaFin, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                m.MET_ID_METODO_PAGO AS METODO_PAGO_ID,
                m.MET_NOMBRE AS METODO_PAGO,
                COUNT(t.TRA_ID_TRANSACCION) AS TOTAL_TRANSACCIONES,
                SUM(NVL(t.TRA_MONTO_TOTAL, 0)) AS MONTO_TOTAL,
                AVG(NVL(t.TRA_MONTO_TOTAL, 0)) AS MONTO_PROMEDIO,
                MAX(t.TRA_ESTADO) KEEP (DENSE_RANK FIRST ORDER BY COUNT(*) DESC) AS ESTADO_PRINCIPAL
            FROM AER_METODOPAGO m
            LEFT JOIN AER_TRANSACCIONPAGO t ON t.TRA_ID_METODO_PAGO = m.MET_ID_METODO_PAGO
                AND (:fechaInicio IS NULL OR TRUNC(t.TRA_FECHA_TRANSACCION) >= TRUNC(:fechaInicio))
                AND (:fechaFin IS NULL OR TRUNC(t.TRA_FECHA_TRANSACCION) <= TRUNC(:fechaFin))
            GROUP BY m.MET_ID_METODO_PAGO, m.MET_NOMBRE
            ORDER BY MONTO_TOTAL DESC, TOTAL_TRANSACCIONES DESC, m.MET_NOMBRE
            """;

        return QueryAsync(sql, reader => new ReporteMetodoPagoDto(
            reader.GetInt32OrDefault("METODO_PAGO_ID"),
            reader.GetStringOrNull("METODO_PAGO") ?? string.Empty,
            reader.GetInt32OrDefault("TOTAL_TRANSACCIONES"),
            reader.GetDecimalOrDefault("MONTO_TOTAL"),
            reader.GetDecimalOrDefault("MONTO_PROMEDIO"),
            reader.GetStringOrNull("ESTADO_PRINCIPAL") ?? string.Empty), cancellationToken, NullableDateParam("fechaInicio", fechaInicio), NullableDateParam("fechaFin", fechaFin));
    }

    private async Task<IReadOnlyList<T>> QueryAsync<T>(
        string sql,
        Func<OracleDataReader, T> map,
        CancellationToken cancellationToken,
        params OracleParameter[] parameters)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.BindByName = true;
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);

        var results = new List<T>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(map(reader));
        }

        return results;
    }

    private static AeropuertoDto MapAirport(OracleDataReader reader)
    {
        return new AeropuertoDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("CODE") ?? string.Empty,
            reader.GetStringOrNull("NAME") ?? string.Empty,
            reader.GetStringOrNull("CITY") ?? string.Empty,
            reader.GetStringOrNull("COUNTRY") ?? string.Empty,
            reader.GetStringOrNull("TIME_ZONE"),
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetStringOrNull("TYPE"),
            ToNullableDecimal(reader["LATITUDE"]),
            ToNullableDecimal(reader["LONGITUDE"]),
            reader.GetStringOrNull("IATA_CODE"),
            reader.GetStringOrNull("ICAO_CODE"));
    }

    private static AvionDto MapAircraft(OracleDataReader reader)
    {
        return new AvionDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("REGISTRATION") ?? string.Empty,
            reader.GetInt32OrDefault("MODEL_ID"),
            reader.GetInt32OrDefault("AIRLINE_ID"),
            ToNullableInt(reader["MANUFACTURING_YEAR"]),
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetDateTimeOrNull("LAST_REVISION"),
            reader.GetDateTimeOrNull("NEXT_REVISION"),
            reader.GetInt32OrDefault("FLIGHT_HOURS"));
    }

    private static VueloDto MapFlight(OracleDataReader reader)
    {
        return new VueloDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("FLIGHT_NUMBER") ?? string.Empty,
            reader.GetStringOrNull("AIRLINE") ?? string.Empty,
            reader.GetStringOrNull("ORIGIN") ?? string.Empty,
            reader.GetStringOrNull("DESTINATION") ?? string.Empty,
            reader.GetDateTimeOrNull("FLIGHT_DATE") ?? DateTime.MinValue,
            reader.GetDateTimeOrNull("ACTUAL_DEPARTURE"),
            reader.GetDateTimeOrNull("ACTUAL_ARRIVAL"),
            reader.GetInt32OrDefault("OCCUPIED_SEATS"),
            reader.GetInt32OrDefault("AVAILABLE_SEATS"),
            reader.GetStringOrNull("STATUS") ?? string.Empty,
            reader.GetInt32OrDefault("DELAY_MINUTES"),
            reader.GetStringOrNull("AIRCRAFT_REGISTRATION") ?? string.Empty);
    }

    private static PasajeroDto MapPassenger(OracleDataReader reader)
    {
        return new PasajeroDto(
            reader.GetInt32OrDefault("ID"),
            reader.GetStringOrNull("DOCUMENT_NUMBER") ?? string.Empty,
            reader.GetStringOrNull("DOCUMENT_TYPE") ?? string.Empty,
            reader.GetStringOrNull("FIRST_NAME") ?? string.Empty,
            reader.GetStringOrNull("MIDDLE_NAME"),
            reader.GetStringOrNull("LAST_NAME") ?? string.Empty,
            reader.GetStringOrNull("SECOND_LAST_NAME"),
            reader.GetDateTimeOrNull("BIRTH_DATE"),
            reader.GetStringOrNull("NATIONALITY"),
            reader.GetStringOrNull("GENDER"),
            reader.GetStringOrNull("PHONE"),
            reader.GetStringOrNull("EMAIL"));
    }

    private static OracleParameter Limit(int limit)
    {
        return Param("limit", Math.Clamp(limit, 1, 500));
    }

    private static OracleParameter Param(string name, int value)
    {
        return new OracleParameter(name, OracleDbType.Int32) { Value = value };
    }

    private static OracleParameter NullableParam(string name, int? value)
    {
        return new OracleParameter(name, OracleDbType.Int32) { Value = value.HasValue ? value.Value : DBNull.Value };
    }

    private static OracleParameter NullableDateParam(string name, DateTime? value)
    {
        return new OracleParameter(name, OracleDbType.Date) { Value = value.HasValue ? value.Value : DBNull.Value };
    }

    private static int? ToNullableInt(object value)
    {
        return value is DBNull ? null : Convert.ToInt32(value);
    }

    private static decimal? ToNullableDecimal(object value)
    {
        return value is DBNull or OracleDecimal { IsNull: true } ? null : Convert.ToDecimal(value);
    }
}
