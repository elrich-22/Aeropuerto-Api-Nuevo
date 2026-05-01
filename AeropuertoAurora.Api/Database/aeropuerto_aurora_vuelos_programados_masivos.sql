--------------------------------------------------------
-- AEROPUERTO AURORA - SEED MASIVA DE VUELOS PROGRAMADOS
-- Motor: Oracle 21c
--
-- Ejecutar despues de:
--   1) aeropuerto_aurora_v2.sql
--   2) aeropuerto_aurora_seed_wow.sql
--
-- No modifica la seed original. Inserta rutas extra, dias de operacion,
-- aeropuertos globales, escalas tecnicas, puertas y miles de vuelos con estado PROGRAMADO para 2026.
-- Nota: AER_VUELO no tiene columna de precio; por eso la tarifa base se
-- conserva como referencia dentro de la matriz de rutas para futuras ventas.
--------------------------------------------------------

SET DEFINE OFF;
ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD';
ALTER SESSION SET NLS_TIMESTAMP_FORMAT = 'YYYY-MM-DD HH24:MI:SS';
ALTER SESSION SET CURRENT_SCHEMA = AEROPUERTO_AURORA;

DECLARE
  v_programa_id NUMBER;
  v_vuelo_id    NUMBER;
  v_fecha       DATE;
  v_salida      TIMESTAMP;
  v_avion       NUMBER;
  v_puerta      NUMBER;
  v_dia_semana  NUMBER;
  v_air_yyz     NUMBER;
  v_air_ord     NUMBER;
  v_air_lhr     NUMBER;
  v_air_cdg     NUMBER;
  v_air_ams     NUMBER;
  v_air_fra     NUMBER;
  v_air_fco     NUMBER;
  v_air_gru     NUMBER;
  v_air_eze     NUMBER;
  v_air_scl     NUMBER;
  v_air_uio     NUMBER;
  v_air_sdq     NUMBER;
  v_air_hav     NUMBER;
  v_air_sju     NUMBER;
  v_air_nrt     NUMBER;
  v_air_icn     NUMBER;
  v_air_dxb     NUMBER;
  v_air_doh     NUMBER;
  v_air_syd     NUMBER;
  v_air_akl     NUMBER;

  FUNCTION avion_por_aerolinea(p_aerolinea NUMBER, p_vuelo NUMBER) RETURN NUMBER IS
  BEGIN
    RETURN CASE p_aerolinea
      WHEN 1 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 18 ELSE 1 END
      WHEN 2 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 12 ELSE 2 END
      WHEN 3 THEN 3
      WHEN 4 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 14 ELSE 4 END
      WHEN 5 THEN 5
      WHEN 6 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 15 ELSE 6 END
      WHEN 7 THEN 7
      WHEN 8 THEN 8
      WHEN 9 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 16 ELSE 9 END
      WHEN 10 THEN 10
      WHEN 11 THEN 11
      WHEN 12 THEN 17
      ELSE 1
    END;
  END;

  PROCEDURE asegurar_aeropuerto(
    p_codigo       VARCHAR2,
    p_nombre       VARCHAR2,
    p_ciudad       VARCHAR2,
    p_pais         VARCHAR2,
    p_zona         VARCHAR2,
    p_tipo         VARCHAR2,
    p_latitud      NUMBER,
    p_longitud     NUMBER,
    p_iata         VARCHAR2,
    p_icao         VARCHAR2,
    p_id       OUT NUMBER
  ) IS
  BEGIN
    SELECT AER_ID
    INTO p_id
    FROM AER_AEROPUERTO
    WHERE AER_CODIGO_IATA = p_iata;
  EXCEPTION
    WHEN NO_DATA_FOUND THEN
      INSERT INTO AER_AEROPUERTO
      (AER_CODIGO_AEROPUERTO, AER_NOMBRE, AER_CIUDAD, AER_PAIS, AER_ZONA_HORARIA,
       AER_ESTADO, AER_TIPO, AER_LATITUD, AER_LONGITUD, AER_CODIGO_IATA, AER_CODIGO_ICAO, AER_FECHA_REGISTRO)
      VALUES
      (p_codigo, p_nombre, p_ciudad, p_pais, p_zona, 'ACTIVO', p_tipo,
       p_latitud, p_longitud, p_iata, p_icao, DATE '2026-01-15')
      RETURNING AER_ID INTO p_id;
  END;

  PROCEDURE crear_ruta(
    p_numero        VARCHAR2,
    p_aerolinea     NUMBER,
    p_origen        NUMBER,
    p_destino       NUMBER,
    p_salida        VARCHAR2,
    p_llegada       VARCHAR2,
    p_duracion      NUMBER,
    p_tipo          VARCHAR2,
    p_dias          VARCHAR2,
    p_precio_base   NUMBER,
    p_escala_aer    NUMBER DEFAULT NULL,
    p_escala_llega  VARCHAR2 DEFAULT NULL,
    p_escala_sale   VARCHAR2 DEFAULT NULL,
    p_escala_min    NUMBER DEFAULT NULL
  ) IS
  BEGIN
    INSERT INTO AER_PROGRAMAVUELO
    (PRV_NUMERO_VUELO, PRV_ID_AEROLINEA, PRV_ID_AEROPUERTO_ORIGEN, PRV_ID_AEROPUERTO_DESTINO,
     PRV_HORA_SALIDA_PROGRAMADA, PRV_HORA_LLEGADA_PROGRAMADA, PRV_DURACION_ESTIMADA,
     PRV_TIPO_VUELO, PRV_ESTADO, PRV_FECHA_CREACION)
    VALUES
    (p_numero, p_aerolinea, p_origen, p_destino, p_salida, p_llegada, p_duracion,
     p_tipo, 'ACTIVO', DATE '2026-01-15')
    RETURNING PRV_ID INTO v_programa_id;

    FOR d IN 1..7 LOOP
      IF INSTR(p_dias, TO_CHAR(d)) > 0 THEN
        INSERT INTO AER_DIASVUELO
        (DIA_ID_PROGRAMA_VUELO, DIA_DIA_SEMANA)
        VALUES (v_programa_id, d);
      END IF;
    END LOOP;

    IF p_escala_aer IS NOT NULL THEN
      INSERT INTO AER_ESCALATECNICA
      (ESC_ID_PROGRAMA_VUELO, ESC_ID_AEROPUERTO, ESC_NUMERO_ORDEN,
       ESC_HORA_LLEGADA_ESTIMADA, ESC_HORA_SALIDA_ESTIMADA, ESC_DURACION_ESCALA)
      VALUES
      (v_programa_id, p_escala_aer, 1, p_escala_llega, p_escala_sale, p_escala_min);
    END IF;

    v_fecha := DATE '2026-01-01';
    WHILE v_fecha <= DATE '2026-12-31' LOOP
      v_dia_semana := TRUNC(v_fecha) - TRUNC(v_fecha, 'IW') + 1;
      IF INSTR(p_dias, TO_CHAR(v_dia_semana)) > 0 THEN
        v_avion := avion_por_aerolinea(p_aerolinea, TO_NUMBER(TO_CHAR(v_fecha, 'DDD')));

        INSERT INTO AER_VUELO
        (VUE_ID_PROGRAMA_VUELO, VUE_ID_AVION, VUE_FECHA_VUELO, VUE_HORA_SALIDA_REAL, VUE_HORA_LLEGADA_REAL,
         VUE_PLAZAS_OCUPADAS, VUE_PLAZAS_VACIAS, VUE_ESTADO, VUE_FECHA_REPROGRAMACION, VUE_MOTIVO_CANCELACION, VUE_RETRASO_MINUTOS)
        VALUES
        (v_programa_id, v_avion, v_fecha,
         NULL, NULL, 0, 156, 'PROGRAMADO', NULL, NULL, 0)
        RETURNING VUE_ID_VUELO INTO v_vuelo_id;

        v_salida := TO_TIMESTAMP(TO_CHAR(v_fecha, 'YYYY-MM-DD') || ' ' || p_salida, 'YYYY-MM-DD HH24:MI');
        v_puerta := CASE WHEN p_tipo IN ('nacional','regional') THEN 100 + MOD(v_vuelo_id, 6) + 1 ELSE MOD(v_vuelo_id, 14) + 1 END;

        INSERT INTO AER_ASIGNACION_PUERTA
        (ASP_ID_VUELO, ASP_ID_PUERTA, ASP_FECHA_HORA_INICIO, ASP_FECHA_HORA_FIN, ASP_ESTADO)
        VALUES
        (v_vuelo_id, v_puerta, v_salida - NUMTODSINTERVAL(90, 'MINUTE'), NULL, 'PROGRAMADA');

      END IF;

      v_fecha := v_fecha + 1;
    END LOOP;
  END;
