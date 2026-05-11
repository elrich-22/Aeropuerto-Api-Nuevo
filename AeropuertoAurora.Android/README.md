# Aeropuerto Aurora Android

App Android nativa para consumir `AeropuertoAurora.Api`. Esta hecha en Java con vistas nativas para evitar dependencias extra.

## Stack

- Java
- Android Gradle Plugin
- `compileSdk 36`
- `minSdk 23`
- Paquete: `com.aeropuertoaurora.android`

## Que trae

- Conexion configurable contra la API.
- Prueba rapida de `GET /api/health`.
- Login con `POST /api/auth/login`.
- Registro de pasajero y usuario.
- Busqueda de vuelos de ida o ida y vuelta.
- Seleccion de fechas, pasajeros, tarifa y servicios.
- Carrito conectado a la API por pasajero.
- Checkout con metodo de pago.
- Compra con `POST /api/compras/vuelos`.
- Confirmacion con numero de reserva.
- Rastreo de vuelos.
- Reportes de destinos mas buscados, ocupacion e incidentes por severidad.
- Operaciones de equipaje, incidentes y mantenimientos.
- Panel administrativo con carga de tablas, edicion y borrado de registros.

## Abrir en Android Studio

1. Abre Android Studio.
2. Selecciona `Open`.
3. Elige esta carpeta:

   ```text
   AeropuertoAurora.Android
   ```

4. Espera a que Gradle sincronice el proyecto.
5. Ejecuta la app en un emulador o telefono.

## Levantar la API

Desde la raiz del repo:

```powershell
.\start-dev.ps1
```

La API queda en:

```text
http://localhost:5185
```

## URL segun donde ejecutes la app

En el emulador Android usa:

```text
http://10.0.2.2:5185
```

En un telefono fisico usa la IP local de tu PC. Ejemplo:

```text
http://192.168.1.50:5185
```

El telefono y la PC deben estar en la misma red. Si tu firewall bloquea el puerto `5185`, permite la conexion entrante para poder consultar la API desde el telefono.

## API key

Tu `appsettings.Development.json` actualmente tiene `ApiSecurity:ApiKey` vacio, asi que puedes dejar el campo API key vacio en la app.

Si luego levantas la API con una key, escribe la misma en la pantalla `Conexion`.

## Flujo recomendado de prueba

1. Levanta la API con `.\start-dev.ps1`.
2. Abre la pantalla `Conexion`.
3. Configura `http://10.0.2.2:5185` si usas emulador.
4. Presiona `Probar` para validar `/api/health`.
5. Inicia sesion o registra un usuario.
6. Explora vuelos, agrega uno al carrito y confirma la compra.

## Donde esta el codigo

- `app/src/main/java/com/aeropuertoaurora/android/MainActivity.java`: interfaz, navegacion y acciones.
- `app/src/main/java/com/aeropuertoaurora/android/ApiClient.java`: llamadas HTTP.
- `app/src/main/java/com/aeropuertoaurora/android/SettingsStore.java`: guarda URL y API key.
- `app/src/main/AndroidManifest.xml`: permisos y configuracion de red.
- `app/src/main/res/xml/network_security_config.xml`: permite trafico HTTP local para desarrollo.

## Notas

- La app usa la misma API que el frontend web.
- El carrito compartido depende de un pasajero autenticado.
- El panel administrativo solo se muestra para usuarios administradores.
- Para probar en telefono fisico, revisa firewall y direccion IP local de la PC.
