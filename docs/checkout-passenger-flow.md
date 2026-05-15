# Flujo de Compra de Boletos

## Objetivo

Este cambio reorganiza el checkout para que funcione mas parecido a una pagina real de compra de boletos y proteja mejor la privacidad:

- separa `comprador` de `pasajeros`
- usa `tipoDocumento + numeroDocumento` como identidad principal del viajero
- no expone datos de otros pasajeros en pantalla
- obliga a que cada boleto tenga su propio documento
- evita mezclar nombres distintos con el mismo documento

## Reglas funcionales

### 1. Comprador

- El comprador es el usuario autenticado que paga la compra.
- La venta queda asociada al pasajero de la cuenta (`VEN_ID_PASAJERO`).
- El email de confirmacion se toma del bloque de comprador.

### 2. Pasajeros

- Cada boleto representa un pasajero.
- Cada pasajero debe tener:
  - tipo de documento
  - numero de documento
  - primer nombre
  - primer apellido
- Si el documento es `PASAPORTE`, tambien debe tener:
  - pais emisor
  - fecha de vencimiento

### 3. Identidad del pasajero

- La identidad principal se define por `tipoDocumento + numeroDocumento`.
- El checkout no muestra datos previos de un pasajero solo por escribir un documento.
- El usuario siempre completa manualmente los datos visibles del boleto.
- La API valida internamente si el documento ya existe y decide si:
  - reutiliza el pasajero existente
  - crea un pasajero nuevo
  - rechaza la compra por inconsistencia de identidad

### 4. Proteccion contra inconsistencias

- Si llega el mismo `tipoDocumento + numeroDocumento` con un nombre distinto al ya registrado, la API rechaza la compra.
- Esto evita sobrescribir accidentalmente la identidad de otro pasajero.
- Tambien evita que el front revele informacion confidencial de terceros.

### 5. Comprador distinto del viajero

- El pasajero principal puede ser distinto al comprador.
- La reserva (`AER_RESERVA`) y el detalle del boleto (`AER_DETALLEVENTABOLETO`) se guardan a nombre del pasajero real.
- La venta (`AER_VENTABOLETO`) sigue quedando asociada al comprador.

## Experiencia esperada en checkout

1. El usuario autenticado entra al checkout.
2. Ve primero el bloque `Comprador y confirmacion`.
3. Luego, para cada pasajero:
   - selecciona `tipo de documento`
   - escribe `numero de documento`
4. Completa manualmente los datos visibles del boleto tal como aparecen en la identificacion.
5. El sistema valida internamente el documento sin exponer datos de otros pasajeros.
6. El pago y la emision de boletos continúan normalmente.

## Archivos principales modificados

- `AeropuertoAurora.Api/Controllers/ComprasController.cs`
- `AeropuertoAurora.Web/src/App.jsx`
- `AeropuertoAurora.Api/DTOs/AuthDtos.cs`

## Validaciones importantes

- vuelo debe estar `PROGRAMADO`
- plazas disponibles suficientes
- datos obligatorios de pasajero completos
  - tipo de documento
  - numero de documento
  - primer nombre
  - primer apellido
- si es pasaporte:
  - formato valido
  - pais emisor obligatorio
  - vencimiento obligatorio
  - no vencido
  - no puede vencer antes del vuelo

## Pruebas recomendadas

### Caso 1. Comprador compra para si mismo

- usar el documento del usuario autenticado
- verificar que autocompleta datos si ya existe
- confirmar compra

### Caso 2. Comprador compra para otra persona existente

- usar documento de otro pasajero ya registrado
- completar manualmente los datos del boleto
- confirmar compra

### Caso 3. Comprador compra para persona nueva

- usar documento nuevo
- completar datos manualmente
- confirmar compra

### Caso 4. Documento existente con nombre distinto

- usar documento existente
- cambiar nombres
- verificar que la API rechaza la compra

### Caso 5. Compra multiple

- comprar 2 o mas boletos
- verificar que cada pasajero tenga documento propio
- verificar que se creen reservas separadas
