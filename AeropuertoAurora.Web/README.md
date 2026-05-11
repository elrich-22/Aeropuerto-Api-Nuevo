# Aeropuerto Aurora Web

Frontend en React, Vite y Bootstrap para consumir la API del proyecto. Incluye experiencia de pasajero y panel administrativo.

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

## Funcionalidades

- Inicio con metricas generales del aeropuerto.
- Busqueda de vuelos por origen, destino, fecha, tipo de viaje y pasajeros.
- Seleccion de tarifa, mejora de clase y resumen de itinerario.
- Login y registro de pasajeros.
- Carrito de compras sincronizado con la API por pasajero.
- Checkout con metodo de pago y confirmacion de compra.
- Pantalla de pago exitoso con codigo de reserva.
- Rastreo de vuelos.
- Destinos mas buscados.
- Operaciones de equipaje, incidentes y mantenimientos.
- Reporteria administrativa.
- Panel administrativo con CRUD generico sobre tablas habilitadas.

## Rutas API usadas con mas frecuencia

- `GET /api/health`
- `POST /api/auth/login`
- `POST /api/auth/register`
- `GET /api/vuelos`
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

El enlace administrativo aparece para usuarios cuyo perfil coincida con administrador. Desde ahi se puede:

- Cargar la lista de tablas habilitadas.
- Consultar metadata de columnas.
- Crear, editar y eliminar registros.
- Ver reportes de ventas, destinos, incidentes, ocupacion y metodos de pago.

## Archivos principales

- `src/App.jsx`: componentes, estado de la aplicacion y flujos principales.
- `src/services/api.js`: cliente HTTP para la API.
- `src/styles.css`: estilos del sitio, panel, carrito y checkout.
- `.env.example`: variables disponibles para configurar la API.
