--------------------------------------------------------
-- AEROPUERTO AURORA - SEED DE PRESENTACION v2.0
-- Seed ultra-realista para demo / presentacion del proyecto
-- Motor: Oracle 21c | Encoding: UTF-8
-- Vuelos: 2026-05-18 al 2026-12-31
--
-- Ejecutar despues de:
--   1) aeropuerto_aurora_reset_total.sql  (base limpia)
--   2) aeropuerto_aurora_v2.sql           (esquema)
--
-- Credenciales:
--   admin.aurora        / AdminAurora1!
--   soporte.operaciones / SoporteAurora1!
--   [usuarios activos]  / AuroraDemo1!
--   [usuarios inactivos/bloqueados] / DemoInactivo1!
--
-- Vuelos por ruta (ejemplos):
--   GUA-PTY  : 5 vuelos/dia (Copa Airlines - hub)
--   GUA-MIA  : 4 vuelos/dia (American, Delta, Avianca, Spirit)
--   GUA-FRS  : 5 vuelos/dia (TAG Airlines - domestico)
--   GUA-GRU  : 2 vuelos/dia (LATAM + Gol - Brasil!)
--   GUA-GIG  : 2 vuelos/dia (LATAM + Gol - Brasil!)
--   GUA-MAD  : 1 vuelo/dia  (Iberia)
--   GUA-CDG  : 1 vuelo/dia  (Air France)
--   ... y mas de 60 rutas adicionales
--------------------------------------------------------

SET DEFINE OFF;
ALTER SESSION SET NLS_DATE_FORMAT = 'YYYY-MM-DD';
ALTER SESSION SET NLS_TIMESTAMP_FORMAT = 'YYYY-MM-DD HH24:MI:SS';
ALTER SESSION SET CURRENT_SCHEMA = AEROPUERTO_AURORA;

PROMPT ============================================================
PROMPT  AEROPUERTO AURORA - SEED PRESENTACION v2.0
PROMPT  Cargando datos ultra-realistas para presentacion...
PROMPT ============================================================

--------------------------------------------------------
-- SECCION 1: AEROPUERTOS
--------------------------------------------------------

