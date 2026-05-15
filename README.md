# Aeropuerto Aurora

Proyecto integrado de Bases de Datos II para gestionar operaciones, venta de boletos y consulta administrativa del Aeropuerto Aurora.

El repositorio contiene tres clientes alrededor de la misma API:

- `AeropuertoAurora.Api/`: API ASP.NET Core conectada a Oracle, con autenticacion JWT y roles.
- `AeropuertoAurora.Web/`: frontend React + Vite para pasajeros y administradores.
- `AeropuertoAurora.Android/`: app Android nativa en Java.
- `AeropuertoAurora.Api/Database/`: scripts de estructura, seed y alteraciones.
- `start-dev.ps1`: script para levantar backend y frontend juntos.

## Estructura

```text
Aeropuerto-Api-Nuevo/
|-- AeropuertoAurora.Api/
|-- AeropuertoAurora.Web/
|-- AeropuertoAurora.Android/
|-- start-dev.ps1
|-- Proyecto2.sln
|-- README.md
```

## Requisitos

- .NET SDK 10
- Node.js + npm
- Oracle 21c local
- Android Studio, solo si vas a correr la app Android

## Base de datos

Los scripts principales estan en `AeropuertoAurora.Api/Database/`.

Orden recomendado de ejecucion:

```sql
@AeropuertoAurora.Api/Database/aeropuerto_aurora_v2.sql
@AeropuertoAurora.Api/Database/aeropuerto_aurora_add_rol.sql
@AeropuertoAurora.Api/Database/alter_add_documento_pasajero.sql
@AeropuertoAurora.Api/Database/alter_add_venta_cantidad_boletos.sql
@AeropuertoAurora.Api/Database/alter_detalle_boleto_pasajero_snapshot.sql
@AeropuertoAurora.Api/Database/aeropuerto_aurora_seed_presentacion.sql
```

El seed de presentacion incluye:

- 37 aeropuertos en 5 continentes, 20 aerolineas, 34 aeronaves.
- 120 pasajeros y 103 usuarios login (80 activos, 10 inactivos, 10 bloqueados, 3 admin).
- 90+ rutas: nacionales, regionales, internacionales e intercontinentales.
- ~18,000 vuelos generados de 2026-05-18 a 2026-12-31.
- 150 reservaciones pre-vuelo con tickets, detalles y transacciones de pago.

Si necesitas reiniciar la base completa:

```sql
@AeropuertoAurora.Api/Database/aeropuerto_aurora_reset_total.sql
```

## Ejecutar API y Web

El flujo recomendado para desarrollo es levantar backend y frontend con un solo comando desde la raiz:

```powershell
.\start-dev.ps1
```

El script hace lo siguiente:

- Detiene procesos previos en los puertos `5185` y `5173`.
- Levanta la API en `http://localhost:5185`.
- Espera a que `http://localhost:5185/api/health` responda.
- Instala dependencias del frontend si falta `node_modules`.
- Levanta el frontend en `http://127.0.0.1:5173`.
- Escribe logs en `dev-api.log`, `dev-api-error.log`, `dev-web.log` y `dev-web-error.log`.

URLs principales:

- Frontend: `http://127.0.0.1:5173`
- Swagger: `http://localhost:5185/swagger`
- Health check: `http://localhost:5185/api/health`

Para detener ambos procesos, presiona `Ctrl+C` en la terminal donde ejecutaste el script.

## Backend manual

Si necesitas ejecutar solo la API:

```powershell
$env:Database__ConnectionString="User Id=AEROPUERTO_AURORA;Password=1234;Data Source=localhost:1521/ORCLPDB"
$env:ApiSecurity__ApiKey="clave-local-segura"
$env:Jwt__Secret="cadena-larga-secreta-min-32-chars"
dotnet run --project AeropuertoAurora.Api --urls http://localhost:5185
```

La API expone Swagger en:

- `http://localhost:5185/swagger`

Si `ApiSecurity:Enabled` esta en `true`, los clientes deben enviar:

```http
X-Api-Key: clave-local-segura
```

En desarrollo `ApiSecurity:Enabled` viene en `false`, asi que no se exige el header localmente.

Para probar correos de confirmacion de compra, configura `Email__Enabled`, `Email__Host`, `Email__User`, `Email__Password` y `Email__FromEmail` como variables de entorno. Las credenciales SMTP no deben subirse al repo.

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

## Android

Abre `AeropuertoAurora.Android/` desde Android Studio y ejecuta la app en un emulador o telefono.

URLs recomendadas para conectar con la API:

- Emulador Android: `http://10.0.2.2:5185`
- Telefono fisico: `http://IP_LOCAL_DE_TU_PC:5185`

El telefono y la PC deben estar en la misma red. Si el firewall bloquea el puerto `5185`, permite la conexion entrante.

## Seguridad

- Autenticacion con JWT (HMAC-SHA256). El token viaja en `Authorization: Bearer ...` y contiene `sub`, `email`, `usuario`, `pasajeroId`, `rol` y `jti`.
- Contrasenas con PBKDF2 (SHA-256, 100k iteraciones) y sal por usuario.
- Bloqueo automatico tras 5 intentos fallidos (30 minutos).
- Rate limiting en `POST /api/auth/login` (5 req/min por IP).
- Roles `ADMIN` y `PASAJERO` en la tabla `AER_USUARIO_LOGIN`. Endpoints administrativos requieren rol `ADMIN`.
- HSTS y `UseHttpsRedirection` activos en produccion.
- API key opcional via header `X-Api-Key` controlado por `ApiSecurity:Enabled`.
- DTOs validados con `StringLength`, formato de email y caracteres permitidos.

## Modulos destacados

- Autenticacion JWT con `POST /api/auth/login` (devuelve token) y `POST /api/auth/register`.
- Busqueda de vuelos por origen, destino y fecha, con calendario de tarifas por ruta.
- Seleccion de tarifa y flujo de compra integrado.
- Carrito compartido por pasajero y checkout con metodo de pago.
- Confirmacion de compras con creacion de reserva, venta, detalle y transaccion de pago.
- Rastreo de vuelos, destinos mas buscados y reportes de ocupacion.
- Operaciones de equipaje, mantenimientos, controles e incidentes.
- Panel administrativo (solo ADMIN) con CRUD generico sobre tablas habilitadas.
- Clientes Web y Android consumiendo la misma API con autenticacion JWT.

## Documentacion por modulo

- API: [AeropuertoAurora.Api/README.md](AeropuertoAurora.Api/README.md)
- Frontend: [AeropuertoAurora.Web/README.md](AeropuertoAurora.Web/README.md)
- Android: [AeropuertoAurora.Android/README.md](AeropuertoAurora.Android/README.md)
