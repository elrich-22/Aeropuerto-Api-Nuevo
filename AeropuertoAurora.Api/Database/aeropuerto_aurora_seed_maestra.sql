--------------------------------------------------------
-- AEROPUERTO AURORA - SEED MAESTRA DE PRESENTACION
-- Base integral para demo / preproduccion
--
-- Requiere unicamente:
--   1) aeropuerto_aurora_v2.sql
-- Ejecuta:
--   @aeropuerto_aurora_seed_maestra.sql
--
-- Esta version es autocontenida: no depende de otros
-- scripts de seed. Incluye la base operativa, la
-- expansion global y el ajuste final de identities.
--------------------------------------------------------

PROMPT Iniciando seed maestra Aeropuerto Aurora...

--------------------------------------------------------
-- AEROPUERTO AURORA - SEED OPERATIVA "WOW"
-- Motor: Oracle 21c
-- Fecha de corte operacional: 2026-05-18
--
-- Ejecutar despues de crear el esquema aeropuerto_aurora_v2.sql.
-- Recomendado: base vacia, usuario AEROPUERTO_AURORA.
--
-- Nota: aeropuertos, aerolineas y rutas son datos publicos/reales.
-- Empleados y pasajeros son ficticios pero consistentes; no son PII real.
--------------------------------------------------------

SET DEFINE OFF;
ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD';
ALTER SESSION SET NLS_TIMESTAMP_FORMAT = 'YYYY-MM-DD HH24:MI:SS';
ALTER SESSION SET CURRENT_SCHEMA = AEROPUERTO_AURORA;

--------------------------------------------------------
-- 1. Red realista de aeropuertos, terminales y puertas
--------------------------------------------------------