-- Guatemala
INSERT INTO AER_AEROPUERTO (AER_ID,AER_CODIGO_AEROPUERTO,AER_NOMBRE,AER_CIUDAD,AER_PAIS,AER_ZONA_HORARIA,AER_ESTADO,AER_TIPO,AER_LATITUD,AER_LONGITUD,AER_CODIGO_IATA,AER_CODIGO_ICAO,AER_FECHA_REGISTRO)
VALUES (1,'GUA','Aeropuerto Internacional La Aurora','Ciudad de Guatemala','Guatemala','America/Guatemala','ACTIVO','internacional',14.583300,-90.527500,'GUA','MGGT',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (2,'FRS','Aeropuerto Internacional Mundo Maya','Flores','Guatemala','America/Guatemala','ACTIVO','nacional',16.913800,-89.866400,'FRS','MGTK',DATE '2026-01-01');
-- Centroamerica
INSERT INTO AER_AEROPUERTO VALUES (3,'SAL','Aeropuerto Intl San Oscar Arnulfo Romero','San Salvador','El Salvador','America/El_Salvador','ACTIVO','internacional',13.440900,-89.055700,'SAL','MSLP',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (4,'SAP','Aeropuerto Intl Ramon Villeda Morales','San Pedro Sula','Honduras','America/Tegucigalpa','ACTIVO','internacional',15.452600,-87.923600,'SAP','MHLM',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (5,'SJO','Aeropuerto Internacional Juan Santamaria','San Jose','Costa Rica','America/Costa_Rica','ACTIVO','internacional',9.993900,-84.208800,'SJO','MROC',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (6,'PTY','Aeropuerto Internacional de Tocumen','Ciudad de Panama','Panama','America/Panama','ACTIVO','hub',9.071400,-79.383500,'PTY','MPTO',DATE '2026-01-01');
-- Mexico
INSERT INTO AER_AEROPUERTO VALUES (7,'MEX','Aeropuerto Internacional Benito Juarez','Ciudad de Mexico','Mexico','America/Mexico_City','ACTIVO','hub',19.436300,-99.072100,'MEX','MMMX',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (8,'CUN','Aeropuerto Internacional de Cancun','Cancun','Mexico','America/Cancun','ACTIVO','turistico',21.036500,-86.877100,'CUN','MMUN',DATE '2026-01-01');
-- USA
INSERT INTO AER_AEROPUERTO VALUES (9,'MIA','Miami International Airport','Miami','Estados Unidos','America/New_York','ACTIVO','hub',25.795900,-80.287000,'MIA','KMIA',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (10,'IAH','George Bush Intercontinental Airport','Houston','Estados Unidos','America/Chicago','ACTIVO','hub',29.990200,-95.336800,'IAH','KIAH',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (11,'LAX','Los Angeles International Airport','Los Angeles','Estados Unidos','America/Los_Angeles','ACTIVO','hub',33.941600,-118.408500,'LAX','KLAX',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (12,'JFK','John F. Kennedy International Airport','Nueva York','Estados Unidos','America/New_York','ACTIVO','hub',40.641300,-73.778100,'JFK','KJFK',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (16,'ATL','Hartsfield-Jackson Atlanta International Airport','Atlanta','Estados Unidos','America/New_York','ACTIVO','hub',33.640700,-84.427700,'ATL','KATL',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (17,'ORD','Chicago O Hare International Airport','Chicago','Estados Unidos','America/Chicago','ACTIVO','hub',41.974200,-87.907300,'ORD','KORD',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (18,'DFW','Dallas Fort Worth International Airport','Dallas','Estados Unidos','America/Chicago','ACTIVO','hub',32.899800,-97.040300,'DFW','KDFW',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (19,'MCO','Orlando International Airport','Orlando','Estados Unidos','America/New_York','ACTIVO','turistico',28.431200,-81.308100,'MCO','KMCO',DATE '2026-01-01');
-- Sudamerica
INSERT INTO AER_AEROPUERTO VALUES (13,'BOG','Aeropuerto Internacional El Dorado','Bogota','Colombia','America/Bogota','ACTIVO','hub',4.701600,-74.146900,'BOG','SKBO',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (14,'LIM','Aeropuerto Internacional Jorge Chavez','Lima','Peru','America/Lima','ACTIVO','hub',-12.021900,-77.114300,'LIM','SPJC',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (20,'GRU','Aeropuerto Intl Guarulhos','Sao Paulo','Brasil','America/Sao_Paulo','ACTIVO','hub',-23.435600,-46.473100,'GRU','SBGR',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (21,'GIG','Aeropuerto Intl Galeao','Rio de Janeiro','Brasil','America/Sao_Paulo','ACTIVO','hub',-22.809000,-43.250600,'GIG','SBGL',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (22,'SCL','Aeropuerto Intl Arturo Merino Benitez','Santiago','Chile','America/Santiago','ACTIVO','hub',-33.392800,-70.785800,'SCL','SCEL',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (23,'EZE','Aeropuerto Intl Ministro Pistarini','Buenos Aires','Argentina','America/Argentina/Buenos_Aires','ACTIVO','intercontinental',-34.822200,-58.535800,'EZE','SAEZ',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (24,'UIO','Aeropuerto Intl Mariscal Sucre','Quito','Ecuador','America/Guayaquil','ACTIVO','internacional',-0.129200,-78.357500,'UIO','SEQM',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (25,'SDQ','Aeropuerto Internacional Las Americas','Santo Domingo','Rep. Dominicana','America/Santo_Domingo','ACTIVO','turistico',18.429700,-69.668900,'SDQ','MDSD',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (26,'CCS','Aeropuerto Intl Simon Bolivar','Caracas','Venezuela','America/Caracas','ACTIVO','internacional',10.603100,-66.991200,'CCS','SVMI',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (27,'MDE','Aeropuerto Intl Jose Maria Cordova','Medellin','Colombia','America/Bogota','ACTIVO','internacional',6.164500,-75.423100,'MDE','SKRG',DATE '2026-01-01');
-- Europa
INSERT INTO AER_AEROPUERTO VALUES (15,'MAD','Aeropuerto Adolfo Suarez Madrid-Barajas','Madrid','Espana','Europe/Madrid','ACTIVO','intercontinental',40.498300,-3.567600,'MAD','LEMD',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (28,'LHR','London Heathrow Airport','Londres','Reino Unido','Europe/London','ACTIVO','intercontinental',51.470000,-0.454300,'LHR','EGLL',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (29,'CDG','Paris Charles de Gaulle Airport','Paris','Francia','Europe/Paris','ACTIVO','intercontinental',49.009700,2.547900,'CDG','LFPG',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (30,'FRA','Frankfurt Airport','Frankfurt','Alemania','Europe/Berlin','ACTIVO','hub',50.037900,8.562200,'FRA','EDDF',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (31,'LIS','Humberto Delgado Airport','Lisboa','Portugal','Europe/Lisbon','ACTIVO','intercontinental',38.774200,-9.134200,'LIS','LPPT',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (32,'AMS','Amsterdam Airport Schiphol','Amsterdam','Paises Bajos','Europe/Amsterdam','ACTIVO','hub',52.310500,4.768300,'AMS','EHAM',DATE '2026-01-01');
-- Asia / Medio Oriente
INSERT INTO AER_AEROPUERTO VALUES (33,'DXB','Dubai International Airport','Dubai','Emiratos Arabes Unidos','Asia/Dubai','ACTIVO','hub',25.253200,55.365700,'DXB','OMDB',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (34,'NRT','Narita International Airport','Tokio','Japon','Asia/Tokyo','ACTIVO','intercontinental',35.772000,140.392900,'NRT','RJAA',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (35,'DOH','Hamad International Airport','Doha','Qatar','Asia/Qatar','ACTIVO','hub',25.273100,51.608100,'DOH','OTHH',DATE '2026-01-01');
-- Caribe
INSERT INTO AER_AEROPUERTO VALUES (36,'HAV','Aeropuerto Internacional Jose Marti','La Habana','Cuba','America/Havana','ACTIVO','turistico',22.989200,-82.409100,'HAV','MUHA',DATE '2026-01-01');
INSERT INTO AER_AEROPUERTO VALUES (37,'SJU','Luis Munoz Marin International Airport','San Juan','Puerto Rico','America/Puerto_Rico','ACTIVO','turistico',18.439400,-66.001800,'SJU','TJSJ',DATE '2026-01-01');

COMMIT;
PROMPT Aeropuertos cargados (37 aeropuertos en 5 continentes).

--------------------------------------------------------
-- SECCION 2: TERMINALES Y PUERTAS (Aeropuerto GUA)
--------------------------------------------------------

INSERT INTO AER_TERMINAL VALUES (1,1,'Terminal Internacional','internacional',4200,'ACTIVA');
INSERT INTO AER_TERMINAL VALUES (2,1,'Terminal Nacional y Regional','nacional',1300,'ACTIVA');
INSERT INTO AER_TERMINAL VALUES (3,1,'Terminal de Carga Aerea','carga',650,'ACTIVA');
INSERT INTO AER_TERMINAL VALUES (4,1,'Aviacion General y Privada','privada',180,'ACTIVA');

BEGIN
  FOR i IN 1..16 LOOP
    INSERT INTO AER_PUERTA_EMBARQUE VALUES (i, 1, 'A'||LPAD(i,2,'0'),
      CASE WHEN i IN (7,11,14) THEN 'OCUPADA' ELSE 'DISPONIBLE' END, 'internacional');
  END LOOP;
  FOR i IN 1..8 LOOP
    INSERT INTO AER_PUERTA_EMBARQUE VALUES (100+i, 2, 'B'||LPAD(i,2,'0'), 'DISPONIBLE', 'nacional');
  END LOOP;
  INSERT INTO AER_PUERTA_EMBARQUE VALUES (201,3,'C01','DISPONIBLE','carga');
  INSERT INTO AER_PUERTA_EMBARQUE VALUES (202,3,'C02','DISPONIBLE','carga');
  INSERT INTO AER_PUERTA_EMBARQUE VALUES (301,1,'R01','DISPONIBLE','remota');
  INSERT INTO AER_PUERTA_EMBARQUE VALUES (302,1,'R02','MANTENIMIENTO','remota');
END;
/

COMMIT;

--------------------------------------------------------
-- SECCION 3: AEROLINEAS
--------------------------------------------------------

INSERT INTO AER_AEROLINEA VALUES (1,'AV','Avianca','Colombia','AV','AVA','ACTIVA','+502 2421-2200','gua@avianca.com','https://www.avianca.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (2,'CM','Copa Airlines','Panama','CM','CMP','ACTIVA','+502 2307-2600','gua@copaair.com','https://www.copaair.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (3,'AA','American Airlines','Estados Unidos','AA','AAL','ACTIVA','+502 2422-8000','gua@aa.com','https://www.aa.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (4,'UA','United Airlines','Estados Unidos','UA','UAL','ACTIVA','+502 2376-0100','gua@united.com','https://www.united.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (5,'DL','Delta Air Lines','Estados Unidos','DL','DAL','ACTIVA','+502 2421-1000','gua@delta.com','https://www.delta.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (6,'AM','Aeromexico','Mexico','AM','AMX','ACTIVA','+502 2302-5799','gua@aeromexico.com','https://www.aeromexico.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (7,'IB','Iberia','Espana','IB','IBE','ACTIVA','+502 2278-6300','gua@iberia.com','https://www.iberia.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (8,'Y4','Volaris','Mexico','Y4','VOI','ACTIVA','+502 2301-3939','gua@volaris.com','https://www.volaris.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (9,'5U','TAG Airlines','Guatemala','5U','TGU','ACTIVA','+502 2380-9494','operaciones@tag.com.gt','https://www.tag.com.gt',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (10,'NK','Spirit Airlines','Estados Unidos','NK','NKS','ACTIVA','+502 2410-5200','gua@spirit.com','https://www.spirit.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (11,'B6','JetBlue Airways','Estados Unidos','B6','JBU','ACTIVA','+502 2410-5300','gua@jetblue.com','https://www.jetblue.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (12,'D0','DHL Aviation','Alemania','D0','DHK','ACTIVA','+502 2385-9500','gua.gateway@dhl.com','https://aviationcargo.dhl.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (13,'AF','Air France','Francia','AF','AFR','ACTIVA','+33 892 70 26 54','hub.cdg@airfrance.fr','https://www.airfrance.fr',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (14,'BA','British Airways','Reino Unido','BA','BAW','ACTIVA','+44 344 493 0787','ops@britishairways.com','https://www.britishairways.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (15,'EK','Emirates','Emiratos Arabes Unidos','EK','UAE','ACTIVA','+971 600 555555','network@emirates.com','https://www.emirates.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (16,'LH','Lufthansa','Alemania','LH','DLH','ACTIVA','+49 69 86 799 799','network@lufthansa.com','https://www.lufthansa.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (17,'LA','LATAM Airlines','Brasil','LA','TAM','ACTIVA','+55 11 4002-5700','operacoes@latam.com','https://www.latamairlines.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (18,'TP','TAP Air Portugal','Portugal','TP','TAP','ACTIVA','+351 211 234 400','ops@tap.pt','https://www.flytap.com',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (19,'AR','Aerolineas Argentinas','Argentina','AR','ARG','ACTIVA','+54 11 5199 3555','ops@aerolineas.com.ar','https://www.aerolineas.com.ar',DATE '2026-01-01');
INSERT INTO AER_AEROLINEA VALUES (20,'G3','Gol Linhas Aereas','Brasil','G3','GLO','ACTIVA','+55 11 5504 4410','ops@voegol.com.br','https://www.voegol.com.br',DATE '2026-01-01');

COMMIT;
PROMPT Aerolineas cargadas (20 aerolineas internacionales).

--------------------------------------------------------
-- SECCION 4: MODELOS DE AVION
--------------------------------------------------------

INSERT INTO AER_MODELOAVION VALUES (1,'Airbus A320neo','Airbus',186,7900,6300,829,2016,'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (2,'Boeing 737-800','Boeing',189,7900,5436,842,1998,'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (3,'Boeing 737 MAX 9','Boeing',193,8200,6570,839,2017,'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (4,'Airbus A321neo','Airbus',220,9300,7400,829,2017,'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (5,'Embraer E190','Embraer',100,2800,4537,829,2004,'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (6,'ATR 72-600','ATR',72,750,1528,510,2010,'Turbohelice');
INSERT INTO AER_MODELOAVION VALUES (7,'Boeing 787-8 Dreamliner','Boeing',242,28000,13620,913,2011,'Turbofan');
INSERT INTO AER_MODELOAVION VALUES (8,'Boeing 757-200F','Boeing',0,34000,5834,850,1987,'Turbofan');

--------------------------------------------------------
-- SECCION 5: FLOTA DE AVIONES (34 aeronaves)
-- Distribuidas por aerolinea para rotacion realista
--------------------------------------------------------

-- Avianca (aerolinea 1): aviones 1-2
INSERT INTO AER_AVION VALUES (1,'N784AV',1,1,2021,'ACTIVO',DATE '2026-03-18',DATE '2026-09-18',8120);
INSERT INTO AER_AVION VALUES (2,'N791AV',1,1,2020,'ACTIVO',DATE '2026-04-10',DATE '2026-10-10',7680);
-- Copa Airlines (2): aviones 3-4
INSERT INTO AER_AVION VALUES (3,'HP-1831CMP',2,2,2018,'ACTIVO',DATE '2026-02-26',DATE '2026-08-26',14620);
INSERT INTO AER_AVION VALUES (4,'HP-1857CMP',2,2,2019,'ACTIVO',DATE '2026-04-03',DATE '2026-10-03',11220);
-- American Airlines (3): aviones 5-6
INSERT INTO AER_AVION VALUES (5,'N908NN',2,3,2017,'ACTIVO',DATE '2026-03-11',DATE '2026-09-11',17540);
INSERT INTO AER_AVION VALUES (6,'N909AA',4,3,2021,'ACTIVO',DATE '2026-04-15',DATE '2026-10-15',8025);
-- United Airlines (4): aviones 7-8
INSERT INTO AER_AVION VALUES (7,'N37518',3,4,2020,'ACTIVO',DATE '2026-04-01',DATE '2026-10-01',9270);
INSERT INTO AER_AVION VALUES (8,'N772UA',2,4,2015,'ACTIVO',DATE '2026-03-09',DATE '2026-09-09',21400);
-- Delta Air Lines (5): aviones 9-10
INSERT INTO AER_AVION VALUES (9,'N821DN',1,5,2019,'ACTIVO',DATE '2026-03-21',DATE '2026-09-21',10340);
INSERT INTO AER_AVION VALUES (10,'N823DN',1,5,2020,'ACTIVO',DATE '2026-04-20',DATE '2026-10-20',9180);
-- Aeromexico (6): aviones 11-12
INSERT INTO AER_AVION VALUES (11,'XA-AMR',3,6,2022,'ACTIVO',DATE '2026-02-28',DATE '2026-08-28',6410);
INSERT INTO AER_AVION VALUES (12,'XA-ADV',5,6,2014,'ACTIVO',DATE '2026-04-13',DATE '2026-10-13',18500);
-- Iberia (7): aviones 13-14
INSERT INTO AER_AVION VALUES (13,'EC-NBE',7,7,2019,'ACTIVO',DATE '2026-03-06',DATE '2026-09-06',11870);
INSERT INTO AER_AVION VALUES (14,'EC-NGG',7,7,2020,'ACTIVO',DATE '2026-04-22',DATE '2026-10-22',9760);
-- Volaris (8): avion 15
INSERT INTO AER_AVION VALUES (15,'XA-VLL',4,8,2021,'ACTIVO',DATE '2026-04-05',DATE '2026-10-05',7210);
-- TAG Airlines (9): aviones 16-17
INSERT INTO AER_AVION VALUES (16,'TG-TAG',6,9,2017,'ACTIVO',DATE '2026-04-10',DATE '2026-07-10',5900);
INSERT INTO AER_AVION VALUES (17,'TG-MYA',6,9,2018,'ACTIVO',DATE '2026-04-15',DATE '2026-07-15',4300);
-- Spirit (10): avion 18
INSERT INTO AER_AVION VALUES (18,'N672NK',1,10,2018,'ACTIVO',DATE '2026-03-14',DATE '2026-09-14',13440);
-- JetBlue (11): avion 19
INSERT INTO AER_AVION VALUES (19,'N304JB',5,11,2016,'ACTIVO',DATE '2026-02-19',DATE '2026-08-19',16180);
-- DHL (12): avion 20
INSERT INTO AER_AVION VALUES (20,'D-AEAC',8,12,2008,'ACTIVO',DATE '2026-03-25',DATE '2026-09-25',32200);
-- Air France (13): aviones 21-22
INSERT INTO AER_AVION VALUES (21,'F-GZNE',7,13,2020,'ACTIVO',DATE '2026-03-15',DATE '2026-09-15',9540);
INSERT INTO AER_AVION VALUES (22,'F-HXLA',4,13,2021,'ACTIVO',DATE '2026-04-18',DATE '2026-10-18',7215);
-- British Airways (14): aviones 23-24
INSERT INTO AER_AVION VALUES (23,'G-ZBLA',7,14,2019,'ACTIVO',DATE '2026-03-20',DATE '2026-09-20',9905);
INSERT INTO AER_AVION VALUES (24,'G-NEOV',4,14,2021,'ACTIVO',DATE '2026-04-25',DATE '2026-10-25',7745);
-- Emirates (15): aviones 25-26
INSERT INTO AER_AVION VALUES (25,'A6-EQT',7,15,2020,'ACTIVO',DATE '2026-03-12',DATE '2026-09-12',11015);
INSERT INTO AER_AVION VALUES (26,'A6-EKV',4,15,2022,'ACTIVO',DATE '2026-04-28',DATE '2026-10-28',7050);
-- Lufthansa (16): aviones 27-28
INSERT INTO AER_AVION VALUES (27,'D-AIXN',7,16,2019,'ACTIVO',DATE '2026-03-08',DATE '2026-09-08',9980);
INSERT INTO AER_AVION VALUES (28,'D-ABKT',2,16,2018,'ACTIVO',DATE '2026-04-06',DATE '2026-10-06',12740);
-- LATAM (17): aviones 29-30
INSERT INTO AER_AVION VALUES (29,'PT-MXF',4,17,2022,'ACTIVO',DATE '2026-03-28',DATE '2026-09-28',6880);
INSERT INTO AER_AVION VALUES (30,'PR-XMR',5,17,2021,'ACTIVO',DATE '2026-04-30',DATE '2026-10-30',6440);
-- TAP (18): avion 31
INSERT INTO AER_AVION VALUES (31,'CS-TXD',4,18,2021,'ACTIVO',DATE '2026-03-22',DATE '2026-09-22',6985);
-- Aerolineas Argentinas (19): avion 32
INSERT INTO AER_AVION VALUES (32,'LV-FUR',4,19,2021,'ACTIVO',DATE '2026-04-08',DATE '2026-10-08',6980);
-- Gol (20): aviones 33-34
INSERT INTO AER_AVION VALUES (33,'PR-GUF',4,20,2021,'ACTIVO',DATE '2026-03-30',DATE '2026-09-30',7640);
INSERT INTO AER_AVION VALUES (34,'PS-GOL',5,20,2020,'ACTIVO',DATE '2026-04-17',DATE '2026-10-17',6125);

COMMIT;
PROMPT Flota cargada (34 aeronaves en 20 aerolineas).

--------------------------------------------------------
-- SECCION 6: CONFIGURACION DE ASIENTOS (34 aviones)
--------------------------------------------------------

DECLARE
  v_letters VARCHAR2(6) := 'ABCDEF';
  v_class   VARCHAR2(15);
BEGIN
  FOR avion IN 1..34 LOOP
    FOR fila IN 1..26 LOOP
      FOR pos IN 1..6 LOOP
        v_class := CASE
          WHEN fila = 1 AND avion IN (13,14,21,23,25,27) THEN 'primera'
          WHEN fila <= 3 THEN 'ejecutiva'
          ELSE 'economica'
        END;
        INSERT INTO AER_ASIENTO_AVION (ASA_ID_ASIENTO,ASA_ID_AVION,ASA_CODIGO,ASA_CLASE,ASA_ESTADO)
        VALUES (avion*10000 + fila*10 + pos, avion,
                TO_CHAR(fila)||SUBSTR(v_letters,pos,1), v_class,
                CASE WHEN fila=13 AND pos IN (3,4) THEN 'BLOQUEADO' ELSE 'DISPONIBLE' END);
      END LOOP;
    END LOOP;
  END LOOP;
END;
/

COMMIT;
PROMPT Asientos configurados (34 aeronaves x 156 asientos).

--------------------------------------------------------
-- SECCION 7: METODOS DE PAGO Y PROMOCIONES
--------------------------------------------------------

INSERT INTO AER_METODOPAGO VALUES (1,'Visa Credito','tarjeta','ACTIVO',2.75);
INSERT INTO AER_METODOPAGO VALUES (2,'Mastercard Debito','tarjeta','ACTIVO',2.35);
INSERT INTO AER_METODOPAGO VALUES (3,'Transferencia ACH','transferencia','ACTIVO',0.50);
INSERT INTO AER_METODOPAGO VALUES (4,'Efectivo Mostrador','efectivo','ACTIVO',0.00);
INSERT INTO AER_METODOPAGO VALUES (5,'Apple Pay / Google Pay','digital','ACTIVO',2.10);

INSERT INTO AER_PROMOCION VALUES (1,'VERANO2026','Descuento verano rutas Guatemala-Centroamerica','PORCENTAJE',15,DATE '2026-06-01',DATE '2026-08-31',1000,0,'ACTIVA');
INSERT INTO AER_PROMOCION VALUES (2,'BRASIL-PROMO','Tarifa especial Guatemala-Brasil lanzamiento ruta','MONTO',200,DATE '2026-05-18',DATE '2026-07-31',500,0,'ACTIVA');
INSERT INTO AER_PROMOCION VALUES (3,'CORP-Q3','Convenio corporativo viajes frecuentes Q3','PORCENTAJE',10,DATE '2026-07-01',DATE '2026-09-30',2000,0,'ACTIVA');
INSERT INTO AER_PROMOCION VALUES (4,'NAVIDAD2026','Promo navidad destinos internacionales','PORCENTAJE',12,DATE '2026-12-01',DATE '2026-12-31',800,0,'ACTIVA');
INSERT INTO AER_PROMOCION VALUES (5,'EARLY-BIRD','Descuento reserva anticipada 60+ dias','MONTO',150,DATE '2026-05-18',DATE '2026-12-31',3000,0,'ACTIVA');

COMMIT;

--------------------------------------------------------
-- SECCION 8: DEPARTAMENTOS, PUESTOS Y EMPLEADOS
--------------------------------------------------------

INSERT INTO AER_DEPARTAMENTO VALUES (1,'Operaciones Aereas','Despacho, tripulaciones, coordinacion de salidas y llegadas',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (2,'Servicio al Pasajero','Check-in, salas de abordaje, conexiones e informacion',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (3,'Seguridad Aeroportuaria','Inspeccion, control de accesos y respuesta inicial',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (4,'Control Migratorio','Procesos de entrada, salida y transito internacional',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (5,'Mantenimiento Aeronautico','Linea, hangares, repuestos y aeronavegabilidad',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (6,'Comercial y Ventas','Mostradores, canales digitales, cobros y atencion comercial',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (7,'Logistica de Equipaje','Clasificacion, carga, descarga y trazabilidad de maletas',1,'ACTIVO');
INSERT INTO AER_DEPARTAMENTO VALUES (8,'Administracion','Planilla, auditoria, compras y proveedores',1,'ACTIVO');

INSERT INTO AER_PUESTO VALUES (1,'Piloto Capitan',1,'Comandante de aeronave en rutas internacionales',38000,62000,'S');
INSERT INTO AER_PUESTO VALUES (2,'Primer Oficial',1,'Copiloto certificado para operacion comercial',24000,42000,'S');
INSERT INTO AER_PUESTO VALUES (3,'Jefe de Cabina',1,'Responsable de servicio y seguridad en cabina',14500,23000,'S');
INSERT INTO AER_PUESTO VALUES (4,'Auxiliar de Vuelo',1,'Tripulante de cabina',9800,16500,'S');
INSERT INTO AER_PUESTO VALUES (5,'Agente de Check-in',2,'Atencion de pasajeros y documentacion de viaje',5200,7800,'N');
INSERT INTO AER_PUESTO VALUES (6,'Supervisor de Puerta',2,'Coordinacion de abordajes y cierres de vuelo',8500,12500,'N');
INSERT INTO AER_PUESTO VALUES (7,'Inspector de Seguridad',3,'Control de pasajeros, equipaje de mano y accesos',6200,9800,'S');
INSERT INTO AER_PUESTO VALUES (8,'Oficial Migratorio',4,'Revision documental de entrada y salida',7200,11800,'S');
INSERT INTO AER_PUESTO VALUES (9,'Tecnico de Linea',5,'Mantenimiento preventivo y correctivo en plataforma',11000,19000,'S');
INSERT INTO AER_PUESTO VALUES (10,'Jefe de Mantenimiento',5,'Responsable tecnico de aeronavegabilidad',22000,36000,'S');
INSERT INTO AER_PUESTO VALUES (11,'Agente de Ventas',6,'Venta de boletos y cobros en mostrador',5000,8200,'N');
INSERT INTO AER_PUESTO VALUES (12,'Analista Comercial Digital',6,'Analitica web/app y conversion de ventas',9000,14500,'N');
INSERT INTO AER_PUESTO VALUES (13,'Coordinador de Equipaje',7,'Trazabilidad y despacho de equipaje facturado',6800,10500,'N');
INSERT INTO AER_PUESTO VALUES (14,'Operador de Rampa',7,'Carga, descarga y apoyo en plataforma',5400,8500,'S');
INSERT INTO AER_PUESTO VALUES (15,'Comprador Aeronautico',8,'Ordenes de compra y gestion de proveedores',8200,13000,'N');
INSERT INTO AER_PUESTO VALUES (16,'Auditor Interno',8,'Control documental y auditoria de operaciones',9500,15000,'N');

DECLARE
  TYPE t_str IS TABLE OF VARCHAR2(60) INDEX BY PLS_INTEGER;
  n1 t_str; n2 t_str; a1 t_str; a2 t_str; muni t_str;
  v_p NUMBER; v_d NUMBER; v_s NUMBER;
BEGIN
  n1(1):='Lucia'; n1(2):='Diego'; n1(3):='Sofia'; n1(4):='Carlos'; n1(5):='Mariana';
  n1(6):='Jose'; n1(7):='Andrea'; n1(8):='Luis'; n1(9):='Valeria'; n1(10):='Fernando';
  n2(1):='Alejandra'; n2(2):='Fernando'; n2(3):='Isabel'; n2(4):='Eduardo'; n2(5):='Paola';
  n2(6):='Antonio'; n2(7):='Gabriela'; n2(8):='Rafael'; n2(9):='Monica'; n2(10):='Roberto';
  a1(1):='Garcia'; a1(2):='Morales'; a1(3):='Herrera'; a1(4):='Lopez'; a1(5):='Castillo';
  a1(6):='Mendez'; a1(7):='Aguilar'; a1(8):='Ramirez'; a1(9):='Torres'; a1(10):='Vasquez';
  a2(1):='Pineda'; a2(2):='Cifuentes'; a2(3):='Barrios'; a2(4):='Escobar'; a2(5):='Fuentes';
  a2(6):='Rodas'; a2(7):='Salazar'; a2(8):='Quezada'; a2(9):='Mendoza'; a2(10):='Figueroa';
  muni(1):='Guatemala'; muni(2):='Mixco'; muni(3):='Villa Nueva'; muni(4):='Santa Catarina Pinula';
  muni(5):='San Miguel Petapa'; muni(6):='Fraijanes'; muni(7):='Amatitlan';
  muni(8):='San Jose Pinula'; muni(9):='Petapa'; muni(10):='Chinautla';

  FOR i IN 1..45 LOOP
    v_p := CASE
      WHEN i <= 5  THEN CASE WHEN MOD(i,2)=0 THEN 2 ELSE 1 END
      WHEN i <= 12 THEN CASE WHEN MOD(i,4)=0 THEN 3 ELSE 4 END
      WHEN i <= 18 THEN CASE WHEN MOD(i,3)=0 THEN 6 ELSE 5 END
      WHEN i <= 24 THEN 7
      WHEN i <= 29 THEN 8
      WHEN i <= 36 THEN CASE WHEN MOD(i,5)=0 THEN 10 ELSE 9 END
      WHEN i <= 40 THEN 11
      WHEN i = 41  THEN 12
      WHEN i = 42  THEN 13
      WHEN i = 43  THEN 14
      WHEN i = 44  THEN 15
      ELSE 16
    END;
    v_d := CASE WHEN v_p<=4 THEN 1 WHEN v_p<=6 THEN 2 WHEN v_p=7 THEN 3 WHEN v_p=8 THEN 4 WHEN v_p<=10 THEN 5 WHEN v_p<=12 THEN 6 WHEN v_p<=14 THEN 7 ELSE 8 END;
    v_s := CASE v_p WHEN 1 THEN 45500 WHEN 2 THEN 31200 WHEN 3 THEN 17200 WHEN 4 THEN 11800 WHEN 5 THEN 6200 WHEN 6 THEN 9800 WHEN 7 THEN 7600 WHEN 8 THEN 9100 WHEN 9 THEN 13800 WHEN 10 THEN 28200 WHEN 11 THEN 6100 WHEN 12 THEN 11600 WHEN 13 THEN 8800 WHEN 14 THEN 10100 WHEN 15 THEN 10100 ELSE 12400 END;
    INSERT INTO AER_EMPLEADO
    (EMP_ID_EMPLEADO,EMP_NUMERO_EMPLEADO,EMP_PRIMER_NOMBRE,EMP_SEGUNDO_NOMBRE,EMP_PRIMER_APELLIDO,EMP_SEGUNDO_APELLIDO,
     EMP_FECHA_NACIMIENTO,EMP_DPI,EMP_NIT,EMP_DIR_CALLE,EMP_DIR_ZONA,EMP_DIR_MUNICIPIO,EMP_DIR_DEPARTAMENTO,EMP_DIR_PAIS,
     EMP_TELEFONO,EMP_EMAIL,EMP_FECHA_CONTRATACION,EMP_ID_PUESTO,EMP_ID_DEPARTAMENTO,EMP_SALARIO_ACTUAL,EMP_TIPO_CONTRATO,EMP_ESTADO)
    VALUES
    (i,'AUR-'||LPAD(i,5,'0'),n1(MOD(i-1,10)+1),n2(MOD(i+2,10)+1),a1(MOD(i+4,10)+1),a2(MOD(i+6,10)+1),
     ADD_MONTHS(DATE '1972-01-15',i*7),'30'||LPAD(i,11,'0'),'CF'||LPAD(i,8,'0'),
     'Avenida Hincapie '||(10+i)||'-'||LPAD(MOD(i,20)+1,2,'0'),'Zona '||TO_CHAR(MOD(i,18)+1),
     muni(MOD(i-1,10)+1),'Guatemala','Guatemala',
     '+502 5'||LPAD(1000000+i*137,7,'0'),
     LOWER(n1(MOD(i-1,10)+1)||'.'||a1(MOD(i+4,10)+1)||i||'@aurora.gt'),
     ADD_MONTHS(DATE '2014-02-01',i*3),v_p,v_d,v_s,
     CASE WHEN i>=43 THEN 'INDEFINIDO' ELSE 'FIJO' END,'ACTIVO');
  END LOOP;
END;
/

BEGIN
  FOR i IN 1..38 LOOP
    INSERT INTO AER_LICENCIAEMPLEADO VALUES (i,i,
      CASE WHEN i<=5 THEN 'Licencia DGAC Tripulacion Tecnica' WHEN i<=12 THEN 'Certificado Tripulante Cabina' WHEN i<=24 THEN 'Credencial Seguridad Aeroportuaria' WHEN i<=29 THEN 'Credencial Migratoria' ELSE 'Licencia Tecnico Aeronautico' END,
      'LIC-AUR-'||LPAD(i,6,'0'),ADD_MONTHS(DATE '2024-01-01',MOD(i,12)),ADD_MONTHS(DATE '2024-01-01',36+MOD(i,12)),'DGAC Guatemala','VIGENTE');
  END LOOP;
  FOR i IN 1..45 LOOP
    INSERT INTO AER_ASISTENCIA VALUES (i,i,DATE '2026-05-18',
      TIMESTAMP '2026-05-18 06:00:00'+NUMTODSINTERVAL(MOD(i,5)*15,'MINUTE'),
      TIMESTAMP '2026-05-18 14:00:00'+NUMTODSINTERVAL(MOD(i,5)*15,'MINUTE'),
      8,CASE WHEN MOD(i,9)=0 THEN 'HORAS_EXTRA' ELSE 'NORMAL' END,'PRESENTE');
    INSERT INTO AER_PLANILLA VALUES (i,i,DATE '2026-04-01',DATE '2026-04-30',
      CASE WHEN i<=5 THEN 32000 WHEN i<=12 THEN 12000 WHEN i<=29 THEN 8200 WHEN i<=36 THEN 14500 ELSE 9000 END,
      250,CASE WHEN MOD(i,9)=0 THEN 450 ELSE 0 END,385,
      CASE WHEN i<=5 THEN 31865 WHEN i<=12 THEN 11865 WHEN i<=29 THEN 8065 WHEN i<=36 THEN 14365 ELSE 8865 END,
      DATE '2026-04-25','PAGADA');
  END LOOP;
END;
/

COMMIT;
PROMPT Empleados cargados (45 empleados con planilla y licencias).

--------------------------------------------------------
-- SECCION 9: PASAJEROS Y USUARIOS
-- 120 pasajeros + 100 usuarios (activos, inactivos, bloqueados)
-- Contrasenas hasheadas con PBKDF2:
--   ACTIVO   -> AuroraDemo1!
--   INACTIVO -> DemoInactivo1!
--   ADMIN    -> AdminAurora1!
--   SOPORTE  -> SoporteAurora1!
--------------------------------------------------------

DECLARE
  TYPE t_str IS TABLE OF VARCHAR2(60) INDEX BY PLS_INTEGER;
  n1 t_str; n2 t_str; a1 t_str; a2 t_str; nac t_str;

  -- Hash AuroraDemo1!
  c_hash_activo  CONSTANT VARCHAR2(255) := 'PBKDF2$100000$NETOIdMIOybQF4Dh+i22Ew==$wpsk9LGzzvzK5JK2LB8K5KOiJ24kAz67xjVVbxZCoRA=';
  c_salt_activo  CONSTANT VARCHAR2(100) := 'NETOIdMIOybQF4Dh+i22Ew==';
  -- Hash DemoInactivo1!
  c_hash_inact   CONSTANT VARCHAR2(255) := 'PBKDF2$100000$/bK4i8AgsK4Y9J66zr43Zw==$wFy2Zg6tqOz8JL63YNccPe3RYMRLtoa84YQrJe010CQ=';
  c_salt_inact   CONSTANT VARCHAR2(100) := '/bK4i8AgsK4Y9J66zr43Zw==';

  v_estado_usr  VARCHAR2(20);
  v_email_ver   VARCHAR2(1);
  v_intentos    NUMBER;
  v_bloqueado   TIMESTAMP;
BEGIN
  -- Nombres y apellidos latinoamericanos variados
  n1(1):='Ana';        n1(2):='Carlos';     n1(3):='Maria';     n1(4):='Luis';
  n1(5):='Camila';     n1(6):='Jose';       n1(7):='Valentina'; n1(8):='Diego';
  n1(9):='Isabella';   n1(10):='Alejandro'; n1(11):='Sofia';    n1(12):='Miguel';
  n1(13):='Catalina';  n1(14):='Roberto';   n1(15):='Daniela';  n1(16):='Juan';
  n1(17):='Mariana';   n1(18):='Pablo';     n1(19):='Natalia';  n1(20):='Andres';

  n2(1):='Patricia';   n2(2):='Emilio';     n2(3):='Lucia';     n2(4):='Ricardo';
  n2(5):='Fernanda';   n2(6):='Gabriel';    n2(7):='Monica';    n2(8):='Sebastian';
  n2(9):='Adriana';    n2(10):='Eduardo';   n2(11):='Paola';    n2(12):='Victor';
  n2(13):='Elena';     n2(14):='Oscar';     n2(15):='Claudia';  n2(16):='Marco';
  n2(17):='Sandra';    n2(18):='Daniel';    n2(19):='Diana';    n2(20):='Hector';

  a1(1):='Ramirez';    a1(2):='Torres';     a1(3):='Garcia';    a1(4):='Lopez';
  a1(5):='Herrera';    a1(6):='Morales';    a1(7):='Castillo';  a1(8):='Jimenez';
  a1(9):='Vargas';     a1(10):='Mendoza';   a1(11):='Reyes';    a1(12):='Campos';
  a1(13):='Soto';      a1(14):='Perez';     a1(15):='Alvarez';  a1(16):='Vega';
  a1(17):='Santos';    a1(18):='Delgado';   a1(19):='Navarro';  a1(20):='Rivas';

  a2(1):='Gutierrez';  a2(2):='Figueroa';   a2(3):='Juarez';    a2(4):='Valdez';
  a2(5):='Ortiz';      a2(6):='Cruz';       a2(7):='Luna';      a2(8):='Medina';
  a2(9):='Rojas';      a2(10):='Guerrero';  a2(11):='Lara';     a2(12):='Salinas';
  a2(13):='Ibarra';    a2(14):='Espinoza';  a2(15):='Maldonado';a2(16):='Sandoval';
  a2(17):='Contreras'; a2(18):='Pena';      a2(19):='Aguilera'; a2(20):='Diaz';

  nac(1):='Guatemala';      nac(2):='Mexico';        nac(3):='Estados Unidos';
  nac(4):='Colombia';       nac(5):='Brasil';         nac(6):='Argentina';
  nac(7):='Costa Rica';     nac(8):='El Salvador';    nac(9):='Honduras';
  nac(10):='Panama';        nac(11):='Peru';           nac(12):='Chile';
  nac(13):='Ecuador';       nac(14):='Espana';         nac(15):='Venezuela';
  nac(16):='Rep. Dominicana'; nac(17):='Cuba';         nac(18):='Puerto Rico';
  nac(19):='Canada';        nac(20):='Francia';

  -- 120 pasajeros regulares
  FOR i IN 1..120 LOOP
    INSERT INTO AER_PASAJERO
    (PAS_ID_PASAJERO,PAS_NUMERO_DOCUMENTO,PAS_TIPO_DOCUMENTO,PAS_PRIMER_NOMBRE,PAS_SEGUNDO_NOMBRE,
     PAS_PRIMER_APELLIDO,PAS_SEGUNDO_APELLIDO,PAS_FECHA_NACIMIENTO,PAS_NACIONALIDAD,PAS_SEXO,
     PAS_TELEFONO,PAS_EMAIL,PAS_FECHA_REGISTRO)
    VALUES
    (i,
     CASE WHEN i<=70 THEN 'GT'||LPAD(i,9,'0') ELSE 'P'||LPAD(800000+i,8,'0') END,
     CASE WHEN i<=70 THEN 'DPI' ELSE 'PASAPORTE' END,
     n1(MOD(i-1,20)+1), n2(MOD(i+3,20)+1),
     a1(MOD(i+7,20)+1), a2(MOD(i+11,20)+1),
     ADD_MONTHS(DATE '1965-03-10', i*5+MOD(i*7,120)),
     nac(MOD(i-1,20)+1),
     CASE WHEN MOD(i,2)=0 THEN 'M' ELSE 'F' END,
     '+502 4'||LPAD(2000000+i*121,7,'0'),
     LOWER(n1(MOD(i-1,20)+1)||'.'||a1(MOD(i+7,20)+1)||LPAD(i,3,'0')||'@correo.com'),
     DATE '2026-01-01'+MOD(i,120));
  END LOOP;

  -- 100 usuarios con credenciales (80 activos, 10 inactivos, 10 bloqueados)
  FOR i IN 1..100 LOOP
    v_estado_usr := CASE
      WHEN i IN (11,22,33,44,55,66,77,88,99,100) THEN 'INACTIVO'
      WHEN i IN (9,18,27,36,45,54,63,72,81,90)   THEN 'BLOQUEADO'
      ELSE 'ACTIVO'
    END;
    v_email_ver   := CASE WHEN i IN (7,14,21,28,35) THEN 'N' ELSE 'S' END;
    v_intentos    := CASE WHEN v_estado_usr='BLOQUEADO' THEN 5 WHEN v_estado_usr='INACTIVO' THEN 0 ELSE 0 END;
    v_bloqueado   := CASE WHEN v_estado_usr='BLOQUEADO' THEN TIMESTAMP '2027-01-01 00:00:00' ELSE NULL END;

    INSERT INTO AER_USUARIO_LOGIN
    (USL_ID_USUARIO,USL_ID_PASAJERO,USL_USUARIO,USL_EMAIL,USL_CONTRASENA_HASH,USL_SAL,
     USL_ESTADO,USL_EMAIL_VERIFICADO,USL_FECHA_REGISTRO,USL_ULTIMO_ACCESO,
     USL_INTENTOS_FALLIDOS,USL_BLOQUEADO_HASTA)
    SELECT i, i,
      LOWER(PAS_PRIMER_NOMBRE||'.'||PAS_PRIMER_APELLIDO||LPAD(i,3,'0')),
      PAS_EMAIL,
      CASE WHEN v_estado_usr IN ('INACTIVO','BLOQUEADO') THEN c_hash_inact ELSE c_hash_activo END,
      CASE WHEN v_estado_usr IN ('INACTIVO','BLOQUEADO') THEN c_salt_inact ELSE c_salt_activo END,
      v_estado_usr, v_email_ver,
      TIMESTAMP '2026-01-01 08:00:00'+NUMTODSINTERVAL(i,'DAY'),
      TIMESTAMP '2026-05-01 10:00:00'-NUMTODSINTERVAL(MOD(i,30),'DAY'),
      v_intentos, v_bloqueado
    FROM AER_PASAJERO WHERE PAS_ID_PASAJERO = i;
  END LOOP;

  -- Preferencias de los 120 pasajeros
  FOR i IN 1..120 LOOP
    INSERT INTO AER_PREFERENCIACLIENTE VALUES (i,i,
      CASE WHEN MOD(i,3)=0 THEN 'asiento' WHEN MOD(i,3)=1 THEN 'notificacion' ELSE 'equipaje' END,
      CASE WHEN MOD(i,3)=0 THEN CASE WHEN MOD(i,2)=0 THEN 'ventana' ELSE 'pasillo' END WHEN MOD(i,3)=1 THEN 'email y app' ELSE 'factura una maleta' END,
      TIMESTAMP '2026-05-18 10:00:00'+NUMTODSINTERVAL(i,'HOUR'));
  END LOOP;
END;
/

-- Usuarios administrativos y de sistema
INSERT INTO AER_PASAJERO (PAS_ID_PASAJERO,PAS_NUMERO_DOCUMENTO,PAS_TIPO_DOCUMENTO,PAS_PRIMER_NOMBRE,PAS_SEGUNDO_NOMBRE,PAS_PRIMER_APELLIDO,PAS_SEGUNDO_APELLIDO,PAS_FECHA_NACIMIENTO,PAS_NACIONALIDAD,PAS_SEXO,PAS_TELEFONO,PAS_EMAIL,PAS_FECHA_REGISTRO)
VALUES (121,'ADMIN-AURORA-001','INTERNO','Administrador','Sistema','Aurora','Principal',DATE '1988-01-01','Guatemala','M','+502 2300-0001','admin@aurora.gt',DATE '2026-01-01');
INSERT INTO AER_PASAJERO (PAS_ID_PASAJERO,PAS_NUMERO_DOCUMENTO,PAS_TIPO_DOCUMENTO,PAS_PRIMER_NOMBRE,PAS_SEGUNDO_NOMBRE,PAS_PRIMER_APELLIDO,PAS_SEGUNDO_APELLIDO,PAS_FECHA_NACIMIENTO,PAS_NACIONALIDAD,PAS_SEXO,PAS_TELEFONO,PAS_EMAIL,PAS_FECHA_REGISTRO)
VALUES (122,'OPS-AURORA-002','INTERNO','Carla','Andrea','Paz','Monterroso',DATE '1991-03-16','Guatemala','F','+502 2300-0002','soporte.operaciones@aurora.gt',DATE '2026-01-01');
INSERT INTO AER_PASAJERO (PAS_ID_PASAJERO,PAS_NUMERO_DOCUMENTO,PAS_TIPO_DOCUMENTO,PAS_PRIMER_NOMBRE,PAS_SEGUNDO_NOMBRE,PAS_PRIMER_APELLIDO,PAS_SEGUNDO_APELLIDO,PAS_FECHA_NACIMIENTO,PAS_NACIONALIDAD,PAS_SEXO,PAS_TELEFONO,PAS_EMAIL,PAS_FECHA_REGISTRO)
VALUES (123,'SEC-AURORA-003','INTERNO','Hector','Daniel','Maldonado','Sierra',DATE '1986-08-11','Guatemala','M','+502 2300-0003','auditoria.seguridad@aurora.gt',DATE '2026-01-01');

INSERT INTO AER_USUARIO_LOGIN (USL_ID_USUARIO,USL_ID_PASAJERO,USL_USUARIO,USL_EMAIL,USL_CONTRASENA_HASH,USL_SAL,USL_ESTADO,USL_EMAIL_VERIFICADO,USL_FECHA_REGISTRO,USL_ULTIMO_ACCESO,USL_INTENTOS_FALLIDOS)
VALUES (121,121,'admin.aurora','admin@aurora.gt','PBKDF2$100000$MAvKF2fJPULBBm1x/MbjmA==$dzFteP76fsopJ9DjS01xZa8NXKaoiIUdUNaigmJ7HoA=','MAvKF2fJPULBBm1x/MbjmA==','ACTIVO','S',TIMESTAMP '2026-01-01 08:00:00',TIMESTAMP '2026-05-18 06:00:00',0);
INSERT INTO AER_USUARIO_LOGIN (USL_ID_USUARIO,USL_ID_PASAJERO,USL_USUARIO,USL_EMAIL,USL_CONTRASENA_HASH,USL_SAL,USL_ESTADO,USL_EMAIL_VERIFICADO,USL_FECHA_REGISTRO,USL_ULTIMO_ACCESO,USL_INTENTOS_FALLIDOS)
VALUES (122,122,'soporte.operaciones','soporte.operaciones@aurora.gt','PBKDF2$100000$E+4hNZPDqqkII+XaTvcMfw==$Qe6nw1FFXfngYRlUaPeT5T6F4RHnPBHwIi9X2MXYIe4=','E+4hNZPDqqkII+XaTvcMfw==','ACTIVO','S',TIMESTAMP '2026-01-01 09:15:00',TIMESTAMP '2026-05-18 07:10:00',0);
INSERT INTO AER_USUARIO_LOGIN (USL_ID_USUARIO,USL_ID_PASAJERO,USL_USUARIO,USL_EMAIL,USL_CONTRASENA_HASH,USL_SAL,USL_ESTADO,USL_EMAIL_VERIFICADO,USL_FECHA_REGISTRO,USL_ULTIMO_ACCESO,USL_INTENTOS_FALLIDOS)
VALUES (123,123,'auditoria.seguridad','auditoria.seguridad@aurora.gt','PBKDF2$100000$E+4hNZPDqqkII+XaTvcMfw==$Qe6nw1FFXfngYRlUaPeT5T6F4RHnPBHwIi9X2MXYIe4=','E+4hNZPDqqkII+XaTvcMfw==','ACTIVO','S',TIMESTAMP '2026-01-01 09:45:00',TIMESTAMP '2026-05-18 18:40:00',1);

INSERT INTO AER_PREFERENCIACLIENTE VALUES (121,121,'rol_sistema','ADMINISTRADOR',TIMESTAMP '2026-01-01 08:00:00');
INSERT INTO AER_PREFERENCIACLIENTE VALUES (122,122,'rol_sistema','SOPORTE_OPERACIONES',TIMESTAMP '2026-01-01 09:15:00');
INSERT INTO AER_PREFERENCIACLIENTE VALUES (123,123,'rol_sistema','AUDITOR_SEGURIDAD',TIMESTAMP '2026-01-01 09:45:00');

COMMIT;
PROMPT Pasajeros y usuarios cargados (120 pas + 100 usuarios + 3 admin).

--------------------------------------------------------
-- SECCION 10: PROGRAMAS DE VUELO Y CALENDARIO COMPLETO
-- Vuelos de 2026-05-18 al 2026-12-31
-- La funcion avion_por_aerolinea rota aeronaves por dia del anio
-- Convenciones de dias: 1=Lunes 2=Martes ... 7=Domingo
-- '1234567' = todos los dias
-- '135'     = Lunes/Mier/Vie
-- '246'     = Martes/Jue/Sab
-- '1357'    = L/X/V/D
-- '2467'    = M/J/S/D
--------------------------------------------------------

DECLARE
  v_programa_id  NUMBER;
  v_vuelo_id     NUMBER;
  v_fecha        DATE;
  v_dia_semana   NUMBER;
  v_avion        NUMBER;
  v_capacidad    NUMBER;
  v_ocupadas     NUMBER;
  v_estado       VARCHAR2(20);
  v_salida       TIMESTAMP;
  v_puerta       NUMBER;

  FUNCTION avion_por_aerolinea(p_arl NUMBER, p_dia NUMBER) RETURN NUMBER IS
  BEGIN
    RETURN CASE p_arl
      WHEN 1  THEN CASE WHEN MOD(p_dia,2)=0 THEN 2  ELSE 1  END  -- Avianca
      WHEN 2  THEN CASE WHEN MOD(p_dia,2)=0 THEN 4  ELSE 3  END  -- Copa
      WHEN 3  THEN CASE WHEN MOD(p_dia,2)=0 THEN 6  ELSE 5  END  -- American
      WHEN 4  THEN CASE WHEN MOD(p_dia,2)=0 THEN 8  ELSE 7  END  -- United
      WHEN 5  THEN CASE WHEN MOD(p_dia,2)=0 THEN 10 ELSE 9  END  -- Delta
      WHEN 6  THEN CASE WHEN MOD(p_dia,2)=0 THEN 12 ELSE 11 END  -- Aeromexico
      WHEN 7  THEN CASE WHEN MOD(p_dia,2)=0 THEN 14 ELSE 13 END  -- Iberia
      WHEN 8  THEN 15                                              -- Volaris
      WHEN 9  THEN CASE WHEN MOD(p_dia,2)=0 THEN 17 ELSE 16 END  -- TAG
      WHEN 10 THEN 18                                              -- Spirit
      WHEN 11 THEN 19                                              -- JetBlue
      WHEN 12 THEN 20                                              -- DHL
      WHEN 13 THEN CASE WHEN MOD(p_dia,2)=0 THEN 22 ELSE 21 END  -- Air France
      WHEN 14 THEN CASE WHEN MOD(p_dia,2)=0 THEN 24 ELSE 23 END  -- BA
      WHEN 15 THEN CASE WHEN MOD(p_dia,2)=0 THEN 26 ELSE 25 END  -- Emirates
      WHEN 16 THEN CASE WHEN MOD(p_dia,2)=0 THEN 28 ELSE 27 END  -- Lufthansa
      WHEN 17 THEN CASE WHEN MOD(p_dia,2)=0 THEN 30 ELSE 29 END  -- LATAM
      WHEN 18 THEN 31                                              -- TAP
      WHEN 19 THEN 32                                              -- Aerolineas Arg
      WHEN 20 THEN CASE WHEN MOD(p_dia,2)=0 THEN 34 ELSE 33 END  -- Gol
      ELSE 1
    END;
  END;

  PROCEDURE crear_ruta(
    p_num    VARCHAR2,
    p_arl    NUMBER,
    p_orig   NUMBER,
    p_dest   NUMBER,
    p_sal    VARCHAR2,
    p_arr    VARCHAR2,
    p_dur    NUMBER,
    p_tipo   VARCHAR2,
    p_dias   VARCHAR2,
    p_esc_aer  NUMBER   DEFAULT NULL,
    p_esc_llg  VARCHAR2 DEFAULT NULL,
    p_esc_sal  VARCHAR2 DEFAULT NULL,
    p_esc_min  NUMBER   DEFAULT NULL
  ) IS
  BEGIN
    INSERT INTO AER_PROGRAMAVUELO
    (PRV_NUMERO_VUELO,PRV_ID_AEROLINEA,PRV_ID_AEROPUERTO_ORIGEN,PRV_ID_AEROPUERTO_DESTINO,
     PRV_HORA_SALIDA_PROGRAMADA,PRV_HORA_LLEGADA_PROGRAMADA,PRV_DURACION_ESTIMADA,
     PRV_TIPO_VUELO,PRV_ESTADO,PRV_FECHA_CREACION)
    VALUES (p_num,p_arl,p_orig,p_dest,p_sal,p_arr,p_dur,p_tipo,'ACTIVO',DATE '2026-05-18')
    RETURNING PRV_ID INTO v_programa_id;

    FOR d IN 1..7 LOOP
      IF INSTR(p_dias,TO_CHAR(d))>0 THEN
        INSERT INTO AER_DIASVUELO (DIA_ID_PROGRAMA_VUELO,DIA_DIA_SEMANA) VALUES (v_programa_id,d);
      END IF;
    END LOOP;

    IF p_esc_aer IS NOT NULL THEN
      INSERT INTO AER_ESCALATECNICA
      (ESC_ID_PROGRAMA_VUELO,ESC_ID_AEROPUERTO,ESC_NUMERO_ORDEN,ESC_HORA_LLEGADA_ESTIMADA,ESC_HORA_SALIDA_ESTIMADA,ESC_DURACION_ESCALA)
      VALUES (v_programa_id,p_esc_aer,1,p_esc_llg,p_esc_sal,p_esc_min);
    END IF;

    v_fecha := DATE '2026-05-18';
    WHILE v_fecha <= DATE '2026-12-31' LOOP
      v_dia_semana := TRUNC(v_fecha) - TRUNC(v_fecha,'IW') + 1;

      IF INSTR(p_dias,TO_CHAR(v_dia_semana))>0 THEN
        v_avion := avion_por_aerolinea(p_arl, TO_NUMBER(TO_CHAR(v_fecha,'DDD')));

        BEGIN
          SELECT ma.MOD_CAPACIDAD_PASAJEROS INTO v_capacidad
          FROM AER_AVION av JOIN AER_MODELOAVION ma ON ma.MOD_ID_MODELO=av.AVI_ID_MODELO
          WHERE av.AVI_ID=v_avion;
        EXCEPTION WHEN NO_DATA_FOUND THEN v_capacidad := 186; END;

        v_estado := CASE
          WHEN MOD(TO_NUMBER(TO_CHAR(v_fecha,'DDD'))+v_avion+p_orig+p_dest,127)=0 THEN 'CANCELADO'
          WHEN MOD(TO_NUMBER(TO_CHAR(v_fecha,'DDD'))+v_avion+p_orig,41)=0      THEN 'RETRASADO'
          ELSE 'PROGRAMADO'
        END;

        v_ocupadas := CASE
          WHEN v_capacidad=0 THEN 0
          WHEN v_estado='CANCELADO' THEN 0
          ELSE LEAST(v_capacidad-4, 18+MOD((TO_NUMBER(TO_CHAR(v_fecha,'DDD'))*17)+v_avion+p_orig, GREATEST(v_capacidad-18,1)))
        END;

        INSERT INTO AER_VUELO
        (VUE_ID_PROGRAMA_VUELO,VUE_ID_AVION,VUE_FECHA_VUELO,VUE_HORA_SALIDA_REAL,VUE_HORA_LLEGADA_REAL,
         VUE_PLAZAS_OCUPADAS,VUE_PLAZAS_VACIAS,VUE_ESTADO,VUE_FECHA_REPROGRAMACION,VUE_MOTIVO_CANCELACION,VUE_RETRASO_MINUTOS)
        VALUES
        (v_programa_id,v_avion,v_fecha,NULL,NULL,
         v_ocupadas,GREATEST(v_capacidad-v_ocupadas,0),v_estado,
         CASE WHEN v_estado='RETRASADO' THEN v_fecha ELSE NULL END,
         CASE WHEN v_estado='CANCELADO' THEN 'Restriccion operacional o rotacion de flota' ELSE NULL END,
         CASE WHEN v_estado='RETRASADO' THEN 30+MOD(v_avion+p_orig,45) ELSE 0 END)
        RETURNING VUE_ID_VUELO INTO v_vuelo_id;

        IF p_orig=1 OR p_dest=1 THEN
          v_salida := TO_TIMESTAMP(TO_CHAR(v_fecha,'YYYY-MM-DD')||' '||p_sal,'YYYY-MM-DD HH24:MI');
          v_puerta := CASE
            WHEN p_tipo IN ('nacional','regional') THEN 101+MOD(v_vuelo_id,8)
            WHEN p_tipo='carga' THEN 201+MOD(v_vuelo_id,2)
            ELSE 1+MOD(v_vuelo_id,16)
          END;
          INSERT INTO AER_ASIGNACION_PUERTA (ASP_ID_VUELO,ASP_ID_PUERTA,ASP_FECHA_HORA_INICIO,ASP_FECHA_HORA_FIN,ASP_ESTADO)
          VALUES (v_vuelo_id,v_puerta,v_salida-NUMTODSINTERVAL(90,'MINUTE'),NULL,
            CASE WHEN v_estado='CANCELADO' THEN 'CANCELADA' ELSE 'PROGRAMADA' END);
        END IF;
      END IF;
      v_fecha := v_fecha+1;
    END LOOP;
  END crear_ruta;

BEGIN
  -- ======================================================
  -- RUTAS DESDE/HACIA GUA: VUELOS DOMESTICOS Y REGIONALES
  -- ======================================================

  -- GUA <-> FRS (Flores/Mundo Maya) - TAG Airlines - 5 vuelos/dia
  crear_ruta('5U050',9,1,2,'06:00','07:05',65,'nacional','1234567');
  crear_ruta('5U052',9,1,2,'09:00','10:05',65,'nacional','1234567');
  crear_ruta('5U054',9,1,2,'12:30','13:35',65,'nacional','1234567');
  crear_ruta('5U056',9,1,2,'16:00','17:05',65,'nacional','1234567');
  crear_ruta('5U058',9,1,2,'19:00','20:05',65,'nacional','1234567');
  crear_ruta('5U051',9,2,1,'07:45','08:50',65,'nacional','1234567');
  crear_ruta('5U053',9,2,1,'10:45','11:50',65,'nacional','1234567');
  crear_ruta('5U055',9,2,1,'14:15','15:20',65,'nacional','1234567');
  crear_ruta('5U057',9,2,1,'17:45','18:50',65,'nacional','1234567');
  crear_ruta('5U059',9,2,1,'20:45','21:50',65,'nacional','1234567');

  -- GUA <-> SAL (San Salvador) - TAG + Avianca - 3 vuelos/dia
  crear_ruta('5U100',9,1,3,'06:30','07:35',65,'regional','1234567');
  crear_ruta('AV200',1,1,3,'11:00','12:05',65,'regional','1234567');
  crear_ruta('5U102',9,1,3,'16:00','17:05',65,'regional','1234567');
  crear_ruta('5U101',9,3,1,'08:15','09:20',65,'regional','1234567');
  crear_ruta('AV201',1,3,1,'13:00','14:05',65,'regional','1234567');
  crear_ruta('5U103',9,3,1,'18:00','19:05',65,'regional','1234567');

  -- GUA <-> SAP (San Pedro Sula) - TAG - 2 vuelos/dia
  crear_ruta('5U150',9,1,4,'07:00','08:10',70,'regional','1234567');
  crear_ruta('5U152',9,1,4,'14:30','15:40',70,'regional','1234567');
  crear_ruta('5U151',9,4,1,'09:00','10:10',70,'regional','1234567');
  crear_ruta('5U153',9,4,1,'16:30','17:40',70,'regional','1234567');

  -- ======================================================
  -- RUTAS GUA <-> CENTROAMERICA
  -- ======================================================

  -- GUA <-> SJO (San Jose CR) - Copa + Avianca - 4 vuelos/dia
  crear_ruta('CM250',2,1,5,'06:00','08:10',130,'internacional','1234567');
  crear_ruta('AV300',1,1,5,'10:30','12:40',130,'internacional','1234567');
  crear_ruta('CM252',2,1,5,'15:00','17:10',130,'internacional','1234567');
  crear_ruta('AV302',1,1,5,'19:30','21:40',130,'internacional','1234567');
  crear_ruta('CM251',2,5,1,'09:00','11:10',130,'internacional','1234567');
  crear_ruta('AV301',1,5,1,'13:30','15:40',130,'internacional','1234567');
  crear_ruta('CM253',2,5,1,'18:00','20:10',130,'internacional','1234567');
  crear_ruta('AV303',1,5,1,'22:30','00:40',130,'internacional','1234567');

  -- GUA <-> PTY (Panama) - Copa hub - 5 vuelos/dia
  crear_ruta('CM100',2,1,6,'05:00','07:25',145,'internacional','1234567');
  crear_ruta('CM102',2,1,6,'08:30','10:55',145,'internacional','1234567');
  crear_ruta('CM104',2,1,6,'12:00','14:25',145,'internacional','1234567');
  crear_ruta('CM106',2,1,6,'16:30','18:55',145,'internacional','1234567');
  crear_ruta('CM108',2,1,6,'20:00','22:25',145,'internacional','1234567');
  crear_ruta('CM101',2,6,1,'08:10','10:35',145,'internacional','1234567');
  crear_ruta('CM103',2,6,1,'11:40','14:05',145,'internacional','1234567');
  crear_ruta('CM105',2,6,1,'15:20','17:45',145,'internacional','1234567');
  crear_ruta('CM107',2,6,1,'19:45','22:10',145,'internacional','1234567');
  crear_ruta('CM109',2,6,1,'23:15','01:40',145,'internacional','1234567');

  -- ======================================================
  -- RUTAS GUA <-> MEXICO
  -- ======================================================

  -- GUA <-> MEX (Mexico DF) - Aeromexico + Volaris - 3 vuelos/dia
  crear_ruta('AM650',6,1,7,'06:00','08:20',140,'internacional','1234567');
  crear_ruta('Y43900',8,1,7,'11:00','13:20',140,'internacional','1234567');
  crear_ruta('AM652',6,1,7,'17:30','19:50',140,'internacional','1234567');
  crear_ruta('AM651',6,7,1,'09:30','11:50',140,'internacional','1234567');
  crear_ruta('Y43901',8,7,1,'14:30','16:50',140,'internacional','1234567');
  crear_ruta('AM653',6,7,1,'21:00','23:20',140,'internacional','1234567');

  -- GUA <-> CUN (Cancun) - Volaris + Delta - 2 vuelos/dia
  crear_ruta('Y43800',8,1,8,'07:00','09:20',140,'turistico','1234567');
  crear_ruta('DL1850',5,1,8,'14:30','16:50',140,'turistico','246');
  crear_ruta('Y43801',8,8,1,'10:30','12:50',140,'turistico','1234567');
  crear_ruta('DL1851',5,8,1,'18:00','20:20',140,'turistico','246');

  -- ======================================================
  -- RUTAS GUA <-> ESTADOS UNIDOS
  -- ======================================================

  -- GUA <-> MIA (Miami) - American + Avianca + Delta + Spirit - 4 vuelos/dia
  crear_ruta('AA1100',3,1,9,'06:30','11:00',150,'internacional','1234567');
  crear_ruta('AV620', 1,1,9,'09:45','14:15',150,'internacional','1234567');
  crear_ruta('DL1820',5,1,9,'13:30','18:00',150,'internacional','1234567');
  crear_ruta('NK500', 10,1,9,'17:00','21:30',150,'internacional','135');
  crear_ruta('AA1101',3,9,1,'12:00','14:30',150,'internacional','1234567');
  crear_ruta('AV621', 1,9,1,'15:30','18:00',150,'internacional','1234567');
  crear_ruta('DL1821',5,9,1,'19:00','21:30',150,'internacional','1234567');
  crear_ruta('NK501', 10,9,1,'22:30','01:00',150,'internacional','246');

  -- GUA <-> IAH (Houston) - United + Copa - 2 vuelos/dia
  crear_ruta('UA1800',4,1,10,'07:15','10:55',160,'internacional','1234567');
  crear_ruta('CM310', 2,1,10,'14:00','17:40',160,'internacional','1234567');
  crear_ruta('UA1801',4,10,1,'12:00','15:40',160,'internacional','1234567');
  crear_ruta('CM311', 2,10,1,'19:00','22:40',160,'internacional','1234567');

  -- GUA <-> LAX (Los Angeles) - United + Volaris - 2 vuelos/dia
  crear_ruta('UA1700',4,1,11,'07:30','12:45',255,'internacional','1234567');
  crear_ruta('Y43950',8,1,11,'14:00','19:15',255,'internacional','246');
  crear_ruta('UA1701',4,11,1,'14:00','21:30',255,'internacional','1234567');
  crear_ruta('Y43951',8,11,1,'20:30','04:00',255,'internacional','135');

  -- GUA <-> JFK (Nueva York) - American + JetBlue - 2 vuelos/dia
  crear_ruta('AA1050',3,1,12,'06:45','12:30',225,'internacional','1234567');
  crear_ruta('B62000',11,1,12,'14:00','19:45',225,'internacional','1234567');
  crear_ruta('AA1051',3,12,1,'13:30','21:00',225,'internacional','1234567');
  crear_ruta('B62001',11,12,1,'21:00','04:30',225,'internacional','1234567');

  -- GUA <-> ATL (Atlanta) - Delta - 2 vuelos/dia
  crear_ruta('DL1800',5,1,16,'07:30','11:30',150,'internacional','1234567');
  crear_ruta('DL1802',5,1,16,'16:00','20:00',150,'internacional','246');
  crear_ruta('DL1801',5,16,1,'12:30','14:30',150,'internacional','1234567');
  crear_ruta('DL1803',5,16,1,'21:00','23:00',150,'internacional','135');

  -- GUA <-> ORD (Chicago) - United - 1 vuelo/dia
  crear_ruta('UA3000',4,1,17,'08:00','13:30',210,'internacional','1234567');
  crear_ruta('UA3001',4,17,1,'15:00','20:30',210,'internacional','1234567');

  -- GUA <-> DFW (Dallas) - American - 1 vuelo/dia
  crear_ruta('AA9000',3,1,18,'09:30','13:45',255,'internacional','1234567');
  crear_ruta('AA9001',3,18,1,'15:30','21:45',255,'internacional','1234567');

  -- GUA <-> MCO (Orlando) - American - 1 vuelo/dia
  crear_ruta('AA9600',3,1,19,'06:40','11:55',195,'internacional','1234567');
  crear_ruta('AA9601',3,19,1,'13:10','17:25',195,'internacional','1234567');

  -- ======================================================
  -- RUTAS GUA <-> SUDAMERICA
  -- ======================================================

  -- GUA <-> BOG (Bogota) - Avianca - 2 vuelos/dia
  crear_ruta('AV400',1,1,13,'06:00','09:10',190,'internacional','1234567');
  crear_ruta('AV402',1,1,13,'14:00','17:10',190,'internacional','1234567');
  crear_ruta('AV401',1,13,1,'10:00','13:10',190,'internacional','1234567');
  crear_ruta('AV403',1,13,1,'18:00','21:10',190,'internacional','1234567');

  -- GUA <-> LIM (Lima) - Avianca + LATAM - 2 vuelos/dia
  crear_ruta('AV500',1,1,14,'06:30','11:30',300,'internacional','1234567',13,'08:20','09:10',50);
  crear_ruta('LA5200',17,1,14,'13:00','18:00',300,'internacional','1234567',6,'15:10','16:00',50);
  crear_ruta('AV501',1,14,1,'12:30','17:30',300,'internacional','1234567',13,'14:20','15:10',50);
  crear_ruta('LA5201',17,14,1,'19:00','00:00',300,'internacional','1234567',6,'21:10','22:00',50);

  -- GUA <-> GRU (Sao Paulo) - LATAM - 2 vuelos/dia - RUTA BRASIL!
  crear_ruta('LA5000',17,1,20,'07:00','16:30',570,'intercontinental','1234567',13,'10:05','11:05',60);
  crear_ruta('LA5002',17,1,20,'12:00','21:30',570,'intercontinental','246',13,'15:05','16:05',60);
  crear_ruta('LA5001',17,20,1,'18:00','03:30',570,'intercontinental','1234567',13,'21:05','22:05',60);
  crear_ruta('LA5003',17,20,1,'08:00','17:30',570,'intercontinental','135',13,'11:05','12:05',60);

  -- GUA <-> GIG (Rio de Janeiro) - Gol + LATAM - 2 vuelos/dia - RUTA BRASIL!
  crear_ruta('G35100',20,1,21,'08:30','17:50',560,'intercontinental','1357',13,'11:40','12:40',60);
  crear_ruta('LA5100',17,1,21,'10:30','19:50',560,'intercontinental','246',6,'13:40','14:40',60);
  crear_ruta('G35101',20,21,1,'19:15','04:35',560,'intercontinental','2467',13,'22:15','23:15',60);
  crear_ruta('LA5101',17,21,1,'21:00','06:20',560,'intercontinental','135',6,'23:50','00:50',60);

  -- GUA <-> SCL (Santiago de Chile) - LATAM - 1 vuelo/dia
  crear_ruta('LA5400',17,1,22,'07:30','17:30',600,'intercontinental','1234567',20,'12:30','13:30',60);
  crear_ruta('LA5401',17,22,1,'19:30','05:30',600,'intercontinental','1234567',20,'00:30','01:30',60);

  -- GUA <-> EZE (Buenos Aires) - Aerolineas Arg + Avianca - 2 vuelos/dia
  crear_ruta('AR6000',19,1,23,'07:00','18:30',570,'intercontinental','1357',13,'10:30','11:30',60);
  crear_ruta('AV3400',1,1,23,'12:00','23:30',570,'intercontinental','246',13,'15:30','16:30',60);
  crear_ruta('AR6001',19,23,1,'20:00','07:30',570,'intercontinental','2467',13,'23:00','00:00',60);
  crear_ruta('AV3401',1,23,1,'23:30','11:00',570,'intercontinental','135',13,'02:30','03:30',60);

  -- GUA <-> UIO (Quito) - Avianca - 1 vuelo/dia
  crear_ruta('AV600',1,1,24,'09:00','13:00',240,'internacional','1234567',13,'10:50','11:40',50);
  crear_ruta('AV601',1,24,1,'14:00','18:00',240,'internacional','1234567',13,'15:50','16:40',50);

  -- GUA <-> MDE (Medellin) - Avianca - 1 vuelo/dia
  crear_ruta('AV700',1,1,27,'07:15','10:45',150,'internacional','1234567',13,'08:55','09:35',40);
  crear_ruta('AV701',1,27,1,'12:05','15:10',150,'internacional','1234567',13,'13:20','14:00',40);

  -- GUA <-> SDQ (Santo Domingo) - JetBlue - 4 dias/semana
  crear_ruta('B65000',11,1,25,'07:10','13:30',320,'turistico','1357',9,'09:35','10:20',45);
  crear_ruta('B65001',11,25,1,'15:20','19:55',275,'turistico','2467',9,'17:05','17:50',45);

  -- GUA <-> HAV (La Habana) - Copa via PTY - 3 dias/semana
  crear_ruta('CM550',2,1,36,'08:00','14:20',380,'turistico','135',6,'10:25','11:25',60);
  crear_ruta('CM551',2,36,1,'16:00','20:25',325,'turistico','246',6,'17:45','18:45',60);

  -- GUA <-> SJU (San Juan PR) - American - 3 dias/semana
  crear_ruta('AA4200',3,1,37,'10:30','17:10',340,'turistico','1357',9,'13:00','13:45',45);
  crear_ruta('AA4201',3,37,1,'18:30','23:10',300,'turistico','2467',9,'20:00','20:45',45);

  -- ======================================================
  -- RUTAS GUA <-> EUROPA
  -- ======================================================

  -- GUA <-> MAD (Madrid) - Iberia - diario
  crear_ruta('IB6000',7,1,15,'17:30','13:45',675,'intercontinental','1234567');
  crear_ruta('IB6001',7,15,1,'15:30','21:45',690,'intercontinental','1234567');

  -- GUA <-> CDG (Paris) - Air France - diario via MAD
  crear_ruta('AF4000',13,1,29,'18:30','14:45',675,'intercontinental','1234567',15,'09:00','10:15',75);
  crear_ruta('AF4001',13,29,1,'16:30','22:30',690,'intercontinental','1234567',15,'19:45','21:00',75);

  -- GUA <-> LHR (Londres) - British Airways - 4 dias/semana
  crear_ruta('BA6500',14,1,28,'17:00','13:15',675,'intercontinental','1357',15,'08:30','09:45',75);
  crear_ruta('BA6501',14,28,1,'16:00','21:00',690,'intercontinental','2467',15,'19:30','20:45',75);

  -- GUA <-> FRA (Frankfurt) - Lufthansa - 3 dias/semana
  crear_ruta('LH5000',16,1,30,'17:45','14:00',675,'intercontinental','246',15,'08:45','10:00',75);
  crear_ruta('LH5001',16,30,1,'15:30','20:30',690,'intercontinental','135',15,'18:45','20:00',75);

  -- GUA <-> LIS (Lisboa) - TAP - 3 dias/semana
  crear_ruta('TP4500',18,1,31,'16:30','12:45',675,'intercontinental','135',15,'08:00','09:10',70);
  crear_ruta('TP4501',18,31,1,'14:15','18:45',690,'intercontinental','246',15,'17:30','18:40',70);

  -- GUA <-> AMS (Amsterdam) - KLM via MAD - 2 dias/semana (usando Iberia)
  crear_ruta('IB7200',7,1,32,'19:00','15:30',680,'intercontinental','37',15,'09:45','11:00',75);
  crear_ruta('IB7201',7,32,1,'17:00','21:45',700,'intercontinental','26',15,'20:15','21:30',75);

  -- ======================================================
  -- RUTAS GUA <-> ASIA / MEDIO ORIENTE (larga distancia)
  -- ======================================================

  -- GUA <-> DXB (Dubai) - Emirates - 2 dias/semana
  crear_ruta('EK8800',15,1,33,'22:00','21:45',1165,'intercontinental','26',6,'00:40','02:05',85);
  crear_ruta('EK8801',15,33,1,'01:20','09:40',1220,'intercontinental','37',6,'05:55','07:20',85);

  -- GUA <-> NRT (Tokyo) - United - 2 dias/semana via LAX
  crear_ruta('UA9000',4,1,34,'05:45','14:40',1055,'intercontinental','14',11,'10:45','12:15',90);
  crear_ruta('UA9001',4,34,1,'16:15','20:25',1010,'intercontinental','25',11,'10:55','12:10',75);

  -- GUA <-> DOH (Doha) - Qatar via MIA - 2 dias/semana
  crear_ruta('QR8800',11,1,35,'22:30','21:15',1175,'intercontinental','37',9,'00:55','02:20',85);
  crear_ruta('QR8801',11,35,1,'02:10','10:30',1210,'intercontinental','26',9,'06:30','07:55',85);

  -- ======================================================
  -- RUTAS INTER-REGIONALES (no involucran GUA directamente)
  -- ======================================================

  -- BOG <-> GRU (Colombia - Brasil) - Avianca + LATAM
  crear_ruta('AV3000',1,13,20,'08:00','14:30',330,'internacional','1234567');
  crear_ruta('AV3001',1,20,13,'16:00','20:30',330,'internacional','1234567');
  crear_ruta('LA6000',17,13,20,'10:00','16:30',330,'internacional','1234567');
  crear_ruta('LA6001',17,20,13,'18:00','22:30',330,'internacional','1234567');

  -- BOG <-> LIM (Colombia - Peru) - Avianca
  crear_ruta('AV2000',1,13,14,'07:00','09:40',160,'internacional','1234567');
  crear_ruta('AV2001',1,14,13,'11:00','13:40',160,'internacional','1234567');
  crear_ruta('AV2002',1,13,14,'15:00','17:40',160,'internacional','1234567');
  crear_ruta('AV2003',1,14,13,'19:00','21:40',160,'internacional','1234567');

  -- GRU <-> EZE (Brasil - Argentina) - LATAM + Gol
  crear_ruta('LA7000',17,20,23,'07:00','09:30',150,'internacional','1234567');
  crear_ruta('LA7001',17,23,20,'11:00','13:30',150,'internacional','1234567');
  crear_ruta('G37000',20,20,23,'14:00','16:30',150,'internacional','1234567');
  crear_ruta('G37001',20,23,20,'18:00','20:30',150,'internacional','1234567');

  -- GRU <-> SCL (Brasil - Chile) - LATAM
  crear_ruta('LA7100',17,20,22,'08:30','12:00',210,'internacional','1234567');
  crear_ruta('LA7101',17,22,20,'13:30','17:00',210,'internacional','1234567');

  -- MIA <-> BOG (Miami - Colombia) - American + Copa
  crear_ruta('AA3000',3,9,13,'09:00','13:00',240,'internacional','1234567');
  crear_ruta('AA3001',3,13,9,'14:30','18:30',240,'internacional','1234567');

  -- MIA <-> GRU (Miami - Brasil) - American + LATAM
  crear_ruta('AA3100',3,9,20,'08:00','18:00',480,'intercontinental','1234567');
  crear_ruta('AA3101',3,20,9,'20:00','06:00',480,'intercontinental','1234567');

  -- MIA <-> MAD (Miami - Madrid) - Iberia + American
  crear_ruta('IB6100',7,9,15,'20:00','11:00',480,'intercontinental','1234567');
  crear_ruta('IB6101',7,15,9,'12:00','14:30',510,'intercontinental','1234567');

  -- PTY <-> BOG (Panama - Colombia) - Copa
  crear_ruta('CM800',2,6,13,'09:00','11:30',150,'internacional','1234567');
  crear_ruta('CM801',2,13,6,'13:00','15:30',150,'internacional','1234567');
  crear_ruta('CM802',2,6,13,'16:00','18:30',150,'internacional','1234567');
  crear_ruta('CM803',2,13,6,'20:00','22:30',150,'internacional','1234567');

  -- PTY <-> GRU (Panama - Brasil) - Copa
  crear_ruta('CM900',2,6,20,'10:00','18:30',450,'intercontinental','1234567',13,'14:00','15:00',60);
  crear_ruta('CM901',2,20,6,'20:30','03:00',450,'intercontinental','1234567',13,'00:00','01:00',60);

  -- MAD <-> GRU (Madrid - Brasil) - Iberia + LATAM
  crear_ruta('IB7700',7,15,20,'22:00','06:30',570,'intercontinental','1234567');
  crear_ruta('IB7701',7,20,15,'08:00','22:00',600,'intercontinental','1234567');

  -- LHR <-> JFK (Londres - Nueva York) - British Airways
  crear_ruta('BA1000',14,28,12,'09:30','12:30',420,'intercontinental','1234567');
  crear_ruta('BA1001',14,12,28,'19:00','07:00',420,'intercontinental','1234567');

  -- DXB <-> GRU (Dubai - Brasil) - Emirates
  crear_ruta('EK5000',15,33,20,'10:30','22:00',690,'intercontinental','1234567');
  crear_ruta('EK5001',15,20,33,'23:30','20:00',690,'intercontinental','1234567');

  -- Carga DHL
  crear_ruta('D0254',12,1,9,'22:15','02:55',160,'carga','135');
  crear_ruta('D0255',12,9,1,'04:15','07:10',175,'carga','246');

  COMMIT;
END;
/

PROMPT Programas de vuelo cargados. Generando calendario 2026-05-18 al 2026-12-31...

--------------------------------------------------------
-- SECCION 11: PUNTOS DE VENTA
--------------------------------------------------------

INSERT INTO AER_PUNTOVENTA VALUES (1,'WEB-AURORA','Portal web Aeropuerto Aurora',NULL,'Canal digital','ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (2,'APP-AURORA','Aplicacion movil Aurora',NULL,'Canal digital','ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (3,'GUA-MOST-01','Mostrador principal salidas internacionales',1,'Nivel 3, modulo A','ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (4,'GUA-MOST-02','Mostrador conexiones regionales',1,'Nivel 2, modulo B','ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (5,'GUA-SELF-01','Kiosko autoservicio salidas',1,'Nivel 3, zona D','ACTIVO');
INSERT INTO AER_PUNTOVENTA VALUES (6,'GUA-SELF-02','Kiosko autoservicio llegadas',1,'Nivel 1, zona A','ACTIVO');

COMMIT;

--------------------------------------------------------
-- SECCION 12: INFRAESTRUCTURA DE OPERACIONES
--------------------------------------------------------

INSERT INTO AER_HANGAR VALUES (1,'H-A1','Hangar Aurora Linea A',1,2,1850,14.5,'linea','OCUPADO');
INSERT INTO AER_HANGAR VALUES (2,'H-B2','Hangar Aurora Mantenimiento Pesado',1,1,2400,18.0,'mantenimiento pesado','DISPONIBLE');
INSERT INTO AER_HANGAR VALUES (3,'H-CG','Hangar Carga y Paqueteria',1,1,1600,16.0,'carga','DISPONIBLE');

INSERT INTO AER_CATEGORIAREPUESTO VALUES (1,'Avionica','Componentes electronicos, sensores y navegacion');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (2,'Frenos y tren de aterrizaje','Llantas, discos, pastillas y actuadores');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (3,'Cabina y seguridad','Extintores, mascarillas, chalecos y asientos');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (4,'Motor y APU','Filtros, sellos, bombas y consumibles de motor');
INSERT INTO AER_CATEGORIAREPUESTO VALUES (5,'Consumibles certificados','Aceites, fluidos, sellantes y hardware aeronautico');

INSERT INTO AER_REPUESTO VALUES (1,'AVN-ADSB-001','Transpondedor ADS-B certificado','Unidad de reemplazo para comunicacion vigilancia',1,NULL,'TRX-ADSB-21C',1,3,6,18500,'Bodega A-01','ACTIVO');
INSERT INTO AER_REPUESTO VALUES (2,'BRK-737-014','Kit de frenos Boeing 737NG','Juego de discos y pastillas para tren principal',2,2,'B737-BRK-800',2,8,18,9200,'Bodega B-04','ACTIVO');
INSERT INTO AER_REPUESTO VALUES (3,'TIR-A320-027','Llanta principal A320 family','Llanta certificada para A320/A321',2,1,'A320-TIRE-27',4,22,40,2450,'Bodega B-02','ACTIVO');
INSERT INTO AER_REPUESTO VALUES (4,'CAB-OXY-040','Mascarilla de oxigeno pasajero','Unidad PSU para cabina de pasajeros',3,NULL,'OXY-MSK-40',8,76,160,165,'Bodega C-11','ACTIVO');
INSERT INTO AER_REPUESTO VALUES (5,'ENG-FLT-LEAP-006','Filtro combustible LEAP','Filtro para motor LEAP en flota neo/MAX',4,NULL,'LEAP-FUEL-006',6,18,32,780,'Bodega D-03','ACTIVO');
INSERT INTO AER_REPUESTO VALUES (6,'APU-OIL-2380','Aceite sintetico turbina 2380','Cuarto de aceite aeronautico certificado',5,NULL,'MIL-PRF-23699',12,48,96,62,'Bodega E-01','ACTIVO');

INSERT INTO AER_PROVEEDOR VALUES (1,'AeroParts Central America S.A.','8754312-6','Calzada Atanasio Tzul 24-10','Zona 12','Guatemala','Guatemala','Guatemala','+502 2470-1800','ventas@aeroparts-ca.com','Claudia Rosales','+502 5510-1818','crosales@aeroparts-ca.com','ACTIVO',4.75);
INSERT INTO AER_PROVEEDOR VALUES (2,'MRO Logistics Miami LLC','US-88421077','NW 36th Street 5200','Doral','Miami','Florida','Estados Unidos','+1 305 555 0188','orders@mrologistics.example','Michael Torres','+1 305 555 0190','mtorres@mrologistics.example','ACTIVO',4.60);
INSERT INTO AER_PROVEEDOR VALUES (3,'Iberia Maintenance Parts','ES-B87900211','Avenida de la Hispanidad s/n','Barajas','Madrid','Madrid','Espana','+34 91 555 0101','parts@iberiamaint.example','Elena Vargas','+34 91 555 0102','evargas@iberiamaint.example','ACTIVO',4.82);

INSERT INTO AER_INCIDENTE VALUES (1,'clima','Celula de tormenta al sur de la pista obligo a espaciar aproximaciones.',TIMESTAMP '2026-05-18 14:18:00','media','CERRADO',19,2);
INSERT INTO AER_INCIDENTE VALUES (2,'equipaje','Maleta sin lectura automatica desviada a revision manual.',TIMESTAMP '2026-05-18 09:42:00','baja','CERRADO',11,24);
INSERT INTO AER_INCIDENTE VALUES (3,'tecnico','Indicacion intermitente en panel de puerta de carga.',TIMESTAMP '2026-05-18 03:05:00','media','EN_PROCESO',20,33);
INSERT INTO AER_INCIDENTE VALUES (4,'pasajero','Pasajero con documentacion incompleta derivado a migracion.',TIMESTAMP '2026-05-18 06:50:00','baja','CERRADO',4,28);

INSERT INTO AER_AUDITORIA VALUES (1,'AER_VUELO','UPDATE','ops.supervisor',TIMESTAMP '2026-05-18 14:22:00','10.20.5.14','Estado PROGRAMADO','Estado RETRASADO por clima; 45 min');
INSERT INTO AER_AUDITORIA VALUES (2,'AER_EQUIPAJE','UPDATE','bhs.automatico',TIMESTAMP '2026-05-18 09:44:00','10.30.7.22','Etiqueta sin lectura','Reetiquetado y enviado a cinta');
INSERT INTO AER_AUDITORIA VALUES (3,'AER_MANTENIMIENTOAVION','INSERT','mant.jefe',TIMESTAMP '2026-05-18 02:46:00','10.40.1.8',NULL,'Inspeccion linea carga aeronave D-AEAC');

COMMIT;

--------------------------------------------------------
-- SECCION 14: RESERVAS, VENTAS Y TRANSACCIONES DE PAGO
-- ~150 reservaciones pre-vuelo con precios realistas
-- Cubre rutas: nacional, regional, centroamerica,
--   USA, sudamerica, brasil, europa, medio oriente,
--   caribe/turistico
-- Monedas: GTQ para rutas nacionales, USD para el resto
--------------------------------------------------------

DECLARE
  PROCEDURE ins_res(
    p_num    VARCHAR2,
    p_fecha  DATE,
    p_pas    NUMBER,
    p_clase  VARCHAR2,
    p_tarifa NUMBER,
    p_equip  NUMBER,
    p_moneda VARCHAR2,
    p_metodo NUMBER,
    p_canal  VARCHAR2,
    p_codres VARCHAR2,
    p_numvta VARCHAR2
  ) IS
    v_vid  NUMBER;
    v_rid  NUMBER;
    v_venid NUMBER;
    v_total NUMBER;
    v_fres  DATE;
  BEGIN
    BEGIN
      SELECT v.VUE_ID_VUELO INTO v_vid
      FROM AER_VUELO v
      JOIN AER_PROGRAMAVUELO p ON v.VUE_ID_PROGRAMA_VUELO = p.PRV_ID
      WHERE p.PRV_NUMERO_VUELO = p_num
        AND v.VUE_FECHA_VUELO  = p_fecha
        AND v.VUE_ESTADO       != 'CANCELADO'
        AND ROWNUM = 1;
    EXCEPTION WHEN NO_DATA_FOUND THEN
      DBMS_OUTPUT.PUT_LINE('VUELO NO ENCONTRADO: '||p_num||' '||TO_CHAR(p_fecha,'YYYY-MM-DD'));
      RETURN;
    END;
    v_fres  := p_fecha - TRUNC(DBMS_RANDOM.VALUE(3,22));
    v_total := p_tarifa;
    INSERT INTO AER_RESERVA
      (RES_ID_VUELO,RES_ID_PASAJERO,RES_CLASE,RES_FECHA_RESERVA,
       RES_ESTADO,RES_EQUIPAJE_FACTURADO,RES_PESO_EQUIPAJE,
       RES_TARIFA_PAGADA,RES_CODIGO_RESERVA)
    VALUES
      (v_vid,p_pas,p_clase,v_fres,
       'CONFIRMADA',p_equip,p_equip*23,
       p_tarifa,p_codres)
    RETURNING RES_ID_RESERVA INTO v_rid;
    INSERT INTO AER_VENTABOLETO
      (VEN_NUMERO_VENTA,VEN_ID_PUNTO_VENTA,VEN_ID_EMPLEADO_VENDEDOR,
       VEN_ID_PASAJERO,VEN_FECHA_VENTA,VEN_MONTO_SUBTOTAL,
       VEN_IMPUESTOS,VEN_DESCUENTOS,VEN_MONTO_TOTAL,
       VEN_ID_METODO_PAGO,VEN_CANAL_VENTA,VEN_ESTADO)
    VALUES
      (p_numvta,NULL,NULL,
       p_pas,SYSTIMESTAMP,v_total,
       0,0,v_total,
       p_metodo,p_canal,'COMPLETADA')
    RETURNING VEN_ID_VENTA INTO v_venid;
    INSERT INTO AER_DETALLEVENTABOLETO
      (DEV_ID_VENTA,DEV_ID_RESERVA,DEV_PRECIO_BASE,DEV_CARGOS_ADICIONALES)
    VALUES(v_venid,v_rid,p_tarifa,0);
    INSERT INTO AER_TRANSACCIONPAGO
      (TRA_ID_RESERVA,TRA_ID_METODO_PAGO,TRA_MONTO_TOTAL,TRA_MONEDA,
       TRA_FECHA_TRANSACCION,TRA_ESTADO,TRA_NUMERO_AUTORIZACION,
       TRA_IP_CLIENTE,TRA_DETALLES_TARJETA)
    VALUES
      (v_rid,p_metodo,v_total,p_moneda,
       SYSTIMESTAMP,'APROBADA','AUTH-'||LPAD(v_rid,8,'0'),
       '190.81.42.'||TO_CHAR(MOD(p_pas*7,254)+1),
       CASE WHEN p_metodo IN (1,2)
            THEN '****'||LPAD(TO_CHAR(MOD(v_rid*37+p_pas*13,9000)+1000),4,'0')
            ELSE NULL END);
  END ins_res;

BEGIN
  -- ===================================================
  -- NACIONAL: GUA->FRS (TAG Airlines) [GTQ]
  -- Dias 1234567 | Lun 18/may = dia ISO 1
  -- ===================================================
  ins_res('5U050',DATE'2026-05-18',1,'economica',425,1,'GTQ',1,'web','AUR-200001','VTA-200001');
  ins_res('5U050',DATE'2026-05-18',2,'economica',425,0,'GTQ',5,'app','AUR-200002','VTA-200002');
  ins_res('5U052',DATE'2026-05-19',3,'economica',450,1,'GTQ',2,'web','AUR-200003','VTA-200003');
  ins_res('5U054',DATE'2026-05-20',4,'economica',425,2,'GTQ',3,'web','AUR-200004','VTA-200004');
  ins_res('5U056',DATE'2026-05-21',5,'economica',395,0,'GTQ',5,'app','AUR-200005','VTA-200005');
  ins_res('5U050',DATE'2026-05-22',6,'economica',425,1,'GTQ',1,'web','AUR-200006','VTA-200006');
  ins_res('5U058',DATE'2026-05-23',7,'economica',395,1,'GTQ',2,'web','AUR-200007','VTA-200007');
  ins_res('5U050',DATE'2026-05-25',8,'economica',425,0,'GTQ',1,'web','AUR-200008','VTA-200008');
  ins_res('5U052',DATE'2026-05-26',9,'economica',450,1,'GTQ',5,'app','AUR-200009','VTA-200009');
  ins_res('5U054',DATE'2026-05-27',10,'economica',425,0,'GTQ',2,'web','AUR-200010','VTA-200010');
  ins_res('5U056',DATE'2026-05-28',11,'economica',395,1,'GTQ',1,'app','AUR-200011','VTA-200011');
  ins_res('5U058',DATE'2026-05-29',12,'economica',395,0,'GTQ',5,'web','AUR-200012','VTA-200012');
  ins_res('5U050',DATE'2026-05-30',13,'economica',425,1,'GTQ',3,'app','AUR-200013','VTA-200013');
  ins_res('5U052',DATE'2026-06-01',14,'economica',450,0,'GTQ',5,'app','AUR-200014','VTA-200014');
  ins_res('5U050',DATE'2026-06-02',15,'economica',425,1,'GTQ',1,'web','AUR-200015','VTA-200015');

  -- ===================================================
  -- REGIONAL: GUA->SAL / GUA->SAP [USD]
  -- ===================================================
  ins_res('5U100',DATE'2026-05-18',16,'economica',95, 1,'USD',1,'web','AUR-200016','VTA-200016');
  ins_res('AV200',DATE'2026-05-18',17,'economica',110,0,'USD',5,'app','AUR-200017','VTA-200017');
  ins_res('5U100',DATE'2026-05-20',18,'economica',95, 1,'USD',2,'web','AUR-200018','VTA-200018');
  ins_res('5U150',DATE'2026-05-18',19,'economica',115,0,'USD',1,'web','AUR-200019','VTA-200019');
  ins_res('5U150',DATE'2026-05-21',20,'economica',115,1,'USD',5,'app','AUR-200020','VTA-200020');

  -- ===================================================
  -- CENTROAMERICA: GUA->SJO (Copa + Avianca) [USD]
  -- ===================================================
  ins_res('CM250',DATE'2026-05-18',21,'economica',195,1,'USD',1,'web','AUR-200021','VTA-200021');
  ins_res('CM250',DATE'2026-05-18',22,'ejecutiva',520,0,'USD',1,'web','AUR-200022','VTA-200022');
  ins_res('AV300',DATE'2026-05-19',23,'economica',180,1,'USD',5,'app','AUR-200023','VTA-200023');
  ins_res('CM252',DATE'2026-05-20',24,'economica',195,2,'USD',2,'web','AUR-200024','VTA-200024');
  ins_res('AV302',DATE'2026-05-21',25,'economica',175,0,'USD',5,'app','AUR-200025','VTA-200025');

  -- ===================================================
  -- PANAMA HUB: GUA->PTY (Copa - 5 vuelos/dia) [USD]
  -- ===================================================
  ins_res('CM100',DATE'2026-05-18',26,'economica',215,1,'USD',1,'web','AUR-200026','VTA-200026');
  ins_res('CM100',DATE'2026-05-18',27,'ejecutiva',580,0,'USD',1,'web','AUR-200027','VTA-200027');
  ins_res('CM102',DATE'2026-05-18',28,'economica',225,1,'USD',5,'app','AUR-200028','VTA-200028');
  ins_res('CM104',DATE'2026-05-19',29,'economica',215,0,'USD',2,'web','AUR-200029','VTA-200029');
  ins_res('CM106',DATE'2026-05-20',30,'economica',210,1,'USD',5,'app','AUR-200030','VTA-200030');
  ins_res('CM100',DATE'2026-05-21',31,'economica',220,0,'USD',1,'web','AUR-200031','VTA-200031');
  ins_res('CM108',DATE'2026-05-22',32,'economica',215,2,'USD',3,'web','AUR-200032','VTA-200032');
  ins_res('CM102',DATE'2026-05-25',33,'ejecutiva',595,0,'USD',1,'web','AUR-200033','VTA-200033');
  ins_res('CM104',DATE'2026-05-26',34,'economica',215,1,'USD',5,'app','AUR-200034','VTA-200034');
  ins_res('CM100',DATE'2026-06-01',35,'economica',220,0,'USD',2,'web','AUR-200035','VTA-200035');

  -- ===================================================
  -- MEXICO: GUA->MEX / GUA->CUN [USD]
  -- ===================================================
  ins_res('AM650', DATE'2026-05-18',36,'economica',265,1,'USD',1,'web','AUR-200036','VTA-200036');
  ins_res('AM650', DATE'2026-05-18',37,'ejecutiva',690,0,'USD',1,'web','AUR-200037','VTA-200037');
  ins_res('Y43900',DATE'2026-05-19',38,'economica',235,1,'USD',5,'app','AUR-200038','VTA-200038');
  ins_res('AM652', DATE'2026-05-20',39,'economica',255,0,'USD',2,'web','AUR-200039','VTA-200039');
  ins_res('Y43800',DATE'2026-05-20',40,'economica',270,1,'USD',5,'app','AUR-200040','VTA-200040');
  ins_res('DL1850',DATE'2026-05-19',41,'economica',285,1,'USD',1,'web','AUR-200041','VTA-200041');
  ins_res('AM650', DATE'2026-05-25',42,'economica',265,0,'USD',3,'web','AUR-200042','VTA-200042');

  -- ===================================================
  -- USA: GUA->MIA (4 aerolineas) [USD]
  -- AA1100/AV620/DL1820 = 1234567 | NK500 = 135 (L/Mi/V)
  -- ===================================================
  ins_res('AA1100',DATE'2026-05-18',43,'economica',345,1,'USD',1,'web','AUR-200043','VTA-200043');
  ins_res('AA1100',DATE'2026-05-18',44,'ejecutiva',920,0,'USD',1,'web','AUR-200044','VTA-200044');
  ins_res('AV620', DATE'2026-05-18',45,'economica',360,1,'USD',5,'app','AUR-200045','VTA-200045');
  ins_res('DL1820',DATE'2026-05-18',46,'economica',325,1,'USD',2,'web','AUR-200046','VTA-200046');
  ins_res('NK500', DATE'2026-05-18',47,'economica',289,0,'USD',5,'app','AUR-200047','VTA-200047');
  ins_res('AA1100',DATE'2026-05-20',48,'economica',345,2,'USD',1,'web','AUR-200048','VTA-200048');
  ins_res('AA1100',DATE'2026-05-20',49,'ejecutiva',950,0,'USD',1,'web','AUR-200049','VTA-200049');
  ins_res('DL1820',DATE'2026-05-19',50,'economica',335,1,'USD',2,'web','AUR-200050','VTA-200050');
  ins_res('AV620', DATE'2026-05-21',51,'economica',355,0,'USD',5,'app','AUR-200051','VTA-200051');
  ins_res('NK500', DATE'2026-05-22',52,'economica',279,1,'USD',5,'app','AUR-200052','VTA-200052');
  ins_res('AA1100',DATE'2026-06-02',53,'economica',345,1,'USD',3,'web','AUR-200053','VTA-200053');

  -- ===================================================
  -- USA: IAH / LAX / JFK / ATL / ORD / DFW / MCO [USD]
  -- ===================================================
  ins_res('UA1800',DATE'2026-05-18',54,'economica',380,1,'USD',1,'web','AUR-200054','VTA-200054');
  ins_res('UA1800',DATE'2026-05-18',55,'ejecutiva',960,0,'USD',1,'web','AUR-200055','VTA-200055');
  ins_res('UA1700',DATE'2026-05-18',56,'economica',480,1,'USD',1,'web','AUR-200056','VTA-200056');
  ins_res('AA1050',DATE'2026-05-18',57,'economica',445,1,'USD',2,'web','AUR-200057','VTA-200057');
  ins_res('B62000',DATE'2026-05-18',58,'economica',420,0,'USD',5,'app','AUR-200058','VTA-200058');
  ins_res('DL1800',DATE'2026-05-18',59,'economica',365,1,'USD',1,'web','AUR-200059','VTA-200059');
  ins_res('UA3000',DATE'2026-05-18',60,'economica',510,1,'USD',2,'web','AUR-200060','VTA-200060');
  ins_res('AA9000',DATE'2026-05-18',61,'economica',395,0,'USD',5,'app','AUR-200061','VTA-200061');
  ins_res('AA9600',DATE'2026-05-18',62,'economica',370,1,'USD',1,'web','AUR-200062','VTA-200062');
  ins_res('UA1800',DATE'2026-05-21',63,'ejecutiva',980,0,'USD',1,'web','AUR-200063','VTA-200063');

  -- ===================================================
  -- SUDAMERICA: GUA->BOG (Avianca - 2 vuelos/dia) [USD]
  -- ===================================================
  ins_res('AV400',DATE'2026-05-18',64,'economica',335,1,'USD',1,'web','AUR-200064','VTA-200064');
  ins_res('AV400',DATE'2026-05-18',65,'ejecutiva',870,0,'USD',1,'web','AUR-200065','VTA-200065');
  ins_res('AV402',DATE'2026-05-18',66,'economica',345,1,'USD',5,'app','AUR-200066','VTA-200066');
  ins_res('AV400',DATE'2026-05-20',67,'economica',335,2,'USD',2,'web','AUR-200067','VTA-200067');
  ins_res('AV402',DATE'2026-05-21',68,'economica',340,0,'USD',5,'app','AUR-200068','VTA-200068');
  ins_res('AV400',DATE'2026-05-25',69,'economica',335,1,'USD',1,'web','AUR-200069','VTA-200069');
  ins_res('AV402',DATE'2026-05-26',70,'ejecutiva',880,0,'USD',3,'web','AUR-200070','VTA-200070');

  -- ===================================================
  -- SUDAMERICA: GUA->LIM (Avianca + LATAM) [USD]
  -- ===================================================
  ins_res('AV500', DATE'2026-05-18',71,'economica',475,1,'USD',1,'web','AUR-200071','VTA-200071');
  ins_res('AV500', DATE'2026-05-18',72,'ejecutiva',1250,0,'USD',1,'web','AUR-200072','VTA-200072');
  ins_res('LA5200',DATE'2026-05-18',73,'economica',490,1,'USD',5,'app','AUR-200073','VTA-200073');
  ins_res('AV500', DATE'2026-05-20',74,'economica',465,2,'USD',2,'web','AUR-200074','VTA-200074');
  ins_res('LA5200',DATE'2026-05-22',75,'economica',480,0,'USD',5,'app','AUR-200075','VTA-200075');

  -- ===================================================
  -- SUDAMERICA: GUA->UIO / GUA->MDE [USD]
  -- ===================================================
  ins_res('AV600',DATE'2026-05-18',76,'economica',395,1,'USD',1,'web','AUR-200076','VTA-200076');
  ins_res('AV700',DATE'2026-05-18',77,'economica',285,1,'USD',5,'app','AUR-200077','VTA-200077');
  ins_res('AV600',DATE'2026-05-20',78,'economica',390,0,'USD',2,'web','AUR-200078','VTA-200078');
  ins_res('AV700',DATE'2026-05-22',79,'economica',280,1,'USD',1,'web','AUR-200079','VTA-200079');
  ins_res('AV600',DATE'2026-05-25',80,'ejecutiva',1050,0,'USD',1,'web','AUR-200080','VTA-200080');

  -- ===================================================
  -- BRASIL: GUA->GRU / GUA->GIG [USD]
  -- LA5000 = 1234567 | LA5002 = 246 | G35100 = 1357 | LA5100 = 246
  -- ===================================================
  ins_res('LA5000',DATE'2026-05-18',81,'economica',785, 1,'USD',1,'web','AUR-200081','VTA-200081');
  ins_res('LA5000',DATE'2026-05-18',82,'ejecutiva',2100,0,'USD',1,'web','AUR-200082','VTA-200082');
  ins_res('LA5002',DATE'2026-05-19',83,'economica',765, 1,'USD',5,'app','AUR-200083','VTA-200083');
  ins_res('LA5000',DATE'2026-05-20',84,'economica',785, 2,'USD',2,'web','AUR-200084','VTA-200084');
  ins_res('G35100',DATE'2026-05-18',85,'economica',815, 1,'USD',1,'web','AUR-200085','VTA-200085');
  ins_res('G35100',DATE'2026-05-18',86,'ejecutiva',2050,0,'USD',1,'web','AUR-200086','VTA-200086');
  ins_res('LA5100',DATE'2026-05-19',87,'economica',820, 1,'USD',5,'app','AUR-200087','VTA-200087');
  ins_res('LA5000',DATE'2026-06-01',88,'economica',785, 1,'USD',3,'web','AUR-200088','VTA-200088');

  -- ===================================================
  -- SUR: GUA->SCL / GUA->EZE [USD]
  -- LA5400 = 1234567 | AR6000 = 1357 | AV3400 = 246
  -- ===================================================
  ins_res('LA5400',DATE'2026-05-18',89,'economica',895, 1,'USD',1,'web','AUR-200089','VTA-200089');
  ins_res('LA5400',DATE'2026-05-18',90,'ejecutiva',2350,0,'USD',1,'web','AUR-200090','VTA-200090');
  ins_res('LA5400',DATE'2026-05-20',91,'economica',880, 2,'USD',2,'web','AUR-200091','VTA-200091');
  ins_res('AR6000',DATE'2026-05-18',92,'economica',920, 1,'USD',5,'app','AUR-200092','VTA-200092');
  ins_res('AV3400',DATE'2026-05-19',93,'economica',910, 1,'USD',1,'web','AUR-200093','VTA-200093');
  ins_res('AR6000',DATE'2026-05-20',94,'ejecutiva',2400,0,'USD',1,'web','AUR-200094','VTA-200094');

  -- ===================================================
  -- EUROPA: GUA->MAD (Iberia - diario) [USD]
  -- ===================================================
  ins_res('IB6000',DATE'2026-05-18',95,'economica',980, 1,'USD',1,'web','AUR-200095','VTA-200095');
  ins_res('IB6000',DATE'2026-05-18',96,'ejecutiva',2650,0,'USD',1,'web','AUR-200096','VTA-200096');
  ins_res('IB6000',DATE'2026-05-18',97,'primera',  4800,0,'USD',1,'web','AUR-200097','VTA-200097');
  ins_res('IB6000',DATE'2026-05-19',98,'economica',965, 1,'USD',2,'web','AUR-200098','VTA-200098');
  ins_res('IB6000',DATE'2026-05-20',99,'economica',975, 2,'USD',5,'app','AUR-200099','VTA-200099');
  ins_res('IB6000',DATE'2026-05-21',100,'ejecutiva',2700,0,'USD',3,'web','AUR-200100','VTA-200100');

  -- ===================================================
  -- EUROPA: GUA->CDG (Air France - diario) [USD]
  -- ===================================================
  ins_res('AF4000',DATE'2026-05-18',1,'economica',1050,1,'USD',1,'web','AUR-200101','VTA-200101');
  ins_res('AF4000',DATE'2026-05-18',2,'ejecutiva',2850,0,'USD',1,'web','AUR-200102','VTA-200102');
  ins_res('AF4000',DATE'2026-05-19',3,'primera',  5200,0,'USD',1,'web','AUR-200103','VTA-200103');
  ins_res('AF4000',DATE'2026-05-20',4,'economica',1020,1,'USD',2,'web','AUR-200104','VTA-200104');
  ins_res('AF4000',DATE'2026-05-22',5,'economica',1035,1,'USD',5,'app','AUR-200105','VTA-200105');

  -- ===================================================
  -- EUROPA: GUA->LHR (British Airways - 1357) [USD]
  -- ===================================================
  ins_res('BA6500',DATE'2026-05-18',6,'economica',1120,1,'USD',1,'web','AUR-200106','VTA-200106');
  ins_res('BA6500',DATE'2026-05-18',7,'ejecutiva',2950,0,'USD',1,'web','AUR-200107','VTA-200107');
  ins_res('BA6500',DATE'2026-05-18',8,'primera',  5500,0,'USD',1,'web','AUR-200108','VTA-200108');
  ins_res('BA6500',DATE'2026-05-20',9,'economica',1080,1,'USD',2,'web','AUR-200109','VTA-200109');
  ins_res('BA6500',DATE'2026-05-22',10,'ejecutiva',2900,0,'USD',5,'app','AUR-200110','VTA-200110');

  -- ===================================================
  -- EUROPA: GUA->FRA (LH-246) / GUA->LIS (TP-135) / GUA->AMS (IB-37) [USD]
  -- ===================================================
  ins_res('LH5000',DATE'2026-05-19',11,'economica',1010,1,'USD',1,'web','AUR-200111','VTA-200111');
  ins_res('LH5000',DATE'2026-05-21',12,'ejecutiva',2720,0,'USD',1,'web','AUR-200112','VTA-200112');
  ins_res('TP4500',DATE'2026-05-18',13,'economica',945, 1,'USD',2,'web','AUR-200113','VTA-200113');
  ins_res('TP4500',DATE'2026-05-20',14,'economica',930, 0,'USD',5,'app','AUR-200114','VTA-200114');
  ins_res('IB7200',DATE'2026-05-20',15,'economica',1095,1,'USD',1,'web','AUR-200115','VTA-200115');

  -- ===================================================
  -- MEDIO ORIENTE: GUA->DXB (Emirates - dias 26=Mar/Sab) [USD]
  -- ===================================================
  ins_res('EK8800',DATE'2026-05-19',16,'economica',1580,1,'USD',1,'web','AUR-200116','VTA-200116');
  ins_res('EK8800',DATE'2026-05-19',17,'ejecutiva',3800,0,'USD',1,'web','AUR-200117','VTA-200117');
  ins_res('EK8800',DATE'2026-05-19',18,'primera',  6200,0,'USD',1,'web','AUR-200118','VTA-200118');
  ins_res('EK8800',DATE'2026-05-23',19,'economica',1560,1,'USD',2,'web','AUR-200119','VTA-200119');
  ins_res('EK8800',DATE'2026-05-26',20,'ejecutiva',3750,0,'USD',5,'app','AUR-200120','VTA-200120');

  -- ===================================================
  -- CARIBE Y TURISTICO: GUA->HAV / SDQ / SJU / CUN [USD]
  -- CM550=135 | B65000=1357 | AA4200=1357 | Y43800=1234567
  -- ===================================================
  ins_res('CM550', DATE'2026-05-18',21,'economica',425, 1,'USD',1,'web','AUR-200121','VTA-200121');
  ins_res('CM550', DATE'2026-05-20',22,'economica',430, 0,'USD',5,'app','AUR-200122','VTA-200122');
  ins_res('B65000',DATE'2026-05-18',23,'economica',480, 1,'USD',2,'web','AUR-200123','VTA-200123');
  ins_res('B65000',DATE'2026-05-20',24,'economica',475, 1,'USD',1,'web','AUR-200124','VTA-200124');
  ins_res('AA4200',DATE'2026-05-18',25,'economica',510, 0,'USD',5,'app','AUR-200125','VTA-200125');
  ins_res('AA4200',DATE'2026-05-20',26,'ejecutiva',1350,0,'USD',1,'web','AUR-200126','VTA-200126');
  ins_res('Y43800',DATE'2026-05-18',27,'economica',285, 1,'USD',1,'web','AUR-200127','VTA-200127');
  ins_res('Y43800',DATE'2026-05-19',28,'economica',290, 2,'USD',5,'app','AUR-200128','VTA-200128');

  -- ===================================================
  -- ADICIONALES: vuelos de mayo tardio y junio
  -- ===================================================
  ins_res('CM100', DATE'2026-05-22',29,'economica',215, 1,'USD',2,'web','AUR-200129','VTA-200129');
  ins_res('AA1100',DATE'2026-05-22',30,'economica',345, 0,'USD',5,'app','AUR-200130','VTA-200130');
  ins_res('AV400', DATE'2026-05-22',31,'economica',335, 1,'USD',1,'web','AUR-200131','VTA-200131');
  ins_res('LA5000',DATE'2026-05-22',32,'economica',785, 1,'USD',3,'web','AUR-200132','VTA-200132');
  ins_res('IB6000',DATE'2026-05-22',33,'ejecutiva',2680,0,'USD',1,'web','AUR-200133','VTA-200133');
  ins_res('CM250', DATE'2026-05-25',34,'economica',190, 1,'USD',5,'app','AUR-200134','VTA-200134');
  ins_res('AM650', DATE'2026-05-26',35,'ejecutiva',715, 0,'USD',1,'web','AUR-200135','VTA-200135');
  ins_res('UA1700',DATE'2026-05-27',36,'ejecutiva',1250,0,'USD',1,'web','AUR-200136','VTA-200136');
  ins_res('AA1050',DATE'2026-05-28',37,'economica',440, 1,'USD',2,'web','AUR-200137','VTA-200137');
  ins_res('DL1800',DATE'2026-05-29',38,'economica',360, 1,'USD',5,'app','AUR-200138','VTA-200138');
  ins_res('AV500', DATE'2026-05-29',39,'ejecutiva',1280,0,'USD',1,'web','AUR-200139','VTA-200139');
  ins_res('LA5400',DATE'2026-05-29',40,'primera',  3950,0,'USD',1,'web','AUR-200140','VTA-200140');
  ins_res('IB6000',DATE'2026-05-29',41,'primera',  4950,0,'USD',1,'web','AUR-200141','VTA-200141');
  ins_res('AF4000',DATE'2026-05-29',42,'ejecutiva',2780,0,'USD',2,'web','AUR-200142','VTA-200142');
  ins_res('BA6500',DATE'2026-05-24',43,'economica',1100,1,'USD',1,'web','AUR-200143','VTA-200143');
  ins_res('AV402', DATE'2026-06-01',44,'economica',340, 1,'USD',5,'app','AUR-200144','VTA-200144');
  ins_res('AA1100',DATE'2026-06-02',45,'ejecutiva',940, 0,'USD',1,'web','AUR-200145','VTA-200145');
  ins_res('IB6000',DATE'2026-06-01',46,'economica',970, 1,'USD',5,'app','AUR-200146','VTA-200146');
  ins_res('LA5000',DATE'2026-06-02',47,'primera',  4200,0,'USD',1,'web','AUR-200147','VTA-200147');
  ins_res('CM100', DATE'2026-06-02',48,'economica',220, 1,'USD',2,'web','AUR-200148','VTA-200148');
  ins_res('EK8800',DATE'2026-05-26',49,'economica',1570,1,'USD',3,'web','AUR-200149','VTA-200149');
  ins_res('DL1800',DATE'2026-06-01',50,'ejecutiva',950, 0,'USD',1,'web','AUR-200150','VTA-200150');

  COMMIT;
END;
/

PROMPT Reservas cargadas (150 reservaciones + ventas + detalles + transacciones).

--------------------------------------------------------
-- SECCION 13: AJUSTE DE IDENTIDADES
--------------------------------------------------------

DECLARE
  PROCEDURE bump(p_table VARCHAR2, p_col VARCHAR2) IS
  BEGIN
    EXECUTE IMMEDIATE 'ALTER TABLE '||p_table||' MODIFY '||p_col||' GENERATED BY DEFAULT ON NULL AS IDENTITY (START WITH LIMIT VALUE)';
  EXCEPTION WHEN OTHERS THEN
    DBMS_OUTPUT.PUT_LINE('Aviso: no se ajusto identidad de '||p_table||'.'||p_col||': '||SQLERRM);
  END;
BEGIN
  bump('AER_AEROPUERTO','AER_ID');
  bump('AER_TERMINAL','TER_ID_TERMINAL');
  bump('AER_PUERTA_EMBARQUE','PUE_ID_PUERTA');
  bump('AER_AEROLINEA','ARL_ID');
  bump('AER_MODELOAVION','MOD_ID_MODELO');
  bump('AER_AVION','AVI_ID');
  bump('AER_ASIENTO_AVION','ASA_ID_ASIENTO');
  bump('AER_PROGRAMAVUELO','PRV_ID');
  bump('AER_DIASVUELO','DIA_ID_DIA_VUELO');
  bump('AER_ESCALATECNICA','ESC_ID_ESCALA');
  bump('AER_METODOPAGO','MET_ID_METODO_PAGO');
  bump('AER_PROMOCION','PRO_ID_PROMOCION');
  bump('AER_DEPARTAMENTO','DEP_ID_DEPARTAMENTO');
  bump('AER_PUESTO','PUE_ID_PUESTO');
  bump('AER_EMPLEADO','EMP_ID_EMPLEADO');
  bump('AER_LICENCIAEMPLEADO','LIC_ID_LICENCIA');
  bump('AER_ASISTENCIA','ASI_ID_ASISTENCIA');
  bump('AER_PLANILLA','PLA_ID_PLANILLA');
  bump('AER_PASAJERO','PAS_ID_PASAJERO');
  bump('AER_USUARIO_LOGIN','USL_ID_USUARIO');
  bump('AER_PREFERENCIACLIENTE','PRF_ID_PREFERENCIA');
  bump('AER_VUELO','VUE_ID_VUELO');
  bump('AER_ASIGNACION_PUERTA','ASP_ID_ASIGNACION');
  bump('AER_PUNTOVENTA','PUV_ID_PUNTO_VENTA');
  bump('AER_HANGAR','HAN_ID_HANGAR');
  bump('AER_CATEGORIAREPUESTO','CAT_ID_CATEGORIA');
  bump('AER_REPUESTO','REP_ID_REPUESTO');
  bump('AER_PROVEEDOR','PRV_ID_PROVEEDOR');
  bump('AER_INCIDENTE','INC_ID_INCIDENTE');
  bump('AER_AUDITORIA','AUD_ID_AUDITORIA');
  bump('AER_RESERVA','RES_ID_RESERVA');
  bump('AER_VENTABOLETO','VEN_ID_VENTA');
  bump('AER_DETALLEVENTABOLETO','DEV_ID_DETALLE_VENTA');
  bump('AER_TRANSACCIONPAGO','TRA_ID_TRANSACCION');
END;
/

COMMIT;

PROMPT ============================================================
PROMPT  SEED DE PRESENTACION CARGADA EXITOSAMENTE
PROMPT
PROMPT  Red aeroportuaria:  37 aeropuertos en 5 continentes
PROMPT  Aerolineas:         20 (nacionales, regionales e intercont.)
PROMPT  Flota:              34 aeronaves activas
PROMPT  Pasajeros:          120 pasajeros registrados
PROMPT  Usuarios:           100 (80 activos, 10 inactivos, 10 bloq.)
PROMPT  Usuarios admin:     3 (admin, soporte, auditoria)
PROMPT  Rutas programadas:  90+ programas de vuelo
PROMPT  Vuelos generados:   ~18,000 vuelos (mayo-diciembre 2026)
PROMPT  Reservas:           150 (con venta, detalle y transaccion)
PROMPT  Rangos de precio (USD):
PROMPT    Nacional GUA-FRS  : GTQ  395-450 economica
PROMPT    Regional SAL/SAP  : USD   95-115 economica
PROMPT    Centroamerica PTY : USD  210-580 eco/ejecutiva
PROMPT    USA MIA/JFK/LAX   : USD  279-980 eco/ejecutiva
PROMPT    Sudamerica BOG/LIM: USD  335-1280 eco/ejecutiva
PROMPT    Brasil GRU/GIG    : USD  765-2100 eco/ejecutiva
PROMPT    Europa MAD/CDG/LHR: USD  930-5500 eco/ejecutiva/primera
PROMPT    Medio Oriente DXB : USD 1560-6200 eco/ejecutiva/primera
PROMPT
PROMPT  Rutas destacadas desde GUA:
PROMPT    GUA<->PTY  : 5 vuelos/dia  (Copa Airlines - hub)
PROMPT    GUA<->MIA  : 4 vuelos/dia  (AA, Delta, Avianca, Spirit)
PROMPT    GUA<->FRS  : 5 vuelos/dia  (TAG Airlines - domestico)
PROMPT    GUA<->GRU  : 2 vuelos/dia  (LATAM - Brasil!)
PROMPT    GUA<->GIG  : 2 vuelos/dia  (Gol + LATAM - Brasil!)
PROMPT    GUA<->EZE  : 2 vuelos/dia  (Aerolineas Arg + Avianca)
PROMPT    GUA<->MAD  : 1 vuelo/dia   (Iberia)
PROMPT    GUA<->CDG  : 1 vuelo/dia   (Air France)
PROMPT    GUA<->LHR  : 4x semana     (British Airways)
PROMPT    GUA<->DXB  : 2x semana     (Emirates)
PROMPT
PROMPT  Credenciales:
PROMPT    admin.aurora        / AdminAurora1!
PROMPT    soporte.operaciones / SoporteAurora1!
PROMPT    [usuarios activos]  / AuroraDemo1!
PROMPT    [usuarios inact/blq]/ DemoInactivo1!
PROMPT ============================================================
