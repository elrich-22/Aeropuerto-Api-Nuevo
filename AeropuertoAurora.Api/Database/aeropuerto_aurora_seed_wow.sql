--------------------------------------------------------
-- AEROPUERTO AURORA - SEED OPERATIVA "WOW"
-- Motor: Oracle 21c
-- Fecha de corte operacional: 2026-04-27
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
VALUES (1, 'GUA', 'Aeropuerto Internacional La Aurora', 'Ciudad de Guatemala', 'Guatemala', 'America/Guatemala', 'ACTIVO', 'internacional', 14.583300, -90.527500, 'GUA', 'MGGT', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (2, 'FRS', 'Aeropuerto Internacional Mundo Maya', 'Flores', 'Guatemala', 'America/Guatemala', 'ACTIVO', 'internacional', 16.913800, -89.866400, 'FRS', 'MGTK', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (3, 'SAL', 'Aeropuerto Internacional de El Salvador San Oscar Arnulfo Romero', 'San Salvador', 'El Salvador', 'America/El_Salvador', 'ACTIVO', 'internacional', 13.440900, -89.055700, 'SAL', 'MSLP', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (4, 'SAP', 'Aeropuerto Internacional Ramon Villeda Morales', 'San Pedro Sula', 'Honduras', 'America/Tegucigalpa', 'ACTIVO', 'internacional', 15.452600, -87.923600, 'SAP', 'MHLM', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (5, 'SJO', 'Aeropuerto Internacional Juan Santamaria', 'San Jose', 'Costa Rica', 'America/Costa_Rica', 'ACTIVO', 'internacional', 9.993900, -84.208800, 'SJO', 'MROC', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (6, 'PTY', 'Aeropuerto Internacional de Tocumen', 'Ciudad de Panama', 'Panama', 'America/Panama', 'ACTIVO', 'hub', 9.071400, -79.383500, 'PTY', 'MPTO', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (7, 'MEX', 'Aeropuerto Internacional Benito Juarez', 'Ciudad de Mexico', 'Mexico', 'America/Mexico_City', 'ACTIVO', 'hub', 19.436300, -99.072100, 'MEX', 'MMMX', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (8, 'CUN', 'Aeropuerto Internacional de Cancun', 'Cancun', 'Mexico', 'America/Cancun', 'ACTIVO', 'turistico', 21.036500, -86.877100, 'CUN', 'MMUN', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (9, 'MIA', 'Miami International Airport', 'Miami', 'Estados Unidos', 'America/New_York', 'ACTIVO', 'hub', 25.795900, -80.287000, 'MIA', 'KMIA', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (10, 'IAH', 'George Bush Intercontinental Airport', 'Houston', 'Estados Unidos', 'America/Chicago', 'ACTIVO', 'hub', 29.990200, -95.336800, 'IAH', 'KIAH', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (11, 'LAX', 'Los Angeles International Airport', 'Los Angeles', 'Estados Unidos', 'America/Los_Angeles', 'ACTIVO', 'hub', 33.941600, -118.408500, 'LAX', 'KLAX', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (12, 'JFK', 'John F. Kennedy International Airport', 'Nueva York', 'Estados Unidos', 'America/New_York', 'ACTIVO', 'hub', 40.641300, -73.778100, 'JFK', 'KJFK', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (13, 'BOG', 'Aeropuerto Internacional El Dorado', 'Bogota', 'Colombia', 'America/Bogota', 'ACTIVO', 'hub', 4.701600, -74.146900, 'BOG', 'SKBO', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (14, 'LIM', 'Aeropuerto Internacional Jorge Chavez', 'Lima', 'Peru', 'America/Lima', 'ACTIVO', 'hub', -12.021900, -77.114300, 'LIM', 'SPJC', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (15, 'MAD', 'Aeropuerto Adolfo Suarez Madrid-Barajas', 'Madrid', 'Espana', 'Europe/Madrid', 'ACTIVO', 'intercontinental', 40.498300, -3.567600, 'MAD', 'LEMD', DATE '2026-01-02');
INSERT INTO AER_AEROPUERTO VALUES (16, 'ATL', 'Hartsfield-Jackson Atlanta International Airport', 'Atlanta', 'Estados Unidos', 'America/New_York', 'ACTIVO', 'hub', 33.640700, -84.427700, 'ATL', 'KATL', DATE '2026-01-02');

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

INSERT INTO AER_AEROLINEA VALUES (1, 'AV', 'Avianca', 'Colombia', 'AV', 'AVA', 'ACTIVA', '+502 2421-2200', 'gua@avianca.com', 'https://www.avianca.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (2, 'CM', 'Copa Airlines', 'Panama', 'CM', 'CMP', 'ACTIVA', '+502 2307-2600', 'gua@copaair.com', 'https://www.copaair.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (3, 'AA', 'American Airlines', 'Estados Unidos', 'AA', 'AAL', 'ACTIVA', '+502 2422-8000', 'gua@aa.com', 'https://www.aa.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (4, 'UA', 'United Airlines', 'Estados Unidos', 'UA', 'UAL', 'ACTIVA', '+502 2376-0100', 'gua@united.com', 'https://www.united.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (5, 'DL', 'Delta Air Lines', 'Estados Unidos', 'DL', 'DAL', 'ACTIVA', '+502 2421-1000', 'gua@delta.com', 'https://www.delta.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (6, 'AM', 'Aeromexico', 'Mexico', 'AM', 'AMX', 'ACTIVA', '+502 2302-5799', 'gua@aeromexico.com', 'https://www.aeromexico.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (7, 'IB', 'Iberia', 'Espana', 'IB', 'IBE', 'ACTIVA', '+502 2278-6300', 'gua@iberia.com', 'https://www.iberia.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (8, 'Y4', 'Volaris', 'Mexico', 'Y4', 'VOI', 'ACTIVA', '+502 2301-3939', 'gua@volaris.com', 'https://www.volaris.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (9, '5U', 'TAG Airlines', 'Guatemala', '5U', 'TGU', 'ACTIVA', '+502 2380-9494', 'operaciones@tag.com.gt', 'https://www.tag.com.gt', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (10, 'NK', 'Spirit Airlines', 'Estados Unidos', 'NK', 'NKS', 'ACTIVA', '+502 2410-5200', 'gua@spirit.com', 'https://www.spirit.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (11, 'B6', 'JetBlue Airways', 'Estados Unidos', 'B6', 'JBU', 'ACTIVA', '+502 2410-5300', 'gua@jetblue.com', 'https://www.jetblue.com', DATE '2026-01-05');
INSERT INTO AER_AEROLINEA VALUES (12, 'D0', 'DHL Aviation', 'Alemania', 'D0', 'DHK', 'ACTIVA', '+502 2385-9500', 'gua.gateway@dhl.com', 'https://aviationcargo.dhl.com', DATE '2026-01-05');

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

INSERT INTO AER_PROGRAMAVUELO VALUES (1, 'AV640', 1, 1, 9, '07:15', '11:50', 155, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (2, 'CM391', 2, 1, 6, '05:39', '08:03', 144, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (3, 'AA1188', 3, 1, 9, '12:42', '17:21', 159, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (4, 'UA1901', 4, 1, 10, '01:30', '05:18', 168, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (5, 'DL1830', 5, 1, 16, '13:05', '17:42', 157, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (6, 'AM679', 6, 1, 7, '16:48', '19:10', 142, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (7, 'IB6342', 7, 1, 15, '17:10', '13:35', 685, 'intercontinental', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (8, 'Y43931', 8, 1, 8, '09:30', '12:22', 172, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (9, '5U110', 9, 1, 2, '06:20', '07:20', 60, 'nacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (10, '5U211', 9, 1, 3, '08:10', '09:05', 55, 'regional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (11, 'NK515', 10, 1, 11, '11:30', '16:26', 176, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (12, 'B62029', 11, 1, 12, '23:40', '05:45', 245, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (13, 'D0254', 12, 1, 9, '22:15', '02:55', 160, 'carga', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (14, 'AV743', 1, 1, 13, '15:05', '18:20', 195, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (15, 'CM407', 2, 1, 5, '10:15', '12:05', 110, 'internacional', 'ACTIVO', DATE '2026-01-10');
INSERT INTO AER_PROGRAMAVUELO VALUES (16, 'AM671', 6, 1, 7, '07:55', '10:12', 137, 'internacional', 'ACTIVO', DATE '2026-01-10');

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
    VALUES (i, i, DATE '2026-04-27',
            TIMESTAMP '2026-04-27 06:00:00' + NUMTODSINTERVAL(MOD(i,5)*15, 'MINUTE'),
            TIMESTAMP '2026-04-27 14:00:00' + NUMTODSINTERVAL(MOD(i,5)*15, 'MINUTE'),
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
     DATE '2026-01-01' + MOD(i, 100));
  END LOOP;

  FOR i IN 1..45 LOOP
    INSERT INTO AER_USUARIO_LOGIN
    (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS)
    SELECT i, i, 'viajero' || LPAD(i,3,'0'), PAS_EMAIL,
           '1234', 'seed-simple',
           CASE WHEN i = 44 THEN 'BLOQUEADO' ELSE 'ACTIVO' END, CASE WHEN i = 43 THEN 'N' ELSE 'S' END,
           TIMESTAMP '2026-02-01 08:00:00' + NUMTODSINTERVAL(i, 'DAY'),
           TIMESTAMP '2026-04-26 18:00:00' - NUMTODSINTERVAL(MOD(i,12), 'HOUR'),
           CASE WHEN i = 44 THEN 5 ELSE 0 END
    FROM AER_PASAJERO WHERE PAS_ID_PASAJERO = i;
  END LOOP;

  FOR i IN 1..70 LOOP
    INSERT INTO AER_SESIONUSUARIO
    VALUES (i, 'SES-202604-' || LPAD(i, 5, '0'),
            CASE WHEN i <= 45 THEN i ELSE NULL END,
            CASE WHEN i <= 45 THEN i ELSE NULL END,
            '190.148.' || MOD(i,255) || '.' || (20 + MOD(i,200)),
            CASE WHEN MOD(i,3)=0 THEN 'Chrome' WHEN MOD(i,3)=1 THEN 'Safari' ELSE 'Edge' END,
            CASE WHEN MOD(i,4)=0 THEN 'iOS' WHEN MOD(i,4)=1 THEN 'Android' WHEN MOD(i,4)=2 THEN 'Windows' ELSE 'macOS' END,
            CASE WHEN MOD(i,3)=0 THEN 'movil' WHEN MOD(i,3)=1 THEN 'desktop' ELSE 'tablet' END,
            TIMESTAMP '2026-04-20 06:00:00' + NUMTODSINTERVAL(i * 37, 'MINUTE'),
            TIMESTAMP '2026-04-20 06:08:00' + NUMTODSINTERVAL(i * 37, 'MINUTE'),
            480 + MOD(i,900));
  END LOOP;

  FOR i IN 1..90 LOOP
    INSERT INTO AER_PREFERENCIACLIENTE
    VALUES (i, i, CASE WHEN MOD(i,3)=0 THEN 'asiento' WHEN MOD(i,3)=1 THEN 'notificacion' ELSE 'equipaje' END,
            CASE WHEN MOD(i,3)=0 THEN 'ventana' WHEN MOD(i,3)=1 THEN 'email y app' ELSE 'factura una maleta' END,
            TIMESTAMP '2026-03-01 10:00:00' + NUMTODSINTERVAL(i, 'HOUR'));
  END LOOP;

  INSERT INTO AER_PASAJERO
  (PAS_ID_PASAJERO, PAS_NUMERO_DOCUMENTO, PAS_TIPO_DOCUMENTO, PAS_PRIMER_NOMBRE, PAS_SEGUNDO_NOMBRE, PAS_PRIMER_APELLIDO, PAS_SEGUNDO_APELLIDO,
   PAS_FECHA_NACIMIENTO, PAS_NACIONALIDAD, PAS_SEXO, PAS_TELEFONO, PAS_EMAIL, PAS_FECHA_REGISTRO)
  VALUES
  (91, 'ADMIN-AURORA-001', 'INTERNO', 'Administrador', 'Sistema', 'Aurora', 'Principal',
   DATE '1988-01-01', 'Guatemala', 'M', '+502 2300-0001', 'admin@aurora.gt', DATE '2026-01-01');

  INSERT INTO AER_USUARIO_LOGIN
  (USL_ID_USUARIO, USL_ID_PASAJERO, USL_USUARIO, USL_EMAIL, USL_CONTRASENA_HASH, USL_SAL, USL_ESTADO, USL_EMAIL_VERIFICADO, USL_TOKEN_VERIFICACION,
   USL_FECHA_REGISTRO, USL_ULTIMO_ACCESO, USL_INTENTOS_FALLIDOS, USL_BLOQUEADO_HASTA, USL_TOKEN_RECUPERACION, USL_VENCIMIENTO_TOKEN)
  VALUES
  (91, 91, 'admin.aurora', 'admin@aurora.gt',
   '1234', 'seed-simple', 'ACTIVO', 'S', NULL,
   TIMESTAMP '2026-01-01 08:00:00', TIMESTAMP '2026-04-27 06:00:00', 0, NULL, NULL, NULL);

  INSERT INTO AER_PREFERENCIACLIENTE
  VALUES (91, 91, 'rol_sistema', 'ADMINISTRADOR', TIMESTAMP '2026-01-01 08:00:00');
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

    v_salida := TIMESTAMP '2026-04-27 05:30:00' + NUMTODSINTERVAL(i * 31, 'MINUTE');
    v_llegada := v_salida + NUMTODSINTERVAL(80 + MOD(i, 6) * 35, 'MINUTE');

    INSERT INTO AER_VUELO
    (VUE_ID_VUELO, VUE_ID_PROGRAMA_VUELO, VUE_ID_AVION, VUE_FECHA_VUELO, VUE_HORA_SALIDA_REAL, VUE_HORA_LLEGADA_REAL,
     VUE_PLAZAS_OCUPADAS, VUE_PLAZAS_VACIAS, VUE_ESTADO, VUE_FECHA_REPROGRAMACION, VUE_MOTIVO_CANCELACION, VUE_RETRASO_MINUTOS)
    VALUES
    (i, v_programa, v_avion, DATE '2026-04-27',
     CASE WHEN v_estado IN ('ATERRIZADO','EN_VUELO','RETRASADO') THEN v_salida + NUMTODSINTERVAL(CASE WHEN v_estado='RETRASADO' THEN 45 ELSE MOD(i,12) END, 'MINUTE') ELSE NULL END,
     CASE WHEN v_estado = 'ATERRIZADO' THEN v_llegada + NUMTODSINTERVAL(MOD(i,10), 'MINUTE') ELSE NULL END,
     0, 0, v_estado,
     CASE WHEN v_estado='RETRASADO' THEN DATE '2026-04-27' ELSE NULL END,
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
    VALUES (i, v_vuelo, v_pasajero, v_clase, DATE '2026-04-10' + MOD(i,15),
            CASE WHEN i IN (28, 71) THEN 'CANCELADA' WHEN i <= 72 THEN 'COMPLETADA' ELSE 'CONFIRMADA' END,
            CASE WHEN MOD(i,4)=0 THEN 2 ELSE 1 END,
            CASE WHEN MOD(i,4)=0 THEN 31.5 ELSE 18.2 + MOD(i,8) END,
            v_tarifa, 'AUR' || TO_CHAR(DATE '2026-04-27','YYMMDD') || LPAD(i,5,'0'));

    INSERT INTO AER_ASIGNACION_ASIENTO
    VALUES (i, v_vuelo, v_pasajero, v_asiento,
            TIMESTAMP '2026-04-10 09:00:00' + NUMTODSINTERVAL(i, 'HOUR'),
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
    VALUES (i, i, 'SES-202604-' || LPAD(i,5,'0'),
            TIMESTAMP '2026-04-26 08:00:00' + NUMTODSINTERVAL(i*9,'MINUTE'),
            TIMESTAMP '2026-04-26 08:20:00' + NUMTODSINTERVAL(i*9,'MINUTE'),
            CASE WHEN i <= 16 THEN 'CONVERTIDO_RESERVA' WHEN i <= 21 THEN 'EXPIRADO' ELSE 'ACTIVO' END);
    INSERT INTO AER_ITEMCARRITO
    VALUES (i, i, MOD(i-1,48)+1, TO_CHAR(4+MOD(i,10)) || CHR(65+MOD(i,6)),
            CASE WHEN MOD(i,5)=0 THEN 'ejecutiva' ELSE 'economica' END,
            CASE WHEN MOD(i,5)=0 THEN 3250 ELSE 1350 END, 1);
  END LOOP;

  FOR i IN 1..70 LOOP
    INSERT INTO AER_BUSQUEDAVUELO
    VALUES (i, CASE WHEN i <= 60 THEN i ELSE NULL END, 1, 2 + MOD(i,15),
            DATE '2026-04-27' + MOD(i,20), CASE WHEN MOD(i,4)=0 THEN DATE '2026-05-06' + MOD(i,12) ELSE NULL END,
            1 + MOD(i,4), CASE WHEN MOD(i,8)=0 THEN 'ejecutiva' ELSE 'economica' END,
            TIMESTAMP '2026-04-20 07:00:00' + NUMTODSINTERVAL(i*23,'MINUTE'),
            CASE WHEN i <= 42 THEN 'S' ELSE 'N' END);
    INSERT INTO AER_CLICKDESTINO
    VALUES (i, CASE WHEN i <= 60 THEN i ELSE NULL END, 2 + MOD(i,15),
            TIMESTAMP '2026-04-20 07:02:00' + NUMTODSINTERVAL(i*23,'MINUTE'),
            CASE WHEN MOD(i,2)=0 THEN 'web' ELSE 'app' END,
            DATE '2026-04-27' + MOD(i,20), 1 + MOD(i,4),
            CASE WHEN MOD(i,8)=0 THEN 'ejecutiva' ELSE 'economica' END);
  END LOOP;

  FOR i IN 1..80 LOOP
    INSERT INTO AER_VENTABOLETO
    VALUES (i, 'VEN-20260427-' || LPAD(i,5,'0'),
            CASE WHEN MOD(i,4) IN (0,1) THEN NULL ELSE 2 + MOD(i,2) END,
            CASE WHEN MOD(i,4) IN (0,1) THEN NULL ELSE 35 + MOD(i,3) END,
            MOD(i-1,90)+1,
            TIMESTAMP '2026-04-12 08:00:00' + NUMTODSINTERVAL(i*47,'MINUTE'),
            1180 + MOD(i,7)*95,
            ROUND((1180 + MOD(i,7)*95) * 0.12, 2),
            CASE WHEN MOD(i,10)=0 THEN 125 ELSE 0 END,
            (1180 + MOD(i,7)*95) + ROUND((1180 + MOD(i,7)*95) * 0.12, 2) - CASE WHEN MOD(i,10)=0 THEN 125 ELSE 0 END,
            MOD(i-1,5)+1,
            1,
            CASE WHEN MOD(i,4)=0 THEN 'web' WHEN MOD(i,4)=1 THEN 'app' WHEN MOD(i,4)=2 THEN 'mostrador' ELSE 'telefono' END,
            CASE WHEN i IN (28,71) THEN 'CANCELADA' ELSE 'COMPLETADA' END);

    INSERT INTO AER_DETALLEVENTABOLETO
    SELECT i, i, i, 1180 + MOD(i,7)*95, CASE WHEN MOD(i,4)=0 THEN 180 ELSE 75 END,
           TRIM(PAS_PRIMER_NOMBRE || ' ' || NVL(PAS_SEGUNDO_NOMBRE || ' ', '') || PAS_PRIMER_APELLIDO || ' ' || NVL(PAS_SEGUNDO_APELLIDO, '')),
           PAS_TIPO_DOCUMENTO,
           PAS_NUMERO_DOCUMENTO
      FROM AER_PASAJERO
     WHERE PAS_ID_PASAJERO = i;

    INSERT INTO AER_TRANSACCIONPAGO
    VALUES (i, i, MOD(i-1,5)+1,
            (1180 + MOD(i,7)*95) + ROUND((1180 + MOD(i,7)*95) * 0.12, 2) - CASE WHEN MOD(i,10)=0 THEN 125 ELSE 0 END,
            CASE WHEN MOD(i,6)=0 THEN 'USD' ELSE 'GTQ' END,
            TIMESTAMP '2026-04-12 08:02:00' + NUMTODSINTERVAL(i*47,'MINUTE'),
            CASE WHEN i IN (28,71) THEN 'REEMBOLSADA' ELSE 'APROBADA' END,
            'AUTH' || LPAD(i*391, 8, '0'), 'AURPAY-' || LPAD(i, 8, '0'),
            '190.148.' || MOD(i,255) || '.' || (30 + MOD(i,180)),
            CASE WHEN MOD(i,5)=4 THEN 'Pago efectivo en mostrador' ELSE 'Tarjeta terminada en ' || LPAD(MOD(i*73,10000),4,'0') END);

    IF MOD(i,10)=0 THEN
      INSERT INTO AER_USOPROMOCION VALUES (i/10, 2, i, TIMESTAMP '2026-04-12 08:03:00' + NUMTODSINTERVAL(i*47,'MINUTE'), 125);
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
           TIMESTAMP '2026-04-26 20:00:00' + NUMTODSINTERVAL(i*17,'MINUTE'),
           CASE WHEN MOD(i,4)=0 THEN 'app' WHEN MOD(i,4)=1 THEN 'web' WHEN MOD(i,4)=2 THEN 'kiosko' ELSE 'mostrador' END,
           'COMPLETADO'
    FROM AER_RESERVA WHERE RES_ID_RESERVA = i AND RES_ESTADO <> 'CANCELADA';

    IF SQL%ROWCOUNT = 1 THEN
      INSERT INTO AER_TARJETA_EMBARQUE
      VALUES (i, i, 'QR-AUR-' || LPAD(i,6,'0') || '-GUA', CHR(65 + MOD(i,4)), 'Z' || TO_CHAR(1 + MOD(i,5)),
              TIMESTAMP '2026-04-26 20:01:00' + NUMTODSINTERVAL(i*17,'MINUTE'));

      IF MOD(i,3) <> 0 THEN
        INSERT INTO AER_EQUIPAJE
        SELECT i, RES_ID_PASAJERO, RES_ID_VUELO, 'GUA20260427' || LPAD(i,5,'0'),
               15 + MOD(i,17) + 0.35, CASE WHEN i <= 18 THEN 'ENTREGADO' WHEN i <= 38 THEN 'CARGADO' ELSE 'REGISTRADO' END,
               TIMESTAMP '2026-04-26 20:05:00' + NUMTODSINTERVAL(i*17,'MINUTE')
        FROM AER_RESERVA WHERE RES_ID_RESERVA = i;

        INSERT INTO AER_MOVIMIENTO_EQUIPAJE VALUES (i*10+1, i, 'Mostrador de equipaje GUA', 'REGISTRADO',
          TIMESTAMP '2026-04-26 20:06:00' + NUMTODSINTERVAL(i*17,'MINUTE'), 'Etiqueta impresa y peso validado');
        INSERT INTO AER_MOVIMIENTO_EQUIPAJE VALUES (i*10+2, i, 'Sistema BHS - cinta primaria', 'EN_TRANSITO',
          TIMESTAMP '2026-04-26 20:18:00' + NUMTODSINTERVAL(i*17,'MINUTE'), 'Lectura automatica de codigo de barras');
        IF i <= 38 THEN
          INSERT INTO AER_MOVIMIENTO_EQUIPAJE VALUES (i*10+3, i, 'Bodega aeronave', 'CARGADO',
            TIMESTAMP '2026-04-26 20:42:00' + NUMTODSINTERVAL(i*17,'MINUTE'), 'Cargado por cuadrilla de rampa');
        END IF;
      END IF;

      INSERT INTO AER_CONTROL_SEGURIDAD
      SELECT i, RES_ID_PASAJERO, RES_ID_VUELO, 21 + MOD(i,5),
             CASE WHEN MOD(i,31)=0 THEN 'REVISION' ELSE 'APROBADO' END,
             TIMESTAMP '2026-04-26 21:00:00' + NUMTODSINTERVAL(i*13,'MINUTE'),
             CASE WHEN MOD(i,31)=0 THEN 'Revision secundaria por articulo metalico; liberado sin novedad' ELSE 'Control normal' END
      FROM AER_RESERVA WHERE RES_ID_RESERVA = i;

      INSERT INTO AER_CONTROL_MIGRATORIO
      SELECT i, RES_ID_PASAJERO, RES_ID_VUELO, 26 + MOD(i,4), 'salida',
             CASE WHEN MOD(i,37)=0 THEN 'REVISION' ELSE 'APROBADO' END,
             TIMESTAMP '2026-04-26 21:10:00' + NUMTODSINTERVAL(i*13,'MINUTE'),
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
INSERT INTO AER_ASIGNACIONHANGAR VALUES (2, 2, 17, TIMESTAMP '2026-04-27 02:30:00', TIMESTAMP '2026-04-27 11:30:00', NULL, 'Inspeccion de carga y puerta lateral', 980, NULL, 'ACTIVA');

INSERT INTO AER_MANTENIMIENTOAVION VALUES (1, 13, 'B-check programado', TIMESTAMP '2026-04-24 06:15:00', TIMESTAMP '2026-04-26 18:00:00', TIMESTAMP '2026-04-26 16:20:00', 'Inspeccion de tren principal, cambio de kit de frenos y prueba funcional de avionica. Aeronave liberada con observaciones menores documentadas.', 34, 18400, 19180, 37580, 'FINALIZADO');
INSERT INTO AER_MANTENIMIENTOAVION VALUES (2, 17, 'Inspeccion linea carga', TIMESTAMP '2026-04-27 02:45:00', TIMESTAMP '2026-04-27 11:30:00', NULL, 'Revision de puerta lateral de carga, chequeo hidraulico y prueba de cierre antes de vuelo nocturno.', 33, 6900, 0, 6900, 'EN_PROCESO');

INSERT INTO AER_REPUESTOUTILIZADO VALUES (1, 1, 2, 2);
INSERT INTO AER_REPUESTOUTILIZADO VALUES (2, 1, 5, 1);
INSERT INTO AER_REPUESTOUTILIZADO VALUES (3, 1, 6, 6);

--------------------------------------------------------
-- 10. Incidentes, objetos perdidos, arrestos y auditoria
--------------------------------------------------------

INSERT INTO AER_INCIDENTE VALUES (1, 'clima', 'Celula de tormenta al sur de la pista obligo a espaciar aproximaciones durante 22 minutos.', TIMESTAMP '2026-04-27 14:18:00', 'media', 'CERRADO', 19, 2);
INSERT INTO AER_INCIDENTE VALUES (2, 'equipaje', 'Maleta sin lectura automatica desviada a revision manual y reetiquetada.', TIMESTAMP '2026-04-27 09:42:00', 'baja', 'CERRADO', 11, 24);
INSERT INTO AER_INCIDENTE VALUES (3, 'tecnico', 'Indicacion intermitente en panel de puerta de carga durante preparacion de vuelo DHL.', TIMESTAMP '2026-04-27 03:05:00', 'media', 'EN_PROCESO', 13, 33);
INSERT INTO AER_INCIDENTE VALUES (4, 'pasajero', 'Pasajero con documentacion incompleta derivado a aerolinea y migracion para validacion.', TIMESTAMP '2026-04-27 06:50:00', 'baja', 'CERRADO', 4, 28);

INSERT INTO AER_OBJETOPERDIDO
VALUES (1, 9, 1, 'Mochila negra con laptop Lenovo y cuaderno azul', DATE '2026-04-27', 'Sala A05, fila de espera', 'ENTREGADO',
        'Rosa', 'Maria', 'Paz', 'Molina', '+502 4777-1201', DATE '2026-04-27', 'Rosa', 'Maria', 'Paz', 'Molina');
INSERT INTO AER_OBJETOPERDIDO
VALUES (2, NULL, 1, 'Pasaporte espanol encontrado en control de seguridad', DATE '2026-04-27', 'Filtro internacional 2', 'EN_CUSTODIA',
        'Victor', 'Manuel', 'Reyes', 'Luna', 'seguridad@aurora.gt', NULL, NULL, NULL, NULL, NULL);
INSERT INTO AER_OBJETOPERDIDO
VALUES (3, 15, 1, 'Estuche gris con audifonos inalambricos', DATE '2026-04-26', 'Puerta A11', 'REPORTADO',
        'Camila', 'Sofia', 'Castro', 'Diaz', '+502 4999-3321', NULL, NULL, NULL, NULL, NULL);

INSERT INTO AER_ARRESTO
VALUES (1, 88, NULL, 1, TIMESTAMP '2026-04-27 12:16:00', 'Alteracion del orden en area publica y negativa a retirarse tras advertencia de seguridad',
        'Policia Nacional Civil - Subestacion Aeropuerto', 'La persona fue trasladada al area de seguridad aeroportuaria. No se afecto la operacion de vuelos.', 'Lobby salidas internacionales', 'CERRADO', 'PNC-AUR-2026-0441');

INSERT INTO AER_AUDITORIA VALUES (1, 'AER_VUELO', 'UPDATE', 'ops.supervisor', TIMESTAMP '2026-04-27 14:22:00', '10.20.5.14', 'Estado PROGRAMADO vuelo 19', 'Estado RETRASADO por clima; retraso 45 minutos');
INSERT INTO AER_AUDITORIA VALUES (2, 'AER_EQUIPAJE', 'UPDATE', 'bhs.automatico', TIMESTAMP '2026-04-27 09:44:00', '10.30.7.22', 'Etiqueta sin lectura automatica', 'Reetiquetado y enviado a cinta primaria');
INSERT INTO AER_AUDITORIA VALUES (3, 'AER_MANTENIMIENTOAVION', 'INSERT', 'mant.jefe', TIMESTAMP '2026-04-27 02:46:00', '10.40.1.8', NULL, 'Creada inspeccion linea carga para aeronave D-AEAC');

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
  INSERT INTO AER_DOCUMENTO_PASAJERO (
    DCP_ID_DOCUMENTO,
    DCP_ID_PASAJERO,
    DCP_TIPO_DOCUMENTO,
    DCP_NUMERO_DOCUMENTO,
    DCP_PAIS_EMISOR,
    DCP_FECHA_VENCIMIENTO,
    DCP_ESTADO_VALIDACION
  )
  SELECT
    PAS_ID_PASAJERO,
    PAS_ID_PASAJERO,
    UPPER(PAS_TIPO_DOCUMENTO),
    PAS_NUMERO_DOCUMENTO,
    CASE WHEN UPPER(PAS_TIPO_DOCUMENTO) = 'PASAPORTE' THEN NVL(PAS_NACIONALIDAD, 'Internacional') ELSE 'Guatemala' END,
    CASE WHEN UPPER(PAS_TIPO_DOCUMENTO) = 'PASAPORTE' THEN ADD_MONTHS(TRUNC(SYSDATE), 24) ELSE NULL END,
    CASE WHEN UPPER(PAS_TIPO_DOCUMENTO) = 'PASAPORTE' THEN 'PENDIENTE' ELSE 'NO_REQUIERE' END
  FROM AER_PASAJERO;

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
  bump_identity('AER_DOCUMENTO_PASAJERO', 'DCP_ID_DOCUMENTO');
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
