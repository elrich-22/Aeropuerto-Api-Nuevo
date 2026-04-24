# Aeropuerto Aurora

Proyecto integrado de Bases de Datos II para la gestion de operaciones del Aeropuerto Aurora.

Este repositorio incluye:

- `AeropuertoAurora.Api/`: API en ASP.NET Core conectada a Oracle.
- `AeropuertoAurora.Web/`: frontend en React + Vite para consumir la API.
- `AeropuertoAurora.Api/Database/`: scripts de estructura, seed y ajuste de identities.

## Estructura

```text
Proyecto2/
|-- AeropuertoAurora.Api/
|-- AeropuertoAurora.Web/
|-- Proyecto2.sln
```

## Requisitos

- .NET SDK 10
- Node.js + npm
- Oracle 21c local

## Base de datos

Los scripts principales estan en `AeropuertoAurora.Api/Database/`.

Orden recomendado de ejecucion:

```sql
@AeropuertoAurora.Api/Database/aeropuerto_aurora_v2.sql
@AeropuertoAurora.Api/Database/aeropuerto_aurora_seed.sql
@AeropuertoAurora.Api/Database/reset-identities.sql
```

Si despues del seed Oracle devuelve `ORA-00001` en inserts nuevos, vuelve a ejecutar:

```sql
@AeropuertoAurora.Api/Database/reset-identities.sql
```

## Backend

Para ejecutar la API:

```powershell
$env:Database__ConnectionString="User Id=AEROPUERTO_AURORA;Password=1234;Data Source=localhost:1521/ORCLPDB"
$env:ApiSecurity__ApiKey="clave-local-segura"
dotnet run --project AeropuertoAurora.Api
```

La API expone Swagger en:

- `http://localhost:5185/swagger`

Si `ApiSecurity:ApiKey` esta configurada, el cliente debe enviar:

```http
X-Api-Key: clave-local-segura
```

## Frontend

Para ejecutar el frontend:

```powershell
cd AeropuertoAurora.Web
npm install
npm run dev
```

Por defecto consume `http://localhost:5185`. Si necesitas otra URL o API key, crea un `.env` basado en `AeropuertoAurora.Web/.env.example`.

Para compilar:

```powershell
cd AeropuertoAurora.Web
npm run build
```

## Modulos destacados

- Operaciones de vuelos, pasajeros, equipaje, mantenimiento y seguridad.
- Consulta generica de tablas habilitadas desde la API.
- Reporteria para ventas, destinos, incidentes, ocupacion de vuelos y metodos de pago.
- Frontend con panel visual conectado a endpoints operativos y de reporteria.

## Documentacion por modulo

- API: [AeropuertoAurora.Api/README.md](AeropuertoAurora.Api/README.md)
- Frontend: [AeropuertoAurora.Web/README.md](AeropuertoAurora.Web/README.md)
