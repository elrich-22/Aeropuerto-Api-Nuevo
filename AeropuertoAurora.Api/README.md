# Aeropuerto Aurora API

API ASP.NET Core para consultar la base Oracle del proyecto Aeropuerto Aurora.

## Configuracion

La cadena de conexion no debe quemarse en codigo. En desarrollo puedes usar `appsettings.Development.json` o variables de entorno:

```powershell
$env:Database__ConnectionString="User Id=AEROPUERTO_AURORA;Password=1234;Data Source=localhost:1521/ORCLPDB"
$env:ApiSecurity__ApiKey="clave-local-segura"
dotnet run
```

Si `ApiSecurity:ApiKey` esta configurada, el frontend debe enviar:

```http
X-Api-Key: clave-local-segura
```

## Base de datos

Los scripts de Oracle quedaron versionados en [Database](./Database):

- `Database/aeropuerto_aurora_v2.sql`: crea la estructura completa de la base de datos.
- `Database/aeropuerto_aurora_seed.sql`: carga data de prueba con IDs fijos.
- `Database/reset-identities.sql`: resincroniza las columnas identity despues del seed.

Orden recomendado de ejecucion:

```sql
@Database/aeropuerto_aurora_v2.sql
@Database/aeropuerto_aurora_seed.sql
@Database/reset-identities.sql
```

## Despues de ejecutar el seed

El seed inserta IDs fijos. Si luego haces `POST` desde Swagger y Oracle devuelve `ORA-00001` sobre una PK, ejecuta:

```sql
@Database/reset-identities.sql
```

Esto avanza las columnas identity al siguiente valor disponible.

## Endpoints iniciales

- Swagger UI: `GET /swagger`
- OpenAPI JSON: `GET /swagger/v1/swagger.json`
- `GET /api/health`
- `GET /api/tablas`
- `GET /api/tablas/{tabla}?limit=100`
- `GET /api/aeropuertos`
- `GET /api/aeropuertos/{id}`
- `GET /api/aeropuertos/terminales?aeropuertoId=1`
- `GET /api/aeropuertos/puertas?terminalId=1`
- `GET /api/aerolineas`
- `GET /api/aviones`
- `GET /api/asientos-avion`
- `GET /api/programas-vuelo`
- `GET /api/dias-vuelo`
- `GET /api/escalas-tecnicas`
- `GET /api/metodos-pago`
- `GET /api/promociones`
- `GET /api/vuelos?fecha=2026-04-24`
- `GET /api/vuelos/{id}`
- `GET /api/vuelos/programas`
- `GET /api/pasajeros`
- `GET /api/pasajeros/{id}/reservas`
- `GET /api/reservas?pasajeroId=1`
- `GET /api/empleados`
- `GET /api/operaciones/equipaje?vueloId=1`
- `GET /api/operaciones/mantenimientos`
- `GET /api/operaciones/controles-seguridad`
- `GET /api/operaciones/incidentes`

Todos los listados aceptan `limit`, por ejemplo `GET /api/vuelos?limit=50`.

## Consulta de cualquier tabla

Para listar todas las tablas disponibles:

```http
GET /api/tablas
```

Para consultar filas de cualquier tabla habilitada:

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

Este endpoint usa lista blanca interna, por lo que no ejecuta nombres de tabla arbitrarios enviados por el cliente.
