# Aeropuerto Aurora API

API ASP.NET Core para consultar y administrar la base Oracle del proyecto Aeropuerto Aurora. Sirve al frontend web y a la app Android, con autenticacion JWT y roles.

## Stack

- ASP.NET Core sobre `.NET 10`.
- `Oracle.ManagedDataAccess.Core` para conexion con Oracle.
- Swagger/OpenAPI con `Swashbuckle.AspNetCore`.
- JWT con `Microsoft.AspNetCore.Authentication.JwtBearer`.
- Rate limiting integrado de ASP.NET Core (`AddRateLimiter`).
- Scripts SQL versionados en `Database/`.

## Configuracion

La cadena de conexion no debe quemarse en codigo. En desarrollo puedes usar `appsettings.Development.json` o variables de entorno:

```powershell
$env:Database__ConnectionString="User Id=AEROPUERTO_AURORA;Password=1234;Data Source=localhost:1521/ORCLPDB"
$env:ApiSecurity__ApiKey="clave-local-segura"
$env:Jwt__Secret="cadena-larga-secreta-min-32-chars"
$env:Jwt__Issuer="aeropuerto-aurora"
$env:Jwt__Audience="aeropuerto-aurora-clients"
dotnet run --urls http://localhost:5185
```

### API key

Si `ApiSecurity:Enabled` esta en `true`, los clientes deben enviar:

```http
X-Api-Key: clave-local-segura
```

En el archivo de desarrollo actual `ApiSecurity:Enabled` viene en `false`, por lo que no se exige ese header localmente.

### JWT

Si la configuracion `Jwt:Secret` esta presente, la API activa autenticacion JWT. El token se obtiene desde `POST /api/auth/login` y se envia en cada request:

```http
Authorization: Bearer eyJhbGciOi...
```

El token incluye claims: `sub`, `email`, `usuario`, `pasajeroId`, `rol` y `jti`.

## Correo SMTP

El modulo de compras puede enviar confirmaciones de reserva por correo. La configuracion vive en la seccion `Email`, pero las credenciales no deben versionarse.

Ejemplo local con variables de entorno:

```powershell
$env:Email__Enabled="true"
$env:Email__Host="smtp.gmail.com"
$env:Email__Port="587"
$env:Email__UseSsl="true"
$env:Email__User="tu-correo@gmail.com"
$env:Email__Password="tu-app-password"
$env:Email__FromName="Aeropuerto Aurora"
$env:Email__FromEmail="tu-correo@gmail.com"
```

Si `Email:Enabled` esta en `false` o falta algun dato SMTP, la compra se confirma igual y el envio de correo se omite.

## Base de datos

Los scripts de Oracle estan en [Database](./Database):

- `Database/aeropuerto_aurora_v2.sql`: estructura completa de la base de datos.
- `Database/aeropuerto_aurora_add_rol.sql`: agrega columna `USL_ROL` y constraint para roles.
- `Database/alter_add_documento_pasajero.sql`: agrega columnas de documento al pasajero.
- `Database/alter_add_venta_cantidad_boletos.sql`: agrega cantidad de boletos en `AER_VENTABOLETO`.
- `Database/alter_detalle_boleto_pasajero_snapshot.sql`: snapshot del pasajero en detalle de boleto.
- `Database/aeropuerto_aurora_seed_presentacion.sql`: seed con ~18,000 vuelos, 120 pasajeros y 150 reservas.
- `Database/aeropuerto_aurora_reset_total.sql`: limpia tablas y reinicia identities.

Credenciales demo del seed de presentacion:

- `admin.aurora` / `AdminAurora1!` (rol ADMIN)
- `soporte.operaciones` / `SoporteAurora1!` (rol ADMIN)
- `auditoria.seguridad` / `SoporteAurora1!` (rol ADMIN)
- Usuarios pasajero genericos / `AuroraDemo1!`
- Usuarios inactivos o bloqueados / `DemoInactivo1!`

Orden recomendado de ejecucion:

```sql
@Database/aeropuerto_aurora_v2.sql
@Database/aeropuerto_aurora_add_rol.sql
@Database/alter_add_documento_pasajero.sql
@Database/alter_add_venta_cantidad_boletos.sql
@Database/alter_detalle_boleto_pasajero_snapshot.sql
@Database/aeropuerto_aurora_seed_presentacion.sql
```

## Ejecutar

Desde la raiz del repo:

```powershell
dotnet run --project AeropuertoAurora.Api --urls http://localhost:5185
```

O desde esta carpeta:

```powershell
dotnet run --urls http://localhost:5185
```

URLs utiles:

- Swagger UI: `http://localhost:5185/swagger`
- OpenAPI JSON: `http://localhost:5185/swagger/v1/swagger.json`
- Health check: `http://localhost:5185/api/health`

