# Aeropuerto Aurora Web

Front en React y Bootstrap para consumir el API del proyecto.

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
npm install
npm run dev
```

En desarrollo, Vite usa proxy para enviar `/api` a `http://localhost:5185`. Por eso el backend debe estar levantado para que se pinten los datos.

Si el API usa otra URL o requiere API key, crea un archivo `.env` basado en `.env.example`.

## Compilar

```powershell
npm run build
```
