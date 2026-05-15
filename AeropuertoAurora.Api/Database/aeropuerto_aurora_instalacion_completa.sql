PROMPT =========================================================
PROMPT AEROPUERTO AURORA - INSTALACION COMPLETA
PROMPT =========================================================
PROMPT Este script ejecuta en orden:
PROMPT 1. Esquema base
PROMPT 2. Seed operativa principal
PROMPT 3. Vuelos programados masivos
PROMPT 4. Cambios agregados durante la implementacion
PROMPT 5. Reset de identidades
PROMPT =========================================================
PROMPT Requisito:
PROMPT Ejecutar este script desde la carpeta Database en SQL*Plus o SQLcl
PROMPT para que los @@ funcionen correctamente.
PROMPT =========================================================

SET DEFINE OFF;
WHENEVER SQLERROR EXIT SQL.SQLCODE;

PROMPT
PROMPT [1/7] Creando esquema base...
@@aeropuerto_aurora_v2.sql

PROMPT
PROMPT [2/7] Cargando seed operativa...
@@aeropuerto_aurora_seed_wow.sql

PROMPT
PROMPT [3/7] Cargando vuelos programados masivos...
@@aeropuerto_aurora_vuelos_programados_masivos.sql

PROMPT
PROMPT [4/7] Aplicando cambio: cantidad de boletos en venta...
@@alter_add_venta_cantidad_boletos.sql

PROMPT
PROMPT [5/7] Aplicando cambio: snapshot de pasajero en detalle de boleto...
@@alter_detalle_boleto_pasajero_snapshot.sql

PROMPT
PROMPT [6/7] Aplicando cambio: tabla de documento de pasajero...
@@alter_add_documento_pasajero.sql

PROMPT
PROMPT [7/7] Ajustando identidades...
@@reset-identities.sql

PROMPT
PROMPT =========================================================
PROMPT INSTALACION COMPLETA FINALIZADA
PROMPT =========================================================

