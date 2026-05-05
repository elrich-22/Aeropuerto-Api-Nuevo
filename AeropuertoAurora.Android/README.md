# Aeropuerto Aurora Android

App Android nativa para consumir la API de `AeropuertoAurora.Api`.

## Que trae

- Conexion configurable contra la API.
- Prueba rapida de `GET /api/health`.
- Login con `POST /api/auth/login`.
- Consulta de vuelos.
- Reportes de destinos mas buscados y ocupacion.
- Operaciones de incidentes, equipaje y mantenimientos.

La app esta hecha en Java y vistas nativas para evitar dependencias extra. Es una base simple para abrir, correr y extender en Android Studio.

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

## Donde esta el codigo

- `app/src/main/java/com/aeropuertoaurora/android/MainActivity.java`: interfaz y acciones.
- `app/src/main/java/com/aeropuertoaurora/android/ApiClient.java`: llamadas HTTP.
- `app/src/main/java/com/aeropuertoaurora/android/SettingsStore.java`: guarda URL y API key.

## Siguiente mejora sugerida

Cuando esta base ya corra, lo siguiente natural es separar cada modulo en su propia pantalla: vuelos, compras, reservas, reporteria y administracion.
