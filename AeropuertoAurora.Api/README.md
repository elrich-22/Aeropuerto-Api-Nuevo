# Aeropuerto Aurora API

API ASP.NET Core para consultar y administrar la base Oracle del proyecto Aeropuerto Aurora. Sirve al frontend web y a la app Android.

## Stack

- ASP.NET Core sobre `.NET 10`.
- `Oracle.ManagedDataAccess.Core` para conexion con Oracle.
- Swagger/OpenAPI con `Swashbuckle.AspNetCore`.
- Scripts SQL versionados en `Database/`.

## Configuracion

La cadena de conexion no debe quemarse en codigo. En desarrollo puedes usar `appsettings.Development.json` o variables de entorno:

```powershell
$env:Database__ConnectionString="User Id=AEROPUERTO_AURORA;Password=1234;Data Source=localhost:1521/ORCLPDB"
$env:ApiSecurity__ApiKey="clave-local-segura"
dotnet run --urls http://localhost:5185
```

Si `ApiSecurity:ApiKey` esta configurada, los clientes deben enviar:

```http
X-Api-Key: clave-local-segura
```

En el archivo de desarrollo actual la API key esta vacia, por lo que no se exige ese header localmente.

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

- `Database/aeropuerto_aurora_v2.sql`: crea la estructura completa de la base de datos.
- `Database/aeropuerto_aurora_seed_maestra.sql`: seed principal de demo/preproduccion con expansion global y datos masivos.

Credenciales demo utiles del seed maestra:

- `admin.aurora` / `AdminAurora1!`
- `soporte.operaciones` / `SoporteAurora1!`
- `auditoria.seguridad` / `SoporteAurora1!`
- Usuarios demo activos genericos / `AuroraDemo1!`
- Usuarios demo inactivos o bloqueados / `DemoInactivo1!`

Orden recomendado de ejecucion:

```sql
@Database/aeropuerto_aurora_v2.sql
@Database/aeropuerto_aurora_seed_maestra.sql
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

## Endpoints principales

Autenticacion:

- `POST /api/auth/login`
- `POST /api/auth/register`

Vuelos y compra:

- `GET /api/vuelos?fecha=2026-04-24&limit=50`
- `GET /api/vuelos/{id}`
- `GET /api/vuelos/programas`
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

La API incluye controladores CRUD para catalogos y tablas operativas. La mayoria exponen:

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
