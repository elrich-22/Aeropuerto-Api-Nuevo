--------------------------------------------------------
-- AEROPUERTO AURORA - RESET TOTAL DEL ESQUEMA
--
-- Uso recomendado:
--   1) Ejecutar este script sobre el esquema actual
--   2) Ejecutar aeropuerto_aurora_v2.sql
--   3) Ejecutar aeropuerto_aurora_seed_maestra.sql
--
-- Deja el esquema practicamente "como nuevo" eliminando
-- tablas y demas objetos creados para el proyecto.
--------------------------------------------------------

SET DEFINE OFF;
ALTER SESSION SET CURRENT_SCHEMA = AEROPUERTO_AURORA;

BEGIN
  FOR obj IN (
    SELECT object_name, object_type
    FROM user_objects
    WHERE object_type IN ('VIEW', 'MATERIALIZED VIEW', 'SEQUENCE', 'PROCEDURE', 'FUNCTION', 'PACKAGE', 'TRIGGER', 'TYPE')
    ORDER BY
      CASE object_type
        WHEN 'TRIGGER' THEN 1
        WHEN 'VIEW' THEN 2
        WHEN 'MATERIALIZED VIEW' THEN 3
        WHEN 'PROCEDURE' THEN 4
        WHEN 'FUNCTION' THEN 5
        WHEN 'PACKAGE' THEN 6
        WHEN 'TYPE' THEN 7
        WHEN 'SEQUENCE' THEN 8
        ELSE 9
      END,
      object_name
  ) LOOP
    BEGIN
      IF obj.object_type = 'VIEW' THEN
        EXECUTE IMMEDIATE 'DROP VIEW "' || obj.object_name || '"';
      ELSIF obj.object_type = 'MATERIALIZED VIEW' THEN
        EXECUTE IMMEDIATE 'DROP MATERIALIZED VIEW "' || obj.object_name || '"';
      ELSIF obj.object_type = 'SEQUENCE' THEN
        EXECUTE IMMEDIATE 'DROP SEQUENCE "' || obj.object_name || '"';
      ELSIF obj.object_type = 'PROCEDURE' THEN
        EXECUTE IMMEDIATE 'DROP PROCEDURE "' || obj.object_name || '"';
      ELSIF obj.object_type = 'FUNCTION' THEN
        EXECUTE IMMEDIATE 'DROP FUNCTION "' || obj.object_name || '"';
      ELSIF obj.object_type = 'PACKAGE' THEN
        EXECUTE IMMEDIATE 'DROP PACKAGE "' || obj.object_name || '"';
      ELSIF obj.object_type = 'TRIGGER' THEN
        EXECUTE IMMEDIATE 'DROP TRIGGER "' || obj.object_name || '"';
      ELSIF obj.object_type = 'TYPE' THEN
        EXECUTE IMMEDIATE 'DROP TYPE "' || obj.object_name || '" FORCE';
      END IF;
    EXCEPTION
      WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Aviso al eliminar ' || obj.object_type || ' ' || obj.object_name || ': ' || SQLERRM);
    END;
  END LOOP;
END;
/

BEGIN
  FOR tbl IN (
    SELECT table_name
    FROM user_tables
    ORDER BY table_name
  ) LOOP
    BEGIN
      EXECUTE IMMEDIATE 'DROP TABLE "' || tbl.table_name || '" CASCADE CONSTRAINTS PURGE';
    EXCEPTION
      WHEN OTHERS THEN
        DBMS_OUTPUT.PUT_LINE('Aviso al eliminar tabla ' || tbl.table_name || ': ' || SQLERRM);
    END;
  END LOOP;
END;
/

COMMIT;

PROMPT Reset total completado. Ahora ejecuta aeropuerto_aurora_v2.sql y luego aeropuerto_aurora_seed_maestra.sql
