-- Migracion: agrega columna de rol a AER_USUARIO_LOGIN
-- Ejecutar UNA SOLA VEZ sobre la BD que ya tiene el esquema v2 aplicado.

ALTER TABLE AER_USUARIO_LOGIN
ADD (USL_ROL VARCHAR2(20) DEFAULT 'PASAJERO' NOT NULL);

-- Marca los usuarios cuyo nombre de usuario contiene 'admin' como ADMIN
UPDATE AER_USUARIO_LOGIN
SET USL_ROL = 'ADMIN'
WHERE UPPER(USL_USUARIO) LIKE '%ADMIN%';

COMMIT;
