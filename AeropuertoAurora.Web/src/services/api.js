const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '';
const API_KEY = import.meta.env.VITE_API_KEY || '';

async function request(path, options = {}) {
  const headers = {
    Accept: 'application/json',
    ...(options.body ? { 'Content-Type': 'application/json' } : {}),
    ...(API_KEY ? { 'X-Api-Key': API_KEY } : {}),
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
      message = body.message || body.title || message;
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
  health: () => request('/api/health'),
  airports: (limit = 100) => request(`/api/aeropuertos?limit=${limit}`),
  flights: (limit = 8) => request(`/api/vuelos?limit=${limit}`),
  topDestinations: (limit = 5) => request(`/api/reportes/destinos-mas-buscados?limit=${limit}`),
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
  incidents: (limit = 6) => request(`/api/operaciones/incidentes?limit=${limit}`),
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
