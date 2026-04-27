param(
    [string]$ApiUrl = "http://localhost:5185",
    [string]$WebHost = "127.0.0.1",
    [int]$WebPort = 5173
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$apiProject = Join-Path $root "AeropuertoAurora.Api\AeropuertoAurora.Api.csproj"
$webProject = Join-Path $root "AeropuertoAurora.Web"

if (-not (Test-Path $apiProject)) {
    throw "No se encontro el proyecto API en: $apiProject"
}

if (-not (Test-Path $webProject)) {
    throw "No se encontro el proyecto Web en: $webProject"
}

if (-not (Test-Path (Join-Path $webProject "node_modules"))) {
    Write-Host "Instalando dependencias del frontend..." -ForegroundColor Cyan
    Push-Location $webProject
    try {
        npm install
    }
    finally {
        Pop-Location
    }
}

$apiLog = Join-Path $root "dev-api.log"
$apiErrorLog = Join-Path $root "dev-api-error.log"
$webLog = Join-Path $root "dev-web.log"
$webErrorLog = Join-Path $root "dev-web-error.log"

function Stop-ProcessOnPort {
    param([int]$Port)

    $connections = netstat -ano | Select-String ":$Port\s+.*LISTENING"
    foreach ($connection in $connections) {
        $parts = ($connection.Line -split "\s+") | Where-Object { $_ }
        $processId = [int]$parts[-1]

        if ($processId -gt 0) {
            $process = Get-Process -Id $processId -ErrorAction SilentlyContinue
            if ($process) {
                Write-Host "Deteniendo proceso existente en puerto $Port (PID $processId)..." -ForegroundColor Yellow
                Stop-Process -Id $processId -Force
            }
        }
    }
}

Stop-ProcessOnPort -Port 5185
Stop-ProcessOnPort -Port $WebPort

Write-Host "Levantando backend en $ApiUrl" -ForegroundColor Cyan
$apiProcess = Start-Process `
    -FilePath "dotnet" `
    -ArgumentList "run --project `"$apiProject`" --urls $ApiUrl" `
    -WorkingDirectory $root `
    -RedirectStandardOutput $apiLog `
    -RedirectStandardError $apiErrorLog `
    -PassThru `
    -WindowStyle Hidden

Write-Host "Esperando a que el backend responda..." -ForegroundColor Cyan
$healthUrl = "$ApiUrl/api/health"
$apiReady = $false

for ($attempt = 1; $attempt -le 45; $attempt++) {
    $apiProcess.Refresh()

    if ($apiProcess.HasExited) {
        throw "El backend se detuvo antes de responder. Revisa $apiLog y $apiErrorLog"
    }

    try {
        $response = Invoke-WebRequest -UseBasicParsing -Uri $healthUrl -TimeoutSec 2
        if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
            $apiReady = $true
            break
        }
    }
    catch {
        Start-Sleep -Seconds 1
    }
}

if (-not $apiReady) {
    throw "El backend no respondio en $healthUrl. Revisa $apiLog y $apiErrorLog"
}

Write-Host "Levantando frontend en http://${WebHost}:$WebPort" -ForegroundColor Cyan
$webProcess = Start-Process `
    -FilePath "npm.cmd" `
    -ArgumentList "run dev -- --host $WebHost --port $WebPort" `
    -WorkingDirectory $webProject `
    -RedirectStandardOutput $webLog `
    -RedirectStandardError $webErrorLog `
    -PassThru `
    -WindowStyle Hidden

Write-Host ""
Write-Host "Backend:  $ApiUrl/swagger" -ForegroundColor Green
Write-Host "Frontend: http://${WebHost}:$WebPort" -ForegroundColor Green
Write-Host ""
Write-Host "Logs:" -ForegroundColor Yellow
Write-Host "  API: $apiLog"
Write-Host "  API errores: $apiErrorLog"
Write-Host "  Web: $webLog"
Write-Host "  Web errores: $webErrorLog"
Write-Host ""
Write-Host "Presiona Ctrl+C para detener ambos procesos." -ForegroundColor Yellow

try {
    while (-not $apiProcess.HasExited -and -not $webProcess.HasExited) {
        Start-Sleep -Seconds 1
        $apiProcess.Refresh()
        $webProcess.Refresh()
    }
}
finally {
    foreach ($process in @($apiProcess, $webProcess)) {
        if ($process -and -not $process.HasExited) {
            Stop-Process -Id $process.Id -Force
        }
    }

    Write-Host "Procesos detenidos." -ForegroundColor Yellow
}
