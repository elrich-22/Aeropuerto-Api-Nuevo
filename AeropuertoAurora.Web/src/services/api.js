const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5185';
const API_KEY = import.meta.env.VITE_API_KEY || '';

async function request(path, options = {}) {
  const headers = {
    Accept: 'application/json',
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
  baseUrl: API_BASE_URL,
  health: () => request('/api/health'),
  flights: (limit = 8) => request(`/api/vuelos?limit=${limit}`),
  topDestinations: (limit = 5) => request(`/api/reportes/destinos-mas-buscados?limit=${limit}`),
  occupancy: (limit = 6) => request(`/api/reportes/ocupacion-vuelos?limit=${limit}`),
  incidentsBySeverity: () => request('/api/reportes/incidentes-por-severidad'),
  baggage: (limit = 6) => request(`/api/operaciones/equipaje?limit=${limit}`),
  incidents: (limit = 6) => request(`/api/operaciones/incidentes?limit=${limit}`)
};