INSERT INTO AER_AEROPUERTO
(AER_ID, AER_CODIGO_AEROPUERTO, AER_NOMBRE, AER_CIUDAD, AER_PAIS, AER_ZONA_HORARIA, AER_ESTADO, AER_TIPO, AER_LATITUD, AER_LONGITUD, AER_CODIGO_IATA, AER_CODIGO_ICAO, AER_FECHA_REGISTRO)
VALUES (1, 'GUA', 'Aeropuerto Internacional La Aurora', 'Ciudad de Guatemala', 'Guatemala', 'America/Guatemala', 'ACTIVO', 'internacional', 14.583300, -90.527500, 'GUA', 'MGGT', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (2, 'FRS', 'Aeropuerto Internacional Mundo Maya', 'Flores', 'Guatemala', 'America/Guatemala', 'ACTIVO', 'internacional', 16.913800, -89.866400, 'FRS', 'MGTK', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (3, 'SAL', 'Aeropuerto Internacional de El Salvador San Oscar Arnulfo Romero', 'San Salvador', 'El Salvador', 'America/El_Salvador', 'ACTIVO', 'internacional', 13.440900, -89.055700, 'SAL', 'MSLP', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (4, 'SAP', 'Aeropuerto Internacional Ramon Villeda Morales', 'San Pedro Sula', 'Honduras', 'America/Tegucigalpa', 'ACTIVO', 'internacional', 15.452600, -87.923600, 'SAP', 'MHLM', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (5, 'SJO', 'Aeropuerto Internacional Juan Santamaria', 'San Jose', 'Costa Rica', 'America/Costa_Rica', 'ACTIVO', 'internacional', 9.993900, -84.208800, 'SJO', 'MROC', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (6, 'PTY', 'Aeropuerto Internacional de Tocumen', 'Ciudad de Panama', 'Panama', 'America/Panama', 'ACTIVO', 'hub', 9.071400, -79.383500, 'PTY', 'MPTO', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (7, 'MEX', 'Aeropuerto Internacional Benito Juarez', 'Ciudad de Mexico', 'Mexico', 'America/Mexico_City', 'ACTIVO', 'hub', 19.436300, -99.072100, 'MEX', 'MMMX', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (8, 'CUN', 'Aeropuerto Internacional de Cancun', 'Cancun', 'Mexico', 'America/Cancun', 'ACTIVO', 'turistico', 21.036500, -86.877100, 'CUN', 'MMUN', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (9, 'MIA', 'Miami International Airport', 'Miami', 'Estados Unidos', 'America/New_York', 'ACTIVO', 'hub', 25.795900, -80.287000, 'MIA', 'KMIA', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (10, 'IAH', 'George Bush Intercontinental Airport', 'Houston', 'Estados Unidos', 'America/Chicago', 'ACTIVO', 'hub', 29.990200, -95.336800, 'IAH', 'KIAH', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (11, 'LAX', 'Los Angeles International Airport', 'Los Angeles', 'Estados Unidos', 'America/Los_Angeles', 'ACTIVO', 'hub', 33.941600, -118.408500, 'LAX', 'KLAX', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (12, 'JFK', 'John F. Kennedy International Airport', 'Nueva York', 'Estados Unidos', 'America/New_York', 'ACTIVO', 'hub', 40.641300, -73.778100, 'JFK', 'KJFK', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (13, 'BOG', 'Aeropuerto Internacional El Dorado', 'Bogota', 'Colombia', 'America/Bogota', 'ACTIVO', 'hub', 4.701600, -74.146900, 'BOG', 'SKBO', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (14, 'LIM', 'Aeropuerto Internacional Jorge Chavez', 'Lima', 'Peru', 'America/Lima', 'ACTIVO', 'hub', -12.021900, -77.114300, 'LIM', 'SPJC', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (15, 'MAD', 'Aeropuerto Adolfo Suarez Madrid-Barajas', 'Madrid', 'Espana', 'Europe/Madrid', 'ACTIVO', 'intercontinental', 40.498300, -3.567600, 'MAD', 'LEMD', DATE '2026-05-18');
INSERT INTO AER_AEROPUERTO VALUES (16, 'ATL', 'Hartsfield-Jackson Atlanta International Airport', 'Atlanta', 'Estados Unidos', 'America/New_York', 'ACTIVO', 'hub', 33.640700, -84.427700, 'ATL', 'KATL', DATE '2026-05-18');

INSERT INTO AER_TERMINAL VALUES (1, 1, 'Terminal Central Internacional', 'internacional', 4200, 'ACTIVA');
INSERT INTO AER_TERMINAL VALUES (2, 1, 'Terminal Nacional y Regional', 'nacional', 1300, 'ACTIVA');
INSERT INTO AER_TERMINAL VALUES (3, 1, 'Terminal de Carga Aerea', 'carga', 650, 'ACTIVA');
INSERT INTO AER_TERMINAL VALUES (4, 1, 'Aviacion General y Privada', 'privada', 180, 'ACTIVA');

BEGIN
  FOR i IN 1..14 LOOP
    INSERT INTO AER_PUERTA_EMBARQUE
    VALUES (i, 1, 'A' || LPAD(i, 2, '0'), CASE WHEN i IN (7, 11) THEN 'OCUPADA' ELSE 'DISPONIBLE' END, 'internacional');
  END LOOP;
  FOR i IN 1..6 LOOP
    INSERT INTO AER_PUERTA_EMBARQUE
    VALUES (100 + i, 2, 'B' || LPAD(i, 2, '0'), 'DISPONIBLE', 'nacional');
  END LOOP;
  INSERT INTO AER_PUERTA_EMBARQUE VALUES (201, 1, 'R01', 'DISPONIBLE', 'remota');
  INSERT INTO AER_PUERTA_EMBARQUE VALUES (202, 1, 'R02', 'MANTENIMIENTO', 'remota');
END;
/

--------------------------------------------------------
-- 2. Aerolineas, flota, modelos y asientos
--------------------------------------------------------

INSERT INTO AER_AEROLINEA VALUES (1, 'AV', 'Avianca', 'Colombia', 'AV', 'AVA', 'ACTIVA', '+502 2421-2200', 'gua@avianca.com', 'https://www.avianca.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (2, 'CM', 'Copa Airlines', 'Panama', 'CM', 'CMP', 'ACTIVA', '+502 2307-2600', 'gua@copaair.com', 'https://www.copaair.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (3, 'AA', 'American Airlines', 'Estados Unidos', 'AA', 'AAL', 'ACTIVA', '+502 2422-8000', 'gua@aa.com', 'https://www.aa.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (4, 'UA', 'United Airlines', 'Estados Unidos', 'UA', 'UAL', 'ACTIVA', '+502 2376-0100', 'gua@united.com', 'https://www.united.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (5, 'DL', 'Delta Air Lines', 'Estados Unidos', 'DL', 'DAL', 'ACTIVA', '+502 2421-1000', 'gua@delta.com', 'https://www.delta.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (6, 'AM', 'Aeromexico', 'Mexico', 'AM', 'AMX', 'ACTIVA', '+502 2302-5799', 'gua@aeromexico.com', 'https://www.aeromexico.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (7, 'IB', 'Iberia', 'Espana', 'IB', 'IBE', 'ACTIVA', '+502 2278-6300', 'gua@iberia.com', 'https://www.iberia.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (8, 'Y4', 'Volaris', 'Mexico', 'Y4', 'VOI', 'ACTIVA', '+502 2301-3939', 'gua@volaris.com', 'https://www.volaris.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (9, '5U', 'TAG Airlines', 'Guatemala', '5U', 'TGU', 'ACTIVA', '+502 2380-9494', 'operaciones@tag.com.gt', 'https://www.tag.com.gt', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (10, 'NK', 'Spirit Airlines', 'Estados Unidos', 'NK', 'NKS', 'ACTIVA', '+502 2410-5200', 'gua@spirit.com', 'https://www.spirit.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (11, 'B6', 'JetBlue Airways', 'Estados Unidos', 'B6', 'JBU', 'ACTIVA', '+502 2410-5300', 'gua@jetblue.com', 'https://www.jetblue.com', DATE '2026-05-18');
INSERT INTO AER_AEROLINEA VALUES (12, 'D0', 'DHL Aviation', 'Alemania', 'D0', 'DHK', 'ACTIVA', '+502 2385-9500', 'gua.gateway@dhl.com', 'https://aviationcargo.dhl.com', DATE '2026-05-18');

INSERT INTO AER_MODELOAVION VALUES (1, 'Airbus A320neo', 'Airbus', 186, 7900, 6300, 829, 2016, 'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (2, 'Boeing 737-800', 'Boeing', 189, 7900, 5436, 842, 1998, 'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (3, 'Boeing 737 MAX 9', 'Boeing', 193, 8200, 6570, 839, 2017, 'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (4, 'Airbus A321neo', 'Airbus', 220, 9300, 7400, 829, 2017, 'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (5, 'Embraer E190', 'Embraer', 100, 2800, 4537, 829, 2004, 'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (6, 'ATR 72-600', 'ATR', 72, 750, 1528, 510, 2010, 'Turbohelice');
INSERT INTO AER_MODELOAVION VALUES (7, 'Boeing 787-8 Dreamliner', 'Boeing', 242, 28000, 13620, 913, 2011, 'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (8, 'Boeing 757-200F', 'Boeing', 0, 34000, 5834, 850, 1987, 'Turbofan');

INSERT INTO AER_AVION VALUES (1, 'N784AV', 1, 1, 2021, 'ACTIVO', DATE '2026-03-18', DATE '2026-06-18', 8120);
INSERT INTO AER_AVION VALUES (2, 'HP-1831CMP', 2, 2, 2018, 'ACTIVO', DATE '2026-02-26', DATE '2026-05-26', 14620);
INSERT INTO AER_AVION VALUES (3, 'N908NN', 2, 3, 2017, 'ACTIVO', DATE '2026-03-11', DATE '2026-06-11', 17540);
INSERT INTO AER_AVION VALUES (4, 'N37518', 3, 4, 2020, 'ACTIVO', DATE '2026-04-01', DATE '2026-07-01', 9270);
INSERT INTO AER_AVION VALUES (5, 'N821DN', 1, 5, 2019, 'ACTIVO', DATE '2026-03-21', DATE '2026-06-21', 10340);
INSERT INTO AER_AVION VALUES (6, 'XA-AMR', 3, 6, 2022, 'ACTIVO', DATE '2026-02-28', DATE '2026-05-28', 6410);
INSERT INTO AER_AVION VALUES (7, 'EC-NBE', 7, 7, 2019, 'ACTIVO', DATE '2026-03-06', DATE '2026-06-06', 11870);
INSERT INTO AER_AVION VALUES (8, 'XA-VLL', 4, 8, 2021, 'ACTIVO', DATE '2026-04-05', DATE '2026-07-05', 7210);
INSERT INTO AER_AVION VALUES (9, 'TG-TAG', 6, 9, 2017, 'ACTIVO', DATE '2026-04-10', DATE '2026-05-10', 5900);
INSERT INTO AER_AVION VALUES (10, 'N672NK', 1, 10, 2018, 'ACTIVO', DATE '2026-03-14', DATE '2026-06-14', 13440);
INSERT INTO AER_AVION VALUES (11, 'N304JB', 5, 11, 2016, 'ACTIVO', DATE '2026-02-19', DATE '2026-05-19', 16180);
INSERT INTO AER_AVION VALUES (12, 'HP-1857CMP', 2, 2, 2019, 'ACTIVO', DATE '2026-04-03', DATE '2026-07-03', 11220);
INSERT INTO AER_AVION VALUES (13, 'N966AN', 2, 3, 2016, 'MANTENIMIENTO', DATE '2026-04-24', DATE '2026-05-02', 19830);
INSERT INTO AER_AVION VALUES (14, 'N772UA', 2, 4, 2015, 'ACTIVO', DATE '2026-03-09', DATE '2026-06-09', 21400);
INSERT INTO AER_AVION VALUES (15, 'XA-ADV', 5, 6, 2014, 'ACTIVO', DATE '2026-04-13', DATE '2026-07-13', 18500);
INSERT INTO AER_AVION VALUES (16, 'TG-MYA', 6, 9, 2018, 'ACTIVO', DATE '2026-04-15', DATE '2026-05-15', 4300);
INSERT INTO AER_AVION VALUES (17, 'D-AEAC', 8, 12, 2008, 'ACTIVO', DATE '2026-03-25', DATE '2026-06-25', 32200);
INSERT INTO AER_AVION VALUES (18, 'N791AV', 1, 1, 2020, 'ACTIVO', DATE '2026-03-30', DATE '2026-06-30', 8680);

DECLARE
  v_letters VARCHAR2(6) := 'ABCDEF';
  v_class   VARCHAR2(15);
BEGIN
  FOR avion IN 1..18 LOOP
    FOR fila IN 1..26 LOOP
      FOR pos IN 1..6 LOOP
        v_class := CASE WHEN fila = 1 AND avion = 7 THEN 'primera'
                        WHEN fila <= 3 THEN 'ejecutiva'
                        ELSE 'economica' END;
        INSERT INTO AER_ASIENTO_AVION
        (ASA_ID_ASIENTO, ASA_ID_AVION, ASA_CODIGO, ASA_CLASE, ASA_ESTADO)
        VALUES (avion * 10000 + fila * 10 + pos, avion, TO_CHAR(fila) || SUBSTR(v_letters, pos, 1), v_class,
                CASE WHEN fila = 13 AND pos IN (3, 4) THEN 'BLOQUEADO' ELSE 'DISPONIBLE' END);
      END LOOP;
    END LOOP;
  END LOOP;
END;
/

--------------------------------------------------------
-- 3. Programas de vuelo y calendario de operaciones
--------------------------------------------------------

INSERT INTO AER_PROGRAMAVUELO VALUES (1, 'AV640', 1, 1, 9, '07:15', '11:50', 155, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (2, 'CM391', 2, 1, 6, '05:39', '08:03', 144, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (3, 'AA1188', 3, 1, 9, '12:42', '17:21', 159, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (4, 'UA1901', 4, 1, 10, '01:30', '05:18', 168, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (5, 'DL1830', 5, 1, 16, '13:05', '17:42', 157, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (6, 'AM679', 6, 1, 7, '16:48', '19:10', 142, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (7, 'IB6342', 7, 1, 15, '17:10', '13:35', 685, 'intercontinental', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (8, 'Y43931', 8, 1, 8, '09:30', '12:22', 172, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (9, '5U110', 9, 1, 2, '06:20', '07:20', 60, 'nacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (10, '5U211', 9, 1, 3, '08:10', '09:05', 55, 'regional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (11, 'NK515', 10, 1, 11, '11:30', '16:26', 176, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (12, 'B62029', 11, 1, 12, '23:40', '05:45', 245, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (13, 'D0254', 12, 1, 9, '22:15', '02:55', 160, 'carga', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (14, 'AV743', 1, 1, 13, '15:05', '18:20', 195, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (15, 'CM407', 2, 1, 5, '10:15', '12:05', 110, 'internacional', 'ACTIVO', DATE '2026-05-18');
INSERT INTO AER_PROGRAMAVUELO VALUES (16, 'AM671', 6, 1, 7, '07:55', '10:12', 137, 'internacional', 'ACTIVO', DATE '2026-05-18');

DECLARE
  v_dias SYS.ODCINUMBERLIST := SYS.ODCINUMBERLIST(1,2,3,4,5,6,7);
  v_id NUMBER := 1;
BEGIN
  FOR p IN 1..16 LOOP
    FOR d IN 1..v_dias.COUNT LOOP
      IF p IN (7, 12, 13) AND d NOT IN (1,3,5,7) THEN
        NULL;
      ELSE
        INSERT INTO AER_DIASVUELO VALUES (v_id, p, v_dias(d));
        v_id := v_id + 1;
      END IF;
    END LOOP;
  END LOOP;
END;
/

INSERT INTO AER_ESCALATECNICA VALUES (1, 7, 13, 1, '20:55', '22:10', 75);
INSERT INTO AER_ESCALATECNICA VALUES (2, 12, 9, 1, '02:20', '03:05', 45);
INSERT INTO AER_ESCALATECNICA VALUES (3, 13, 10, 1, '00:10', '00:55', 45);

--------------------------------------------------------
-- 4. Metodos de pago, promociones, departamentos y empleados
--------------------------------------------------------

INSERT INTO AER_METODOPAGO VALUES (1, 'Visa Credito', 'tarjeta', 'ACTIVO', 2.75);
INSERT INTO AER_METODOPAGO VALUES (2, 'Mastercard Debito', 'tarjeta', 'ACTIVO', 2.35);
INSERT INTO AER_METODOPAGO VALUES (3, 'Transferencia ACH', 'transferencia', 'ACTIVO', 0.50);
INSERT INTO AER_METODOPAGO VALUES (4, 'Efectivo Mostrador', 'efectivo', 'ACTIVO', 0.00);
INSERT INTO AER_METODOPAGO VALUES (5, 'Apple Pay / Google Pay', 'digital', 'ACTIVO', 2.10);

INSERT INTO AER_PROMOCION VALUES (1, 'SEMANA-SANTA-GT', 'Descuento para rutas Guatemala-Mundo Maya y region centroamericana', 'PORCENTAJE', 12, DATE '2026-03-15', DATE '2026-04-30', 600, 184, 'ACTIVA');
INSERT INTO AER_PROMOCION VALUES (2, 'MAYA-WEEKEND', 'Tarifa promocional para fines de semana hacia Flores', 'MONTO', 125, DATE '2026-04-01', DATE '2026-06-30', 300, 63, 'ACTIVA');
INSERT INTO AER_PROMOCION VALUES (3, 'CORPORATIVO-Q2', 'Convenios empresariales para viajes frecuentes desde GUA', 'PORCENTAJE', 8, DATE '2026-04-01', DATE '2026-06-30', 900, 241, 'ACTIVA');

INSERT INTO AER_DEPARTAMENTO VALUES (1, 'Operaciones Aereas', 'Despacho, tripulaciones, coordinacion de salidas y llegadas', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (2, 'Servicio al Pasajero', 'Check-in, salas de abordaje, conexiones e informacion', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (3, 'Seguridad Aeroportuaria', 'Inspeccion, control de accesos y respuesta inicial', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (4, 'Control Migratorio', 'Procesos de entrada, salida y transito internacional', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (5, 'Mantenimiento Aeronautico', 'Linea, hangares, repuestos y aeronavegabilidad', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (6, 'Comercial y Ventas', 'Mostradores, canales digitales, cobros y atencion comercial', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (7, 'Logistica de Equipaje', 'Clasificacion, carga, descarga y trazabilidad de maletas', 1, 'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (8, 'Administracion', 'Planilla, auditoria, compras y proveedores', 1, 'ACTIVO');

INSERT INTO AER_PUESTO VALUES (1, 'Piloto Capitan', 1, 'Comandante de aeronave en rutas internacionales', 38000, 62000, 'S');
INSERT INTO AER_PUESTO VALUES (2, 'Primer Oficial', 1, 'Copiloto certificado para operacion comercial', 24000, 42000, 'S');
INSERT INTO AER_PUESTO VALUES (3, 'Jefe de Cabina', 1, 'Responsable de servicio y seguridad en cabina', 14500, 23000, 'S');
INSERT INTO AER_PUESTO VALUES (4, 'Auxiliar de Vuelo', 1, 'Tripulante de cabina', 9800, 16500, 'S');
INSERT INTO AER_PUESTO VALUES (5, 'Agente de Check-in', 2, 'Atencion de pasajeros y documentacion de viaje', 5200, 7800, 'N');
INSERT INTO AER_PUESTO VALUES (6, 'Supervisor de Puerta', 2, 'Coordinacion de abordajes y cierres de vuelo', 8500, 12500, 'N');
INSERT INTO AER_PUESTO VALUES (7, 'Inspector de Seguridad', 3, 'Control de pasajeros, equipaje de mano y accesos', 6200, 9800, 'S');
INSERT INTO AER_PUESTO VALUES (8, 'Oficial Migratorio', 4, 'Revision documental de entrada y salida', 7200, 11800, 'S');
INSERT INTO AER_PUESTO VALUES (9, 'Tecnico de Linea', 5, 'Mantenimiento preventivo y correctivo en plataforma', 11000, 19000, 'S');
INSERT INTO AER_PUESTO VALUES (10, 'Jefe de Mantenimiento', 5, 'Responsable tecnico de aeronavegabilidad', 22000, 36000, 'S');
INSERT INTO AER_PUESTO VALUES (11, 'Agente de Ventas', 6, 'Venta de boletos y cobros en mostrador', 5000, 8200, 'N');
INSERT INTO AER_PUESTO VALUES (12, 'Analista Comercial Digital', 6, 'Analitica web/app y conversion de ventas', 9000, 14500, 'N');
INSERT INTO AER_PUESTO VALUES (13, 'Coordinador de Equipaje', 7, 'Trazabilidad y despacho de equipaje facturado', 6800, 10500, 'N');
INSERT INTO AER_PUESTO VALUES (14, 'Operador de Rampa', 7, 'Carga, descarga y apoyo en plataforma', 5400, 8500, 'S');
INSERT INTO AER_PUESTO VALUES (15, 'Comprador Aeronautico', 8, 'Ordenes de compra y gestion de proveedores', 8200, 13000, 'N');
INSERT INTO AER_PUESTO VALUES (16, 'Auditor Interno', 8, 'Control documental y auditoria de operaciones', 9500, 15000, 'N');

DECLARE
  TYPE t_str IS TABLE OF VARCHAR2(100) INDEX BY PLS_INTEGER;
  n1 t_str; n2 t_str; a1 t_str; a2 t_str; muni t_str;
  v_puesto NUMBER; v_depto NUMBER; v_sal NUMBER;
BEGIN
  n1(1):='Lucia'; n1(2):='Diego'; n1(3):='Sofia'; n1(4):='Carlos'; n1(5):='Mariana'; n1(6):='Jose'; n1(7):='Andrea'; n1(8):='Luis';
  n2(1):='Alejandra'; n2(2):='Fernando'; n2(3):='Isabel'; n2(4):='Eduardo'; n2(5):='Paola'; n2(6):='Antonio'; n2(7):='Gabriela'; n2(8):='Rafael';
  a1(1):='Garcia'; a1(2):='Morales'; a1(3):='Herrera'; a1(4):='Lopez'; a1(5):='Castillo'; a1(6):='Mendez'; a1(7):='Aguilar'; a1(8):='Ramirez';
  a2(1):='Pineda'; a2(2):='Cifuentes'; a2(3):='Barrios'; a2(4):='Escobar'; a2(5):='Fuentes'; a2(6):='Rodas'; a2(7):='Salazar'; a2(8):='Quezada';
  muni(1):='Guatemala'; muni(2):='Mixco'; muni(3):='Villa Nueva'; muni(4):='Santa Catarina Pinula'; muni(5):='San Miguel Petapa'; muni(6):='Fraijanes'; muni(7):='Amatitlan'; muni(8):='San Jose Pinula';

  FOR i IN 1..40 LOOP
    v_puesto := CASE
      WHEN i <= 6 THEN CASE MOD(i,2) WHEN 0 THEN 2 ELSE 1 END
      WHEN i <= 14 THEN CASE WHEN MOD(i,4)=0 THEN 3 ELSE 4 END
      WHEN i <= 20 THEN CASE WHEN MOD(i,3)=0 THEN 6 ELSE 5 END
      WHEN i <= 25 THEN 7
      WHEN i <= 29 THEN 8
      WHEN i <= 34 THEN CASE WHEN MOD(i,5)=0 THEN 10 ELSE 9 END
      WHEN i <= 37 THEN 11
      WHEN i = 38 THEN 12
      WHEN i = 39 THEN 15
      ELSE 16 END;
    v_depto := CASE WHEN v_puesto <= 4 THEN 1 WHEN v_puesto <= 6 THEN 2 WHEN v_puesto = 7 THEN 3 WHEN v_puesto = 8 THEN 4 WHEN v_puesto <= 10 THEN 5 WHEN v_puesto <= 12 THEN 6 WHEN v_puesto <= 14 THEN 7 ELSE 8 END;
    v_sal := CASE v_puesto
      WHEN 1 THEN 45500 WHEN 2 THEN 31200 WHEN 3 THEN 17200 WHEN 4 THEN 11800
      WHEN 5 THEN 6200 WHEN 6 THEN 9800 WHEN 7 THEN 7600 WHEN 8 THEN 9100
      WHEN 9 THEN 13800 WHEN 10 THEN 28200 WHEN 11 THEN 6100 WHEN 12 THEN 11600
      WHEN 15 THEN 10100 ELSE 12400 END;

    INSERT INTO AER_EMPLEADO
    (EMP_ID_EMPLEADO, EMP_NUMERO_EMPLEADO, EMP_PRIMER_NOMBRE, EMP_SEGUNDO_NOMBRE, EMP_PRIMER_APELLIDO, EMP_SEGUNDO_APELLIDO,
     EMP_FECHA_NACIMIENTO, EMP_DPI, EMP_NIT, EMP_DIR_CALLE, EMP_DIR_ZONA, EMP_DIR_MUNICIPIO, EMP_DIR_DEPARTAMENTO, EMP_DIR_PAIS,
     EMP_TELEFONO, EMP_EMAIL, EMP_FECHA_CONTRATACION, EMP_ID_PUESTO, EMP_ID_DEPARTAMENTO, EMP_SALARIO_ACTUAL, EMP_TIPO_CONTRATO, EMP_ESTADO)
    VALUES
    (i, 'AUR-' || LPAD(i, 5, '0'), n1(MOD(i-1,8)+1), n2(MOD(i+2,8)+1), a1(MOD(i+4,8)+1), a2(MOD(i+6,8)+1),
     ADD_MONTHS(DATE '1972-01-15', i * 7), '30' || LPAD(i, 11, '0'), 'CF' || LPAD(i, 8, '0'),
     'Avenida Hincapie ' || (10 + i) || '-' || LPAD(MOD(i,20)+1,2,'0'), 'Zona ' || TO_CHAR(MOD(i,18)+1), muni(MOD(i-1,8)+1), 'Guatemala', 'Guatemala',
     '+502 5' || LPAD(1000000 + i * 137, 7, '0'), LOWER(n1(MOD(i-1,8)+1) || '.' || a1(MOD(i+4,8)+1) || i || '@aurora.gt'),
     ADD_MONTHS(DATE '2014-02-01', i * 3), v_puesto, v_depto, v_sal, CASE WHEN i IN (38,39,40) THEN 'INDEFINIDO' ELSE 'FIJO' END, 'ACTIVO');
  END LOOP;
END;
/

BEGIN
  FOR i IN 1..34 LOOP
    INSERT INTO AER_LICENCIAEMPLEADO
    VALUES (i, i, CASE WHEN i <= 6 THEN 'Licencia DGAC Tripulacion Tecnica'
                       WHEN i <= 14 THEN 'Certificado Tripulante Cabina'
                       WHEN i <= 25 THEN 'Credencial Seguridad Aeroportuaria'
                       WHEN i <= 29 THEN 'Credencial Migratoria'
                       ELSE 'Licencia Tecnico Aeronautico' END,
            'LIC-AUR-' || LPAD(i, 6, '0'), ADD_MONTHS(DATE '2024-01-01', MOD(i, 12)),
            ADD_MONTHS(DATE '2024-01-01', 36 + MOD(i, 12)), 'DGAC Guatemala', 'VIGENTE');
  END LOOP;

  FOR i IN 1..40 LOOP
    INSERT INTO AER_ASISTENCIA
    VALUES (i, i, DATE '2026-05-18',
            TIMESTAMP '2026-05-18 06:00:00' + NUMTODSINTERVAL(MOD(i,5)*15, 'MINUTE'),
            TIMESTAMP '2026-05-18 14:00:00' + NUMTODSINTERVAL(MOD(i,5)*15, 'MINUTE'),
            8, CASE WHEN MOD(i,9)=0 THEN 'HORAS_EXTRA' ELSE 'NORMAL' END, 'PRESENTE');
    INSERT INTO AER_PLANILLA
    VALUES (i, i, DATE '2026-04-01', DATE '2026-04-30',
            CASE WHEN i <= 6 THEN 32000 WHEN i <= 14 THEN 12000 WHEN i <= 29 THEN 8200 WHEN i <= 34 THEN 14500 ELSE 9000 END,
            250, CASE WHEN MOD(i,9)=0 THEN 450 ELSE 0 END, 385,
            CASE WHEN i <= 6 THEN 31865 WHEN i <= 14 THEN 11865 WHEN i <= 29 THEN 8065 WHEN i <= 34 THEN 14365 ELSE 8865 END,
            DATE '2026-04-25', 'PAGADA');
  END LOOP;
END;
/

--------------------------------------------------------
-- 5. Pasajeros, login, sesiones, preferencias y analitica
--------------------------------------------------------

-- Credenciales demo del seed maestra:
--   Usuarios generales activos: AuroraDemo1!
--   Usuarios demo inactivos/bloqueados: DemoInactivo1!
--   admin.aurora: AdminAurora1!
--   soporte.operaciones y auditoria.seguridad: SoporteAurora1!

DECLARE
  TYPE t_str IS TABLE OF VARCHAR2(100) INDEX BY PLS_INTEGER;
  n1 t_str; n2 t_str; a1 t_str; a2 t_str; nac t_str;
BEGIN
  n1(1):='Ana'; n1(2):='Mateo'; n1(3):='Valeria'; n1(4):='Sebastian'; n1(5):='Camila'; n1(6):='Daniel'; n1(7):='Paula'; n1(8):='Nicolas'; n1(9):='Elena'; n1(10):='Javier';
  n2(1):='Maria'; n2(2):='Andres'; n2(3):='Lucia'; n2(4):='Emilio'; n2(5):='Sofia'; n2(6):='Gabriel'; n2(7):='Fernanda'; n2(8):='Alejandro'; n2(9):='Isabel'; n2(10):='Ricardo';
  a1(1):='Alvarez'; a1(2):='Torres'; a1(3):='Vasquez'; a1(4):='Molina'; a1(5):='Santos'; a1(6):='Reyes'; a1(7):='Delgado'; a1(8):='Navarro'; a1(9):='Rivas'; a1(10):='Campos';
  a2(1):='Mendoza'; a2(2):='Figueroa'; a2(3):='Juarez'; a2(4):='Valdez'; a2(5):='Ortiz'; a2(6):='Mejia'; a2(7):='Luna'; a2(8):='Cruz'; a2(9):='Vega'; a2(10):='Leon';
  nac(1):='Guatemala'; nac(2):='El Salvador'; nac(3):='Honduras'; nac(4):='Costa Rica'; nac(5):='Mexico'; nac(6):='Estados Unidos'; nac(7):='Colombia'; nac(8):='Panama'; nac(9):='Espana'; nac(10):='Peru';

  FOR i IN 1..90 LOOP
    INSERT INTO AER_PASAJERO
    (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
     PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
    VALUES
    (i, CASE WHEN i <= 55 THEN 'GT' || LPAD(i, 9, '0') ELSE 'P' || LPAD(800000 + i, 8, '0') END,
     CASE WHEN i <= 55 THEN 'DPI' ELSE 'PASAPORTE' END,
     n1(MOD(i-1,10)+1), n2(MOD(i+1,10)+1), a1(MOD(i+3,10)+1), a2(MOD(i+5,10)+1),
     ADD_MONTHS(DATE '1975-05-10', i * 5), nac(MOD(i-1,10)+1), CASE WHEN MOD(i,2)=0 THEN 'M' ELSE 'F' END,
     '+502 4' || LPAD(2000000 + i * 121, 7, '0'), LOWER(n1(MOD(i-1,10)+1) || '.' || a1(MOD(i+3,10)+1) || i || '@correo.com'),
     DATE '2026-05-18' + MOD(i, 100));
  END LOOP;

  FOR i IN 1..60 LOOP
    INSERT INTO AER_USUARIO_LOGIN
    (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS)
    SELECT i, i, LOWER(PAS_PRIMER_NOMBRE || '.' || PAS_PRIMER_APELLIDO || LPAD(i, 2, '0')), PAS_EMAIL,
           CASE
             WHEN i IN (12, 24, 36, 48, 60) THEN 'PBKDF2$100000$/bK4i8AgsK4Y9J66zr43Zw==$wFy2Zg6tqOz8JL63YNccPe3RYMRLtoa84YQrJe010CQ='
             ELSE 'PBKDF2$100000$NETOIdMIOybQF4Dh+i22Ew==$wpsk9LGzzvzK5JK2LB8K5KOiJ24kAz67xjVVbxZCoRA='
           END,
           CASE
             WHEN i IN (12, 24, 36, 48, 60) THEN '/bK4i8AgsK4Y9J66zr43Zw=='
             ELSE 'NETOIdMIOybQF4Dh+i22Ew=='
           END,
           CASE
             WHEN i IN (12, 24, 36, 48) THEN 'INACTIVO'
             WHEN i IN (44, 60) THEN 'BLOQUEADO'
             WHEN i IN (18, 30, 54) THEN 'BLOQUEADO'
             ELSE 'ACTIVO'
           END,
           CASE WHEN i IN (9, 27, 43, 57) THEN 'N' ELSE 'S' END,
           TIMESTAMP '2026-05-18 08:00:00' + NUMTODSINTERVAL(i, 'DAY'),
           TIMESTAMP '2026-04-26 18:00:00' - NUMTODSINTERVAL(MOD(i,12), 'HOUR'),
           CASE WHEN i IN (44, 60) THEN 5 WHEN i IN (18, 30, 54) THEN 3 ELSE 0 END
    FROM AER_PASAJERO WHERE PAS_ID_PASAJERO = i;
  END LOOP;

  FOR i IN 1..70 LOOP
    INSERT INTO AER_SESIONUSUARIO
    VALUES (i, 'SES-202605-' || LPAD(i, 5, '0'),
            CASE WHEN i <= 45 THEN i ELSE NULL END,
            CASE WHEN i <= 45 THEN i ELSE NULL END,
            '190.148.' || MOD(i,255) || '.' || (20 + MOD(i,200)),
            CASE WHEN MOD(i,3)=0 THEN 'Chrome' WHEN MOD(i,3)=1 THEN 'Safari' ELSE 'Edge' END,
            CASE WHEN MOD(i,4)=0 THEN 'iOS' WHEN MOD(i,4)=1 THEN 'Android' WHEN MOD(i,4)=2 THEN 'Windows' ELSE 'macOS' END,
            CASE WHEN MOD(i,3)=0 THEN 'movil' WHEN MOD(i,3)=1 THEN 'desktop' ELSE 'tablet' END,
            TIMESTAMP '2026-05-18 06:00:00' + NUMTODSINTERVAL(i * 37, 'MINUTE'),
            TIMESTAMP '2026-05-18 06:08:00' + NUMTODSINTERVAL(i * 37, 'MINUTE'),
            480 + MOD(i,900));
  END LOOP;

  FOR i IN 1..90 LOOP
    INSERT INTO AER_PREFERENCIACLIENTE
    VALUES (i, i, CASE WHEN MOD(i,3)=0 THEN 'asiento' WHEN MOD(i,3)=1 THEN 'notificacion' ELSE 'equipaje' END,
            CASE WHEN MOD(i,3)=0 THEN 'ventana' WHEN MOD(i,3)=1 THEN 'email y app' ELSE 'factura una maleta' END,
            TIMESTAMP '2026-05-18 10:00:00' + NUMTODSINTERVAL(i, 'HOUR'));
  END LOOP;

  INSERT INTO AER_PASAJERO
  (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
   PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
  VALUES
  (91, 'ADMIN-AURORA-001', 'INTERNO', 'Administrador', 'Sistema', 'Aurora', 'Principal',
   DATE '1988-01-01', 'Guatemala', 'M', '+502 2300-0001', 'admin@aurora.gt', DATE '2026-05-18');

  INSERT INTO AER_PASAJERO
  (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
   PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
  VALUES
  (92, 'OPS-AURORA-002', 'INTERNO', 'Carla', 'Andrea', 'Paz', 'Monterroso',
   DATE '1991-03-16', 'Guatemala', 'F', '+502 2300-0002', 'soporte.operaciones@aurora.gt', DATE '2026-05-18');

  INSERT INTO AER_PASAJERO
  (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
   PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
  VALUES
  (93, 'SEC-AURORA-003', 'INTERNO', 'Hector', 'Daniel', 'Maldonado', 'Sierra',
   DATE '1986-08-11', 'Guatemala', 'M', '+502 2300-0003', 'auditoria.seguridad@aurora.gt', DATE '2026-05-18');

  INSERT INTO AER_PASAJERO
  (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
   PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
  VALUES
  (94, 'DEM-AURORA-004', 'INTERNO', 'Usuario', 'Demo', 'Inactivo', 'Portal',
   DATE '1995-11-05', 'Guatemala', 'M', '+502 2300-0004', 'demo.inactivo@aurora.gt', DATE '2026-05-18');

  INSERT INTO AER_PASAJERO
  (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
   PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
  VALUES
  (95, 'DEM-AURORA-005', 'INTERNO', 'Usuario', 'Demo', 'Bloqueado', 'Portal',
   DATE '1994-07-14', 'Guatemala', 'F', '+502 2300-0005', 'demo.bloqueado@aurora.gt', DATE '2026-05-18');

  INSERT INTO AER_USUARIO_LOGIN
  (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_TOKEN_VERIFICACION,
   USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
  VALUES
  (91, 91, 'admin.aurora', 'admin@aurora.gt',
   'PBKDF2$100000$MAvKF2fJPULBBm1x/MbjmA==$dzFteP76fsopJ9DjS01xZa8NXKaoiIUdUNaigmJ7HoA=',
   'MAvKF2fJPULBBm1x/MbjmA==', 'ACTIVO', 'S', NULL,
   TIMESTAMP '2026-05-18 08:00:00', TIMESTAMP '2026-05-18 06:00:00', 0, NULL, NULL, NULL);

  INSERT INTO AER_USUARIO_LOGIN
  (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_TOKEN_VERIFICACION,
   USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
  VALUES
  (92, 92, 'soporte.operaciones', 'soporte.operaciones@aurora.gt',
   'PBKDF2$100000$E+4hNZPDqqkII+XaTvcMfw==$Qe6nw1FFXfngYRlUaPeT5T6F4RHnPBHwIi9X2MXYIe4=',
   'E+4hNZPDqqkII+XaTvcMfw==', 'ACTIVO', 'S', NULL,
   TIMESTAMP '2026-05-18 09:15:00', TIMESTAMP '2026-05-18 07:10:00', 0, NULL, NULL, NULL);

  INSERT INTO AER_USUARIO_LOGIN
  (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_TOKEN_VERIFICACION,
   USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
  VALUES
  (93, 93, 'auditoria.seguridad', 'auditoria.seguridad@aurora.gt',
   'PBKDF2$100000$E+4hNZPDqqkII+XaTvcMfw==$Qe6nw1FFXfngYRlUaPeT5T6F4RHnPBHwIi9X2MXYIe4=',
   'E+4hNZPDqqkII+XaTvcMfw==', 'ACTIVO', 'S', NULL,
   TIMESTAMP '2026-05-18 09:45:00', TIMESTAMP '2026-05-18 18:40:00', 1, NULL, NULL, NULL);

  INSERT INTO AER_USUARIO_LOGIN
  (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_TOKEN_VERIFICACION,
   USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
  VALUES
  (94, 94, 'demo.inactivo', 'demo.inactivo@aurora.gt',
   'PBKDF2$100000$/bK4i8AgsK4Y9J66zr43Zw==$wFy2Zg6tqOz8JL63YNccPe3RYMRLtoa84YQrJe010CQ=',
   '/bK4i8AgsK4Y9J66zr43Zw==', 'INACTIVO', 'S', NULL,
   TIMESTAMP '2026-05-18 10:15:00', TIMESTAMP '2026-05-19 16:00:00', 0, NULL, NULL, NULL);

  INSERT INTO AER_USUARIO_LOGIN
  (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_TOKEN_VERIFICACION,
   USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
  VALUES
  (95, 95, 'demo.bloqueado', 'demo.bloqueado@aurora.gt',
   'PBKDF2$100000$/bK4i8AgsK4Y9J66zr43Zw==$wFy2Zg6tqOz8JL63YNccPe3RYMRLtoa84YQrJe010CQ=',
   '/bK4i8AgsK4Y9J66zr43Zw==', 'BLOQUEADO', 'S', NULL,
   TIMESTAMP '2026-05-18 10:45:00', TIMESTAMP '2026-05-20 09:20:00', 5, TIMESTAMP '2026-12-31 23:59:59', NULL, NULL);

  INSERT INTO AER_PREFERENCIACLIENTE
  VALUES (91, 91, 'rol_sistema', 'ADMINISTRADOR', TIMESTAMP '2026-05-18 08:00:00');
  INSERT INTO AER_PREFERENCIACLIENTE VALUES (92, 92, 'rol_sistema', 'SOPORTE_OPERACIONES', TIMESTAMP '2026-05-18 09:15:00');
  INSERT INTO AER_PREFERENCIACLIENTE VALUES (93, 93, 'rol_sistema', 'AUDITOR_SEGURIDAD', TIMESTAMP '2026-05-18 09:45:00');
  INSERT INTO AER_PREFERENCIACLIENTE VALUES (94, 94, 'rol_sistema', 'DEMO_INACTIVO', TIMESTAMP '2026-05-18 10:15:00');
  INSERT INTO AER_PREFERENCIACLIENTE VALUES (95, 95, 'rol_sistema', 'DEMO_BLOQUEADO', TIMESTAMP '2026-05-18 10:45:00');
END;
/

--------------------------------------------------------
-- 6. Vuelos reales del dia, puertas, tripulacion y reservas
--------------------------------------------------------

DECLARE
  v_estado VARCHAR2(20);
  v_salida TIMESTAMP;
  v_llegada TIMESTAMP;
  v_programa NUMBER;
  v_avion NUMBER;
BEGIN
  FOR i IN 1..48 LOOP
    v_programa := MOD(i-1,16)+1;
    v_avion := CASE v_programa
      WHEN 1 THEN CASE WHEN MOD(i,2)=0 THEN 18 ELSE 1 END
      WHEN 2 THEN CASE WHEN MOD(i,2)=0 THEN 12 ELSE 2 END
      WHEN 3 THEN 3
      WHEN 4 THEN CASE WHEN MOD(i,2)=0 THEN 14 ELSE 4 END
      WHEN 5 THEN 5
      WHEN 6 THEN CASE WHEN MOD(i,2)=0 THEN 15 ELSE 6 END
      WHEN 7 THEN 7
      WHEN 8 THEN 8
      WHEN 9 THEN 9
      WHEN 10 THEN 16
      WHEN 11 THEN 10
      WHEN 12 THEN 11
      WHEN 13 THEN 17
      WHEN 14 THEN CASE WHEN MOD(i,2)=0 THEN 18 ELSE 1 END
      WHEN 15 THEN CASE WHEN MOD(i,2)=0 THEN 12 ELSE 2 END
      ELSE CASE WHEN MOD(i,2)=0 THEN 15 ELSE 6 END
    END;
    v_estado := CASE
      WHEN i IN (4, 19) THEN 'RETRASADO'
      WHEN i IN (7, 23) THEN 'EN_VUELO'
      WHEN i <= 18 THEN 'ATERRIZADO'
      WHEN i <= 30 THEN 'ABORDANDO'
      ELSE 'PROGRAMADO' END;

    v_salida := TIMESTAMP '2026-05-18 05:30:00' + NUMTODSINTERVAL(i * 31, 'MINUTE');
    v_llegada := v_salida + NUMTODSINTERVAL(80 + MOD(i, 6) * 35, 'MINUTE');

    INSERT INTO AER_VUELO
    (VUE_ID_VUELO, VUE_ID_PROGRAMA_VUELO, VUE_ID_AVION, VUE_FECHA_VUELO, VUE_HORA_SALIDA_REAL, VUE_HORA_LLEGADA_REAL,
     VUE_PLAZAS_OCUPADAS, VUE_PLAZAS_VACIAS, VUE_ESTADO, VUE_FECHA_REPROGRAMACION, VUE_MOTIVO_CANCELACION, VUE_RETRASO_MINUTOS)
    VALUES
    (i, v_programa, v_avion, DATE '2026-05-18',
     CASE WHEN v_estado IN ('ATERRIZADO','EN_VUELO','RETRASADO') THEN v_salida + NUMTODSINTERVAL(CASE WHEN v_estado='RETRASADO' THEN 45 ELSE MOD(i,12) END, 'MINUTE') ELSE NULL END,
     CASE WHEN v_estado = 'ATERRIZADO' THEN v_llegada + NUMTODSINTERVAL(MOD(i,10), 'MINUTE') ELSE NULL END,
     0, 0, v_estado,
     CASE WHEN v_estado='RETRASADO' THEN DATE '2026-05-18' ELSE NULL END,
     NULL,
     CASE WHEN v_estado='RETRASADO' THEN 45 ELSE 0 END);

    INSERT INTO AER_ASIGNACION_PUERTA
    VALUES (i, i,
            CASE WHEN MOD(i,10)=0 THEN 100 + MOD(i,6) + 1 ELSE MOD(i-1,14)+1 END,
            v_salida - NUMTODSINTERVAL(90, 'MINUTE'),
            CASE WHEN v_estado IN ('ATERRIZADO','EN_VUELO') THEN v_salida + NUMTODSINTERVAL(20, 'MINUTE') ELSE NULL END,
            CASE WHEN v_estado IN ('ATERRIZADO','EN_VUELO') THEN 'FINALIZADA'
                 WHEN v_estado = 'ABORDANDO' THEN 'ACTIVA'
                 ELSE 'PROGRAMADA' END);

    INSERT INTO AER_TRIPULACION VALUES (i*10+1, i, MOD(i-1,6)+1, 'piloto', 2.4 + MOD(i,4));
    INSERT INTO AER_TRIPULACION VALUES (i*10+2, i, MOD(i,6)+1, 'copiloto', 2.4 + MOD(i,4));
    INSERT INTO AER_TRIPULACION VALUES (i*10+3, i, 7 + MOD(i,8), 'jefe cabina', 2.4 + MOD(i,4));
    INSERT INTO AER_TRIPULACION VALUES (i*10+4, i, 7 + MOD(i+3,8), 'auxiliar cabina', 2.4 + MOD(i,4));
  END LOOP;
END;
/

DECLARE
  v_vuelo NUMBER;
  v_pasajero NUMBER;
  v_avion NUMBER;
  v_fila NUMBER;
  v_pos NUMBER;
  v_asiento NUMBER;
  v_clase VARCHAR2(20);
  v_tarifa NUMBER(10,2);
BEGIN
  FOR i IN 1..110 LOOP
    v_vuelo := MOD(i-1,48) + 1;
    v_pasajero := MOD(i-1,90) + 1;
    SELECT VUE_ID_AVION INTO v_avion FROM AER_VUELO WHERE VUE_ID_VUELO = v_vuelo;
    v_fila := 4 + TRUNC((i-1) / 48);
    v_pos := MOD(i-1, 6) + 1;
    v_asiento := v_avion * 10000 + v_fila * 10 + v_pos;
    v_clase := CASE WHEN MOD(i,10)=0 THEN 'ejecutiva' ELSE 'economica' END;
    v_tarifa := CASE WHEN v_clase='ejecutiva' THEN 3250 ELSE 1180 + MOD(i,7)*95 END;

    INSERT INTO AER_RESERVA
    VALUES (i, v_vuelo, v_pasajero, v_clase, DATE '2026-05-01' + MOD(i,15),
            CASE WHEN i IN (28, 71) THEN 'CANCELADA' WHEN i <= 72 THEN 'COMPLETADA' ELSE 'CONFIRMADA' END,
            CASE WHEN MOD(i,4)=0 THEN 2 ELSE 1 END,
            CASE WHEN MOD(i,4)=0 THEN 31.5 ELSE 18.2 + MOD(i,8) END,
            v_tarifa, 'AUR' || TO_CHAR(DATE '2026-05-18','YYMMDD') || LPAD(i,5,'0'));

    INSERT INTO AER_ASIGNACION_ASIENTO
    VALUES (i, v_vuelo, v_pasajero, v_asiento,
            TIMESTAMP '2026-05-01 09:00:00' + NUMTODSINTERVAL(i, 'HOUR'),
            CASE WHEN i IN (28, 71) THEN 'CANCELADA' ELSE 'CONFIRMADA' END);
  END LOOP;

  UPDATE AER_VUELO v
  SET VUE_PLAZAS_OCUPADAS = (SELECT COUNT(*) FROM AER_RESERVA r WHERE r.RES_ID_VUELO = v.VUE_ID_VUELO AND r.RES_ESTADO <> 'CANCELADA'),
      VUE_PLAZAS_VACIAS = 156 - (SELECT COUNT(*) FROM AER_RESERVA r WHERE r.RES_ID_VUELO = v.VUE_ID_VUELO AND r.RES_ESTADO <> 'CANCELADA');
END;
/

--------------------------------------------------------
-- 7. Carritos, busquedas, clicks, ventas y pagos
--------------------------------------------------------

INSERT INTO AER_PUNTOVENTA VALUES (1, 'WEB-AURORA', 'Portal web Aurora', NULL, 'Canal digital', 'ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (2, 'APP-AURORA', 'Aplicacion movil Aurora', NULL, 'Canal digital', 'ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (3, 'GUA-MOST-01', 'Mostrador principal salidas internacionales', 1, 'Nivel 3, modulo A', 'ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (4, 'GUA-MOST-02', 'Mostrador conexiones regionales', 1, 'Nivel 2, modulo B', 'ACTIVO');

BEGIN
  FOR i IN 1..25 LOOP
    INSERT INTO AER_CARRITOCOMPRA
    VALUES (i, i, 'SES-202605-' || LPAD(i,5,'0'),
            TIMESTAMP '2026-05-17 08:00:00' + NUMTODSINTERVAL(i*9,'MINUTE'),
            TIMESTAMP '2026-05-17 08:20:00' + NUMTODSINTERVAL(i*9,'MINUTE'),
            CASE WHEN i <= 16 THEN 'CONVERTIDO_RESERVA' WHEN i <= 21 THEN 'EXPIRADO' ELSE 'ACTIVO' END);
    INSERT INTO AER_ITEMCARRITO
    VALUES (i, i, MOD(i-1,48)+1, TO_CHAR(4+MOD(i,10)) || CHR(65+MOD(i,6)),
            CASE WHEN MOD(i,5)=0 THEN 'ejecutiva' ELSE 'economica' END,
            CASE WHEN MOD(i,5)=0 THEN 3250 ELSE 1350 END, 1);
  END LOOP;

  FOR i IN 1..70 LOOP
    INSERT INTO AER_BUSQUEDAVUELO
    VALUES (i, CASE WHEN i <= 60 THEN i ELSE NULL END, 1, 2 + MOD(i,15),
            DATE '2026-05-18' + MOD(i,20), CASE WHEN MOD(i,4)=0 THEN DATE '2026-05-27' + MOD(i,12) ELSE NULL END,
            1 + MOD(i,4), CASE WHEN MOD(i,8)=0 THEN 'ejecutiva' ELSE 'economica' END,
            TIMESTAMP '2026-05-11 07:00:00' + NUMTODSINTERVAL(i*23,'MINUTE'),
            CASE WHEN i <= 42 THEN 'S' ELSE 'N' END);
    INSERT INTO AER_CLICKDESTINO
    VALUES (i, CASE WHEN i <= 60 THEN i ELSE NULL END, 2 + MOD(i,15),
            TIMESTAMP '2026-05-11 07:02:00' + NUMTODSINTERVAL(i*23,'MINUTE'),
            CASE WHEN MOD(i,2)=0 THEN 'web' ELSE 'app' END,
            DATE '2026-05-18' + MOD(i,20), 1 + MOD(i,4),
            CASE WHEN MOD(i,8)=0 THEN 'ejecutiva' ELSE 'economica' END);
  END LOOP;

  FOR i IN 1..80 LOOP
    INSERT INTO AER_VENTABOLETO
    VALUES (i, 'VEN-20260427-' || LPAD(i,5,'0'),
            CASE WHEN MOD(i,4) IN (0,1) THEN NULL ELSE 2 + MOD(i,2) END,
            CASE WHEN MOD(i,4) IN (0,1) THEN NULL ELSE 35 + MOD(i,3) END,
            MOD(i-1,90)+1,
            TIMESTAMP '2026-05-12 08:00:00' + NUMTODSINTERVAL(i*47,'MINUTE'),
            1180 + MOD(i,7)*95,
            ROUND((1180 + MOD(i,7)*95) * 0.12, 2),
            CASE WHEN MOD(i,10)=0 THEN 125 ELSE 0 END,
            (1180 + MOD(i,7)*95) + ROUND((1180 + MOD(i,7)*95) * 0.12, 2) - CASE WHEN MOD(i,10)=0 THEN 125 ELSE 0 END,
            MOD(i-1,5)+1,
            CASE WHEN MOD(i,4)=0 THEN 'web' WHEN MOD(i,4)=1 THEN 'app' WHEN MOD(i,4)=2 THEN 'mostrador' ELSE 'telefono' END,
            CASE WHEN i IN (28,71) THEN 'CANCELADA' ELSE 'COMPLETADA' END);

    INSERT INTO AER_DETALLEVENTABOLETO
    VALUES (i, i, i, 1180 + MOD(i,7)*95, CASE WHEN MOD(i,4)=0 THEN 180 ELSE 75 END);

    INSERT INTO AER_TRANSACCIONPAGO
    VALUES (i, i, MOD(i-1,5)+1,
            (1180 + MOD(i,7)*95) + ROUND((1180 + MOD(i,7)*95) * 0.12, 2) - CASE WHEN MOD(i,10)=0 THEN 125 ELSE 0 END,
            CASE WHEN MOD(i,6)=0 THEN 'USD' ELSE 'GTQ' END,
            TIMESTAMP '2026-05-12 08:02:00' + NUMTODSINTERVAL(i*47,'MINUTE'),
            CASE WHEN i IN (28,71) THEN 'REEMBOLSADA' ELSE 'APROBADA' END,
            'AUTH' || LPAD(i*391, 8, '0'), 'AURPAY-' || LPAD(i, 8, '0'),
            '190.148.' || MOD(i,255) || '.' || (30 + MOD(i,180)),
            CASE WHEN MOD(i,5)=4 THEN 'Pago efectivo en mostrador' ELSE 'Tarjeta terminada en ' || LPAD(MOD(i*73,10000),4,'0') END);

    IF MOD(i,10)=0 THEN
      INSERT INTO AER_USOPROMOCION VALUES (i/10, 2, i, TIMESTAMP '2026-05-12 08:03:00' + NUMTODSINTERVAL(i*47,'MINUTE'), 125);
    END IF;
  END LOOP;
END;
/

--------------------------------------------------------
-- 8. Check-in, tarjetas, equipaje, seguridad y migracion
--------------------------------------------------------

BEGIN
  FOR i IN 1..70 LOOP
    INSERT INTO AER_CHECKIN
    SELECT i, RES_ID_RESERVA, RES_ID_PASAJERO, RES_ID_VUELO,
           TIMESTAMP '2026-05-17 20:00:00' + NUMTODSINTERVAL(i*17,'MINUTE'),
           CASE WHEN MOD(i,4)=0 THEN 'app' WHEN MOD(i,4)=1 THEN 'web' WHEN MOD(i,4)=2 THEN 'kiosko' ELSE 'mostrador' END,
           'COMPLETADO'
    FROM AER_RESERVA WHERE RES_ID_RESERVA = i AND RES_ESTADO <> 'CANCELADA';

    IF SQL%ROWCOUNT = 1 THEN
      INSERT INTO AER_TARJETA_EMBARQUE
      VALUES (i, i, 'QR-AUR-' || LPAD(i,6,'0') || '-GUA', CHR(65 + MOD(i,4)), 'Z' || TO_CHAR(1 + MOD(i,5)),
              TIMESTAMP '2026-05-17 20:01:00' + NUMTODSINTERVAL(i*17,'MINUTE'));

      IF MOD(i,3) <> 0 THEN
        INSERT INTO AER_EQUIPAJE
        SELECT i, RES_ID_PASAJERO, RES_ID_VUELO, 'GUA20260518' || LPAD(i,5,'0'),
               15 + MOD(i,17) + 0.35, CASE WHEN i <= 18 THEN 'ENTREGADO' WHEN i <= 38 THEN 'CARGADO' ELSE 'REGISTRADO' END,
               TIMESTAMP '2026-05-17 20:05:00' + NUMTODSINTERVAL(i*17,'MINUTE')
        FROM AER_RESERVA WHERE RES_ID_RESERVA = i;

        INSERT INTO AER_MOVIMIENTO_EQUIPAJE VALUES (i*10+1, i, 'Mostrador de equipaje GUA', 'REGISTRADO',
          TIMESTAMP '2026-05-17 20:06:00' + NUMTODSINTERVAL(i*17,'MINUTE'), 'Etiqueta impresa y peso validado');
        INSERT INTO AER_MOVIMIENTO_EQUIPAJE VALUES (i*10+2, i, 'Sistema BHS - cinta primaria', 'EN_TRANSITO',
          TIMESTAMP '2026-05-17 20:18:00' + NUMTODSINTERVAL(i*17,'MINUTE'), 'Lectura automatica de codigo de barras');
        IF i <= 38 THEN
          INSERT INTO AER_MOVIMIENTO_EQUIPAJE VALUES (i*10+3, i, 'Bodega aeronave', 'CARGADO',
            TIMESTAMP '2026-05-17 20:42:00' + NUMTODSINTERVAL(i*17,'MINUTE'), 'Cargado por cuadrilla de rampa');
        END IF;
      END IF;

      INSERT INTO AER_CONTROL_SEGURIDAD
      SELECT i, RES_ID_PASAJERO, RES_ID_VUELO, 21 + MOD(i,5),
             CASE WHEN MOD(i,31)=0 THEN 'REVISION' ELSE 'APROBADO' END,
             TIMESTAMP '2026-05-17 21:00:00' + NUMTODSINTERVAL(i*13,'MINUTE'),
             CASE WHEN MOD(i,31)=0 THEN 'Revision secundaria por articulo metalico; liberado sin novedad' ELSE 'Control normal' END
      FROM AER_RESERVA WHERE RES_ID_RESERVA = i;

      INSERT INTO AER_CONTROL_MIGRATORIO
      SELECT i, RES_ID_PASAJERO, RES_ID_VUELO, 26 + MOD(i,4), 'salida',
             CASE WHEN MOD(i,37)=0 THEN 'REVISION' ELSE 'APROBADO' END,
             TIMESTAMP '2026-05-17 21:10:00' + NUMTODSINTERVAL(i*13,'MINUTE'),
             CASE WHEN MOD(i,37)=0 THEN 'Validacion manual de itinerario y documento' ELSE 'Documento vigente' END
      FROM AER_RESERVA WHERE RES_ID_RESERVA = i;
    END IF;
  END LOOP;
END;
/

--------------------------------------------------------
-- 9. Mantenimiento, hangares, repuestos y compras
--------------------------------------------------------

INSERT INTO AER_HANGAR VALUES (1, 'H-A1', 'Hangar Aurora Linea A', 1, 2, 1850, 14.5, 'linea', 'OCUPADO');
INSERT INTO AER_HANGAR VALUES (2, 'H-B2', 'Hangar Aurora Mantenimiento Pesado', 1, 1, 2400, 18.0, 'mantenimiento pesado', 'DISPONIBLE');
INSERT INTO AER_HANGAR VALUES (3, 'H-CG', 'Hangar Carga y Paqueteria', 1, 1, 1600, 16.0, 'carga', 'DISPONIBLE');

INSERT INTO AER_CATEGORIAREPUESTO VALUES (1, 'Avionica', 'Componentes electronicos, sensores y navegacion');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (2, 'Frenos y tren de aterrizaje', 'Llantas, discos, pastillas y actuadores');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (3, 'Cabina y seguridad', 'Extintores, mascarillas, chalecos y asientos');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (4, 'Motor y APU', 'Filtros, sellos, bombas y consumibles de motor');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (5, 'Consumibles certificados', 'Aceites, fluidos, sellantes y hardware aeronautico');

INSERT INTO AER_REPUESTO VALUES (1, 'AVN-ADSB-001', 'Transpondedor ADS-B certificado', 'Unidad de reemplazo para comunicacion vigilancia dependiente', 1, NULL, 'TRX-ADSB-21C', 1, 3, 6, 18500, 'Bodega A-01', 'ACTIVO');
INSERT INTO AER_REPUESTO VALUES (2, 'BRK-737-800-014', 'Kit de frenos Boeing 737NG', 'Juego de discos y pastillas para tren principal', 2, 2, 'B737-BRK-800', 2, 8, 18, 9200, 'Bodega B-04', 'ACTIVO');
INSERT INTO AER_REPUESTO VALUES (3, 'TIR-A320-027', 'Llanta principal A320 family', 'Llanta certificada para A320/A321', 2, 1, 'A320-TIRE-27', 4, 22, 40, 2450, 'Bodega B-02', 'ACTIVO');
INSERT INTO AER_REPUESTO VALUES (4, 'CAB-OXY-040', 'Mascarilla de oxigeno pasajero', 'Unidad PSU para cabina de pasajeros', 3, NULL, 'OXY-MSK-40', 8, 76, 160, 165, 'Bodega C-11', 'ACTIVO');
INSERT INTO AER_REPUESTO VALUES (5, 'ENG-FLT-LEAP-006', 'Filtro combustible LEAP', 'Filtro para motor LEAP en flota neo/MAX', 4, NULL, 'LEAP-FUEL-006', 6, 18, 32, 780, 'Bodega D-03', 'ACTIVO');
INSERT INTO AER_REPUESTO VALUES (6, 'APU-OIL-2380', 'Aceite sintetico turbina 2380', 'Cuarto de aceite aeronautico certificado', 5, NULL, 'MIL-PRF-23699', 12, 48, 96, 62, 'Bodega E-01', 'ACTIVO');

INSERT INTO AER_PROVEEDOR VALUES (1, 'AeroParts Central America, S.A.', '8754312-6', 'Calzada Atanasio Tzul 24-10', 'Zona 12', 'Guatemala', 'Guatemala', 'Guatemala', '+502 2470-1800', 'ventas@aeroparts-ca.com', 'Claudia Rosales', '+502 5510-1818', 'crosales@aeroparts-ca.com', 'ACTIVO', 4.75);
INSERT INTO AER_PROVEEDOR VALUES (2, 'MRO Logistics Miami LLC', 'US-88421077', 'NW 36th Street 5200', 'Doral', 'Miami', 'Florida', 'Estados Unidos', '+1 305 555 0188', 'orders@mrologistics.example', 'Michael Torres', '+1 305 555 0190', 'mtorres@mrologistics.example', 'ACTIVO', 4.60);
INSERT INTO AER_PROVEEDOR VALUES (3, 'Iberia Maintenance Parts', 'ES-B87900211', 'Avenida de la Hispanidad s/n', 'Barajas', 'Madrid', 'Madrid', 'Espana', '+34 91 555 0101', 'parts@iberiamaint.example', 'Elena Vargas', '+34 91 555 0102', 'evargas@iberiamaint.example', 'ACTIVO', 4.82);

INSERT INTO AER_ORDENCOMPRAREPUESTO VALUES (1, 'OC-AUR-2026-0418', 1, DATE '2026-04-18', DATE '2026-04-23', DATE '2026-04-22', 38520, 'RECIBIDA', 39, 'Reposicion por consumo de Semana Santa');
INSERT INTO AER_ORDENCOMPRAREPUESTO VALUES (2, 'OC-AUR-2026-0422', 2, DATE '2026-04-22', DATE '2026-04-29', NULL, 50400, 'EN_TRANSITO', 39, 'Pedido urgente por mantenimiento B737');

INSERT INTO AER_DETALLEORDENCOMPRA VALUES (1, 1, 3, 10, 2450, 24500);
INSERT INTO AER_DETALLEORDENCOMPRA VALUES (2, 1, 6, 80, 62, 4960);
INSERT INTO AER_DETALLEORDENCOMPRA VALUES (3, 1, 4, 55, 165, 9075);
INSERT INTO AER_DETALLEORDENCOMPRA VALUES (4, 2, 2, 4, 9200, 36800);
INSERT INTO AER_DETALLEORDENCOMPRA VALUES (5, 2, 5, 12, 780, 9360);

INSERT INTO AER_MOVIMIENTOREPUESTO VALUES (1, 3, 'ENTRADA', 10, TIMESTAMP '2026-04-22 10:20:00', 39, 'Recepcion OC-AUR-2026-0418', 'OC-AUR-2026-0418');
INSERT INTO AER_MOVIMIENTOREPUESTO VALUES (2, 6, 'ENTRADA', 80, TIMESTAMP '2026-04-22 10:25:00', 39, 'Recepcion OC-AUR-2026-0418', 'OC-AUR-2026-0418');
INSERT INTO AER_MOVIMIENTOREPUESTO VALUES (3, 2, 'SALIDA', 2, TIMESTAMP '2026-04-24 07:45:00', 31, 'Cambio preventivo por inspeccion', 'MAN-0001');
INSERT INTO AER_MOVIMIENTOREPUESTO VALUES (4, 5, 'SALIDA', 1, TIMESTAMP '2026-04-24 08:10:00', 32, 'Cambio filtro combustible', 'MAN-0001');

INSERT INTO AER_ASIGNACIONHANGAR VALUES (1, 1, 13, TIMESTAMP '2026-04-24 06:00:00', TIMESTAMP '2026-04-26 18:00:00', TIMESTAMP '2026-04-26 16:35:00', 'Mantenimiento preventivo tren principal y revision avionica', 920, 55200, 'FINALIZADA');
INSERT INTO AER_ASIGNACIONHANGAR VALUES (2, 2, 17, TIMESTAMP '2026-05-18 02:30:00', TIMESTAMP '2026-05-18 11:30:00', NULL, 'Inspeccion de carga y puerta lateral', 980, NULL, 'ACTIVA');

INSERT INTO AER_MANTENIMIENTOAVION VALUES (1, 13, 'B-check programado', TIMESTAMP '2026-04-24 06:15:00', TIMESTAMP '2026-04-26 18:00:00', TIMESTAMP '2026-04-26 16:20:00', 'Inspeccion de tren principal, cambio de kit de frenos y prueba funcional de avionica. Aeronave liberada con observaciones menores documentadas.', 34, 18400, 19180, 37580, 'FINALIZADO');
INSERT INTO AER_MANTENIMIENTOAVION VALUES (2, 17, 'Inspeccion linea carga', TIMESTAMP '2026-05-18 02:45:00', TIMESTAMP '2026-05-18 11:30:00', NULL, 'Revision de puerta lateral de carga, chequeo hidraulico y prueba de cierre antes de vuelo nocturno.', 33, 6900, 0, 6900, 'EN_PROCESO');

INSERT INTO AER_REPUESTOUTILIZADO VALUES (1, 1, 2, 2);
INSERT INTO AER_REPUESTOUTILIZADO VALUES (2, 1, 5, 1);
INSERT INTO AER_REPUESTOUTILIZADO VALUES (3, 1, 6, 6);

--------------------------------------------------------
-- 10. Incidentes, objetos perdidos, arrestos y auditoria
--------------------------------------------------------

INSERT INTO AER_INCIDENTE VALUES (1, 'clima', 'Celula de tormenta al sur de la pista obligo a espaciar aproximaciones durante 22 minutos.', TIMESTAMP '2026-05-18 14:18:00', 'media', 'CERRADO', 19, 2);
INSERT INTO AER_INCIDENTE VALUES (2, 'equipaje', 'Maleta sin lectura automatica desviada a revision manual y reetiquetada.', TIMESTAMP '2026-05-18 09:42:00', 'baja', 'CERRADO', 11, 24);
INSERT INTO AER_INCIDENTE VALUES (3, 'tecnico', 'Indicacion intermitente en panel de puerta de carga durante preparacion de vuelo DHL.', TIMESTAMP '2026-05-18 03:05:00', 'media', 'EN_PROCESO', 13, 33);
INSERT INTO AER_INCIDENTE VALUES (4, 'pasajero', 'Pasajero con documentacion incompleta derivado a aerolinea y migracion para validacion.', TIMESTAMP '2026-05-18 06:50:00', 'baja', 'CERRADO', 4, 28);

INSERT INTO AER_OBJETOPERDIDO
VALUES (1, 9, 1, 'Mochila negra con laptop Lenovo y cuaderno azul', DATE '2026-05-18', 'Sala A05, fila de espera', 'ENTREGADO',
        'Rosa', 'Maria', 'Paz', 'Molina', '+502 4777-1201', DATE '2026-05-18', 'Rosa', 'Maria', 'Paz', 'Molina');
INSERT INTO AER_OBJETOPERDIDO
VALUES (2, NULL, 1, 'Pasaporte espanol encontrado en control de seguridad', DATE '2026-05-18', 'Filtro internacional 2', 'EN_CUSTODIA',
        'Victor', 'Manuel', 'Reyes', 'Luna', 'seguridad@aurora.gt', NULL, NULL, NULL, NULL, NULL);
INSERT INTO AER_OBJETOPERDIDO
VALUES (3, 15, 1, 'Estuche gris con audifonos inalambricos', DATE '2026-05-17', 'Puerta A11', 'REPORTADO',
        'Camila', 'Sofia', 'Castro', 'Diaz', '+502 4999-3321', NULL, NULL, NULL, NULL, NULL);

INSERT INTO AER_ARRESTO
VALUES (1, 88, NULL, 1, TIMESTAMP '2026-05-18 12:16:00', 'Alteracion del orden en area publica y negativa a retirarse tras advertencia de seguridad',
        'Policia Nacional Civil - Subestacion Aeropuerto', 'La persona fue trasladada al area de seguridad aeroportuaria. No se afecto la operacion de vuelos.', 'Lobby salidas internacionales', 'CERRADO', 'PNC-AUR-2026-0441');

INSERT INTO AER_AUDITORIA VALUES (1, 'AER_VUELO', 'UPDATE', 'ops.supervisor', TIMESTAMP '2026-05-18 14:22:00', '10.20.5.14', 'Estado PROGRAMADO vuelo 19', 'Estado RETRASADO por clima; retraso 45 minutos');
INSERT INTO AER_AUDITORIA VALUES (2, 'AER_EQUIPAJE', 'UPDATE', 'bhs.automatico', TIMESTAMP '2026-05-18 09:44:00', '10.30.7.22', 'Etiqueta sin lectura automatica', 'Reetiquetado y enviado a cinta primaria');
INSERT INTO AER_AUDITORIA VALUES (3, 'AER_MANTENIMIENTOAVION', 'INSERT', 'mant.jefe', TIMESTAMP '2026-05-18 02:46:00', '10.40.1.8', NULL, 'Creada inspeccion linea carga para aeronave D-AEAC');

--------------------------------------------------------
-- 11. Ajuste de identidades para futuros inserts sin ID
--------------------------------------------------------

DECLARE
  PROCEDURE bump_identity(p_table VARCHAR2, p_col VARCHAR2) IS
  BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE ' || p_table || ' MODIFY ' || p_col || ' GENERATED BY DEFAULT ON NULL AS IDENTITY (START WITH LIMIT VALUE)';
  EXCEPTION
    WHEN OTHERS THEN
      DBMS_OUTPUT.PUT_LINE('Aviso: no se pudo ajustar identidad de ' || p_table || '.' || p_col || ': ' || SQLERRM);
  END;
BEGIN
  bump_identity('AER_AEROPUERTO', 'AER_ID');
  bump_identity('AER_TERMINAL', 'TER_ID_TERMINAL');
  bump_identity('AER_PUERTA_EMBARQUE', 'PUE_ID_PUERTA');
  bump_identity('AER_AEROLINEA', 'ARL_ID');
  bump_identity('AER_MODELOAVION', 'MOD_ID_MODELO');
  bump_identity('AER_AVION', 'AVI_ID');
  bump_identity('AER_ASIENTO_AVION', 'ASA_ID_ASIENTO');
  bump_identity('AER_PROGRAMAVUELO', 'PRV_ID');
  bump_identity('AER_DIASVUELO', 'DIA_ID_DIA_VUELO');
  bump_identity('AER_ESCALATECNICA', 'ESC_ID_ESCALA');
  bump_identity('AER_METODOPAGO', 'MET_ID_METODO_PAGO');
  bump_identity('AER_PROMOCION', 'PRO_ID_PROMOCION');
  bump_identity('AER_DEPARTAMENTO', 'DEP_ID_DEPARTAMENTO');
  bump_identity('AER_PUESTO', 'PUE_ID_PUESTO');
  bump_identity('AER_EMPLEADO', 'EMP_ID_EMPLEADO');
  bump_identity('AER_LICENCIAEMPLEADO', 'LIC_ID_LICENCIA');
  bump_identity('AER_ASISTENCIA', 'ASI_ID_ASISTENCIA');
  bump_identity('AER_PLANILLA', 'PLA_ID_PLANILLA');
  bump_identity('AER_PASAJERO', 'PAS_ID_PASAJERO');
  bump_identity('AER_USUARIO_LOGIN', 'USL_ID_USUARIO');
  bump_identity('AER_SESIONUSUARIO', 'SES_ID_SESION');
  bump_identity('AER_PREFERENCIACLIENTE', 'PRF_ID_PREFERENCIA');
  bump_identity('AER_VUELO', 'VUE_ID_VUELO');
  bump_identity('AER_TRIPULACION', 'TRI_ID_TRIPULACION');
  bump_identity('AER_ASIGNACION_PUERTA', 'ASP_ID_ASIGNACION');
  bump_identity('AER_ASIGNACION_ASIENTO', 'AAS_ID_ASIGNACION');
  bump_identity('AER_RESERVA', 'RES_ID_RESERVA');
  bump_identity('AER_CARRITOCOMPRA', 'CAR_ID_CARRITO');
  bump_identity('AER_ITEMCARRITO', 'ITE_ID_ITEM_CARRITO');
  bump_identity('AER_BUSQUEDAVUELO', 'BUS_ID_BUSQUEDA');
  bump_identity('AER_CLICKDESTINO', 'CLI_ID_CLICK');
  bump_identity('AER_PUNTOVENTA', 'PUV_ID_PUNTO_VENTA');
  bump_identity('AER_VENTABOLETO', 'VEN_ID_VENTA');
  bump_identity('AER_DETALLEVENTABOLETO', 'DEV_ID_DETALLE_VENTA');
  bump_identity('AER_TRANSACCIONPAGO', 'TRA_ID_TRANSACCION');
  bump_identity('AER_USOPROMOCION', 'USO_ID_USO');
  bump_identity('AER_CHECKIN', 'CHK_ID_CHECKIN');
  bump_identity('AER_TARJETA_EMBARQUE', 'TAE_ID_TARJETA');
  bump_identity('AER_EQUIPAJE', 'EQU_ID_EQUIPAJE');
  bump_identity('AER_MOVIMIENTO_EQUIPAJE', 'MEQ_ID_MOVIMIENTO');
  bump_identity('AER_CONTROL_SEGURIDAD', 'CSE_ID_CONTROL');
  bump_identity('AER_CONTROL_MIGRATORIO', 'CMI_ID_CONTROL');
  bump_identity('AER_HANGAR', 'HAN_ID_HANGAR');
  bump_identity('AER_ASIGNACIONHANGAR', 'ASH_ID_ASIGNACION');
  bump_identity('AER_CATEGORIAREPUESTO', 'CAT_ID_CATEGORIA');
  bump_identity('AER_REPUESTO', 'REP_ID_REPUESTO');
  bump_identity('AER_PROVEEDOR', 'PRV_ID_PROVEEDOR');
  bump_identity('AER_ORDENCOMPRAREPUESTO', 'ORC_ID_ORDEN_COMPRA');
  bump_identity('AER_DETALLEORDENCOMPRA', 'DET_ID_DETALLE');
  bump_identity('AER_MOVIMIENTOREPUESTO', 'MOV_ID_MOVIMIENTO');
  bump_identity('AER_MANTENIMIENTOAVION', 'MAN_ID_MANTENIMIENTO');
  bump_identity('AER_REPUESTOUTILIZADO', 'RUT_ID_REPUESTO_UTILIZADO');
  bump_identity('AER_INCIDENTE', 'INC_ID_INCIDENTE');
  bump_identity('AER_OBJETOPERDIDO', 'OBJ_ID_OBJETO');
  bump_identity('AER_ARRESTO', 'ARR_ID_ARRESTO');
  bump_identity('AER_AUDITORIA', 'AUD_ID_AUDITORIA');
END;
/

COMMIT;

PROMPT Seed Aeropuerto Aurora cargada: red aeroportuaria, flota, empleados, pasajeros, vuelos, ventas, equipaje, seguridad, mantenimiento y auditoria.

-- 1. Catalogos globales y expansion de red
--------------------------------------------------------

DECLARE
  v_programa_id NUMBER;
  v_vuelo_id NUMBER;
  v_fecha DATE;
  v_salida TIMESTAMP;
  v_avion NUMBER;
  v_puerta NUMBER;
  v_dia_semana NUMBER;
  v_capacidad NUMBER;
  v_ocupadas NUMBER;
  v_estado VARCHAR2(20);
  v_air_yyz NUMBER;
  v_air_ord NUMBER;
  v_air_lhr NUMBER;
  v_air_cdg NUMBER;
  v_air_ams NUMBER;
  v_air_fra NUMBER;
  v_air_fco NUMBER;
  v_air_gru NUMBER;
  v_air_eze NUMBER;
  v_air_scl NUMBER;
  v_air_uio NUMBER;
  v_air_sdq NUMBER;
  v_air_hav NUMBER;
  v_air_sju NUMBER;
  v_air_nrt NUMBER;
  v_air_icn NUMBER;
  v_air_dxb NUMBER;
  v_air_doh NUMBER;
  v_air_syd NUMBER;
  v_air_akl NUMBER;
  v_air_svo NUMBER;
  v_air_led NUMBER;
  v_air_lis NUMBER;
  v_air_bcn NUMBER;
  v_air_muc NUMBER;
  v_air_zrh NUMBER;
  v_air_bru NUMBER;
  v_air_vie NUMBER;
  v_air_ist NUMBER;
  v_air_cai NUMBER;
  v_air_jnb NUMBER;
  v_air_cmn NUMBER;
  v_air_sin NUMBER;
  v_air_hkg NUMBER;
  v_air_del NUMBER;
  v_air_bom NUMBER;
  v_air_bsb NUMBER;
  v_air_rec NUMBER;
  v_air_mvd NUMBER;
  v_air_gig NUMBER;
  v_air_bos NUMBER;
  v_air_sea NUMBER;
  v_air_dfw NUMBER;
  v_air_iad NUMBER;
  v_air_mco NUMBER;
  v_air_las NUMBER;
  v_air_sfo NUMBER;
  v_air_den NUMBER;
  v_air_clt NUMBER;
  v_air_ccs NUMBER;
  v_air_mde NUMBER;
  v_air_cnf NUMBER;
  v_air_for NUMBER;
  v_air_asu NUMBER;
  v_air_cwb NUMBER;
  v_air_dub NUMBER;
  v_air_arn NUMBER;
  v_air_cph NUMBER;
  v_air_hel NUMBER;
  v_air_gva NUMBER;
  v_air_bkk NUMBER;
  v_air_kul NUMBER;
  v_air_mnl NUMBER;
  v_air_tpe NUMBER;
  v_air_pvg NUMBER;
  v_air_nbo NUMBER;
  v_air_add NUMBER;
  v_air_mel NUMBER;

  FUNCTION avion_por_aerolinea(p_aerolinea NUMBER, p_vuelo NUMBER) RETURN NUMBER IS
  BEGIN
    RETURN CASE p_aerolinea
      WHEN 1 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 18 ELSE 1 END
      WHEN 2 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 12 ELSE 2 END
      WHEN 3 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 25 ELSE 3 END
      WHEN 4 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 14 ELSE 4 END
      WHEN 5 THEN 5
      WHEN 6 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 15 ELSE 6 END
      WHEN 7 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 26 ELSE 7 END
      WHEN 8 THEN 8
      WHEN 9 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 16 ELSE 9 END
      WHEN 10 THEN 10
      WHEN 11 THEN 11
      WHEN 12 THEN 17
      WHEN 13 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 20 ELSE 19 END
      WHEN 14 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 22 ELSE 21 END
      WHEN 15 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 24 ELSE 23 END
      WHEN 16 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 28 ELSE 27 END
      WHEN 17 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 30 ELSE 29 END
      WHEN 18 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 32 ELSE 31 END
      WHEN 19 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 34 ELSE 33 END
      WHEN 20 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 36 ELSE 35 END
      WHEN 21 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 38 ELSE 37 END
      WHEN 22 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 40 ELSE 39 END
      WHEN 23 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 42 ELSE 41 END
      WHEN 24 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 44 ELSE 43 END
      WHEN 25 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 46 ELSE 45 END
      WHEN 26 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 48 ELSE 47 END
      WHEN 27 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 50 ELSE 49 END
      WHEN 28 THEN CASE WHEN MOD(p_vuelo, 2) = 0 THEN 52 ELSE 51 END
      ELSE 1
    END;
  END;

  PROCEDURE asegurar_aeropuerto(
    p_codigo VARCHAR2,
    p_nombre VARCHAR2,
    p_ciudad VARCHAR2,
    p_pais VARCHAR2,
    p_zona VARCHAR2,
    p_tipo VARCHAR2,
    p_latitud NUMBER,
    p_longitud NUMBER,
    p_iata VARCHAR2,
    p_icao VARCHAR2,
    p_id OUT NUMBER
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
       p_latitud, p_longitud, p_iata, p_icao, DATE '2026-05-18')
      RETURNING AER_ID INTO p_id;
  END;

  PROCEDURE asegurar_aerolinea(
    p_id NUMBER,
    p_codigo VARCHAR2,
    p_nombre VARCHAR2,
    p_pais VARCHAR2,
    p_iata VARCHAR2,
    p_icao VARCHAR2,
    p_telefono VARCHAR2,
    p_email VARCHAR2,
    p_web VARCHAR2
  ) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM AER_AEROLINEA
    WHERE ARL_ID = p_id;

    IF v_count = 0 THEN
      INSERT INTO AER_AEROLINEA
      VALUES (p_id, p_codigo, p_nombre, p_pais, p_iata, p_icao, 'ACTIVA', p_telefono, p_email, p_web, DATE '2026-05-18');
    END IF;
  END;

  PROCEDURE asegurar_avion(
    p_id NUMBER,
    p_matricula VARCHAR2,
    p_modelo NUMBER,
    p_aerolinea NUMBER,
    p_anio NUMBER,
    p_horas NUMBER
  ) IS
    v_count NUMBER;
  BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM AER_AVION
    WHERE AVI_ID = p_id;

    IF v_count = 0 THEN
      INSERT INTO AER_AVION
      VALUES (p_id, p_matricula, p_modelo, p_aerolinea, p_anio, 'ACTIVO', DATE '2026-05-10', DATE '2026-08-10', p_horas);
    END IF;
  END;

  PROCEDURE asegurar_asientos(p_avion NUMBER) IS
    v_count NUMBER;
    v_letters VARCHAR2(6) := 'ABCDEF';
    v_class VARCHAR2(15);
  BEGIN
    SELECT COUNT(*)
    INTO v_count
    FROM AER_ASIENTO_AVION
    WHERE ASA_ID_AVION = p_avion;

    IF v_count = 0 THEN
      FOR fila IN 1..26 LOOP
        FOR pos IN 1..6 LOOP
          v_class := CASE
            WHEN fila = 1 AND p_avion IN (19, 21, 27, 29, 31, 33, 37, 39, 43, 49, 51) THEN 'primera'
            WHEN fila <= 3 THEN 'ejecutiva'
            ELSE 'economica'
          END;

          INSERT INTO AER_ASIENTO_AVION
          (ASA_ID_ASIENTO, ASA_ID_AVION, ASA_CODIGO, ASA_CLASE, ASA_ESTADO)
          VALUES
          (p_avion * 10000 + fila * 10 + pos, p_avion, TO_CHAR(fila) || SUBSTR(v_letters, pos, 1), v_class, 'DISPONIBLE');
        END LOOP;
      END LOOP;
    END IF;
  END;

  PROCEDURE crear_ruta(
    p_numero VARCHAR2,
    p_aerolinea NUMBER,
    p_origen NUMBER,
    p_destino NUMBER,
    p_salida VARCHAR2,
    p_llegada VARCHAR2,
    p_duracion NUMBER,
    p_tipo VARCHAR2,
    p_dias VARCHAR2,
    p_escala_aer NUMBER DEFAULT NULL,
    p_escala_llega VARCHAR2 DEFAULT NULL,
    p_escala_sale VARCHAR2 DEFAULT NULL,
    p_escala_min NUMBER DEFAULT NULL
  ) IS
  BEGIN
    INSERT INTO AER_PROGRAMAVUELO
    (PRV_NUMERO_VUELO, PRV_ID_AEROLINEA, PRV_ID_AEROPUERTO_ORIGEN, PRV_ID_AEROPUERTO_DESTINO,
     PRV_HORA_SALIDA_PROGRAMADA, PRV_HORA_LLEGADA_PROGRAMADA, PRV_DURACION_ESTIMADA,
     PRV_TIPO_VUELO, PRV_ESTADO, PRV_FECHA_CREACION)
    VALUES
    (p_numero, p_aerolinea, p_origen, p_destino, p_salida, p_llegada, p_duracion,
     p_tipo, 'ACTIVO', DATE '2026-05-18')
    RETURNING PRV_ID INTO v_programa_id;

    FOR d IN 1..7 LOOP
      IF INSTR(p_dias, TO_CHAR(d)) > 0 THEN
        INSERT INTO AER_DIASVUELO (DIA_ID_PROGRAMA_VUELO, DIA_DIA_SEMANA)
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

    v_fecha := DATE '2026-05-18';
    WHILE v_fecha <= DATE '2026-12-31' LOOP
      v_dia_semana := TRUNC(v_fecha) - TRUNC(v_fecha, 'IW') + 1;

      IF INSTR(p_dias, TO_CHAR(v_dia_semana)) > 0 THEN
        v_avion := avion_por_aerolinea(p_aerolinea, TO_NUMBER(TO_CHAR(v_fecha, 'DDD')));

        SELECT ma.MOD_CAPACIDAD_PASAJEROS
        INTO v_capacidad
        FROM AER_AVION av
        JOIN AER_MODELOAVION ma ON ma.MOD_ID_MODELO = av.AVI_ID_MODELO
        WHERE av.AVI_ID = v_avion;

        v_estado := CASE
          WHEN MOD(TO_NUMBER(TO_CHAR(v_fecha, 'DDD')) + v_avion + p_origen + p_destino, 113) = 0 THEN 'CANCELADO'
          WHEN MOD(TO_NUMBER(TO_CHAR(v_fecha, 'DDD')) + v_avion + p_origen, 37) = 0 THEN 'RETRASADO'
          ELSE 'PROGRAMADO'
        END;

        v_ocupadas := CASE
          WHEN v_capacidad = 0 THEN 0
          WHEN v_estado = 'CANCELADO' THEN 0
          ELSE LEAST(v_capacidad - 4, 22 + MOD((TO_NUMBER(TO_CHAR(v_fecha, 'DDD')) * 13) + v_avion + p_origen, GREATEST(v_capacidad - 22, 1)))
        END;

        INSERT INTO AER_VUELO
        (VUE_ID_PROGRAMA_VUELO, VUE_ID_AVION, VUE_FECHA_VUELO, VUE_HORA_SALIDA_REAL, VUE_HORA_LLEGADA_REAL,
         VUE_PLAZAS_OCUPADAS, VUE_PLAZAS_VACIAS, VUE_ESTADO, VUE_FECHA_REPROGRAMACION, VUE_MOTIVO_CANCELACION, VUE_RETRASO_MINUTOS)
        VALUES
        (v_programa_id, v_avion, v_fecha, NULL, NULL,
         v_ocupadas, GREATEST(v_capacidad - v_ocupadas, 0), v_estado,
         CASE WHEN v_estado = 'RETRASADO' THEN v_fecha ELSE NULL END,
         CASE WHEN v_estado = 'CANCELADO' THEN 'Rotacion de flota o restriccion operacional' ELSE NULL END,
         CASE WHEN v_estado = 'RETRASADO' THEN 35 ELSE 0 END)
        RETURNING VUE_ID_VUELO INTO v_vuelo_id;

        IF p_origen = 1 OR p_destino = 1 THEN
          v_salida := TO_TIMESTAMP(TO_CHAR(v_fecha, 'YYYY-MM-DD') || ' ' || p_salida, 'YYYY-MM-DD HH24:MI');
          v_puerta := CASE
            WHEN p_tipo IN ('nacional', 'regional') THEN 101 + MOD(v_vuelo_id, 6)
            WHEN p_tipo = 'carga' THEN CASE WHEN MOD(v_vuelo_id, 2) = 0 THEN 201 ELSE 202 END
            ELSE 1 + MOD(v_vuelo_id, 14)
          END;

          INSERT INTO AER_ASIGNACION_PUERTA
          (ASP_ID_VUELO, ASP_ID_PUERTA, ASP_FECHA_HORA_INICIO, ASP_FECHA_HORA_FIN, ASP_ESTADO)
          VALUES
          (v_vuelo_id, v_puerta, v_salida - NUMTODSINTERVAL(90, 'MINUTE'), NULL,
           CASE WHEN v_estado = 'CANCELADO' THEN 'CANCELADA' ELSE 'PROGRAMADA' END);
        END IF;
      END IF;

      v_fecha := v_fecha + 1;
    END LOOP;
  END;
BEGIN
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
  asegurar_aeropuerto('SVO', 'Sheremetyevo International Airport', 'Moscu', 'Rusia', 'Europe/Moscow', 'intercontinental', 55.972600, 37.414600, 'SVO', 'UUEE', v_air_svo);
  asegurar_aeropuerto('LED', 'Pulkovo Airport', 'San Petersburgo', 'Rusia', 'Europe/Moscow', 'internacional', 59.800300, 30.262500, 'LED', 'ULLI', v_air_led);
  asegurar_aeropuerto('LIS', 'Humberto Delgado Airport', 'Lisboa', 'Portugal', 'Europe/Lisbon', 'intercontinental', 38.774200, -9.134200, 'LIS', 'LPPT', v_air_lis);
  asegurar_aeropuerto('BCN', 'Barcelona El Prat Airport', 'Barcelona', 'Espana', 'Europe/Madrid', 'intercontinental', 41.297400, 2.083300, 'BCN', 'LEBL', v_air_bcn);
  asegurar_aeropuerto('MUC', 'Munich Airport', 'Munich', 'Alemania', 'Europe/Berlin', 'hub', 48.353800, 11.786100, 'MUC', 'EDDM', v_air_muc);
  asegurar_aeropuerto('ZRH', 'Zurich Airport', 'Zurich', 'Suiza', 'Europe/Zurich', 'hub', 47.458100, 8.555500, 'ZRH', 'LSZH', v_air_zrh);
  asegurar_aeropuerto('BRU', 'Brussels Airport', 'Bruselas', 'Belgica', 'Europe/Brussels', 'hub', 50.901400, 4.484400, 'BRU', 'EBBR', v_air_bru);
  asegurar_aeropuerto('VIE', 'Vienna International Airport', 'Viena', 'Austria', 'Europe/Vienna', 'hub', 48.110300, 16.569700, 'VIE', 'LOWW', v_air_vie);
  asegurar_aeropuerto('IST', 'Istanbul Airport', 'Estambul', 'Turquia', 'Europe/Istanbul', 'hub', 41.275300, 28.751900, 'IST', 'LTFM', v_air_ist);
  asegurar_aeropuerto('CAI', 'Cairo International Airport', 'El Cairo', 'Egipto', 'Africa/Cairo', 'intercontinental', 30.121900, 31.405600, 'CAI', 'HECA', v_air_cai);
  asegurar_aeropuerto('JNB', 'OR Tambo International Airport', 'Johannesburgo', 'Sudafrica', 'Africa/Johannesburg', 'intercontinental', -26.133700, 28.242000, 'JNB', 'FAOR', v_air_jnb);
  asegurar_aeropuerto('CMN', 'Mohammed V International Airport', 'Casablanca', 'Marruecos', 'Africa/Casablanca', 'intercontinental', 33.367500, -7.589970, 'CMN', 'GMMN', v_air_cmn);
  asegurar_aeropuerto('SIN', 'Singapore Changi Airport', 'Singapur', 'Singapur', 'Asia/Singapore', 'intercontinental', 1.364400, 103.991500, 'SIN', 'WSSS', v_air_sin);
  asegurar_aeropuerto('HKG', 'Hong Kong International Airport', 'Hong Kong', 'China', 'Asia/Hong_Kong', 'intercontinental', 22.308000, 113.918500, 'HKG', 'VHHH', v_air_hkg);
  asegurar_aeropuerto('DEL', 'Indira Gandhi International Airport', 'Nueva Delhi', 'India', 'Asia/Kolkata', 'intercontinental', 28.556200, 77.100000, 'DEL', 'VIDP', v_air_del);
  asegurar_aeropuerto('BOM', 'Chhatrapati Shivaji Maharaj International Airport', 'Mumbai', 'India', 'Asia/Kolkata', 'intercontinental', 19.089600, 72.865600, 'BOM', 'VABB', v_air_bom);
  asegurar_aeropuerto('BSB', 'Brasilia International Airport', 'Brasilia', 'Brasil', 'America/Sao_Paulo', 'hub', -15.869700, -47.920800, 'BSB', 'SBBR', v_air_bsb);
  asegurar_aeropuerto('REC', 'Recife Guararapes International Airport', 'Recife', 'Brasil', 'America/Recife', 'internacional', -8.126500, -34.923600, 'REC', 'SBRF', v_air_rec);
  asegurar_aeropuerto('MVD', 'Carrasco International Airport', 'Montevideo', 'Uruguay', 'America/Montevideo', 'internacional', -34.838400, -56.030800, 'MVD', 'SUMU', v_air_mvd);
  asegurar_aeropuerto('GIG', 'Rio de Janeiro Galeao International Airport', 'Rio de Janeiro', 'Brasil', 'America/Sao_Paulo', 'hub', -22.809000, -43.250600, 'GIG', 'SBGL', v_air_gig);
  asegurar_aeropuerto('BOS', 'Logan International Airport', 'Boston', 'Estados Unidos', 'America/New_York', 'hub', 42.365600, -71.009600, 'BOS', 'KBOS', v_air_bos);
  asegurar_aeropuerto('SEA', 'Seattle Tacoma International Airport', 'Seattle', 'Estados Unidos', 'America/Los_Angeles', 'hub', 47.450200, -122.308800, 'SEA', 'KSEA', v_air_sea);
  asegurar_aeropuerto('DFW', 'Dallas Fort Worth International Airport', 'Dallas', 'Estados Unidos', 'America/Chicago', 'hub', 32.899800, -97.040300, 'DFW', 'KDFW', v_air_dfw);
  asegurar_aeropuerto('IAD', 'Washington Dulles International Airport', 'Washington', 'Estados Unidos', 'America/New_York', 'hub', 38.953100, -77.456500, 'IAD', 'KIAD', v_air_iad);
  asegurar_aeropuerto('MCO', 'Orlando International Airport', 'Orlando', 'Estados Unidos', 'America/New_York', 'turistico', 28.431200, -81.308100, 'MCO', 'KMCO', v_air_mco);
  asegurar_aeropuerto('LAS', 'Harry Reid International Airport', 'Las Vegas', 'Estados Unidos', 'America/Los_Angeles', 'turistico', 36.084000, -115.153700, 'LAS', 'KLAS', v_air_las);
  asegurar_aeropuerto('SFO', 'San Francisco International Airport', 'San Francisco', 'Estados Unidos', 'America/Los_Angeles', 'hub', 37.621300, -122.379000, 'SFO', 'KSFO', v_air_sfo);
  asegurar_aeropuerto('DEN', 'Denver International Airport', 'Denver', 'Estados Unidos', 'America/Denver', 'hub', 39.856100, -104.673700, 'DEN', 'KDEN', v_air_den);
  asegurar_aeropuerto('CLT', 'Charlotte Douglas International Airport', 'Charlotte', 'Estados Unidos', 'America/New_York', 'hub', 35.214400, -80.947300, 'CLT', 'KCLT', v_air_clt);
  asegurar_aeropuerto('CCS', 'Aeropuerto Internacional Simon Bolivar', 'Caracas', 'Venezuela', 'America/Caracas', 'internacional', 10.603100, -66.991200, 'CCS', 'SVMI', v_air_ccs);
  asegurar_aeropuerto('MDE', 'Aeropuerto Internacional Jose Maria Cordova', 'Medellin', 'Colombia', 'America/Bogota', 'internacional', 6.164500, -75.423100, 'MDE', 'SKRG', v_air_mde);
  asegurar_aeropuerto('CNF', 'Aeroporto Internacional de Belo Horizonte', 'Belo Horizonte', 'Brasil', 'America/Sao_Paulo', 'internacional', -19.624400, -43.971900, 'CNF', 'SBCF', v_air_cnf);
  asegurar_aeropuerto('FOR', 'Pinto Martins International Airport', 'Fortaleza', 'Brasil', 'America/Fortaleza', 'internacional', -3.776300, -38.532600, 'FOR', 'SBFZ', v_air_for);
  asegurar_aeropuerto('ASU', 'Aeropuerto Internacional Silvio Pettirossi', 'Asuncion', 'Paraguay', 'America/Asuncion', 'internacional', -25.239900, -57.519100, 'ASU', 'SGAS', v_air_asu);
  asegurar_aeropuerto('CWB', 'Afonso Pena International Airport', 'Curitiba', 'Brasil', 'America/Sao_Paulo', 'internacional', -25.528500, -49.175800, 'CWB', 'SBCT', v_air_cwb);
  asegurar_aeropuerto('DUB', 'Dublin Airport', 'Dublin', 'Irlanda', 'Europe/Dublin', 'intercontinental', 53.421300, -6.270100, 'DUB', 'EIDW', v_air_dub);
  asegurar_aeropuerto('ARN', 'Stockholm Arlanda Airport', 'Estocolmo', 'Suecia', 'Europe/Stockholm', 'hub', 59.649800, 17.923800, 'ARN', 'ESSA', v_air_arn);
  asegurar_aeropuerto('CPH', 'Copenhagen Airport', 'Copenhague', 'Dinamarca', 'Europe/Copenhagen', 'hub', 55.618100, 12.656100, 'CPH', 'EKCH', v_air_cph);
  asegurar_aeropuerto('HEL', 'Helsinki Airport', 'Helsinki', 'Finlandia', 'Europe/Helsinki', 'hub', 60.317200, 24.963300, 'HEL', 'EFHK', v_air_hel);
  asegurar_aeropuerto('GVA', 'Geneva Airport', 'Ginebra', 'Suiza', 'Europe/Zurich', 'internacional', 46.238100, 6.109000, 'GVA', 'LSGG', v_air_gva);
  asegurar_aeropuerto('BKK', 'Suvarnabhumi Airport', 'Bangkok', 'Tailandia', 'Asia/Bangkok', 'intercontinental', 13.690000, 100.750100, 'BKK', 'VTBS', v_air_bkk);
  asegurar_aeropuerto('KUL', 'Kuala Lumpur International Airport', 'Kuala Lumpur', 'Malasia', 'Asia/Kuala_Lumpur', 'intercontinental', 2.745600, 101.709900, 'KUL', 'WMKK', v_air_kul);
  asegurar_aeropuerto('MNL', 'Ninoy Aquino International Airport', 'Manila', 'Filipinas', 'Asia/Manila', 'intercontinental', 14.508600, 121.019800, 'MNL', 'RPLL', v_air_mnl);
  asegurar_aeropuerto('TPE', 'Taiwan Taoyuan International Airport', 'Taoyuan', 'Taiwan', 'Asia/Taipei', 'intercontinental', 25.079700, 121.234200, 'TPE', 'RCTP', v_air_tpe);
  asegurar_aeropuerto('PVG', 'Shanghai Pudong International Airport', 'Shanghai', 'China', 'Asia/Shanghai', 'intercontinental', 31.144300, 121.808300, 'PVG', 'ZSPD', v_air_pvg);
  asegurar_aeropuerto('NBO', 'Jomo Kenyatta International Airport', 'Nairobi', 'Kenia', 'Africa/Nairobi', 'intercontinental', -1.319200, 36.927500, 'NBO', 'HKJK', v_air_nbo);
  asegurar_aeropuerto('ADD', 'Addis Ababa Bole International Airport', 'Addis Ababa', 'Etiopia', 'Africa/Addis_Ababa', 'intercontinental', 8.977900, 38.799300, 'ADD', 'HAAB', v_air_add);
  asegurar_aeropuerto('MEL', 'Melbourne Airport', 'Melbourne', 'Australia', 'Australia/Melbourne', 'intercontinental', -37.669000, 144.841000, 'MEL', 'YMML', v_air_mel);

  asegurar_aerolinea(13, 'AF', 'Air France', 'Francia', 'AF', 'AFR', '+33 892 70 26 54', 'hub.cdg@airfrance.fr', 'https://wwws.airfrance.fr');
  asegurar_aerolinea(14, 'SU', 'Aeroflot', 'Rusia', 'SU', 'AFL', '+7 495 223-55-55', 'network@aeroflot.ru', 'https://www.aeroflot.ru');
  asegurar_aerolinea(15, 'LA', 'LATAM Brasil', 'Brasil', 'LA', 'TAM', '+55 11 4002-5700', 'operacoes@latam.com', 'https://www.latamairlines.com');
  asegurar_aerolinea(16, 'BA', 'British Airways', 'Reino Unido', 'BA', 'BAW', '+44 344 493 0787', 'ops@britishairways.com', 'https://www.britishairways.com');
  asegurar_aerolinea(17, 'EK', 'Emirates', 'Emiratos Arabes Unidos', 'EK', 'UAE', '+971 600 555555', 'network@emirates.com', 'https://www.emirates.com');
  asegurar_aerolinea(18, 'QR', 'Qatar Airways', 'Qatar', 'QR', 'QTR', '+974 4023 0000', 'ops@qatarairways.com', 'https://www.qatarairways.com');
  asegurar_aerolinea(19, 'LH', 'Lufthansa', 'Alemania', 'LH', 'DLH', '+49 69 86 799 799', 'network@lufthansa.com', 'https://www.lufthansa.com');
  asegurar_aerolinea(20, 'TP', 'TAP Air Portugal', 'Portugal', 'TP', 'TAP', '+351 211 234 400', 'ops@tap.pt', 'https://www.flytap.com');
  asegurar_aerolinea(21, 'AC', 'Air Canada', 'Canada', 'AC', 'ACA', '+1 888 247 2262', 'ops@aircanada.ca', 'https://www.aircanada.com');
  asegurar_aerolinea(22, 'KL', 'KLM', 'Paises Bajos', 'KL', 'KLM', '+31 20 474 7747', 'ops@klm.com', 'https://www.klm.com');
  asegurar_aerolinea(23, 'AZ', 'ITA Airways', 'Italia', 'AZ', 'ITY', '+39 06 85960020', 'ops@ita-airways.com', 'https://www.ita-airways.com');
  asegurar_aerolinea(24, 'TK', 'Turkish Airlines', 'Turquia', 'TK', 'THY', '+90 212 463 63 63', 'ops@thy.com', 'https://www.turkishairlines.com');
  asegurar_aerolinea(25, 'G3', 'Gol Linhas Aereas', 'Brasil', 'G3', 'GLO', '+55 11 5504 4410', 'ops@voegol.com.br', 'https://www.voegol.com.br');
  asegurar_aerolinea(26, 'AR', 'Aerolineas Argentinas', 'Argentina', 'AR', 'ARG', '+54 11 5199 3555', 'ops@aerolineas.com.ar', 'https://www.aerolineas.com.ar');
  asegurar_aerolinea(27, 'NH', 'All Nippon Airways', 'Japon', 'NH', 'ANA', '+81 3 6735 1111', 'network@ana.co.jp', 'https://www.ana.co.jp');
  asegurar_aerolinea(28, 'CX', 'Cathay Pacific', 'Hong Kong', 'CX', 'CPA', '+852 2747 3333', 'ops@cathaypacific.com', 'https://www.cathaypacific.com');

  asegurar_avion(19, 'F-GZNE', 7, 13, 2020, 9540);
  asegurar_avion(20, 'F-HXLA', 4, 13, 2021, 7215);
  asegurar_avion(21, 'RA-73142', 7, 14, 2019, 10180);
  asegurar_avion(22, 'RA-73728', 2, 14, 2018, 11460);
  asegurar_avion(23, 'PT-MXF', 4, 15, 2022, 6880);
  asegurar_avion(24, 'PR-XMR', 5, 15, 2021, 6440);
  asegurar_avion(25, 'N909AA', 4, 3, 2021, 8025);
  asegurar_avion(26, 'EC-NGG', 7, 7, 2020, 9760);
  asegurar_avion(27, 'G-ZBLA', 7, 16, 2019, 9905);
  asegurar_avion(28, 'G-NEOV', 4, 16, 2021, 7745);
  asegurar_avion(29, 'A6-EQT', 7, 17, 2020, 11015);
  asegurar_avion(30, 'A6-EKV', 4, 17, 2022, 7050);
  asegurar_avion(31, 'A7-BHB', 7, 18, 2021, 10340);
  asegurar_avion(32, 'A7-AHU', 4, 18, 2022, 7450);
  asegurar_avion(33, 'D-AIXN', 7, 19, 2019, 9980);
  asegurar_avion(34, 'D-ABKT', 2, 19, 2018, 12740);
  asegurar_avion(35, 'CS-TXD', 4, 20, 2021, 6985);
  asegurar_avion(36, 'CS-TVK', 5, 20, 2020, 6150);
  asegurar_avion(37, 'C-GHPY', 7, 21, 2020, 10020);
  asegurar_avion(38, 'C-FNND', 4, 21, 2022, 7420);
  asegurar_avion(39, 'PH-BVA', 7, 22, 2019, 9825);
  asegurar_avion(40, 'PH-EXW', 4, 22, 2021, 7150);
  asegurar_avion(41, 'EI-HJN', 4, 23, 2022, 6885);
  asegurar_avion(42, 'EI-DTI', 5, 23, 2021, 6210);
  asegurar_avion(43, 'TC-LPA', 7, 24, 2020, 10980);
  asegurar_avion(44, 'TC-JRO', 4, 24, 2022, 7340);
  asegurar_avion(45, 'PR-GUF', 4, 25, 2021, 7640);
  asegurar_avion(46, 'PS-GOL', 5, 25, 2020, 6125);
  asegurar_avion(47, 'LV-FUR', 4, 26, 2021, 6980);
  asegurar_avion(48, 'LV-CHQ', 5, 26, 2020, 6030);
  asegurar_avion(49, 'JA873A', 7, 27, 2020, 10160);
  asegurar_avion(50, 'JA221A', 4, 27, 2022, 7190);
  asegurar_avion(51, 'B-KQX', 7, 28, 2020, 10085);
  asegurar_avion(52, 'B-LCY', 4, 28, 2021, 7280);

  FOR i IN 19..52 LOOP
    asegurar_asientos(i);
  END LOOP;

  crear_ruta('CM392M', 2, 6, 1, '18:30', '19:54', 144, 'internacional', '1234567');
  crear_ruta('AA1189M', 3, 9, 1, '19:10', '20:05', 175, 'internacional', '1234567');
  crear_ruta('UA1902M', 4, 10, 1, '09:10', '11:02', 172, 'internacional', '1234567');
  crear_ruta('DL1831M', 5, 16, 1, '20:20', '21:18', 178, 'internacional', '1234567');
  crear_ruta('AM678M', 6, 7, 1, '11:35', '13:45', 130, 'internacional', '1234567');
  crear_ruta('IB6341M', 7, 15, 1, '12:20', '16:20', 720, 'intercontinental', '1357', 9, '15:10', '16:05', 55);
  crear_ruta('Y43930M', 8, 8, 1, '13:05', '13:57', 112, 'turistico', '1234567');
  crear_ruta('5U111M', 9, 2, 1, '08:05', '09:05', 60, 'nacional', '1234567');
  crear_ruta('5U212M', 9, 3, 1, '09:45', '10:40', 55, 'regional', '1234567');
  crear_ruta('NK516M', 10, 11, 1, '17:40', '22:42', 182, 'internacional', '1234567');
  crear_ruta('B62030M', 11, 12, 1, '07:25', '10:28', 243, 'internacional', '1234567', 9, '08:55', '09:40', 45);
  crear_ruta('D0255M', 12, 9, 1, '04:15', '05:58', 163, 'carga', '1357', 10, '04:55', '05:35', 40);
  crear_ruta('AV744M', 1, 13, 1, '20:15', '23:30', 195, 'internacional', '1234567');
  crear_ruta('CM408M', 2, 5, 1, '13:10', '15:05', 115, 'regional', '1234567');
  crear_ruta('AM670M', 6, 7, 1, '21:45', '23:55', 130, 'internacional', '1234567');
  crear_ruta('CM701M', 2, 1, 6, '12:25', '14:50', 145, 'internacional', '1234567');
  crear_ruta('CM702M', 2, 6, 1, '16:05', '17:30', 145, 'internacional', '1234567');
  crear_ruta('AM901M', 6, 1, 7, '06:30', '08:45', 135, 'internacional', '1234567');
  crear_ruta('AM902M', 6, 7, 1, '14:15', '16:25', 130, 'internacional', '1234567');
  crear_ruta('AA882M', 3, 1, 9, '08:25', '13:00', 155, 'internacional', '1234567');
  crear_ruta('AA883M', 3, 9, 1, '14:35', '15:35', 180, 'internacional', '1234567');
  crear_ruta('UA1563M', 4, 1, 10, '13:50', '17:35', 165, 'internacional', '1234567');
  crear_ruta('UA1564M', 4, 10, 1, '18:55', '20:45', 170, 'internacional', '1234567');
  crear_ruta('DL1845M', 5, 1, 16, '07:05', '11:45', 160, 'internacional', '1234567');
  crear_ruta('DL1846M', 5, 16, 1, '12:55', '14:00', 185, 'internacional', '1234567');
  crear_ruta('AV129M', 1, 1, 13, '05:35', '08:55', 200, 'internacional', '1234567');
  crear_ruta('AV130M', 1, 13, 1, '10:15', '13:35', 200, 'internacional', '1234567');
  crear_ruta('AV751M', 1, 1, 14, '11:15', '15:05', 230, 'internacional', '1234567', 13, '13:05', '13:50', 45);
  crear_ruta('AV752M', 1, 14, 1, '16:20', '20:05', 225, 'internacional', '1234567', 13, '18:10', '18:55', 45);
  crear_ruta('UA3100M', 4, 1, v_air_ord, '08:40', '13:45', 245, 'internacional', '1234567', 10, '11:05', '11:55', 50);
  crear_ruta('UA3101M', 4, v_air_ord, 1, '15:30', '20:45', 255, 'internacional', '1234567', 10, '17:50', '18:40', 50);
  crear_ruta('AV2200M', 1, 1, v_air_yyz, '09:15', '16:25', 370, 'internacional', '1357', 9, '12:20', '13:20', 60);
  crear_ruta('AV2201M', 1, v_air_yyz, 1, '18:05', '23:45', 340, 'internacional', '1357', 9, '20:25', '21:15', 50);
  crear_ruta('IB7000M', 7, 1, v_air_lhr, '18:20', '14:10', 650, 'intercontinental', '246', 15, '10:20', '11:35', 75);
  crear_ruta('IB7001M', 7, v_air_lhr, 1, '16:35', '21:05', 690, 'intercontinental', '246', 15, '19:55', '21:00', 65);
  crear_ruta('AF4500M', 13, 1, v_air_cdg, '19:10', '15:25', 675, 'intercontinental', '135', 15, '10:55', '12:00', 65);
  crear_ruta('AF4501M', 13, v_air_cdg, 1, '17:40', '22:15', 695, 'intercontinental', '135', 15, '20:45', '21:50', 65);
  crear_ruta('IB7200M', 7, 1, v_air_ams, '20:35', '16:40', 665, 'intercontinental', '257', 15, '11:40', '12:55', 75);
  crear_ruta('IB7201M', 7, v_air_ams, 1, '18:05', '22:45', 700, 'intercontinental', '257', 15, '21:15', '22:20', 65);
  crear_ruta('LH7300M', 19, 1, v_air_fra, '17:55', '14:30', 675, 'intercontinental', '146', 15, '10:15', '11:30', 75);
  crear_ruta('LH7301M', 19, v_air_fra, 1, '15:50', '20:35', 705, 'intercontinental', '146', 15, '19:05', '20:10', 65);
  crear_ruta('IB7400M', 7, 1, v_air_fco, '21:15', '18:35', 725, 'intercontinental', '36', 15, '12:20', '13:35', 75);
  crear_ruta('IB7401M', 7, v_air_fco, 1, '19:30', '00:20', 710, 'intercontinental', '47', 15, '22:10', '23:20', 70);
  crear_ruta('LA6100M', 15, 1, v_air_gru, '06:50', '16:20', 510, 'intercontinental', '1234567', 13, '10:05', '11:05', 60);
  crear_ruta('LA6101M', 15, v_air_gru, 1, '18:00', '01:30', 510, 'intercontinental', '1234567', 13, '21:00', '22:00', 60);
  crear_ruta('LA6200M', 15, 1, v_air_gig, '08:15', '17:30', 495, 'intercontinental', '246', 13, '11:15', '12:05', 50);
  crear_ruta('LA6201M', 15, v_air_gig, 1, '19:10', '02:20', 485, 'intercontinental', '246', 13, '22:20', '23:10', 50);
  crear_ruta('LA6300M', 15, 1, v_air_bsb, '10:05', '18:50', 455, 'intercontinental', '1357', 13, '13:25', '14:10', 45);
  crear_ruta('LA6301M', 15, v_air_bsb, 1, '20:40', '03:55', 445, 'intercontinental', '1357', 13, '23:55', '00:40', 45);
  crear_ruta('LA6400M', 15, 1, v_air_rec, '07:35', '16:15', 465, 'internacional', '27', 13, '11:05', '11:50', 45);
  crear_ruta('LA6401M', 15, v_air_rec, 1, '18:25', '01:20', 455, 'internacional', '27', 13, '21:35', '22:20', 45);
  crear_ruta('AV3400M', 1, 1, v_air_eze, '07:25', '18:05', 580, 'intercontinental', '1357', 13, '10:55', '12:05', 70);
  crear_ruta('AV3401M', 1, v_air_eze, 1, '20:10', '04:40', 570, 'intercontinental', '1357', 13, '23:15', '00:20', 65);
  crear_ruta('AV3500M', 1, 1, v_air_scl, '08:05', '17:55', 530, 'intercontinental', '246', 14, '12:05', '13:10', 65);
  crear_ruta('AV3501M', 1, v_air_scl, 1, '19:25', '03:10', 525, 'intercontinental', '246', 14, '22:25', '23:30', 65);
  crear_ruta('AV3600M', 1, 1, v_air_uio, '10:35', '15:05', 270, 'internacional', '1234567', 13, '12:35', '13:20', 45);
  crear_ruta('AV3601M', 1, v_air_uio, 1, '16:15', '20:35', 260, 'internacional', '1234567', 13, '18:05', '18:50', 45);
  crear_ruta('B65000M', 11, 1, v_air_sdq, '07:10', '13:30', 320, 'turistico', '1234567', 9, '09:35', '10:20', 45);
  crear_ruta('B65001M', 11, v_air_sdq, 1, '15:20', '19:55', 275, 'turistico', '1234567', 9, '17:05', '17:50', 45);
  crear_ruta('Y45100M', 8, 1, v_air_hav, '06:20', '11:15', 295, 'turistico', '246', 8, '08:40', '09:25', 45);
  crear_ruta('Y45101M', 8, v_air_hav, 1, '12:45', '16:40', 235, 'turistico', '246', 8, '14:05', '14:50', 45);
  crear_ruta('B65200M', 11, 1, v_air_sju, '11:50', '18:10', 320, 'turistico', '1357', 9, '14:15', '15:05', 50);
  crear_ruta('B65201M', 11, v_air_sju, 1, '19:30', '00:10', 280, 'turistico', '1357', 9, '21:15', '22:05', 50);
  crear_ruta('EK8800M', 17, 1, v_air_dxb, '22:10', '22:55', 1185, 'intercontinental', '26', 6, '00:35', '02:00', 85);
  crear_ruta('EK8801M', 17, v_air_dxb, 1, '01:20', '09:40', 1220, 'intercontinental', '37', 6, '05:55', '07:20', 85);
  crear_ruta('QR8810M', 18, 1, v_air_doh, '21:40', '21:35', 1175, 'intercontinental', '15', 6, '00:05', '01:30', 85);
  crear_ruta('QR8811M', 18, v_air_doh, 1, '02:10', '10:20', 1210, 'intercontinental', '26', 6, '06:30', '07:55', 85);
  crear_ruta('UA9000M', 4, 1, v_air_nrt, '05:45', '14:40', 1040, 'intercontinental', '147', 11, '10:40', '12:10', 90);
  crear_ruta('UA9001M', 4, v_air_nrt, 1, '16:15', '20:25', 1010, 'intercontinental', '258', 11, '10:55', '12:10', 75);
  crear_ruta('UA9010M', 4, 1, v_air_icn, '06:35', '16:10', 1075, 'intercontinental', '258', 11, '11:25', '12:55', 90);
  crear_ruta('UA9011M', 4, v_air_icn, 1, '18:20', '22:50', 1045, 'intercontinental', '369', 11, '13:30', '14:45', 75);
  crear_ruta('UA9100M', 4, 1, v_air_syd, '04:50', '08:25', 1275, 'intercontinental', '17', 11, '10:15', '12:00', 105);
  crear_ruta('UA9101M', 4, v_air_syd, 1, '10:55', '16:15', 1290, 'intercontinental', '28', 11, '04:25', '06:00', 95);
  crear_ruta('UA9110M', 4, 1, v_air_akl, '03:40', '06:55', 1215, 'intercontinental', '36', 11, '09:10', '10:45', 95);
  crear_ruta('UA9111M', 4, v_air_akl, 1, '08:30', '14:05', 1300, 'intercontinental', '47', 11, '02:35', '04:05', 90);
  crear_ruta('TP5000M', 20, 1, v_air_lis, '16:10', '11:20', 645, 'intercontinental', '257', 15, '08:10', '09:15', 65);
  crear_ruta('TP5001M', 20, v_air_lis, 1, '13:45', '18:25', 690, 'intercontinental', '257', 15, '16:55', '18:00', 65);
  crear_ruta('IB5100M', 7, 1, v_air_bcn, '15:25', '10:55', 635, 'intercontinental', '146', 15, '07:55', '08:55', 60);
  crear_ruta('IB5101M', 7, v_air_bcn, 1, '12:50', '17:35', 680, 'intercontinental', '146', 15, '15:55', '17:00', 65);
  crear_ruta('LH5200M', 19, 1, v_air_muc, '17:10', '13:30', 660, 'intercontinental', '47', 15, '09:55', '11:05', 70);
  crear_ruta('LH5201M', 19, v_air_muc, 1, '15:20', '20:10', 700, 'intercontinental', '47', 15, '18:35', '19:40', 65);
  crear_ruta('LH5300M', 19, 1, v_air_zrh, '18:40', '14:45', 650, 'intercontinental', '36', 15, '10:30', '11:40', 70);
  crear_ruta('LH5301M', 19, v_air_zrh, 1, '16:00', '20:45', 690, 'intercontinental', '36', 15, '19:10', '20:15', 65);
  crear_ruta('AF5400M', 13, 1, v_air_bru, '19:55', '16:15', 670, 'intercontinental', '15', 15, '11:30', '12:45', 75);
  crear_ruta('AF5401M', 13, v_air_bru, 1, '17:45', '22:20', 700, 'intercontinental', '15', 15, '20:40', '21:50', 70);
  crear_ruta('AF5500M', 13, 1, v_air_vie, '20:25', '16:55', 690, 'intercontinental', '26', 15, '12:05', '13:20', 75);
  crear_ruta('AF5501M', 13, v_air_vie, 1, '18:20', '22:55', 710, 'intercontinental', '26', 15, '21:15', '22:25', 70);
  crear_ruta('QR5600M', 18, 1, v_air_ist, '20:50', '18:20', 820, 'intercontinental', '37', 6, '00:30', '01:35', 65);
  crear_ruta('QR5601M', 18, v_air_ist, 1, '22:10', '04:50', 835, 'intercontinental', '37', 6, '02:20', '03:25', 65);
  crear_ruta('EK5700M', 17, 1, v_air_cai, '19:20', '17:45', 850, 'intercontinental', '47', 6, '23:55', '01:15', 80);
  crear_ruta('EK5701M', 17, v_air_cai, 1, '20:55', '03:40', 865, 'intercontinental', '47', 6, '01:10', '02:25', 75);
  crear_ruta('EK5800M', 17, 1, v_air_jnb, '18:05', '20:45', 1015, 'intercontinental', '17', 6, '00:30', '02:10', 100);
  crear_ruta('EK5801M', 17, v_air_jnb, 1, '22:40', '07:25', 1005, 'intercontinental', '17', 6, '04:55', '06:25', 90);
  crear_ruta('TP5900M', 20, 1, v_air_cmn, '16:35', '11:10', 760, 'intercontinental', '135', 15, '08:25', '09:25', 60);
  crear_ruta('TP5901M', 20, v_air_cmn, 1, '13:50', '18:20', 785, 'intercontinental', '135', 15, '16:25', '17:30', 65);
  crear_ruta('UA9200M', 4, 1, v_air_bos, '07:50', '14:20', 330, 'internacional', '1234567', 12, '11:05', '11:50', 45);
  crear_ruta('UA9201M', 4, v_air_bos, 1, '16:05', '21:15', 315, 'internacional', '1234567', 12, '18:35', '19:20', 45);
  crear_ruta('UA9300M', 4, 1, v_air_sea, '06:40', '13:50', 420, 'internacional', '246', 10, '10:20', '11:10', 50);
  crear_ruta('UA9301M', 4, v_air_sea, 1, '15:45', '22:05', 405, 'internacional', '246', 10, '18:50', '19:40', 50);
  crear_ruta('AA9400M', 3, 1, v_air_dfw, '09:25', '14:35', 260, 'internacional', '1234567', 10, '11:45', '12:35', 50);
  crear_ruta('AA9401M', 3, v_air_dfw, 1, '16:20', '20:30', 255, 'internacional', '1234567', 10, '18:35', '19:20', 45);
  crear_ruta('UA9500M', 4, 1, v_air_iad, '10:10', '16:25', 325, 'internacional', '1357', 12, '13:25', '14:10', 45);
  crear_ruta('UA9501M', 4, v_air_iad, 1, '18:05', '22:40', 315, 'internacional', '1357', 12, '20:10', '20:55', 45);
  crear_ruta('SU4700M', 14, v_air_svo, v_air_cdg, '08:15', '11:45', 210, 'internacional', '1234567');
  crear_ruta('SU4701M', 14, v_air_cdg, v_air_svo, '13:20', '18:45', 205, 'internacional', '1234567');
  crear_ruta('SU4710M', 14, v_air_led, v_air_fra, '06:40', '09:15', 195, 'internacional', '1357');
  crear_ruta('SU4711M', 14, v_air_fra, v_air_led, '11:05', '15:35', 190, 'internacional', '1357');
  crear_ruta('LA6600M', 15, v_air_gru, 15, '14:10', '05:45', 615, 'intercontinental', '1357');
  crear_ruta('LA6601M', 15, 15, v_air_gru, '08:20', '18:45', 600, 'intercontinental', '1357');
  crear_ruta('BA6700M', 16, v_air_lhr, 12, '09:15', '12:05', 470, 'intercontinental', '1234567');
  crear_ruta('BA6701M', 16, 12, v_air_lhr, '15:40', '03:35', 465, 'intercontinental', '1234567');
  crear_ruta('EK6800M', 17, v_air_dxb, v_air_sin, '10:20', '21:45', 470, 'intercontinental', '1234567');
  crear_ruta('EK6801M', 17, v_air_sin, v_air_dxb, '23:55', '03:40', 455, 'intercontinental', '1234567');
  crear_ruta('QR6900M', 18, v_air_doh, v_air_hkg, '08:45', '21:10', 530, 'intercontinental', '1234567');
  crear_ruta('QR6901M', 18, v_air_hkg, v_air_doh, '23:25', '04:30', 520, 'intercontinental', '1234567');
  crear_ruta('EK6950M', 17, v_air_dxb, v_air_del, '06:55', '11:20', 195, 'intercontinental', '1234567');
  crear_ruta('EK6951M', 17, v_air_del, v_air_dxb, '13:40', '16:15', 200, 'intercontinental', '1234567');
  crear_ruta('EK6960M', 17, v_air_dxb, v_air_bom, '17:25', '21:40', 185, 'intercontinental', '1234567');
  crear_ruta('EK6961M', 17, v_air_bom, v_air_dxb, '23:10', '01:20', 190, 'intercontinental', '1234567');
  crear_ruta('AC7100M', 21, 1, v_air_yyz, '06:15', '12:35', 380, 'internacional', '1234567', 12, '09:20', '10:05', 45);
  crear_ruta('AC7101M', 21, v_air_yyz, 1, '14:25', '20:15', 350, 'internacional', '1234567', 12, '16:45', '17:30', 45);
  crear_ruta('AC7200M', 21, 1, v_air_yyz, '15:45', '22:05', 380, 'internacional', '1234567', 9, '18:50', '19:40', 50);
  crear_ruta('AC7201M', 21, v_air_yyz, 1, '23:25', '05:15', 350, 'internacional', '1234567', 9, '01:50', '02:35', 45);
  crear_ruta('KL7300M', 22, 1, v_air_ams, '17:20', '13:10', 665, 'intercontinental', '1234567', 15, '09:35', '10:40', 65);
  crear_ruta('KL7301M', 22, v_air_ams, 1, '15:05', '19:50', 695, 'intercontinental', '1234567', 15, '18:10', '19:15', 65);
  crear_ruta('AZ7400M', 23, 1, v_air_fco, '16:35', '12:55', 710, 'intercontinental', '1357', 15, '08:55', '10:05', 70);
  crear_ruta('AZ7401M', 23, v_air_fco, 1, '14:50', '19:35', 700, 'intercontinental', '1357', 15, '17:25', '18:30', 65);
  crear_ruta('TK7500M', 24, 1, v_air_ist, '19:05', '17:20', 835, 'intercontinental', '1234567', 15, '10:40', '11:45', 65);
  crear_ruta('TK7501M', 24, v_air_ist, 1, '21:10', '04:05', 845, 'intercontinental', '1234567', 15, '00:30', '01:35', 65);
  crear_ruta('G37600M', 25, 1, v_air_gig, '09:30', '19:10', 500, 'intercontinental', '1234567', 13, '12:35', '13:25', 50);
  crear_ruta('G37601M', 25, v_air_gig, 1, '21:25', '04:45', 490, 'intercontinental', '1234567', 13, '00:15', '01:05', 50);
  crear_ruta('G37610M', 25, 1, v_air_cnf, '08:55', '17:50', 470, 'internacional', '246', 13, '12:05', '12:55', 50);
  crear_ruta('G37611M', 25, v_air_cnf, 1, '19:20', '02:10', 460, 'internacional', '246', 13, '22:15', '23:05', 50);
  crear_ruta('AR7700M', 26, 1, v_air_eze, '12:10', '22:45', 590, 'intercontinental', '1234567', 13, '15:40', '16:45', 65);
  crear_ruta('AR7701M', 26, v_air_eze, 1, '23:55', '08:20', 575, 'intercontinental', '1234567', 13, '03:00', '04:00', 60);
  crear_ruta('NH7800M', 27, 1, v_air_nrt, '07:05', '16:20', 1055, 'intercontinental', '1357', 11, '12:15', '13:45', 90);
  crear_ruta('NH7801M', 27, v_air_nrt, 1, '18:50', '23:30', 1020, 'intercontinental', '1357', 11, '13:25', '14:40', 75);
  crear_ruta('CX7900M', 28, 1, v_air_hkg, '06:55', '18:35', 1110, 'intercontinental', '246', 6, '10:20', '11:45', 85);
  crear_ruta('CX7901M', 28, v_air_hkg, 1, '20:55', '02:40', 1085, 'intercontinental', '246', 6, '01:10', '02:35', 85);
  crear_ruta('AA9600M', 3, 1, v_air_mco, '06:40', '11:55', 195, 'internacional', '1234567', 9, '09:00', '09:45', 45);
  crear_ruta('AA9601M', 3, v_air_mco, 1, '13:10', '17:25', 190, 'internacional', '1234567', 9, '15:10', '15:55', 45);
  crear_ruta('AA9610M', 3, 1, v_air_clt, '10:55', '16:30', 235, 'internacional', '1234567', 12, '13:50', '14:35', 45);
  crear_ruta('AA9611M', 3, v_air_clt, 1, '18:25', '22:35', 225, 'internacional', '1234567', 12, '20:15', '21:00', 45);
  crear_ruta('UA9620M', 4, 1, v_air_den, '05:55', '11:20', 265, 'internacional', '1234567', 10, '08:20', '09:05', 45);
  crear_ruta('UA9621M', 4, v_air_den, 1, '13:35', '18:05', 255, 'internacional', '1234567', 10, '15:30', '16:15', 45);
  crear_ruta('UA9630M', 4, 1, v_air_sfo, '07:45', '14:15', 385, 'internacional', '1357', 10, '10:50', '11:40', 50);
  crear_ruta('UA9631M', 4, v_air_sfo, 1, '16:40', '22:15', 375, 'internacional', '1357', 10, '19:20', '20:10', 50);
  crear_ruta('DL9640M', 5, 1, v_air_las, '11:40', '18:25', 360, 'turistico', '246', 16, '15:10', '15:55', 45);
  crear_ruta('DL9641M', 5, v_air_las, 1, '20:10', '01:45', 350, 'turistico', '246', 16, '22:45', '23:30', 45);
  crear_ruta('AV9650M', 1, 1, v_air_mde, '07:15', '10:45', 150, 'internacional', '1234567', 13, '08:55', '09:35', 40);
  crear_ruta('AV9651M', 1, v_air_mde, 1, '12:05', '15:10', 145, 'internacional', '1234567', 13, '13:20', '14:00', 40);
  crear_ruta('AV9660M', 1, 1, v_air_ccs, '10:20', '15:30', 220, 'internacional', '1357', 13, '12:30', '13:20', 50);
  crear_ruta('AV9661M', 1, v_air_ccs, 1, '17:05', '21:40', 215, 'internacional', '1357', 13, '18:55', '19:45', 50);
  crear_ruta('LA9670M', 15, 1, v_air_for, '09:05', '17:45', 480, 'internacional', '357', 13, '12:35', '13:25', 50);
  crear_ruta('LA9671M', 15, v_air_for, 1, '19:15', '02:00', 470, 'internacional', '357', 13, '22:15', '23:05', 50);
  crear_ruta('LA9680M', 15, 1, v_air_cwb, '08:25', '17:10', 470, 'internacional', '146', 13, '11:50', '12:40', 50);
  crear_ruta('LA9681M', 15, v_air_cwb, 1, '18:50', '01:30', 460, 'internacional', '146', 13, '21:55', '22:45', 50);
  crear_ruta('AR9690M', 26, 1, v_air_asu, '06:45', '14:50', 410, 'internacional', '257', 13, '10:00', '10:45', 45);
  crear_ruta('AR9691M', 26, v_air_asu, 1, '16:25', '22:35', 400, 'internacional', '257', 13, '19:25', '20:10', 45);
  crear_ruta('KL9710M', 22, v_air_ams, v_air_dub, '09:15', '09:55', 100, 'internacional', '1234567');
  crear_ruta('KL9711M', 22, v_air_dub, v_air_ams, '12:10', '14:45', 95, 'internacional', '1234567');
  crear_ruta('LH9720M', 19, v_air_fra, v_air_arn, '07:20', '09:25', 125, 'internacional', '1234567');
  crear_ruta('LH9721M', 19, v_air_arn, v_air_fra, '11:20', '13:35', 130, 'internacional', '1234567');
  crear_ruta('LH9730M', 19, v_air_muc, v_air_cph, '08:05', '10:00', 115, 'internacional', '1234567');
  crear_ruta('LH9731M', 19, v_air_cph, v_air_muc, '12:30', '14:30', 120, 'internacional', '1234567');
  crear_ruta('TP9740M', 20, v_air_lis, v_air_gva, '07:50', '11:10', 140, 'internacional', '1234567');
  crear_ruta('TP9741M', 20, v_air_gva, v_air_lis, '13:20', '15:05', 145, 'internacional', '1234567');
  crear_ruta('TK9750M', 24, v_air_ist, v_air_hel, '09:05', '12:30', 205, 'internacional', '1234567');
  crear_ruta('TK9751M', 24, v_air_hel, v_air_ist, '14:25', '18:05', 210, 'internacional', '1234567');
  crear_ruta('CX9760M', 28, v_air_hkg, v_air_tpe, '08:10', '10:00', 110, 'internacional', '1234567');
  crear_ruta('CX9761M', 28, v_air_tpe, v_air_hkg, '12:05', '14:00', 105, 'internacional', '1234567');
  crear_ruta('CX9770M', 28, v_air_hkg, v_air_mnl, '16:10', '18:30', 140, 'internacional', '1234567');
  crear_ruta('CX9771M', 28, v_air_mnl, v_air_hkg, '20:15', '22:35', 145, 'internacional', '1234567');
  crear_ruta('EK9780M', 17, v_air_dxb, v_air_bkk, '07:20', '16:40', 370, 'intercontinental', '1234567');
  crear_ruta('EK9781M', 17, v_air_bkk, v_air_dxb, '18:55', '22:50', 365, 'intercontinental', '1234567');
  crear_ruta('EK9790M', 17, v_air_dxb, v_air_kul, '09:25', '20:35', 430, 'intercontinental', '1234567');
  crear_ruta('EK9791M', 17, v_air_kul, v_air_dxb, '23:05', '02:25', 425, 'intercontinental', '1234567');
  crear_ruta('NH9800M', 27, v_air_nrt, v_air_pvg, '10:15', '12:55', 220, 'internacional', '1234567');
  crear_ruta('NH9801M', 27, v_air_pvg, v_air_nrt, '15:15', '19:05', 225, 'internacional', '1234567');
  crear_ruta('QR9810M', 18, v_air_doh, v_air_nbo, '07:45', '13:15', 300, 'intercontinental', '1234567');
  crear_ruta('QR9811M', 18, v_air_nbo, v_air_doh, '15:05', '20:30', 295, 'intercontinental', '1234567');
  crear_ruta('QR9820M', 18, v_air_doh, v_air_add, '09:10', '12:45', 235, 'intercontinental', '1234567');
  crear_ruta('QR9821M', 18, v_air_add, v_air_doh, '14:20', '17:55', 240, 'intercontinental', '1234567');
  crear_ruta('UA9830M', 4, v_air_sfo, v_air_mel, '20:30', '06:45', 900, 'intercontinental', '146');
  crear_ruta('UA9831M', 4, v_air_mel, v_air_sfo, '10:25', '17:15', 885, 'intercontinental', '257');

  COMMIT;
END;
/

--------------------------------------------------------
-- 2. Pasajeros, usuarios, sesiones y preferencias extra
--------------------------------------------------------

DECLARE
  TYPE t_str IS TABLE OF VARCHAR2(100) INDEX BY PLS_INTEGER;
  n1 t_str; n2 t_str; a1 t_str; a2 t_str; nac t_str;
  v_pasajero_id NUMBER;
  v_hash_activo VARCHAR2(255) := 'PBKDF2$100000$NETOIdMIOybQF4Dh+i22Ew==$wpsk9LGzzvzK5JK2LB8K5KOiJ24kAz67xjVVbxZCoRA=';
  v_salt_activo VARCHAR2(100) := 'NETOIdMIOybQF4Dh+i22Ew==';
  v_hash_inactivo VARCHAR2(255) := 'PBKDF2$100000$/bK4i8AgsK4Y9J66zr43Zw==$wFy2Zg6tqOz8JL63YNccPe3RYMRLtoa84YQrJe010CQ=';
  v_salt_inactivo VARCHAR2(100) := '/bK4i8AgsK4Y9J66zr43Zw==';
BEGIN
  n1(1):='Alejandro'; n1(2):='Valentina'; n1(3):='Mateo'; n1(4):='Isabella'; n1(5):='Santiago'; n1(6):='Camila'; n1(7):='Sebastian'; n1(8):='Daniela'; n1(9):='Nicolas'; n1(10):='Gabriela';
  n1(11):='Thiago'; n1(12):='Martina'; n1(13):='Emilia'; n1(14):='Samuel'; n1(15):='Lucia'; n1(16):='Diego'; n1(17):='Renata'; n1(18):='Bruno'; n1(19):='Paula'; n1(20):='Javier';
  n2(1):='Andres'; n2(2):='Maria'; n2(3):='Sofia'; n2(4):='Eduardo'; n2(5):='Fernanda'; n2(6):='Ricardo'; n2(7):='Alejandra'; n2(8):='Patricia'; n2(9):='Antonio'; n2(10):='Carolina';
  n2(11):='Rafael'; n2(12):='Andrea'; n2(13):='Victoria'; n2(14):='Esteban'; n2(15):='Nicole'; n2(16):='Miguel'; n2(17):='Elena'; n2(18):='Gabriel'; n2(19):='Raul'; n2(20):='Natalia';
  a1(1):='Lopez'; a1(2):='Herrera'; a1(3):='Castro'; a1(4):='Silva'; a1(5):='Morales'; a1(6):='Vargas'; a1(7):='Pereira'; a1(8):='Santos'; a1(9):='Romero'; a1(10):='Delgado';
  a1(11):='Rios'; a1(12):='Campos'; a1(13):='Navarro'; a1(14):='Mendez'; a1(15):='Duarte'; a1(16):='Salazar'; a1(17):='Benitez'; a1(18):='Gimenez'; a1(19):='Paz'; a1(20):='Molina';
  a2(1):='Cruz'; a2(2):='Juarez'; a2(3):='Alvarado'; a2(4):='Ortega'; a2(5):='Pineda'; a2(6):='Rojas'; a2(7):='Luna'; a2(8):='Farias'; a2(9):='Solis'; a2(10):='Monterroso';
  a2(11):='Amaya'; a2(12):='Carrillo'; a2(13):='Arias'; a2(14):='Galvez'; a2(15):='Quesada'; a2(16):='Ponce'; a2(17):='Escobar'; a2(18):='Acosta'; a2(19):='Mejia'; a2(20):='Cardona';
  nac(1):='Guatemala'; nac(2):='El Salvador'; nac(3):='Honduras'; nac(4):='Costa Rica'; nac(5):='Mexico'; nac(6):='Estados Unidos'; nac(7):='Colombia'; nac(8):='Panama'; nac(9):='Espana'; nac(10):='Peru';
  nac(11):='Brasil'; nac(12):='Argentina'; nac(13):='Chile'; nac(14):='Francia'; nac(15):='Alemania'; nac(16):='Portugal'; nac(17):='Japon'; nac(18):='Corea del Sur'; nac(19):='Canada'; nac(20):='Uruguay';

  FOR i IN 1..180 LOOP
    INSERT INTO AER_PASAJERO
    (PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
     PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
    VALUES
    (CASE WHEN MOD(i,5)=0 THEN 'P' || LPAD(900000 + i, 8, '0') ELSE 'GTX' || LPAD(5000000 + i, 8, '0') END,
     CASE WHEN MOD(i,5)=0 THEN 'PASAPORTE' ELSE 'DPI' END,
     n1(MOD(i-1,20)+1), n2(MOD(i+3,20)+1), a1(MOD(i+5,20)+1), a2(MOD(i+7,20)+1),
     ADD_MONTHS(DATE '1970-01-15', 180 + (i * 4)), nac(MOD(i-1,20)+1), CASE WHEN MOD(i,2)=0 THEN 'M' ELSE 'F' END,
     '+502 3' || LPAD(3000000 + i * 83, 7, '0'),
     LOWER(n1(MOD(i-1,20)+1) || '.' || a1(MOD(i+5,20)+1) || '.m' || LPAD(i,3,'0') || '@aurora-demo.com'),
     DATE '2026-05-18' + MOD(i, 180))
    RETURNING PAS_ID_PASAJERO INTO v_pasajero_id;

    INSERT INTO AER_PREFERENCIACLIENTE
    (PRF_ID_PASAJERO, PRF_TIPO_PREFERENCIA, PRF_VALOR_PREFERENCIA, PRF_FECHA_REGISTRO)
    VALUES
    (v_pasajero_id,
     CASE WHEN MOD(i,4)=0 THEN 'asiento' WHEN MOD(i,4)=1 THEN 'notificacion' WHEN MOD(i,4)=2 THEN 'equipaje' ELSE 'idioma' END,
     CASE WHEN MOD(i,4)=0 THEN 'pasillo'
          WHEN MOD(i,4)=1 THEN 'email y sms'
          WHEN MOD(i,4)=2 THEN 'dos maletas'
          ELSE 'es' END,
     TIMESTAMP '2026-05-18 09:00:00' + NUMTODSINTERVAL(i * 5, 'HOUR'));

    IF i <= 120 THEN
      INSERT INTO AER_USUARIO_LOGIN
      (USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO,
       USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_VERIFICACION,
       USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
      VALUES
      (v_pasajero_id,
       LOWER(n1(MOD(i-1,20)+1) || '.' || a1(MOD(i+5,20)+1) || '.u' || LPAD(i,3,'0')),
       LOWER(n1(MOD(i-1,20)+1) || '.' || a1(MOD(i+5,20)+1) || '.m' || LPAD(i,3,'0') || '@aurora-demo.com'),
       CASE WHEN MOD(i,16)=0 OR MOD(i,25)=0 THEN v_hash_inactivo ELSE v_hash_activo END,
       CASE WHEN MOD(i,16)=0 OR MOD(i,25)=0 THEN v_salt_inactivo ELSE v_salt_activo END,
       CASE
         WHEN MOD(i,25)=0 THEN 'BLOQUEADO'
         WHEN MOD(i,16)=0 THEN 'INACTIVO'
        WHEN MOD(i,11)=0 THEN 'BLOQUEADO'
         ELSE 'ACTIVO'
       END,
       CASE WHEN MOD(i,9)=0 THEN 'N' ELSE 'S' END,
       TIMESTAMP '2026-05-18 08:00:00' + NUMTODSINTERVAL(i, 'DAY'),
       TIMESTAMP '2026-10-01 08:00:00' + NUMTODSINTERVAL(MOD(i, 90), 'DAY'),
       CASE WHEN MOD(i,25)=0 THEN 5 WHEN MOD(i,11)=0 THEN 3 ELSE 0 END,
       CASE WHEN MOD(i,25)=0 THEN TIMESTAMP '2026-12-31 23:59:59' ELSE NULL END,
       NULL, NULL, NULL);
    END IF;

    IF i <= 150 THEN
      INSERT INTO AER_SESIONUSUARIO
      (SES_SESION_ID, SES_ID_USUARIO, SES_ID_PASAJERO, SES_IP_ADDRESS, SES_NAVEGADOR, SES_SISTEMA_OPERATIVO,
       SES_DISPOSITIVO, SES_FECHA_INICIO, SES_FECHA_FIN, SES_DURACION_SEGUNDOS)
      VALUES
      ('SES-MASTER-' || LPAD(i, 6, '0'),
       CASE WHEN i <= 120 THEN (SELECT MAX(USL_ID_USUARIO) FROM AER_USUARIO_LOGIN WHERE USL_ID_PASAJERO = v_pasajero_id) ELSE NULL END,
       v_pasajero_id,
       '181.174.' || MOD(i,255) || '.' || (10 + MOD(i,200)),
       CASE WHEN MOD(i,4)=0 THEN 'Chrome' WHEN MOD(i,4)=1 THEN 'Safari' WHEN MOD(i,4)=2 THEN 'Edge' ELSE 'Firefox' END,
       CASE WHEN MOD(i,4)=0 THEN 'Windows' WHEN MOD(i,4)=1 THEN 'iOS' WHEN MOD(i,4)=2 THEN 'Android' ELSE 'macOS' END,
       CASE WHEN MOD(i,3)=0 THEN 'movil' WHEN MOD(i,3)=1 THEN 'desktop' ELSE 'tablet' END,
       TIMESTAMP '2026-09-01 06:00:00' + NUMTODSINTERVAL(i * 3, 'HOUR'),
       TIMESTAMP '2026-09-01 06:12:00' + NUMTODSINTERVAL(i * 3, 'HOUR'),
       720 + MOD(i * 29, 1800));
    END IF;
  END LOOP;

  COMMIT;
END;
/

--------------------------------------------------------
-- 3. Tripulacion para vuelos nuevos
--------------------------------------------------------

INSERT INTO AER_TRIPULACION (TRI_ID_VUELO, TRI_ID_EMPLEADO, TRI_ROL, TRI_HORAS_VUELO)
SELECT v.VUE_ID_VUELO, CASE WHEN MOD(v.VUE_ID_VUELO,2)=0 THEN 1 ELSE 2 END, 'piloto', ROUND(p.PRV_DURACION_ESTIMADA / 60, 2)
FROM AER_VUELO v
JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
WHERE p.PRV_NUMERO_VUELO LIKE '%M';

INSERT INTO AER_TRIPULACION (TRI_ID_VUELO, TRI_ID_EMPLEADO, TRI_ROL, TRI_HORAS_VUELO)
SELECT v.VUE_ID_VUELO, CASE WHEN MOD(v.VUE_ID_VUELO,2)=0 THEN 3 ELSE 4 END, 'copiloto', ROUND(p.PRV_DURACION_ESTIMADA / 60, 2)
FROM AER_VUELO v
JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
WHERE p.PRV_NUMERO_VUELO LIKE '%M';

INSERT INTO AER_TRIPULACION (TRI_ID_VUELO, TRI_ID_EMPLEADO, TRI_ROL, TRI_HORAS_VUELO)
SELECT v.VUE_ID_VUELO, 7 + MOD(v.VUE_ID_VUELO, 8), 'jefe cabina', ROUND(p.PRV_DURACION_ESTIMADA / 60, 2)
FROM AER_VUELO v
JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
WHERE p.PRV_NUMERO_VUELO LIKE '%M';

INSERT INTO AER_TRIPULACION (TRI_ID_VUELO, TRI_ID_EMPLEADO, TRI_ROL, TRI_HORAS_VUELO)
SELECT v.VUE_ID_VUELO, 7 + MOD(v.VUE_ID_VUELO + 3, 8), 'auxiliar cabina', ROUND(p.PRV_DURACION_ESTIMADA / 60, 2)
FROM AER_VUELO v
JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
WHERE p.PRV_NUMERO_VUELO LIKE '%M';

COMMIT;

--------------------------------------------------------
-- 4. Reservas, ventas, pagos y operaciones de pasajero
--------------------------------------------------------

DECLARE
  CURSOR c_vuelos IS
    SELECT v.VUE_ID_VUELO,
           v.VUE_ID_AVION,
           v.VUE_FECHA_VUELO,
           v.VUE_ESTADO,
           p.PRV_ID_AEROPUERTO_ORIGEN,
           p.PRV_ID_AEROPUERTO_DESTINO,
           p.PRV_TIPO_VUELO
    FROM AER_VUELO v
    JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
    WHERE p.PRV_NUMERO_VUELO LIKE '%M'
      AND v.VUE_FECHA_VUELO <= DATE '2026-12-31'
      AND p.PRV_TIPO_VUELO <> 'carga'
      AND v.VUE_ESTADO <> 'CANCELADO'
    ORDER BY v.VUE_FECHA_VUELO, v.VUE_ID_VUELO;

  v_reserva_id NUMBER;
  v_venta_id NUMBER;
  v_checkin_id NUMBER;
  v_equipaje_id NUMBER;
  v_pasajero NUMBER := 96;
  v_fila NUMBER;
  v_pos NUMBER;
  v_asiento NUMBER;
  v_clase VARCHAR2(20);
  v_tarifa NUMBER(10,2);
  v_descuento NUMBER(10,2);
  v_impuesto NUMBER(10,2);
  v_total NUMBER(10,2);
  v_bolsas NUMBER;
  v_peso NUMBER(6,2);
  v_local_index NUMBER := 0;
BEGIN
  FOR r IN c_vuelos LOOP
    FOR j IN 1..(3 + MOD(r.VUE_ID_VUELO, 4)) LOOP
      v_pasajero := CASE WHEN v_pasajero >= 275 THEN 1 ELSE v_pasajero + 1 END;
      v_local_index := v_local_index + 1;
      v_fila := 4 + MOD(v_local_index, 18);
      v_pos := 1 + MOD(v_local_index, 6);
      v_asiento := r.VUE_ID_AVION * 10000 + v_fila * 10 + v_pos;
      v_clase := CASE WHEN MOD(v_local_index, 9) = 0 THEN 'ejecutiva' ELSE 'economica' END;
      v_tarifa := CASE
        WHEN r.PRV_TIPO_VUELO = 'nacional' THEN 420 + MOD(v_local_index, 80)
        WHEN r.PRV_TIPO_VUELO = 'regional' THEN 680 + MOD(v_local_index, 180)
        WHEN r.PRV_TIPO_VUELO = 'internacional' THEN 1180 + MOD(v_local_index * 17, 900)
        WHEN r.PRV_TIPO_VUELO = 'turistico' THEN 980 + MOD(v_local_index * 13, 700)
        WHEN r.PRV_TIPO_VUELO = 'intercontinental' THEN 2950 + MOD(v_local_index * 31, 4200)
        ELSE 850 + MOD(v_local_index, 300)
      END;
      IF v_clase = 'ejecutiva' THEN
        v_tarifa := ROUND(v_tarifa * 1.75, 2);
      END IF;

      v_descuento := CASE WHEN MOD(v_local_index, 10) = 0 THEN 125 ELSE 0 END;
      v_impuesto := ROUND(v_tarifa * 0.12, 2);
      v_total := v_tarifa + v_impuesto - v_descuento;
      v_bolsas := CASE WHEN MOD(v_local_index, 4) = 0 THEN 2 WHEN MOD(v_local_index, 3) = 0 THEN 1 ELSE 0 END;
      v_peso := CASE WHEN v_bolsas = 0 THEN 0 ELSE 16 + MOD(v_local_index * 3, 18) END;

      INSERT INTO AER_RESERVA
      (RES_ID_VUELO, RES_ID_PASAJERO, RES_CLASE, RES_FECHA_RESERVA, RES_ESTADO, RES_EQUIPAJE_FACTURADO, RES_PESO_EQUIPAJE, RES_TARIFA_PAGADA, RES_CODIGO_RESERVA)
      VALUES
      (r.VUE_ID_VUELO, v_pasajero, v_clase, GREATEST(DATE '2026-05-18', r.VUE_FECHA_VUELO - (15 + MOD(v_local_index, 60))),
       CASE WHEN MOD(v_local_index, 27) = 0 THEN 'CANCELADA' WHEN r.VUE_FECHA_VUELO <= DATE '2026-11-30' THEN 'COMPLETADA' ELSE 'CONFIRMADA' END,
       v_bolsas, CASE WHEN v_bolsas = 0 THEN NULL ELSE v_peso END, v_tarifa,
       'AURM' || TO_CHAR(r.VUE_FECHA_VUELO, 'YYMMDD') || LPAD(v_local_index, 6, '0'))
      RETURNING RES_ID_RESERVA INTO v_reserva_id;

      INSERT INTO AER_ASIGNACION_ASIENTO
      (AAS_ID_VUELO, AAS_ID_PASAJERO, AAS_ID_ASIENTO, AAS_FECHA_ASIGNACION, AAS_ESTADO)
      VALUES
      (r.VUE_ID_VUELO, v_pasajero, v_asiento, SYSTIMESTAMP - NUMTODSINTERVAL(MOD(v_local_index, 360), 'HOUR'),
       CASE WHEN MOD(v_local_index, 27) = 0 THEN 'CANCELADA' ELSE 'CONFIRMADA' END);

      INSERT INTO AER_VENTABOLETO
      (VEN_NUMERO_VENTA, VEN_ID_PUNTO_VENTA, VEN_ID_EMPLEADO_VENDEDOR, VEN_ID_PASAJERO, VEN_FECHA_VENTA, VEN_MONTO_SUBTOTAL,
       VEN_IMPUESTOS, VEN_DESCUENTOS, VEN_MONTO_TOTAL, VEN_ID_METODO_PAGO, VEN_CANAL_VENTA, VEN_ESTADO)
      VALUES
      ('VEN-MASTER-' || LPAD(v_reserva_id, 8, '0'),
       CASE WHEN MOD(v_local_index, 4) IN (0,1) THEN NULL ELSE 3 + MOD(v_local_index, 2) END,
       CASE WHEN MOD(v_local_index, 4) IN (0,1) THEN NULL ELSE 35 + MOD(v_local_index, 3) END,
       v_pasajero,
       SYSTIMESTAMP - NUMTODSINTERVAL(MOD(v_local_index, 720), 'HOUR'),
       v_tarifa, v_impuesto, v_descuento, v_total, 1 + MOD(v_local_index, 5),
       CASE WHEN MOD(v_local_index,4)=0 THEN 'web' WHEN MOD(v_local_index,4)=1 THEN 'app' WHEN MOD(v_local_index,4)=2 THEN 'mostrador' ELSE 'telefono' END,
       CASE WHEN MOD(v_local_index, 27) = 0 THEN 'CANCELADA' ELSE 'COMPLETADA' END)
      RETURNING VEN_ID_VENTA INTO v_venta_id;

      INSERT INTO AER_DETALLEVENTABOLETO
      (DEV_ID_VENTA, DEV_ID_RESERVA, DEV_PRECIO_BASE, DEV_CARGOS_ADICIONALES)
      VALUES
      (v_venta_id, v_reserva_id, v_tarifa, CASE WHEN v_bolsas >= 2 THEN 180 ELSE 75 END);

      INSERT INTO AER_TRANSACCIONPAGO
      (TRA_ID_RESERVA, TRA_ID_METODO_PAGO, TRA_MONTO_TOTAL, TRA_MONEDA, TRA_FECHA_TRANSACCION, TRA_ESTADO,
       TRA_NUMERO_AUTORIZACION, TRA_REFERENCIA_EXTERNA, TRA_IP_CLIENTE, TRA_DETALLES_TARJETA)
      VALUES
      (v_reserva_id, 1 + MOD(v_local_index, 5), v_total, CASE WHEN MOD(v_local_index, 5)=0 THEN 'USD' ELSE 'GTQ' END,
       SYSTIMESTAMP - NUMTODSINTERVAL(MOD(v_local_index, 715), 'HOUR'),
       CASE WHEN MOD(v_local_index, 27) = 0 THEN 'REEMBOLSADA' ELSE 'APROBADA' END,
       'AUTHM' || LPAD(v_reserva_id * 13, 10, '0'),
       'AURMASTER-' || LPAD(v_reserva_id, 8, '0'),
       '190.148.' || MOD(v_local_index,255) || '.' || (40 + MOD(v_local_index,180)),
       'Tarjeta terminada en ' || LPAD(MOD(v_reserva_id * 91, 10000), 4, '0'));

      IF MOD(v_local_index, 10) = 0 THEN
        INSERT INTO AER_USOPROMOCION
        (USO_ID_PROMOCION, USO_ID_RESERVA, USO_FECHA_USO, USO_MONTO_DESCUENTO)
        VALUES
        (2, v_reserva_id, SYSTIMESTAMP - NUMTODSINTERVAL(MOD(v_local_index, 720), 'HOUR'), 125);
      END IF;

      IF r.PRV_ID_AEROPUERTO_ORIGEN = 1 AND r.VUE_FECHA_VUELO <= DATE '2026-12-31' AND MOD(v_local_index, 27) <> 0 THEN
        INSERT INTO AER_CHECKIN
        (CHK_ID_RESERVA, CHK_ID_PASAJERO, CHK_ID_VUELO, CHK_FECHA_HORA, CHK_METODO, CHK_ESTADO)
        VALUES
        (v_reserva_id, v_pasajero, r.VUE_ID_VUELO,
         TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 04:00', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 180), 'MINUTE'),
         CASE WHEN MOD(v_local_index,4)=0 THEN 'app' WHEN MOD(v_local_index,4)=1 THEN 'web' WHEN MOD(v_local_index,4)=2 THEN 'kiosko' ELSE 'mostrador' END,
         'COMPLETADO')
        RETURNING CHK_ID_CHECKIN INTO v_checkin_id;

        INSERT INTO AER_TARJETA_EMBARQUE
        (TAE_ID_CHECKIN, TAE_CODIGO_QR, TAE_GRUPO_ABORDAJE, TAE_ZONA, TAE_FECHA_EMISION)
        VALUES
        (v_checkin_id, 'QR-MASTER-' || LPAD(v_reserva_id, 8, '0') || '-GUA',
         CHR(65 + MOD(v_local_index, 4)), 'Z' || TO_CHAR(1 + MOD(v_local_index, 5)),
         TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 04:05', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 180), 'MINUTE'));

        IF v_bolsas > 0 THEN
          INSERT INTO AER_EQUIPAJE
          (EQU_ID_PASAJERO, EQU_ID_VUELO, EQU_CODIGO_BARRAS, EQU_PESO_KG, EQU_ESTADO, EQU_FECHA_REGISTRO)
          VALUES
          (v_pasajero, r.VUE_ID_VUELO, 'GUA-M-' || LPAD(v_reserva_id, 8, '0'), v_peso,
           CASE WHEN r.VUE_FECHA_VUELO <= DATE '2026-12-15' THEN 'CARGADO' ELSE 'REGISTRADO' END,
           TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 03:30', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 150), 'MINUTE'))
          RETURNING EQU_ID_EQUIPAJE INTO v_equipaje_id;

          INSERT INTO AER_MOVIMIENTO_EQUIPAJE
          (MEQ_ID_EQUIPAJE, MEQ_UBICACION, MEQ_ESTADO, MEQ_FECHA_HORA, MEQ_OBSERVACION)
          VALUES
          (v_equipaje_id, 'Mostrador de equipaje GUA', 'REGISTRADO',
           TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 03:32', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 150), 'MINUTE'),
           'Peso validado y etiqueta emitida');

          INSERT INTO AER_MOVIMIENTO_EQUIPAJE
          (MEQ_ID_EQUIPAJE, MEQ_UBICACION, MEQ_ESTADO, MEQ_FECHA_HORA, MEQ_OBSERVACION)
          VALUES
          (v_equipaje_id, 'Sistema BHS - cinta primaria', 'EN_TRANSITO',
           TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 03:44', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 150), 'MINUTE'),
           'Lectura automatica satisfactoria');

          IF r.VUE_FECHA_VUELO <= DATE '2026-12-15' THEN
            INSERT INTO AER_MOVIMIENTO_EQUIPAJE
            (MEQ_ID_EQUIPAJE, MEQ_UBICACION, MEQ_ESTADO, MEQ_FECHA_HORA, MEQ_OBSERVACION)
            VALUES
            (v_equipaje_id, 'Bodega aeronave', 'CARGADO',
             TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 04:10', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 150), 'MINUTE'),
             'Cargado por cuadrilla de rampa');
          END IF;
        END IF;

        INSERT INTO AER_CONTROL_SEGURIDAD
        (CSE_ID_PASAJERO, CSE_ID_VUELO, CSE_ID_EMPLEADO, CSE_RESULTADO, CSE_FECHA_HORA, CSE_OBSERVACION)
        VALUES
        (v_pasajero, r.VUE_ID_VUELO, 21 + MOD(v_local_index, 5),
         CASE WHEN MOD(v_local_index, 41)=0 THEN 'REVISION' ELSE 'APROBADO' END,
         TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 04:20', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 130), 'MINUTE'),
         CASE WHEN MOD(v_local_index, 41)=0 THEN 'Inspeccion manual por articulo metalico; liberado sin novedad' ELSE 'Control normal' END);

        INSERT INTO AER_CONTROL_MIGRATORIO
        (CMI_ID_PASAJERO, CMI_ID_VUELO, CMI_ID_EMPLEADO, CMI_TIPO, CMI_RESULTADO, CMI_FECHA_HORA, CMI_OBSERVACION)
        VALUES
        (v_pasajero, r.VUE_ID_VUELO, 26 + MOD(v_local_index, 4), 'salida',
         CASE WHEN MOD(v_local_index, 53)=0 THEN 'REVISION' ELSE 'APROBADO' END,
         TO_TIMESTAMP(TO_CHAR(r.VUE_FECHA_VUELO, 'YYYY-MM-DD') || ' 04:35', 'YYYY-MM-DD HH24:MI') + NUMTODSINTERVAL(MOD(v_local_index, 120), 'MINUTE'),
         CASE WHEN MOD(v_local_index, 53)=0 THEN 'Validacion manual de documento e itinerario' ELSE 'Documento vigente' END);
      END IF;
    END LOOP;
  END LOOP;

  UPDATE AER_VUELO v
  SET VUE_PLAZAS_OCUPADAS = (
        SELECT COUNT(*)
        FROM AER_RESERVA r
        WHERE r.RES_ID_VUELO = v.VUE_ID_VUELO
          AND r.RES_ESTADO <> 'CANCELADA'),
      VUE_PLAZAS_VACIAS = GREATEST(0, (
        SELECT ma.MOD_CAPACIDAD_PASAJEROS
        FROM AER_AVION av
        JOIN AER_MODELOAVION ma ON ma.MOD_ID_MODELO = av.AVI_ID_MODELO
        WHERE av.AVI_ID = v.VUE_ID_AVION
      ) - (
        SELECT COUNT(*)
        FROM AER_RESERVA r
        WHERE r.RES_ID_VUELO = v.VUE_ID_VUELO
          AND r.RES_ESTADO <> 'CANCELADA'))
  WHERE EXISTS (
    SELECT 1
    FROM AER_PROGRAMAVUELO p
    WHERE p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
      AND p.PRV_NUMERO_VUELO LIKE '%M');

  COMMIT;
END;
/

--------------------------------------------------------
-- 5. Carritos, busquedas y comportamiento digital
--------------------------------------------------------

DECLARE
  v_carr_count NUMBER := 0;
BEGIN
  FOR i IN 1..180 LOOP
    INSERT INTO AER_CARRITOCOMPRA
    (CAR_ID_PASAJERO, CAR_SESION_ID, CAR_FECHA_CREACION, CAR_FECHA_EXPIRACION, CAR_ESTADO)
    VALUES
    (1 + MOD(i, 260), 'SES-CART-' || LPAD(i, 6, '0'),
     TIMESTAMP '2026-05-18 08:00:00' + NUMTODSINTERVAL(i * 11, 'MINUTE'),
     TIMESTAMP '2026-05-18 08:25:00' + NUMTODSINTERVAL(i * 11, 'MINUTE'),
     CASE WHEN i <= 95 THEN 'CONVERTIDO_RESERVA' WHEN i <= 130 THEN 'EXPIRADO' ELSE 'ACTIVO' END);
  END LOOP;

  FOR i IN 1..180 LOOP
    INSERT INTO AER_ITEMCARRITO
    (ITE_ID_CARRITO, ITE_ID_VUELO, ITE_NUMERO_ASIENTO, ITE_CLASE, ITE_PRECIO_UNITARIO, ITE_CANTIDAD)
    SELECT i,
           v.VUE_ID_VUELO,
           TO_CHAR(6 + MOD(i, 18)) || CHR(65 + MOD(i, 6)),
           CASE WHEN MOD(i, 6)=0 THEN 'ejecutiva' ELSE 'economica' END,
           CASE WHEN MOD(i, 6)=0 THEN 3250 ELSE 1425 + MOD(i * 17, 950) END,
           1
    FROM (
      SELECT v.VUE_ID_VUELO, ROW_NUMBER() OVER (ORDER BY v.VUE_FECHA_VUELO, v.VUE_ID_VUELO) rn
      FROM AER_VUELO v
      JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
      WHERE p.PRV_NUMERO_VUELO LIKE '%M'
        AND v.VUE_FECHA_VUELO BETWEEN DATE '2026-05-18' AND DATE '2026-12-31'
    ) v
    WHERE v.rn = 1 + MOD(i * 3, 250);
  END LOOP;

  FOR i IN 1..360 LOOP
    INSERT INTO AER_BUSQUEDAVUELO
    (BUS_ID_SESION, BUS_ID_AEROPUERTO_ORIGEN, BUS_ID_AEROPUERTO_DESTINO, BUS_FECHA_IDA, BUS_FECHA_VUELTA,
     BUS_NUMERO_PASAJEROS, BUS_CLASE, BUS_FECHA_BUSQUEDA, BUS_SE_CONVIRTIO_COMPRA)
    VALUES
    (CASE WHEN i <= 240 THEN 1 + MOD(i, 210) ELSE NULL END,
     CASE WHEN MOD(i, 9) IN (0,1,2) THEN 1 ELSE 2 + MOD(i, 40) END,
     CASE WHEN MOD(i, 9) IN (0,1,2) THEN 6 + MOD(i, 30) ELSE 1 END,
     DATE '2026-05-18' + MOD(i, 220),
     CASE WHEN MOD(i, 4)=0 THEN DATE '2026-05-22' + MOD(i, 215) ELSE NULL END,
     1 + MOD(i, 4),
     CASE WHEN MOD(i, 7)=0 THEN 'ejecutiva' ELSE 'economica' END,
     TIMESTAMP '2026-05-18 06:00:00' + NUMTODSINTERVAL(i * 19, 'MINUTE'),
     CASE WHEN MOD(i, 5) IN (0,1) THEN 'S' ELSE 'N' END);

    INSERT INTO AER_CLICKDESTINO
    (CLI_ID_SESION, CLI_ID_AEROPUERTO_DESTINO, CLI_FECHA_CLICK, CLI_ORIGEN_BUSQUEDA, CLI_FECHA_VIAJE_BUSCADA, CLI_NUMERO_PASAJEROS, CLI_CLASE_BUSCADA)
    VALUES
    (CASE WHEN i <= 240 THEN 1 + MOD(i, 210) ELSE NULL END,
     2 + MOD(i, 44),
     TIMESTAMP '2026-05-18 06:03:00' + NUMTODSINTERVAL(i * 19, 'MINUTE'),
     CASE WHEN MOD(i,3)=0 THEN 'web' WHEN MOD(i,3)=1 THEN 'app' ELSE 'promo' END,
     DATE '2026-05-18' + MOD(i, 220),
     1 + MOD(i, 4),
     CASE WHEN MOD(i, 7)=0 THEN 'ejecutiva' ELSE 'economica' END);
  END LOOP;

  COMMIT;
END;
/

--------------------------------------------------------
-- 6. Mantenimiento, repuestos e inventario adicional
--------------------------------------------------------

DECLARE
  v_hangar NUMBER;
BEGIN
  INSERT INTO AER_HANGAR
  (HAN_CODIGO_HANGAR, HAN_NOMBRE, HAN_ID_AEROPUERTO, HAN_CAPACIDAD_AVIONES, HAN_AREA_M2, HAN_ALTURA_MAXIMA, HAN_TIPO, HAN_ESTADO)
  VALUES ('H-D4', 'Hangar intercontinental largo alcance', 1, 2, 2650, 18.5, 'linea', 'DISPONIBLE');

  INSERT INTO AER_HANGAR
  (HAN_CODIGO_HANGAR, HAN_NOMBRE, HAN_ID_AEROPUERTO, HAN_CAPACIDAD_AVIONES, HAN_AREA_M2, HAN_ALTURA_MAXIMA, HAN_TIPO, HAN_ESTADO)
  VALUES ('H-E5', 'Hangar partners y widebody', 1, 2, 2900, 19.0, 'mantenimiento pesado', 'DISPONIBLE');

  INSERT INTO AER_REPUESTO
  (REP_CODIGO_REPUESTO, REP_NOMBRE, REP_DESCRIPCION, REP_ID_CATEGORIA, REP_ID_MODELO_AVION, REP_NUMERO_PARTE_FABRICANTE,
   REP_STOCK_MINIMO, REP_STOCK_ACTUAL, REP_STOCK_MAXIMO, REP_PRECIO_UNITARIO, REP_UBICACION_BODEGA, REP_ESTADO)
  VALUES
  ('WBY-787-001', 'Actuador tren 787', 'Actuador certificado para tren principal Boeing 787', 2, 7, 'B787-LG-001', 2, 6, 10, 28500, 'Bodega W-02', 'ACTIVO');

  INSERT INTO AER_REPUESTO
  (REP_CODIGO_REPUESTO, REP_NOMBRE, REP_DESCRIPCION, REP_ID_CATEGORIA, REP_ID_MODELO_AVION, REP_NUMERO_PARTE_FABRICANTE,
   REP_STOCK_MINIMO, REP_STOCK_ACTUAL, REP_STOCK_MAXIMO, REP_PRECIO_UNITARIO, REP_UBICACION_BODEGA, REP_ESTADO)
  VALUES
  ('WBY-A321-002', 'Computadora vuelo A321neo', 'Unidad avionica para familia A321neo', 1, 4, 'A321-FMC-002', 1, 4, 8, 22100, 'Bodega W-05', 'ACTIVO');

  INSERT INTO AER_ORDENCOMPRAREPUESTO
  (ORC_NUMERO_ORDEN, ORC_ID_PROVEEDOR, ORC_FECHA_ORDEN, ORC_FECHA_ENTREGA_ESPERADA, ORC_FECHA_ENTREGA_REAL, ORC_MONTO_TOTAL, ORC_ESTADO, ORC_ID_EMPLEADO_SOLICITA, ORC_OBSERVACIONES)
  VALUES
  ('OC-AUR-2026-0608', 2, DATE '2026-06-08', DATE '2026-06-16', DATE '2026-06-15', 109200, 'RECIBIDA', 39, 'Reposicion para operacion de red intercontinental');

  INSERT INTO AER_DETALLEORDENCOMPRA
  (DET_ID_ORDEN_COMPRA, DET_ID_REPUESTO, DET_CANTIDAD, DET_PRECIO_UNITARIO, DET_SUBTOTAL)
  SELECT MAX(ORC_ID_ORDEN_COMPRA), MAX(REP_ID_REPUESTO) - 1, 2, 28500, 57000 FROM AER_ORDENCOMPRAREPUESTO, AER_REPUESTO;

  INSERT INTO AER_DETALLEORDENCOMPRA
  (DET_ID_ORDEN_COMPRA, DET_ID_REPUESTO, DET_CANTIDAD, DET_PRECIO_UNITARIO, DET_SUBTOTAL)
  SELECT MAX(ORC_ID_ORDEN_COMPRA), MAX(REP_ID_REPUESTO), 2, 22100, 44200 FROM AER_ORDENCOMPRAREPUESTO, AER_REPUESTO;

  INSERT INTO AER_MOVIMIENTOREPUESTO
  (MOV_ID_REPUESTO, MOV_TIPO_MOVIMIENTO, MOV_CANTIDAD, MOV_FECHA_MOVIMIENTO, MOV_ID_EMPLEADO, MOV_MOTIVO, MOV_REFERENCIA)
  SELECT MAX(REP_ID_REPUESTO) - 1, 'ENTRADA', 2, TIMESTAMP '2026-06-15 11:15:00', 39, 'Recepcion repuesto widebody', 'OC-AUR-2026-0608' FROM AER_REPUESTO;

  INSERT INTO AER_MOVIMIENTOREPUESTO
  (MOV_ID_REPUESTO, MOV_TIPO_MOVIMIENTO, MOV_CANTIDAD, MOV_FECHA_MOVIMIENTO, MOV_ID_EMPLEADO, MOV_MOTIVO, MOV_REFERENCIA)
  SELECT MAX(REP_ID_REPUESTO), 'ENTRADA', 2, TIMESTAMP '2026-06-15 11:25:00', 39, 'Recepcion avionica A321neo', 'OC-AUR-2026-0608' FROM AER_REPUESTO;

  INSERT INTO AER_ASIGNACIONHANGAR
  (ASH_ID_HANGAR, ASH_ID_AVION, ASH_FECHA_ENTRADA, ASH_FECHA_SALIDA_PROGRAMADA, ASH_FECHA_SALIDA_REAL, ASH_MOTIVO, ASH_COSTO_HORA, ASH_COSTO_TOTAL, ASH_ESTADO)
  VALUES
  (4, 19, TIMESTAMP '2026-06-19 01:00:00', TIMESTAMP '2026-06-20 12:00:00', TIMESTAMP '2026-06-20 10:40:00',
   'Inspeccion overnight por rotacion larga Francia-Guatemala', 1250, 41500, 'FINALIZADA');

  INSERT INTO AER_ASIGNACIONHANGAR
  (ASH_ID_HANGAR, ASH_ID_AVION, ASH_FECHA_ENTRADA, ASH_FECHA_SALIDA_PROGRAMADA, ASH_FECHA_SALIDA_REAL, ASH_MOTIVO, ASH_COSTO_HORA, ASH_COSTO_TOTAL, ASH_ESTADO)
  VALUES
  (5, 29, TIMESTAMP '2026-07-05 02:30:00', TIMESTAMP '2026-07-05 14:30:00', NULL,
   'Revision amplia por tramo largo Dubai-Guatemala', 1380, NULL, 'ACTIVA');

  INSERT INTO AER_MANTENIMIENTOAVION
  (MAN_ID_AVION, MAN_TIPO_MANTENIMIENTO, MAN_FECHA_INICIO, MAN_FECHA_FIN_ESTIMADA, MAN_FECHA_FIN_REAL, MAN_DESCRIPCION_TRABAJO,
   MAN_ID_EMPLEADO_RESPONSABLE, MAN_COSTO_MANO_OBRA, MAN_COSTO_REPUESTOS, MAN_COSTO_TOTAL, MAN_ESTADO)
  VALUES
  (19, 'Inspeccion A-check', TIMESTAMP '2026-06-19 01:15:00', TIMESTAMP '2026-06-20 12:00:00', TIMESTAMP '2026-06-20 10:20:00',
   'Inspeccion programada de widebody con enfasis en avionica, tren y sistemas de cabina.', 34, 21400, 28500, 49900, 'FINALIZADO');

  INSERT INTO AER_MANTENIMIENTOAVION
  (MAN_ID_AVION, MAN_TIPO_MANTENIMIENTO, MAN_FECHA_INICIO, MAN_FECHA_FIN_ESTIMADA, MAN_FECHA_FIN_REAL, MAN_DESCRIPCION_TRABAJO,
   MAN_ID_EMPLEADO_RESPONSABLE, MAN_COSTO_MANO_OBRA, MAN_COSTO_REPUESTOS, MAN_COSTO_TOTAL, MAN_ESTADO)
  VALUES
  (29, 'Revision operacional larga distancia', TIMESTAMP '2026-07-05 02:45:00', TIMESTAMP '2026-07-05 14:30:00', NULL,
   'Chequeo estructural, sistemas hidraulicos, prueba de cabina y revision de tren.', 33, 18800, 0, 18800, 'EN_PROCESO');

  INSERT INTO AER_REPUESTOUTILIZADO
  (RUT_ID_MANTENIMIENTO, RUT_ID_REPUESTO, RUT_CANTIDAD)
  SELECT MAX(MAN_ID_MANTENIMIENTO) - 1, MAX(REP_ID_REPUESTO) - 1, 1 FROM AER_MANTENIMIENTOAVION, AER_REPUESTO;

  COMMIT;
END;
/

--------------------------------------------------------
-- 7. Incidentes, objetos perdidos, arrestos y auditoria
--------------------------------------------------------

BEGIN
  INSERT INTO AER_INCIDENTE
  (INC_TIPO, INC_DESCRIPCION, INC_FECHA_HORA, INC_SEVERIDAD, INC_ESTADO, INC_ID_VUELO, INC_ID_EMPLEADO_REPORTA)
  SELECT 'clima',
         'Ajuste de secuencia de salida por actividad electrica cercana en aproximacion sur.',
         TIMESTAMP '2026-06-11 17:40:00',
         'media', 'CERRADO', MIN(VUE_ID_VUELO), 2
  FROM AER_VUELO v
  JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
  WHERE p.PRV_NUMERO_VUELO = 'AF4500M';

  INSERT INTO AER_INCIDENTE
  (INC_TIPO, INC_DESCRIPCION, INC_FECHA_HORA, INC_SEVERIDAD, INC_ESTADO, INC_ID_VUELO, INC_ID_EMPLEADO_REPORTA)
  SELECT 'equipaje',
         'Maleta con lectura incompleta corregida en cinta secundaria antes de carga.',
         TIMESTAMP '2026-06-14 06:25:00',
         'baja', 'CERRADO', MIN(VUE_ID_VUELO), 24
  FROM AER_VUELO v
  JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
  WHERE p.PRV_NUMERO_VUELO = 'LA6100M';

  INSERT INTO AER_INCIDENTE
  (INC_TIPO, INC_DESCRIPCION, INC_FECHA_HORA, INC_SEVERIDAD, INC_ESTADO, INC_ID_VUELO, INC_ID_EMPLEADO_REPORTA)
  SELECT 'tecnico',
         'Mensaje intermitente en sistema de entretenimiento reportado por tripulacion en inspeccion preembarque.',
         TIMESTAMP '2026-06-28 19:05:00',
         'baja', 'EN_PROCESO', MIN(VUE_ID_VUELO), 33
  FROM AER_VUELO v
  JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
  WHERE p.PRV_NUMERO_VUELO = 'EK8800M';

  INSERT INTO AER_OBJETOPERDIDO
  (OBJ_ID_VUELO, OBJ_ID_AEROPUERTO, OBJ_DESCRIPCION, OBJ_FECHA_REPORTE, OBJ_UBICACION_ENCONTRADO, OBJ_ESTADO,
   OBJ_REP_PRIMER_NOMBRE, OBJ_REP_SEGUNDO_NOMBRE, OBJ_REP_PRIMER_APELLIDO, OBJ_REP_SEGUNDO_APELLIDO, OBJ_CONTACTO_REPORTANTE,
   OBJ_FECHA_ENTREGA, OBJ_REC_PRIMER_NOMBRE, OBJ_REC_SEGUNDO_NOMBRE, OBJ_REC_PRIMER_APELLIDO, OBJ_REC_SEGUNDO_APELLIDO)
  SELECT MIN(VUE_ID_VUELO), 1, 'Tablet gris con funda ejecutiva encontrada en sala internacional',
         DATE '2026-06-12', 'Sala A09', 'ENTREGADO',
         'Maria', 'Elena', 'Santos', 'Paz', '+502 4111-9021',
         DATE '2026-06-12', 'Carlos', 'Andres', 'Lopez', 'Rios'
  FROM AER_VUELO v
  JOIN AER_PROGRAMAVUELO p ON p.PRV_ID = v.VUE_ID_PROGRAMA_VUELO
  WHERE p.PRV_NUMERO_VUELO = 'UA3100M';

  INSERT INTO AER_ARRESTO
  (ARR_ID_PASAJERO, ARR_ID_VUELO, ARR_ID_AEROPUERTO, ARR_FECHA_HORA_ARRESTO, ARR_MOTIVO, ARR_AUTORIDAD_CARGO,
   ARR_DESCRIPCION_INCIDENTE, ARR_UBICACION_ARRESTO, ARR_ESTADO_CASO, ARR_NUMERO_EXPEDIENTE)
  VALUES
  (95, NULL, 1, TIMESTAMP '2026-07-03 11:26:00',
   'Intento de acceso no autorizado a area restringida de plataforma',
   'Policia Nacional Civil - Subestacion Aeropuerto',
   'Persona retenida por personal de seguridad y entregada a autoridad competente sin impacto operacional mayor.',
   'Corredor de servicio plataforma norte', 'CERRADO', 'PNC-AUR-2026-0723');

  INSERT INTO AER_AUDITORIA
  (AUD_TABLA_AFECTADA, AUD_OPERACION, AUD_USUARIO, AUD_FECHA_HORA, AUD_IP_ADDRESS, AUD_DATOS_ANTERIORES, AUD_DATOS_NUEVOS)
  VALUES
  ('AER_VUELO', 'UPDATE', 'ops.network', TIMESTAMP '2026-06-11 17:42:00', '10.20.8.11',
   'Salida AF4500M en estado PROGRAMADO',
   'Salida AF4500M ajustada a RETRASADO por clima; demora 35 min');

  INSERT INTO AER_AUDITORIA
  (AUD_TABLA_AFECTADA, AUD_OPERACION, AUD_USUARIO, AUD_FECHA_HORA, AUD_IP_ADDRESS, AUD_DATOS_ANTERIORES, AUD_DATOS_NUEVOS)
  VALUES
  ('AER_TRANSACCIONPAGO', 'INSERT', 'checkout.web', TIMESTAMP '2026-06-14 07:03:00', '181.174.15.88',
   NULL, 'Pago aprobado en checkout multimoneda para reserva intercontinental');

  INSERT INTO AER_AUDITORIA
  (AUD_TABLA_AFECTADA, AUD_OPERACION, AUD_USUARIO, AUD_FECHA_HORA, AUD_IP_ADDRESS, AUD_DATOS_ANTERIORES, AUD_DATOS_NUEVOS)
  VALUES
  ('AER_MANTENIMIENTOAVION', 'INSERT', 'mant.jefe', TIMESTAMP '2026-07-05 02:46:00', '10.40.2.18',
   NULL, 'Creada revision operacional larga distancia para widebody Emirates');

  COMMIT;
END;
/

PROMPT Seed maestra cargada: catalogo global, usuarios hash, vuelos masivos y cadena operativa/comercial completa.

--------------------------------------------------------
-- AEROPUERTO AURORA - RESET DE IDENTITY COLUMNS
-- Ejecutar despues del seed si insertaste IDs fijos.
-- Oracle 21c soporta START WITH LIMIT VALUE para avanzar cada
-- identity al siguiente valor seguro segun la data existente.
--------------------------------------------------------

ALTER SESSION SET CURRENT_SCHEMA = AEROPUERTO_AURORA;

BEGIN
    FOR identity_column IN (
        SELECT table_name, column_name
        FROM user_tab_identity_cols
        ORDER BY table_name
    ) LOOP
        EXECUTE IMMEDIATE
            'ALTER TABLE "' || identity_column.table_name || '" MODIFY "' ||
            identity_column.column_name ||
            '" GENERATED BY DEFAULT ON NULL AS IDENTITY (START WITH LIMIT VALUE)';
    END LOOP;
END;
/

COMMIT;
