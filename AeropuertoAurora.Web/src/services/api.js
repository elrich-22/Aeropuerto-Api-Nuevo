const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';
const API_KEY = import.meta.env.VITE_API_KEY || '';
const SESSION_KEY = 'aeropuertoAurora.user';

function currentUserHeader() {
  try {
    const stored = window.localStorage.getItem(SESSION_KEY);
    if (!stored) return {};

    const user = JSON.parse(stored);
    const value = user?.usuario || user?.email || user?.nombreCompleto;
    return value ? { 'X-User': value } : {};
  } catch {
    return {};
  }
}

async function request(path, options = {}) {
  const headers = {
    Accept: 'application/json',
    ...(options.body ? { 'Content-Type': 'application/json' } : {}),
    ...(API_KEY ? { 'X-Api-Key': API_KEY } : {}),
    ...currentUserHeader(),
    ...options.headers
  };

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers
  });

  if (!response.ok) {
    let message = `Error ${response.status}`;

    try {
      const body = await response.json();
      if (body.errors && typeof body.errors === 'object') {
        const validationMessages = Object.entries(body.errors)
          .flatMap(([field, errors]) => (Array.isArray(errors) ? errors.map((error) => `${field}: ${error}`) : []));
        message = validationMessages.length > 0 ? validationMessages.join(' ') : body.title || message;
      } else {
        message = body.message || body.title || message;
      }
    } catch {
      message = response.statusText || message;
    }

    throw new Error(message);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export const api = {
  baseUrl: API_BASE_URL || 'Proxy local /api',
  login: (credentials) =>
    request('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials)
    }),
  register: (payload) =>
    request('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify(payload)
    }),
  buyFlight: (payload) =>
    request('/api/compras/vuelos', {
      method: 'POST',
      body: JSON.stringify(payload)
    }),
  sendPurchaseConfirmation: (payload) =>
    request('/api/compras/confirmacion-correo', {
      method: 'POST',
      body: JSON.stringify(payload)
    }),
  health: () => request('/api/health'),
  airports: (limit = 100) => request(`/api/aeropuertos?limit=${limit}`),
  flights: (limit = 8) => request(`/api/vuelos?limit=${limit}`),
  topDestinations: (limit = 5) => request(`/api/reportes/destinos-mas-buscados?limit=${limit}`),
  salesByDate: () => request('/api/reportes/ventas-por-fecha'),
  paymentMethodsReport: () => request('/api/reportes/metodos-pago'),
  createFlightSearch: (payload) => request('/api/busquedas-vuelo', {
    method: 'POST',
    body: JSON.stringify(payload)
  }),
  createDestinationClick: (payload) => request('/api/clicks-destino', {
    method: 'POST',
    body: JSON.stringify(payload)
  }),
  occupancy: (limit = 6) => request(`/api/reportes/ocupacion-vuelos?limit=${limit}`),
  incidentsBySeverity: () => request('/api/reportes/incidentes-por-severidad'),
  baggage: (limit = 6) => request(`/api/operaciones/equipaje?limit=${limit}`),
  baggageMovements: (limit = 100) => request(`/api/movimientos-equipaje?limit=${limit}`),
  incidents: (limit = 6) => request(`/api/operaciones/incidentes?limit=${limit}`),
  maintenance: (limit = 20) => request(`/api/operaciones/mantenimientos?limit=${limit}`),
  securityControls: (limit = 20) => request(`/api/operaciones/controles-seguridad?limit=${limit}`),
  reservations: (passengerId, limit = 100) => request(`/api/reservas?pasajeroId=${passengerId}&limit=${limit}`),
  checkIns: (limit = 100) => request(`/api/checkin?limit=${limit}`),
  createCheckIn: (payload) => request('/api/checkin', {
    method: 'POST',
    body: JSON.stringify(payload)
  }),
  boardingPasses: (limit = 100) => request(`/api/tarjetas-embarque?limit=${limit}`),
  createBoardingPass: (payload) => request('/api/tarjetas-embarque', {
    method: 'POST',
    body: JSON.stringify(payload)
  }),
  promotions: (limit = 50) => request(`/api/promociones?limit=${limit}`),
  lostObjects: (limit = 50) => request(`/api/objetos-perdidos?limit=${limit}`),
  createLostObject: (payload) => request('/api/objetos-perdidos', {
    method: 'POST',
    body: JSON.stringify(payload)
  }),
  audit: (limit = 50) => request(`/api/auditoria?limit=${limit}`),
  cartItems: (passengerId) => request(`/api/carritos-compra/pasajero/${passengerId}/items`),
  addCartItem: (passengerId, payload) => request(`/api/carritos-compra/pasajero/${passengerId}/items`, {
    method: 'POST',
    body: JSON.stringify(payload)
  }),
  deleteCartItem: (passengerId, itemId) => request(`/api/carritos-compra/pasajero/${passengerId}/items/${itemId}`, {
    method: 'DELETE'
  }),
  tables: () => request('/api/tablas'),
  tableRows: (table, limit = 100) => request(`/api/tablas/${table}?limit=${limit}`),
  tableMetadata: (table) => request(`/api/tablas/${table}/metadata`),
  createRow: (table, values) => request(`/api/tablas/${table}`, {
    method: 'POST',
    body: JSON.stringify(values)
  }),
  updateRow: (table, id, values) => request(`/api/tablas/${table}/${encodeURIComponent(id)}`, {
    method: 'PUT',
    body: JSON.stringify(values)
  }),
  deleteRow: (table, id) => request(`/api/tablas/${table}/${encodeURIComponent(id)}`, {
    method: 'DELETE'
  })
};