BEGIN
  DELETE FROM AER_TRIPULACION
  WHERE TRI_ID_VUELO IN (
    SELECT v.VUE_ID_VUELO
    FROM AER_VUELO v
    JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
    WHERE p.PRV_NUMERO_VUELO LIKE '%X'
  );

  DELETE FROM AER_ASIGNACION_PUERTA
  WHERE ASP_ID_VUELO IN (
    SELECT v.VUE_ID_VUELO
    FROM AER_VUELO v
    JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
    WHERE p.PRV_NUMERO_VUELO LIKE '%X'
  );

  DELETE FROM AER_VUELO
  WHERE VUE_ID_PROGRAMA_VUELO IN (
    SELECT PRV_ID FROM AER_PROGRAMAVUELO WHERE PRV_NUMERO_VUELO LIKE '%X'
  );

  DELETE FROM AER_ESCALATECNICA
  WHERE ESC_ID_PROGRAMA_VUELO IN (
    SELECT PRV_ID FROM AER_PROGRAMAVUELO WHERE PRV_NUMERO_VUELO LIKE '%X'
  );

  DELETE FROM AER_DIASVUELO
  WHERE DIA_ID_PROGRAMA_VUELO IN (
    SELECT PRV_ID FROM AER_PROGRAMAVUELO WHERE PRV_NUMERO_VUELO LIKE '%X'
  );

  DELETE FROM AER_PROGRAMAVUELO
  WHERE PRV_NUMERO_VUELO LIKE '%X';

  asegurar_aeropuerto('YYZ', 'Toronto Pearson International Airport', 'Toronto', 'Canada', 'America/Toronto', 'hub', 43.677200, -79.630600, 'YYZ', 'CYYZ', v_air_yyz);
  asegurar_aeropuerto('ORD', 'Chicago O Hare International Airport', 'Chicago', 'Estados Unidos', 'America/Chicago', 'hub', 41.974200, -87.907300, 'ORD', 'KORD', v_air_ord);
  asegurar_aeropuerto('LHR', 'London Heathrow Airport', 'Londres', 'Reino Unido', 'Europe/London', 'intercontinental', 51.470000, -0.454300, 'LHR', 'EGLL', v_air_lhr);
  asegurar_aeropuerto('CDG', 'Paris Charles de Gaulle Airport', 'Paris', 'Francia', 'Europe/Paris', 'intercontinental', 49.009700, 2.547900, 'CDG', 'LFPG', v_air_cdg);
  asegurar_aeropuerto('AMS', 'Amsterdam Airport Schiphol', 'Amsterdam', 'Paises Bajos', 'Europe/Amsterdam', 'hub', 52.310500, 4.768300, 'AMS', 'EHAM', v_air_ams);
  asegurar_aeropuerto('FRA', 'Frankfurt Airport', 'Frankfurt', 'Alemania', 'Europe/Berlin', 'hub', 50.037900, 8.562200, 'FRA', 'EDDF', v_air_fra);
  asegurar_aeropuerto('FCO', 'Rome Fiumicino Leonardo da Vinci Airport', 'Roma', 'Italia', 'Europe/Rome', 'intercontinental', 41.800300, 12.238900, 'FCO', 'LIRF', v_air_fco);
  asegurar_aeropuerto('GRU', 'Sao Paulo Guarulhos International Airport', 'Sao Paulo', 'Brasil', 'America/Sao_Paulo', 'hub', -23.435600, -46.473100, 'GRU', 'SBGR', v_air_gru);
  asegurar_aeropuerto('EZE', 'Aeropuerto Internacional Ministro Pistarini', 'Buenos Aires', 'Argentina', 'America/Argentina/Buenos_Aires', 'intercontinental', -34.822200, -58.535800, 'EZE', 'SAEZ', v_air_eze);
  asegurar_aeropuerto('SCL', 'Aeropuerto Internacional Arturo Merino Benitez', 'Santiago', 'Chile', 'America/Santiago', 'hub', -33.392800, -70.785800, 'SCL', 'SCEL', v_air_scl);
  asegurar_aeropuerto('UIO', 'Aeropuerto Internacional Mariscal Sucre', 'Quito', 'Ecuador', 'America/Guayaquil', 'internacional', -0.129200, -78.357500, 'UIO', 'SEQM', v_air_uio);
  asegurar_aeropuerto('SDQ', 'Aeropuerto Internacional Las Americas', 'Santo Domingo', 'Republica Dominicana', 'America/Santo_Domingo', 'turistico', 18.429700, -69.668900, 'SDQ', 'MDSD', v_air_sdq);
  asegurar_aeropuerto('HAV', 'Aeropuerto Internacional Jose Marti', 'La Habana', 'Cuba', 'America/Havana', 'turistico', 22.989200, -82.409100, 'HAV', 'MUHA', v_air_hav);
  asegurar_aeropuerto('SJU', 'Luis Munoz Marin International Airport', 'San Juan', 'Puerto Rico', 'America/Puerto_Rico', 'turistico', 18.439400, -66.001800, 'SJU', 'TJSJ', v_air_sju);
  asegurar_aeropuerto('NRT', 'Narita International Airport', 'Tokio', 'Japon', 'Asia/Tokyo', 'intercontinental', 35.772000, 140.392900, 'NRT', 'RJAA', v_air_nrt);
  asegurar_aeropuerto('ICN', 'Incheon International Airport', 'Seul', 'Corea del Sur', 'Asia/Seoul', 'intercontinental', 37.460200, 126.440700, 'ICN', 'RKSI', v_air_icn);
  asegurar_aeropuerto('DXB', 'Dubai International Airport', 'Dubai', 'Emiratos Arabes Unidos', 'Asia/Dubai', 'hub', 25.253200, 55.365700, 'DXB', 'OMDB', v_air_dxb);
  asegurar_aeropuerto('DOH', 'Hamad International Airport', 'Doha', 'Qatar', 'Asia/Qatar', 'hub', 25.273100, 51.608100, 'DOH', 'OTHH', v_air_doh);
  asegurar_aeropuerto('SYD', 'Sydney Kingsford Smith Airport', 'Sydney', 'Australia', 'Australia/Sydney', 'intercontinental', -33.939900, 151.175300, 'SYD', 'YSSY', v_air_syd);
  asegurar_aeropuerto('AKL', 'Auckland Airport', 'Auckland', 'Nueva Zelanda', 'Pacific/Auckland', 'intercontinental', -37.008200, 174.785000, 'AKL', 'NZAA', v_air_akl);

  crear_ruta('AV641X', 1, 9, 1, '13:20', '14:05', 165, 'internacional', '1234567', 1380);
  crear_ruta('CM392X', 2, 6, 1, '18:30', '19:54', 144, 'internacional', '1234567', 1125);
  crear_ruta('AA1189X', 3, 9, 1, '19:10', '20:05', 175, 'internacional', '1234567', 1460);
  crear_ruta('UA1902X', 4, 10, 1, '09:10', '11:02', 172, 'internacional', '1234567', 1510);
  crear_ruta('DL1831X', 5, 16, 1, '20:20', '21:18', 178, 'internacional', '1234567', 1585);
  crear_ruta('AM678X', 6, 7, 1, '11:35', '13:45', 130, 'internacional', '1234567', 980);
  crear_ruta('IB6341X', 7, 15, 1, '12:20', '16:20', 720, 'intercontinental', '1357', 6240, 9, '15:10', '16:05', 55);
  crear_ruta('Y43930X', 8, 8, 1, '13:05', '13:57', 112, 'turistico', '1234567', 870);
  crear_ruta('5U111X', 9, 2, 1, '08:05', '09:05', 60, 'nacional', '1234567', 520);
  crear_ruta('5U212X', 9, 3, 1, '09:45', '10:40', 55, 'regional', '1234567', 455);
  crear_ruta('NK516X', 10, 11, 1, '17:40', '22:42', 182, 'internacional', '1234567', 1180);
  crear_ruta('B62030X', 11, 12, 1, '07:25', '10:28', 243, 'internacional', '1234567', 1725, 9, '08:55', '09:40', 45);
  crear_ruta('D0255X', 12, 9, 1, '04:15', '05:58', 163, 'carga', '1357', 0, 10, '04:55', '05:35', 40);
  crear_ruta('AV744X', 1, 13, 1, '20:15', '23:30', 195, 'internacional', '1234567', 1325);
  crear_ruta('CM408X', 2, 5, 1, '13:10', '15:05', 115, 'regional', '1234567', 760);
  crear_ruta('AM670X', 6, 7, 1, '21:45', '23:55', 130, 'internacional', '1234567', 1015);
  crear_ruta('AV551X', 1, 1, 3, '06:05', '06:55', 50, 'regional', '1234567', 430);
  crear_ruta('AV552X', 1, 3, 1, '07:40', '08:30', 50, 'regional', '1234567', 430);
  crear_ruta('CM822X', 2, 1, 4, '06:45', '08:10', 85, 'regional', '1234567', 610);
  crear_ruta('CM823X', 2, 4, 1, '09:05', '10:30', 85, 'regional', '1234567', 610);
  crear_ruta('CM701X', 2, 1, 6, '12:25', '14:50', 145, 'internacional', '1234567', 1080);
  crear_ruta('CM702X', 2, 6, 1, '16:05', '17:30', 145, 'internacional', '1234567', 1080);
  crear_ruta('AM901X', 6, 1, 7, '06:30', '08:45', 135, 'internacional', '1234567', 960);
  crear_ruta('AM902X', 6, 7, 1, '14:15', '16:25', 130, 'internacional', '1234567', 960);
  crear_ruta('Y45021X', 8, 1, 8, '05:55', '08:45', 170, 'turistico', '1234567', 820);
  crear_ruta('Y45022X', 8, 8, 1, '15:20', '16:10', 110, 'turistico', '1234567', 820);
  crear_ruta('AA882X', 3, 1, 9, '08:25', '13:00', 155, 'internacional', '1234567', 1395);
  crear_ruta('AA883X', 3, 9, 1, '14:35', '15:35', 180, 'internacional', '1234567', 1395);
  crear_ruta('UA1563X', 4, 1, 10, '13:50', '17:35', 165, 'internacional', '1234567', 1440);
  crear_ruta('UA1564X', 4, 10, 1, '18:55', '20:45', 170, 'internacional', '1234567', 1440);
  crear_ruta('DL1845X', 5, 1, 16, '07:05', '11:45', 160, 'internacional', '1234567', 1520);
  crear_ruta('DL1846X', 5, 16, 1, '12:55', '14:00', 185, 'internacional', '1234567', 1520);
  crear_ruta('B62131X', 11, 1, 12, '10:40', '16:50', 250, 'internacional', '1234567', 1775, 9, '13:15', '14:00', 45);
  crear_ruta('B62132X', 11, 12, 1, '18:25', '21:35', 250, 'internacional', '1234567', 1775, 9, '20:15', '21:00', 45);
  crear_ruta('AV129X', 1, 1, 13, '05:35', '08:55', 200, 'internacional', '1234567', 1280);
  crear_ruta('AV130X', 1, 13, 1, '10:15', '13:35', 200, 'internacional', '1234567', 1280);
  crear_ruta('AV751X', 1, 1, 14, '11:15', '15:05', 230, 'internacional', '1234567', 1610, 13, '13:05', '13:50', 45);
  crear_ruta('AV752X', 1, 14, 1, '16:20', '20:05', 225, 'internacional', '1234567', 1610, 13, '18:10', '18:55', 45);
  crear_ruta('IB221X', 7, 1, 15, '21:25', '17:45', 680, 'intercontinental', '1357', 6180, 9, '00:10', '01:10', 60);
  crear_ruta('IB222X', 7, 15, 1, '23:10', '03:15', 725, 'intercontinental', '1357', 6180, 9, '01:20', '02:15', 55);
  crear_ruta('5U330X', 9, 1, 2, '12:10', '13:10', 60, 'nacional', '1234567', 515);
  crear_ruta('5U331X', 9, 2, 1, '14:00', '15:00', 60, 'nacional', '1234567', 515);
  crear_ruta('5U401X', 9, 1, 3, '17:45', '18:40', 55, 'regional', '1234567', 450);
  crear_ruta('5U402X', 9, 3, 1, '19:20', '20:15', 55, 'regional', '1234567', 450);
  crear_ruta('NK631X', 10, 1, 11, '02:10', '07:05', 175, 'internacional', '1234567', 1160);
  crear_ruta('NK632X', 10, 11, 1, '08:20', '13:25', 185, 'internacional', '1234567', 1160);
  crear_ruta('D0310X', 12, 1, 9, '01:20', '05:55', 155, 'carga', '1357', 0);
  crear_ruta('D0311X', 12, 9, 1, '23:05', '00:45', 160, 'carga', '1357', 0);
  crear_ruta('AV804X', 1, 1, 5, '14:45', '16:35', 110, 'regional', '1234567', 710);
  crear_ruta('AV805X', 1, 5, 1, '17:25', '19:15', 110, 'regional', '1234567', 710);
  crear_ruta('CM905X', 2, 1, 6, '20:30', '22:55', 145, 'internacional', '1234567', 1095);
  crear_ruta('CM906X', 2, 6, 1, '23:45', '01:10', 145, 'internacional', '1234567', 1095);
  crear_ruta('AM377X', 6, 1, 7, '12:40', '14:55', 135, 'internacional', '1234567', 995);
  crear_ruta('AM378X', 6, 7, 1, '18:05', '20:15', 130, 'internacional', '1234567', 995);
  crear_ruta('AA1770X', 3, 1, 9, '17:05', '21:45', 160, 'internacional', '1234567', 1415);
  crear_ruta('AA1771X', 3, 9, 1, '06:30', '07:25', 175, 'internacional', '1234567', 1415);
  crear_ruta('UA2230X', 4, 1, 10, '07:35', '11:20', 165, 'internacional', '1234567', 1435);
  crear_ruta('UA2231X', 4, 10, 1, '12:45', '14:35', 170, 'internacional', '1234567', 1435);
  crear_ruta('DL2040X', 5, 1, 16, '18:15', '22:55', 160, 'internacional', '1234567', 1535);
  crear_ruta('DL2041X', 5, 16, 1, '06:15', '07:20', 185, 'internacional', '1234567', 1535);
  crear_ruta('IB7788X', 7, 1, 15, '16:40', '13:00', 680, 'intercontinental', '1357', 6295, 13, '20:15', '21:25', 70);
  crear_ruta('IB7789X', 7, 15, 1, '15:30', '19:35', 725, 'intercontinental', '1357', 6295, 9, '17:45', '18:45', 60);
  crear_ruta('Y4845X', 8, 1, 8, '20:50', '23:40', 170, 'turistico', '1234567', 845);
  crear_ruta('Y4846X', 8, 8, 1, '06:10', '07:00', 110, 'turistico', '1234567', 845);
  crear_ruta('UA3100X', 4, 1, v_air_ord, '08:40', '13:45', 245, 'internacional', '1234567', 1740, 10, '11:05', '11:55', 50);
  crear_ruta('UA3101X', 4, v_air_ord, 1, '15:30', '20:45', 255, 'internacional', '1234567', 1740, 10, '17:50', '18:40', 50);
  crear_ruta('AV2200X', 1, 1, v_air_yyz, '09:15', '16:25', 370, 'internacional', '1357', 2210, 9, '12:20', '13:20', 60);
  crear_ruta('AV2201X', 1, v_air_yyz, 1, '18:05', '23:45', 340, 'internacional', '1357', 2210, 9, '20:25', '21:15', 50);
  crear_ruta('IB7000X', 7, 1, v_air_lhr, '18:20', '14:10', 650, 'intercontinental', '246', 6450, 15, '10:20', '11:35', 75);
  crear_ruta('IB7001X', 7, v_air_lhr, 1, '16:35', '21:05', 690, 'intercontinental', '246', 6450, 15, '19:55', '21:00', 65);
  crear_ruta('IB7100X', 7, 1, v_air_cdg, '19:10', '15:25', 675, 'intercontinental', '135', 6320, 15, '10:55', '12:00', 65);
  crear_ruta('IB7101X', 7, v_air_cdg, 1, '17:40', '22:15', 695, 'intercontinental', '135', 6320, 15, '20:45', '21:50', 65);
  crear_ruta('IB7200X', 7, 1, v_air_ams, '20:35', '16:40', 665, 'intercontinental', '257', 6410, 15, '11:40', '12:55', 75);
  crear_ruta('IB7201X', 7, v_air_ams, 1, '18:05', '22:45', 700, 'intercontinental', '257', 6410, 15, '21:15', '22:20', 65);
  crear_ruta('IB7300X', 7, 1, v_air_fra, '17:55', '14:30', 675, 'intercontinental', '146', 6485, 15, '10:15', '11:30', 75);
  crear_ruta('IB7301X', 7, v_air_fra, 1, '15:50', '20:35', 705, 'intercontinental', '146', 6485, 15, '19:05', '20:10', 65);
  crear_ruta('IB7400X', 7, 1, v_air_fco, '21:15', '18:35', 725, 'intercontinental', '36', 6680, 15, '12:20', '13:35', 75);
  crear_ruta('IB7401X', 7, v_air_fco, 1, '19:30', '00:20', 710, 'intercontinental', '47', 6680, 15, '22:10', '23:20', 70);
  crear_ruta('AV3300X', 1, 1, v_air_gru, '06:50', '16:20', 510, 'intercontinental', '1234567', 3320, 13, '10:05', '11:05', 60);
  crear_ruta('AV3301X', 1, v_air_gru, 1, '18:00', '01:30', 510, 'intercontinental', '1234567', 3320, 13, '21:00', '22:00', 60);
  crear_ruta('AV3400X', 1, 1, v_air_eze, '07:25', '18:05', 580, 'intercontinental', '1357', 3740, 13, '10:55', '12:05', 70);
  crear_ruta('AV3401X', 1, v_air_eze, 1, '20:10', '04:40', 570, 'intercontinental', '1357', 3740, 13, '23:15', '00:20', 65);
  crear_ruta('AV3500X', 1, 1, v_air_scl, '08:05', '17:55', 530, 'intercontinental', '246', 3485, 14, '12:05', '13:10', 65);
  crear_ruta('AV3501X', 1, v_air_scl, 1, '19:25', '03:10', 525, 'intercontinental', '246', 3485, 14, '22:25', '23:30', 65);
  crear_ruta('AV3600X', 1, 1, v_air_uio, '10:35', '15:05', 270, 'internacional', '1234567', 1680, 13, '12:35', '13:20', 45);
  crear_ruta('AV3601X', 1, v_air_uio, 1, '16:15', '20:35', 260, 'internacional', '1234567', 1680, 13, '18:05', '18:50', 45);
  crear_ruta('B65000X', 11, 1, v_air_sdq, '07:10', '13:30', 320, 'turistico', '1234567', 1540, 9, '09:35', '10:20', 45);
  crear_ruta('B65001X', 11, v_air_sdq, 1, '15:20', '19:55', 275, 'turistico', '1234567', 1540, 9, '17:05', '17:50', 45);
  crear_ruta('Y45100X', 8, 1, v_air_hav, '06:20', '11:15', 295, 'turistico', '246', 1360, 8, '08:40', '09:25', 45);
  crear_ruta('Y45101X', 8, v_air_hav, 1, '12:45', '16:40', 235, 'turistico', '246', 1360, 8, '14:05', '14:50', 45);
  crear_ruta('B65200X', 11, 1, v_air_sju, '11:50', '18:10', 320, 'turistico', '1357', 1620, 9, '14:15', '15:05', 50);
  crear_ruta('B65201X', 11, v_air_sju, 1, '19:30', '00:10', 280, 'turistico', '1357', 1620, 9, '21:15', '22:05', 50);
  crear_ruta('CM8800X', 2, 1, v_air_dxb, '22:10', '22:55', 1185, 'intercontinental', '26', 8450, 6, '00:35', '02:00', 85);
  crear_ruta('CM8801X', 2, v_air_dxb, 1, '01:20', '09:40', 1220, 'intercontinental', '37', 8450, 6, '05:55', '07:20', 85);
  crear_ruta('CM8810X', 2, 1, v_air_doh, '21:40', '21:35', 1175, 'intercontinental', '15', 8325, 6, '00:05', '01:30', 85);
  crear_ruta('CM8811X', 2, v_air_doh, 1, '02:10', '10:20', 1210, 'intercontinental', '26', 8325, 6, '06:30', '07:55', 85);
  crear_ruta('UA9000X', 4, 1, v_air_nrt, '05:45', '14:40', 1040, 'intercontinental', '147', 9120, 11, '10:40', '12:10', 90);
  crear_ruta('UA9001X', 4, v_air_nrt, 1, '16:15', '20:25', 1010, 'intercontinental', '258', 9120, 11, '10:55', '12:10', 75);
  crear_ruta('UA9010X', 4, 1, v_air_icn, '06:35', '16:10', 1075, 'intercontinental', '258', 9240, 11, '11:25', '12:55', 90);
  crear_ruta('UA9011X', 4, v_air_icn, 1, '18:20', '22:50', 1045, 'intercontinental', '369', 9240, 11, '13:30', '14:45', 75);
  crear_ruta('UA9100X', 4, 1, v_air_syd, '04:50', '08:25', 1275, 'intercontinental', '17', 10550, 11, '10:15', '12:00', 105);
  crear_ruta('UA9101X', 4, v_air_syd, 1, '10:55', '16:15', 1290, 'intercontinental', '28', 10550, 11, '04:25', '06:00', 95);
  crear_ruta('UA9110X', 4, 1, v_air_akl, '03:40', '06:55', 1215, 'intercontinental', '36', 10980, 11, '09:10', '10:45', 95);
  crear_ruta('UA9111X', 4, v_air_akl, 1, '08:30', '14:05', 1300, 'intercontinental', '47', 10980, 11, '02:35', '04:05', 90);
  COMMIT;
EXCEPTION
  WHEN OTHERS THEN
    ROLLBACK;
    RAISE;
END;
/

PROMPT Fin del script. Si aparecio algun ORA arriba, la seed masiva hizo ROLLBACK y no se cargo.
