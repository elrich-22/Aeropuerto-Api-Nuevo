PROMPT ============================================
PROMPT MIGRACION: CANTIDAD DE BOLETOS POR VENTA
PROMPT ============================================
PROMPT Este script:
PROMPT 1. Agrega VEN_CANTIDAD_BOLETOS en AER_VENTABOLETO
PROMPT 2. Rellena ventas existentes con base en AER_DETALLEVENTABOLETO
PROMPT 3. Corrige valores nulos o invalidos
PROMPT 4. Agrega una restriccion CHECK
PROMPT ============================================

DECLARE
    v_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*)
    INTO v_exists
    FROM USER_TAB_COLUMNS
    WHERE TABLE_NAME = 'AER_VENTABOLETO'
      AND COLUMN_NAME = 'VEN_CANTIDAD_BOLETOS';

    IF v_exists = 0 THEN
        EXECUTE IMMEDIATE '
            ALTER TABLE AER_VENTABOLETO ADD (
                VEN_CANTIDAD_BOLETOS NUMBER DEFAULT 1 NOT NULL
            )';
    END IF;
END;
/

UPDATE AER_VENTABOLETO v
SET VEN_CANTIDAD_BOLETOS = (
    SELECT CASE
        WHEN COUNT(*) > 0 THEN COUNT(*)
        ELSE 1
    END
    FROM AER_DETALLEVENTABOLETO d
    WHERE d.DEV_ID_VENTA = v.VEN_ID_VENTA
);

UPDATE AER_VENTABOLETO
SET VEN_CANTIDAD_BOLETOS = 1
WHERE NVL(VEN_CANTIDAD_BOLETOS, 0) <= 0;

DECLARE
    v_exists NUMBER := 0;
BEGIN
    SELECT COUNT(*)
    INTO v_exists
    FROM USER_CONSTRAINTS
    WHERE TABLE_NAME = 'AER_VENTABOLETO'
      AND CONSTRAINT_NAME = 'CHK_VENTA_CANTIDAD';

    IF v_exists = 0 THEN
        EXECUTE IMMEDIATE '
            ALTER TABLE AER_VENTABOLETO
            ADD CONSTRAINT CHK_VENTA_CANTIDAD
            CHECK (VEN_CANTIDAD_BOLETOS > 0) ENABLE';
    END IF;
END;
/

COMMIT;

PROMPT ============================================
PROMPT MIGRACION COMPLETADA
PROMPT ============================================