## Seguridad

- **JWT**: HMAC-SHA256 con `sub`, `email`, `usuario`, `pasajeroId`, `rol`, `jti`.
- **Contrasenas**: PBKDF2 con SHA-256, 100,000 iteraciones y sal aleatoria por usuario.
- **Bloqueo**: 5 intentos fallidos consecutivos bloquean la cuenta por 30 minutos.
- **Rate limiting**: `POST /api/auth/login` limitado a 5 requests/min por IP.
- **Roles**: `ADMIN` y `PASAJERO`. Endpoints administrativos usan `[Authorize(Roles = "ADMIN")]`.
- **HSTS y HTTPS**: `UseHttpsRedirection` y `UseHsts` activos cuando el entorno no es Development.
- **Validacion de DTOs**: longitudes maximas, regex de email y filtrado de caracteres peligrosos.
- **CORS**: politica `Frontend` configurable desde `appsettings`.

## Endpoints principales

Autenticacion:

- `POST /api/auth/login` - devuelve JWT
- `POST /api/auth/register`
- `GET /api/auth/perfil` - requiere JWT

Vuelos y compra:

- `GET /api/vuelos?fecha=2026-04-24&limit=50`
- `GET /api/vuelos?origen=La%20Aurora&destino=Miami&limit=500`
- `GET /api/vuelos/{id}`
- `GET /api/vuelos/programas`
- `PUT /api/vuelos/{id}` - solo rol ADMIN
- `GET /api/aeropuertos`
- `GET /api/aerolineas`
- `GET /api/metodos-pago`
- `POST /api/compras/vuelos`

Carrito:

- `GET /api/carritos-compra/pasajero/{pasajeroId}/items`
- `POST /api/carritos-compra/pasajero/{pasajeroId}/items`
- `DELETE /api/carritos-compra/pasajero/{pasajeroId}/items/{itemId}`

Pasajeros y reservas:

- `GET /api/pasajeros`
- `GET /api/pasajeros/{id}/reservas`
- `GET /api/reservas?pasajeroId=1`
- `GET /api/tarjetas-embarque`
- `GET /api/checkin`

Operaciones:

- `GET /api/operaciones/equipaje?vueloId=1`
- `GET /api/operaciones/mantenimientos`
- `GET /api/operaciones/controles-seguridad`
- `GET /api/operaciones/incidentes`

Reporteria:

- `GET /api/reportes/ventas-por-fecha?desde=2026-04-01&hasta=2026-04-30`
- `GET /api/reportes/destinos-mas-buscados?limit=10`
- `GET /api/reportes/incidentes-por-severidad`
- `GET /api/reportes/ocupacion-vuelos?limit=20`
- `GET /api/reportes/metodos-pago`

Todos los listados aceptan `limit` cuando el controlador lo soporta, por ejemplo `GET /api/vuelos?limit=50`.

## CRUD administrativo

Los controladores CRUD requieren rol `ADMIN`. La mayoria exponen:

- `GET /api/{recurso}`
- `GET /api/{recurso}/{id}`
- `POST /api/{recurso}`
- `PUT /api/{recurso}/{id}`
- `DELETE /api/{recurso}/{id}`

Algunos recursos disponibles:

- `aeropuertos`, `terminales`, `puertas-embarque`
- `aerolineas`, `aviones`, `modelos-avion`, `asientos-avion`
- `programas-vuelo`, `dias-vuelo`, `escalas-tecnicas`
- `empleados`, `departamentos`, `puestos`, `asistencias`, `planillas`
- `equipajes`, `movimientos-equipaje`, `incidentes`, `objetos-perdidos`
- `mantenimientos-avion`, `hangares`, `repuestos`, `proveedores`
- `promociones`, `ventas-boleto`, `detalles-venta-boleto`, `transacciones-pago`
- `usuarios-login`, `sesiones-usuario`, `auditoria`

## Consulta de cualquier tabla habilitada

Para listar tablas disponibles:

```http
GET /api/tablas
```

Para consultar filas:

```http
GET /api/tablas/aer_aeropuerto?limit=50
GET /api/tablas/aer_empleado?limit=50
GET /api/tablas/aer_reserva?limit=50
```

Tambien puedes usar el alias sin prefijo `AER_`:

```http
GET /api/tablas/aeropuerto
GET /api/tablas/empleado
GET /api/tablas/reserva
```

El panel administrativo usa estos endpoints:

- `GET /api/tablas/{tabla}/metadata`
- `POST /api/tablas/{tabla}`
- `PUT /api/tablas/{tabla}/{id}`
- `DELETE /api/tablas/{tabla}/{id}`

Este modulo usa una lista blanca interna, por lo que no ejecuta nombres de tabla arbitrarios enviados por el cliente.
