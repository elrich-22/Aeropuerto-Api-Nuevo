# Aeropuerto Aurora Web

Frontend en React, Vite y Bootstrap para consumir la API del proyecto. Incluye experiencia de pasajero, autenticacion JWT y panel administrativo.

## Stack

- React 19
- Vite 7
- Bootstrap 5
- JavaScript moderno con modulos ES

## Ejecutar recomendado

Desde la raiz del repositorio:

```powershell
.\start-dev.ps1
```

Ese script levanta primero el backend en `http://localhost:5185`, espera a que responda `/api/health` y luego levanta este frontend en:

```text
http://127.0.0.1:5173
```

## Ejecutar solo el frontend

```powershell
cd AeropuertoAurora.Web
npm install
npm run dev
```

En desarrollo, Vite usa proxy para enviar `/api` a `http://localhost:5185`. Por eso el backend debe estar levantado para que se pinten los datos.

Si la API usa otra URL o requiere API key, crea un archivo `.env` basado en `.env.example`:

```env
VITE_API_BASE_URL=http://localhost:5185
VITE_API_KEY=
```

## Scripts

```powershell
npm run dev
npm run build
npm run preview
```

## Autenticacion

- El login (`POST /api/auth/login`) devuelve un JWT que se guarda en `localStorage` bajo `aeropuertoAurora.user`.
- Todas las requests al backend incluyen `Authorization: Bearer <token>` automaticamente (`src/services/api.js`).
- El menu administrativo solo se muestra si el usuario tiene rol `ADMIN`.
- Si el JWT expira, el cliente vuelve a redirigir al login.

## Funcionalidades

- Inicio con metricas generales del aeropuerto y top destinos.
- Busqueda de vuelos por origen, destino, fecha, tipo de viaje y pasajeros.
- Calendario de tarifas filtrado por ruta seleccionada.
- Seleccion de tarifa, mejora de clase y resumen de itinerario.
- Login y registro de pasajeros con validacion del lado cliente.
- Carrito de compras sincronizado con la API por pasajero.
- Checkout con metodo de pago y confirmacion de compra.
- Pantalla de pago exitoso con codigo de reserva.
- Rastreo de vuelos.
- Destinos mas buscados.
- Operaciones de equipaje, incidentes y mantenimientos.
- Reporteria administrativa.
- Panel administrativo con CRUD generico sobre tablas habilitadas (solo ADMIN).

## Rutas API usadas con mas frecuencia

- `GET /api/health`
- `POST /api/auth/login` (devuelve JWT)
- `POST /api/auth/register`
- `GET /api/auth/perfil`
- `GET /api/vuelos`
- `GET /api/vuelos?origen=X&destino=Y&limit=500` (calendario por ruta)
- `GET /api/aeropuertos`
- `GET /api/metodos-pago`
- `GET /api/reportes/destinos-mas-buscados`
- `GET /api/reportes/ocupacion-vuelos`
- `GET /api/reportes/incidentes-por-severidad`
- `GET /api/carritos-compra/pasajero/{pasajeroId}/items`
- `POST /api/carritos-compra/pasajero/{pasajeroId}/items`
- `DELETE /api/carritos-compra/pasajero/{pasajeroId}/items/{itemId}`
- `POST /api/compras/vuelos`
- `GET /api/tablas`
- `GET /api/tablas/{tabla}/metadata`

## Panel administrativo

El enlace administrativo aparece solo para usuarios con rol `ADMIN`. Desde ahi se puede:

- Cargar la lista de tablas habilitadas.
- Consultar metadata de columnas.
- Crear, editar y eliminar registros.
- Ver reportes de ventas, destinos, incidentes, ocupacion y metodos de pago.
- Cancelar o reprogramar vuelos desde la vista de admin de vuelos.

## Archivos principales

- `src/App.jsx`: componentes, estado de la aplicacion y flujos principales.
- `src/services/api.js`: cliente HTTP para la API. Inyecta `Authorization: Bearer` automaticamente.
- `src/styles.css`: estilos del sitio, panel, carrito y checkout.
- `.env.example`: variables disponibles para configurar la API.
