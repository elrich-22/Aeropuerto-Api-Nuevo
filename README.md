# Aeropuerto Aurora

Proyecto integrado de Bases de Datos II para la gestion de operaciones del Aeropuerto Aurora.

Este repositorio incluye:

- `AeropuertoAurora.Api/`: API en ASP.NET Core conectada a Oracle.
- `AeropuertoAurora.Web/`: frontend en React + Vite para consumir la API.
- `AeropuertoAurora.Api/Database/`: scripts de estructura, seed y ajuste de identities.
- `start-dev.ps1`: script para levantar backend y frontend juntos.

## Estructura

```text
Proyecto2/
|-- AeropuertoAurora.Api/
|-- AeropuertoAurora.Web/
|-- start-dev.ps1
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

## Ejecutar todo

El flujo recomendado para desarrollo es levantar backend y frontend con un solo comando desde la raiz:

```powershell
.\start-dev.ps1
```

El script hace lo siguiente:

- Levanta la API en `http://localhost:5185`.
- Espera a que `http://localhost:5185/api/health` responda.
- Levanta el frontend en `http://127.0.0.1:5173`.
- Instala dependencias del frontend si falta `node_modules`.

URLs principales:

- Frontend: `http://127.0.0.1:5173`
- Swagger: `http://localhost:5185/swagger`

Para detener ambos procesos, presiona `Ctrl+C` en la terminal donde ejecutaste el script.

## Backend manual

Si necesitas ejecutar solo la API:

```powershell
$env:Database__ConnectionString="User Id=AEROPUERTO_AURORA;Password=1234;Data Source=localhost:1521/ORCLPDB"
$env:ApiSecurity__ApiKey="clave-local-segura"
dotnet run --project AeropuertoAurora.Api --urls http://localhost:5185
```

La API expone Swagger en:

- `http://localhost:5185/swagger`

Si `ApiSecurity:ApiKey` esta configurada, el cliente debe enviar:

```http
X-Api-Key: clave-local-segura
```

## Frontend manual

Normalmente no necesitas estos comandos si usas `.\start-dev.ps1`. Para ejecutar solo el frontend:

```powershell
cd AeropuertoAurora.Web
npm install
npm run dev
```

En desarrollo, Vite redirige las llamadas `/api` hacia `http://localhost:5185` mediante proxy. Si necesitas otra URL o API key, crea un `.env` basado en `AeropuertoAurora.Web/.env.example`.

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
