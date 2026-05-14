import { useCallback, useEffect, useMemo, useState } from 'react';
import { api } from './services/api';

const services = [
  {
    code: 'DF',
    title: 'Tiendas y Duty Free',
    text: 'Compras, recuerdos, tecnología y productos de viaje antes de abordar.'
  },
  {
    code: 'RS',
    title: 'Restaurantes',
    text: 'Opciones rápidas y espacios cómodos para esperar el vuelo.'
  },
  {
    code: 'TR',
    title: 'Parqueo y Transporte',
    text: 'Acceso a taxis autorizados, parqueo y movilidad hacia la ciudad.'
  },
  {
    code: 'VIP',
    title: 'Sala VIP',
    text: 'Áreas privadas para descanso, trabajo y espera prioritaria.'
  },
  {
    code: 'WF',
    title: 'WiFi Gratuito',
    text: 'Conexión disponible para pasajeros dentro de la terminal.'
  },
  {
    code: 'MD',
    title: 'Servicio Médico',
    text: 'Atención de primeros auxilios y apoyo ante emergencias.'
  }
];

const formatDate = (value) => {
  if (!value) return 'Pendiente';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'Pendiente';

  return new Intl.DateTimeFormat('es-GT', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

const formatTime = (value) => {
  if (!value) return '--:--';
  const date = value instanceof Date ? value : new Date(value);
  if (Number.isNaN(date.getTime())) return '--:--';

  return new Intl.DateTimeFormat('es-GT', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(date);
};

const formatShortDate = (value) => {
  if (!value) return '';
  const date = new Date(`${value}T00:00:00`);
  if (Number.isNaN(date.getTime())) return '';

  return new Intl.DateTimeFormat('es-GT', {
    weekday: 'short',
    day: '2-digit',
    month: 'short'
  }).format(date);
};

const formatCurrency = (value) =>
  new Intl.NumberFormat('es-GT', {
    style: 'currency',
    currency: 'GTQ'
  }).format(Number(value || 0));

const formatMoney = (value, currency = 'GTQ') =>
  new Intl.NumberFormat('es-GT', {
    style: 'currency',
    currency: CURRENCY_RATES[currency] ? currency : 'GTQ'
  }).format(Number(value || 0) * (CURRENCY_RATES[currency] || 1));

const formatDateOnly = (value) => {
  if (!value) return 'Pendiente';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return 'Pendiente';

  return new Intl.DateTimeFormat('es-GT', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(date);
};

const todayInputValue = () => new Date().toISOString().slice(0, 10);

const boardingCode = (reservationId, passengerId) =>
  `GUA-${String(reservationId || 0).padStart(5, '0')}-${String(passengerId || 0).padStart(4, '0')}`;

const normalize = (value = '') => value.toString().trim().toLowerCase();

const statusClassName = (status = '') => {
  const value = normalize(status);

  if (value.includes('cancel') || value.includes('demor') || value.includes('retras')) return 'delayed';
  if (value.includes('abord') || value.includes('proceso') || value.includes('program')) return 'boarding';
  if (value.includes('final') || value.includes('complet') || value.includes('activo')) return 'on-time';

  return 'neutral';
};

const SESSION_KEY = 'aeropuertoAurora.user';
const CART_KEY = 'aeropuertoAurora.cart';
const BASE_FARE = 1250;
const CURRENCY_RATES = {
  GTQ: 1,
  USD: 0.128,
  EUR: 0.12,
  COP: 512,
  MXN: 2.35,
  CRC: 65.5,
  HNL: 3.18,
  NIO: 4.72,
  PAB: 0.128,
  CAD: 0.176,
  BRL: 0.66,
  GBP: 0.103,
  JPY: 20.1,
  CHF: 0.117,
  AUD: 0.197
};
const CURRENCIES = Object.keys(CURRENCY_RATES);
const CLASS_LABELS = {
  economica: 'Turista',
  ejecutiva: 'Ejecutiva',
  primera: 'Primera clase'
};
const TARIFF_FAMILIES = [
  {
    code: 'turista',
    className: 'economica',
    name: 'Turista',
    tagline: 'Para viajar básico',
    multiplier: 1,
    benefits: [
      ['check', 'Artículo personal'],
      ['x', 'Equipaje de bodega'],
      ['x', 'Cambios sin penalidad']
    ]
  },
  {
    code: 'ejecutiva',
    className: 'ejecutiva',
    name: 'Ejecutiva',
    tagline: 'Más comodidad',
    multiplier: 1.32,
    benefits: [
      ['check', 'Artículo personal'],
      ['check', 'Equipaje de bodega'],
      ['x', 'Reembolso completo']
    ]
  },
  {
    code: 'primera',
    className: 'primera',
    name: 'Primera clase',
    tagline: 'Mayor tranquilidad',
    multiplier: 1.68,
    benefits: [
      ['check', 'Artículo personal'],
      ['check', 'Equipaje de bodega'],
      ['check', 'Cambios y reembolso']
    ]
  }
];
const PASSENGER_GROUPS = [
  { key: 'adults', label: 'Adultos', hint: 'Pasajero adulto', defaultValue: 1 },
  { key: 'youth', label: 'Jóvenes', hint: 'Pasajero joven', defaultValue: 0 },
  { key: 'children', label: 'Niños', hint: 'Pasajero menor', defaultValue: 0 },
  { key: 'babies', label: 'Bebés', hint: 'Bebé en brazos', defaultValue: 0 }
];
const DEFAULT_PASSENGER_GROUPS = PASSENGER_GROUPS.reduce(
  (groups, group) => ({ ...groups, [group.key]: group.defaultValue }),
  {}
);
const KNOWN_AIRPORTS = [
  { name: 'Aeropuerto Internacional La Aurora', country: 'Guatemala', city: 'Ciudad de Guatemala', aliases: ['guatemala', 'la aurora', 'gua'] },
  { name: 'Aeropuerto Internacional El Dorado', country: 'Colombia', city: 'Bogotá', aliases: ['colombia', 'bogota', 'el dorado'] },
  { name: 'Miami International Airport', country: 'Estados Unidos', city: 'Miami', aliases: ['miami', 'mia'] },
  { name: 'John F. Kennedy International Airport', country: 'Estados Unidos', city: 'Nueva York', aliases: ['new york', 'jfk'] },
  { name: 'Los Angeles International Airport', country: 'Estados Unidos', city: 'Los Angeles', aliases: ['los angeles', 'lax'] },
  { name: 'Aeropuerto Internacional Benito Juárez', country: 'México', city: 'Ciudad de México', aliases: ['mexico', 'ciudad de mexico'] },
  { name: 'Aeropuerto Internacional de Cancún', country: 'México', city: 'Cancún', aliases: ['cancun'] },
  { name: 'Aeropuerto Internacional de Tocumen', country: 'Panamá', city: 'Panamá', aliases: ['panama', 'tocumen'] },
  { name: 'Aeropuerto Internacional Juan Santamaria', country: 'Costa Rica', city: 'San Jose', aliases: ['costa rica', 'san jose'] },
  { name: 'Aeropuerto Internacional de El Salvador San Oscar Arnulfo Romero', country: 'El Salvador', city: 'San Salvador', aliases: ['el salvador', 'san salvador'] },
  { name: 'Aeropuerto Adolfo Suárez Madrid-Barajas', country: 'España', city: 'Madrid', aliases: ['madrid', 'españa', 'espana'] }
];
const PAYMENT_METHODS = [
  { id: 1, name: 'Tarjeta de crédito', requiresCard: true },
  { id: 2, name: 'Tarjeta de débito', requiresCard: true },
  { id: 3, name: 'Transferencia', requiresCard: false }
];
const CARD_MONTHS = Array.from({ length: 12 }, (_, index) => {
  const value = String(index + 1).padStart(2, '0');
  return { value, label: value };
});
const CARD_YEARS = Array.from({ length: 16 }, (_, index) => String(new Date().getFullYear() + index));
const TRANSFER_ACCOUNT = {
  holder: 'Aeropuerto La Aurora',
  number: '001-445889-7'
};
const ANCILLARY_SERVICES = [
  { id: 'seat', title: 'Elige tu asiento', description: 'Selecciona ventana, pasillo o salida rapida.', price: 120, icon: 'AS' },
  { id: 'bag', title: 'Equipaje adicional', description: 'Agrega una maleta documentada al viaje.', price: 280, icon: 'EQ' },
  { id: 'vip', title: 'Sala VIP', description: 'Acceso a sala preferencial antes del vuelo.', price: 360, icon: 'VP' },
  { id: 'priority', title: 'Prioridad de abordaje', description: 'Aborda primero y ahorra tiempo en puerta.', price: 95, icon: 'PR' }
];

const DOCUMENT_TYPES = ['DPI', 'Pasaporte', 'Licencia'];
const NATIONALITIES = ['Guatemala', 'El Salvador', 'Honduras', 'Nicaragua', 'Costa Rica', 'Panamá', 'México', 'Estados Unidos', 'Otra'];
const SEX_OPTIONS = [
  { value: 'M', label: 'Masculino' },
  { value: 'F', label: 'Femenino' }
];

const canPurchaseFlight = (status = '') => normalize(status) === 'programado';

const formatCardNumber = (value = '') =>
  value
    .replace(/\D/g, '')
    .slice(0, 19)
    .replace(/(.{4})/g, '$1 ')
    .trim();

const isExpiredCard = (month, year) => {
  if (!month || !year) return false;

  const today = new Date();
  const expiration = new Date(Number(year), Number(month), 0, 23, 59, 59);
  return expiration < today;
};

const passengerCountFromGroups = (groups = DEFAULT_PASSENGER_GROUPS) =>
  Math.max(1, PASSENGER_GROUPS.reduce((sum, group) => sum + Number(groups[group.key] || 0), 0));

const passengerCountFromCriteria = (criteria) =>
  criteria?.passengerGroups ? passengerCountFromGroups(criteria.passengerGroups) : Math.max(1, Number(criteria?.passengers || 1));

const tariffByClassName = (className = 'economica') =>
  TARIFF_FAMILIES.find((family) => family.className === className) || TARIFF_FAMILIES[0];

const calculateFlightFare = (className = 'economica', passengerCount = 1) => {
  const tariff = tariffByClassName(className);
  return Math.round(BASE_FARE * tariff.multiplier * passengerCount * 100) / 100;
};

const serverCartItemToFlight = (item = {}) => {
  const selectedClass = item.selectedClass || item.clase || 'economica';
  const passengers = Number(item.passengerCount || item.cantidad || 1);

  return {
    ...item,
    id: item.vueloId || item.id,
    itemCarritoId: item.id,
    cartId: `server-${item.id}`,
    selectedClass,
    criteria: {
      passengers,
      origin: item.origen || '',
      destination: item.destino || '',
      departureDate: '',
      returnDate: '',
      currency: 'GTQ'
    }
  };
};

const cartItemPayload = (item = {}) => {
  const selectedClass = item.selectedClass || item.clase || 'economica';
  const passengers = passengerCountFromCriteria(item.criteria) || Number(item.passengerCount || 1);
  const flightId = Number(item.vueloId || item.id);

  if (!Number.isFinite(flightId) || flightId <= 0) {
    return null;
  }

  return {
    vueloId: flightId,
    clase: selectedClass,
    precioUnitario: calculateFlightFare(selectedClass, passengers),
    cantidad: passengers
  };
};

const cartItemPayloads = (item = {}) => {
  if (Array.isArray(item.flights) && item.flights.length > 0) {
    return item.flights.map(cartItemPayload).filter(Boolean);
  }

  return [cartItemPayload(item)].filter(Boolean);
};

const nextTariffFamily = (family) => {
  const index = TARIFF_FAMILIES.findIndex((item) => item.code === family.code);
  return index >= 0 ? TARIFF_FAMILIES[index + 1] || null : null;
};

const airportCountryFallback = (airportName = '') => {
  const value = normalize(airportName);
  const known = KNOWN_AIRPORTS.find((airport) => normalize(airport.name) === value);
  if (known) return known.country;
  if (value.includes('aurora')) return 'Guatemala';
  if (value.includes('dorado')) return 'Colombia';
  if (value.includes('miami')) return 'Estados Unidos';
  return 'Internacional';
};

const airportLabel = (airport) => `${airport.name}, ${airport.country}`;

const toAirportOption = (airport) => ({
  id: airportId(airport),
  name: airportName(airport),
  country: airportCountry(airport),
  city: airport?.ciudad ?? airport?.Ciudad ?? airport?.city ?? '',
  aliases: [
    airport?.codigoIata,
    airport?.CodigoIata,
    airport?.codigoIcao,
    airport?.CodigoIcao,
    airport?.codigo,
    airport?.Codigo
  ].filter(Boolean)
});

const resolveAirportQuery = (query = '', airportOptions = []) => {
  const term = normalize(query);
  if (!term) return '';

  const match = airportOptions.find((airport) =>
    [airport.name, airport.country, airport.city, airportLabel(airport), ...(airport.aliases || [])]
      .filter(Boolean)
      .some((value) => {
        const normalized = normalize(value);
        return normalized === term || normalized.includes(term) || term.includes(normalized);
      })
  );

  return match?.name || query;
};

const airportId = (airport) => airport?.id ?? airport?.Id ?? null;
const airportName = (airport) => airport?.nombre ?? airport?.Nombre ?? airport?.name ?? '';
const airportCountry = (airport) => airport?.pais ?? airport?.Pais ?? airport?.country ?? '';
const destinationReportId = (destination) => destination?.aeropuertoId ?? destination?.AeropuertoId ?? null;
const destinationReportName = (destination) => destination?.aeropuerto ?? destination?.Aeropuerto ?? '';

const matchAirportRecord = (airports = [], value = '') => {
  const term = normalize(value);
  if (!term) return null;

  return airports.find((airport) => {
    const name = normalize(airportName(airport));
    const country = normalize(airportCountry(airport));
    const city = normalize(airport?.ciudad ?? airport?.Ciudad ?? '');
    return name === term || name.includes(term) || term.includes(name) || country.includes(term) || city.includes(term);
  }) || null;
};

const incrementDestinationScore = (destinations, destinationId, field, fallbackName = '') => {
  const exists = destinations.some((destination) => destinationReportId(destination) === destinationId);
  const nextDestinations = exists
    ? destinations
    : [
        ...destinations,
        {
          aeropuertoId: destinationId,
          aeropuerto: fallbackName || 'Destino seleccionado',
          totalBusquedas: 0,
          totalClicks: 0,
          totalPasajeros: 0
        }
      ];

  return nextDestinations
    .map((destination) =>
      destinationReportId(destination) === destinationId
        ? { ...destination, [field]: Number(destination[field] || 0) + 1 }
        : destination
    )
    .sort((first, second) => {
      const firstScore = Number(first.totalBusquedas || 0) + Number(first.totalClicks || 0);
      const secondScore = Number(second.totalBusquedas || 0) + Number(second.totalClicks || 0);
      return secondScore - firstScore || destinationReportName(first).localeCompare(destinationReportName(second));
    });
};

const estimateDurationMinutes = (flight) => {
  const seed = Number(flight?.id || 1) + normalize(flight?.destino).length * 11;
  return 70 + (seed % 5) * 25;
};

const addMinutesToDate = (value, minutes) => {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return null;
  date.setMinutes(date.getMinutes() + minutes);
  return date;
};

const hasTechnicalStop = (flight) => Number(flight?.retrasoMinutos || 0) > 0 || Number(flight?.id || 0) % 3 === 0;

const toDateInputValue = (date) => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
};

const dateFromInputValue = (value) => {
  if (!value) return null;
  const [year, month, day] = value.split('-').map(Number);
  return new Date(year, month - 1, day);
};

const fareForDate = (date) => {
  const seed = date.getFullYear() + (date.getMonth() + 1) * 19 + date.getDate() * 37;
  return 710 + (seed % 58) * 10;
};

const formatCompactFare = (value, currency = 'GTQ') =>
  new Intl.NumberFormat('es-GT', {
    style: 'currency',
    currency: CURRENCY_RATES[currency] ? currency : 'GTQ',
    notation: 'compact',
    maximumFractionDigits: 1
  }).format(Number(value || 0) * (CURRENCY_RATES[currency] || 1));

const flightMatchesRoute = (flight, origin, destination) => {
  const normalizedOrigin = normalize(origin);
  const normalizedDestination = normalize(destination);
  const flightOrigin = normalize(flight?.origen);
  const flightDestination = normalize(flight?.destino);
  const originMatch = !normalizedOrigin || flightOrigin === normalizedOrigin || flightOrigin.includes(normalizedOrigin) || (flightOrigin && normalizedOrigin.includes(flightOrigin));
  const destinationMatch = !normalizedDestination || flightDestination === normalizedDestination || flightDestination.includes(normalizedDestination) || (flightDestination && normalizedDestination.includes(flightDestination));
  return originMatch && destinationMatch;
};

const lowestRouteFareForDate = ({ date, flights = [], origin = '', destination = '', direction = 'departure', passengerCount = 1 }) => {
  const dateValue = toDateInputValue(date);
  const routeOrigin = direction === 'return' ? destination : origin;
  const routeDestination = direction === 'return' ? origin : destination;

  if (!routeOrigin || !routeDestination) return null;

  const matchingFlights = flights
    .filter((flight) => canPurchaseFlight(flight.estado))
    .filter((flight) => toDateInputValue(new Date(flight.fechaVuelo)) === dateValue)
    .filter((flight) => flightMatchesRoute(flight, routeOrigin, routeDestination));

  if (matchingFlights.length === 0) return null;

  const baseDateFare = fareForDate(date);
  const lowestFare = Math.min(...matchingFlights.map((flight) => baseDateFare + (Number(flight.id || 0) % 5) * 25 - (hasTechnicalStop(flight) ? 35 : 0)));

  return Math.max(650, Math.round(lowestFare * Math.max(1, Number(passengerCount || 1)) * 100) / 100);
};

const sameDateValue = (date, value) => toDateInputValue(date) === value;
const isAfterDateValue = (later, earlier) => {
  if (!later || !earlier) return false;
  return later > earlier;
};

const prettifyName = (value = '') =>
  value
    .toString()
    .replace(/_/g, ' ')
    .replace(/\b\w/g, (letter) => letter.toUpperCase());

const stringifyValue = (value) => {
  if (value === null || value === undefined) return '';
  if (typeof value === 'object') return JSON.stringify(value);
  return value.toString();
};

const UNSAFE_INPUT_PATTERN = /['"\\/;<>`{}[\]|]/;
const PASSWORD_RULE_PATTERN = /^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*._?-]).{8,}$/;

const sanitizeRegisterValue = (field, value = '') => {
  const clean = value.toString().replace(new RegExp(UNSAFE_INPUT_PATTERN.source, 'g'), '');

  if (field === 'contrasena') return clean;
  if (field === 'email') return clean.replace(/[^A-Za-z0-9@._%+-]/g, '');
  if (field === 'telefono') return clean.replace(/[^0-9+\s()-]/g, '');
  if (field === 'numeroDocumento') return clean.replace(/[^A-Za-z0-9._ -]/g, '');
  if (field === 'fechaNacimiento') return clean;
  if (field === 'sexo') return clean.replace(/[^A-Za-z]/g, '');

  return clean.replace(/[^\p{L}\p{N} ._-]/gu, '');
};

const isAdminUser = (currentUser) => {
  if (!currentUser) return false;

  return [currentUser.usuario, currentUser.email, currentUser.nombreCompleto, currentUser.rol, currentUser.role]
    .filter(Boolean)
    .some((value) => {
      const normalized = normalize(value);
      return normalized === 'admin.aurora' || normalized.includes('administrador');
    });
};
const validateRegisterForm = (form) => {
  const errors = {};
  const requiredFields = [
    ['usuario', 'Ingresa un usuario.'],
    ['email', 'Ingresa un email.'],
    ['contrasena', 'Ingresa una contraseña.'],
    ['numeroDocumento', 'Ingresa tu documento.'],
    ['tipoDocumento', 'Selecciona tipo de documento.'],
    ['primerNombre', 'Ingresa tu primer nombre.'],
    ['segundoNombre', 'Ingresa tu segundo nombre.'],
    ['primerApellido', 'Ingresa tu primer apellido.'],
    ['segundoApellido', 'Ingresa tu segundo apellido.'],
    ['fechaNacimiento', 'Selecciona tu fecha de nacimiento.'],
    ['nacionalidad', 'Selecciona tu nacionalidad.'],
    ['sexo', 'Selecciona tu sexo.'],
    ['telefono', 'Ingresa tu teléfono.']
  ];

  requiredFields.forEach(([field, message]) => {
    if (!form[field]?.toString().trim()) {
      errors[field] = message;
    }
  });

  if (form.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'Usa un formato de email válido.';
  }

  Object.entries(form).forEach(([field, value]) => {
    if (value && UNSAFE_INPUT_PATTERN.test(value.toString())) {
      errors[field] = 'No uses comillas, apóstrofes, slashes ni símbolos peligrosos.';
    }
  });

  if (form.contrasena && !PASSWORD_RULE_PATTERN.test(form.contrasena)) {
    errors.contrasena = 'Usa al menos 8 caracteres, una mayúscula, un número y un símbolo seguro.';
  }

  return errors;
};

function NavBar({ user, adminView, isAdmin, activeView, onAdminView, onNavigate, onLoginClick, onLogout }) {
  const [userMenuOpen, setUserMenuOpen] = useState(false);
  const shortUserName = isAdmin ? 'Administrador' : (user?.nombreCompleto?.split(' ')[0] || user?.usuario || 'Usuario');
  const avatarLetter = (user?.nombreCompleto || user?.usuario || 'A').trim().charAt(0).toUpperCase();

  return (
    <nav className="site-nav">
      <a className="nav-logo" href="#inicio" onClick={(event) => onNavigate(event, 'inicio')}>
        La <span>Aurora</span>
      </a>
      <div className="nav-links">
        <a className={activeView === 'explorar' ? 'active' : ''} href="#explorar" onClick={(event) => onNavigate(event, 'explorar')}>Explorar</a>
        <a className={activeView === 'rastreo' ? 'active' : ''} href="#rastreo" onClick={(event) => onNavigate(event, 'rastreo')}>Rastreo</a>
        <a className={activeView === 'mis-viajes' ? 'active' : ''} href="#mis-viajes" onClick={(event) => onNavigate(event, 'mis-viajes')}>Mis viajes</a>
        <a className={activeView === 'checkin' ? 'active' : ''} href="#checkin" onClick={(event) => onNavigate(event, 'checkin')}>Check-in</a>
        <a className={activeView === 'objetos' ? 'active' : ''} href="#objetos" onClick={(event) => onNavigate(event, 'objetos')}>Objetos</a>
        <a className={activeView === 'promos' ? 'active' : ''} href="#promos" onClick={(event) => onNavigate(event, 'promos')}>Promos</a>
        <a className={activeView === 'ubicacion' ? 'active' : ''} href="#ubicacion" onClick={(event) => onNavigate(event, 'ubicacion')}>Ubicación</a>
        {isAdmin && (
          <>
            <button
              className={adminView === 'reporteria' ? 'nav-admin-link active' : 'nav-admin-link'}
              type="button"
              onClick={() => onAdminView('reporteria')}
            >
              Reporteria
            </button>
            <button
              className={adminView === 'admin' ? 'nav-admin-link active' : 'nav-admin-link'}
              type="button"
              onClick={() => onAdminView('admin')}
            >
              Admin
            </button>
          </>
        )}
        {user ? (
          <div className="nav-user-dropdown">
            <button
              className="nav-user-menu"
              type="button"
              onClick={() => setUserMenuOpen((current) => !current)}
              aria-expanded={userMenuOpen}
            >
              <div className="nav-avatar" aria-hidden="true">{avatarLetter}</div>
              <div className="nav-user-copy">
                <span className="nav-user">{shortUserName}</span>
              </div>
              <span className="nav-user-caret" aria-hidden="true">▾</span>
            </button>
            {userMenuOpen && (
              <div className="nav-user-dropdown-menu">
                <button
                  type="button"
                  onClick={() => {
                    setUserMenuOpen(false);
                    onLogout();
                  }}
                >
                  Cerrar sesión
                </button>
              </div>
            )}
          </div>
        ) : (
          <button className="nav-session-button nav-login-button" type="button" onClick={onLoginClick}>
            <AccountIcon />
            <span>Iniciar sesión</span>
          </button>
        )}
      </div>
    </nav>
  );
}

function AccountIcon() {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      <path d="M12 2a10 10 0 1 0 0 20 10 10 0 0 0 0-20Zm0 3a3 3 0 1 1 0 6 3 3 0 0 1 0-6Zm0 15.2a8.2 8.2 0 0 1-6-3.1c.1-2 4-3.2 6-3.2s5.9 1.2 6 3.2a8.2 8.2 0 0 1-6 3.1Z" />
    </svg>
  );
}

function EyeIcon({ hidden = false }) {
  return (
    <svg viewBox="0 0 24 24" aria-hidden="true" focusable="false">
      {hidden ? (
        <path d="M2.3 3 1 4.3l3 3A12.7 12.7 0 0 0 1 12c1.7 4.4 6 7.5 11 7.5 1.5 0 3-.3 4.3-.8l3.4 3.3 1.3-1.3L2.3 3ZM9 12c0-.2 0-.4.1-.6l3.5 3.5H12a3 3 0 0 1-3-3Zm3-7.5c5 0 9.3 3.1 11 7.5a12.5 12.5 0 0 1-3.8 4.9l-2.3-2.3A5 5 0 0 0 9.4 7.1L7.7 5.4c1.3-.6 2.8-.9 4.3-.9Z" />
      ) : (
        <path d="M12 4.5c-5 0-9.3 3.1-11 7.5 1.7 4.4 6 7.5 11 7.5s9.3-3.1 11-7.5c-1.7-4.4-6-7.5-11-7.5Zm0 12.5a5 5 0 1 1 0-10 5 5 0 0 1 0 10Zm0-8a3 3 0 1 0 0 6 3 3 0 0 0 0-6Z" />
      )}
    </svg>
  );
}

function PasswordField({ value, onChange, className = '', placeholder = '', autoComplete = 'current-password', required = false }) {
  const [visible, setVisible] = useState(false);
  return (
    <div className="password-field">
      <input
        className={className}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        type={visible ? 'text' : 'password'}
        autoComplete={autoComplete}
        required={required}
      />
      <button
        className="password-toggle"
        type="button"
        onClick={() => setVisible((current) => !current)}
        aria-label={visible ? 'Ocultar contraseña' : 'Mostrar contraseña'}
      >
        <EyeIcon hidden={visible} />
      </button>
    </div>
  );
}

function Stars() {
  const stars = useMemo(
    () =>
      Array.from({ length: 95 }, (_, index) => ({
        id: index,
        size: `${Math.random() * 2.2 + 0.6}px`,
        top: `${Math.random() * 100}%`,
        left: `${Math.random() * 100}%`,
        duration: `${2 + Math.random() * 4}s`,
        delay: `${Math.random() * 5}s`
      })),
    []
  );

  return (
    <div className="stars" aria-hidden="true">
      {stars.map((star) => (
        <span
          key={star.id}
          style={{
            width: star.size,
            height: star.size,
            top: star.top,
            left: star.left,
            '--duration': star.duration,
            '--delay': star.delay
          }}
        />
      ))}
    </div>
  );
}

function Plane() {
  return (
    <svg className="plane-svg" viewBox="0 0 160 54" fill="none" aria-hidden="true">
      <path d="M5 28L120 6L155 27L120 34L5 28Z" fill="rgba(30,184,200,0.66)" />
      <path d="M75 27L105 14L119 28L104 36L75 27Z" fill="rgba(200,168,75,0.58)" />
      <path d="M38 27L60 49L75 29L38 27Z" fill="rgba(30,184,200,0.42)" />
    </svg>
  );
}

function Hero({ onExploreClick }) {
  return (
    <section className="hero" id="inicio">
      <Stars />
      <Plane />
      <div className="hero-content">
        <div className="hero-badge">Guatemala City · GUA</div>
        <h1>
          Aeropuerto Internacional
          <span> La Aurora</span>
        </h1>
        <p>Puerta de entrada al corazón de Centroamérica, conectada en tiempo real con la operación aeroportuaria.</p>
        <div className="hero-ctas">
          <button className="btn btn-primary" type="button" onClick={onExploreClick}>Explorar vuelos</button>
        </div>
      </div>
    </section>
  );
}

function StatsStrip({ flights, destinations, baggage, openIncidents }) {
  const routes = new Set(flights.map((flight) => flight.destino).filter(Boolean)).size || destinations.length;

  return (
    <section className="info-strip" aria-label="Resumen">
      <div className="section stat-grid">
        <div className="stat-item">
          <div className="stat-num">{flights.length}</div>
          <div className="stat-label">Vuelos cargados</div>
        </div>
        <div className="stat-item">
          <div className="stat-num">{routes}</div>
          <div className="stat-label">Destinos activos</div>
        </div>
        <div className="stat-item">
          <div className="stat-num">{baggage.length}</div>
          <div className="stat-label">Equipajes monitoreados</div>
        </div>
        <div className="stat-item">
          <div className="stat-num">{openIncidents}</div>
          <div className="stat-label">Incidentes abiertos</div>
        </div>
      </div>
    </section>
  );
}

function AuthModal({ open, onClose, onLogin, onRegister }) {
  const [mode, setMode] = useState('login');
  const [form, setForm] = useState({ usuarioOEmail: '', contrasena: '' });
  const [registerForm, setRegisterForm] = useState({
    usuario: '',
    email: '',
    contrasena: '',
    numeroDocumento: '',
    tipoDocumento: 'DPI',
    primerNombre: '',
    segundoNombre: '',
    primerApellido: '',
    segundoApellido: '',
    fechaNacimiento: '',
    nacionalidad: '',
    sexo: '',
    telefono: ''
  });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [fieldErrors, setFieldErrors] = useState({});

  if (!open) return null;

  const submitLogin = async (event) => {
    event.preventDefault();
    setSubmitting(true);
    setError('');

    try {
      await onLogin(form);
      setForm({ usuarioOEmail: '', contrasena: '' });
    } catch (loginError) {
      setError(loginError.message);
    } finally {
      setSubmitting(false);
    }
  };

  const submitRegister = async (event) => {
    event.preventDefault();
    setSubmitting(true);
    setError('');
    setFieldErrors({});

    const nextErrors = validateRegisterForm(registerForm);
    if (Object.keys(nextErrors).length > 0) {
      setFieldErrors(nextErrors);
      setError('Revisa los campos marcados antes de crear la cuenta.');
      setSubmitting(false);
      return;
    }

    try {
      await onRegister({
        ...registerForm,
        fechaNacimiento: registerForm.fechaNacimiento,
        sexo: registerForm.sexo
      });
      setRegisterForm({
        usuario: '',
        email: '',
        contrasena: '',
        numeroDocumento: '',
        tipoDocumento: 'DPI',
        primerNombre: '',
        segundoNombre: '',
        primerApellido: '',
        segundoApellido: '',
        fechaNacimiento: '',
        nacionalidad: '',
        sexo: '',
        telefono: ''
      });
    } catch (registerError) {
      setError(registerError.message);
    } finally {
      setSubmitting(false);
    }
  };

  const updateRegister = (field, value) => {
    setRegisterForm((current) => ({ ...current, [field]: sanitizeRegisterValue(field, value) }));
    setFieldErrors((current) => {
      const next = { ...current };
      delete next[field];
      return next;
    });
  };

  const registerInputClass = (field) => fieldErrors[field] ? 'field-invalid' : '';

  return (
    <div className="modal-backdrop" role="presentation">
      <div className="login-modal" role="dialog" aria-modal="true" aria-labelledby="login-title">
        <button className="modal-close" type="button" onClick={onClose} aria-label="Cerrar">x</button>
        <div className="section-label">Portal de compra</div>
        <h2 id="login-title">{mode === 'login' ? 'Iniciar sesión' : 'Crear usuario'}</h2>
        <p>{mode === 'login' ? 'Entra para comprar vuelos y ver tus viajes.' : 'Crea tu pasajero y usuario para comprar boletos en línea.'}</p>
        <div className="auth-tabs">
          <button type="button" className={mode === 'login' ? 'active' : ''} onClick={() => { setMode('login'); setError(''); }}>Entrar</button>
          <button type="button" className={mode === 'register' ? 'active' : ''} onClick={() => { setMode('register'); setError(''); }}>Crear cuenta</button>
        </div>
        {mode === 'login' ? (
          <form onSubmit={submitLogin} noValidate>
            <label>
              Usuario o email
              <input
                value={form.usuarioOEmail}
                onChange={(event) => setForm((current) => ({ ...current, usuarioOEmail: event.target.value }))}
                placeholder="luis.perez"
                autoComplete="username"
                required
              />
            </label>
            <label>
              Contraseña
              <PasswordField
                value={form.contrasena}
                onChange={(event) => setForm((current) => ({ ...current, contrasena: event.target.value }))}
                placeholder="1234"
                autoComplete="current-password"
                required
              />
            </label>
            {error && <div className="form-error">{error}</div>}
            <button className="btn btn-primary" type="submit" disabled={submitting}>
              {submitting ? 'Entrando' : 'Entrar'}
            </button>
          </form>
        ) : (
          <form onSubmit={submitRegister} noValidate>
            <div className="form-grid">
              <label>
                Usuario
                <input className={registerInputClass('usuario')} value={registerForm.usuario} onChange={(event) => updateRegister('usuario', event.target.value)} />
                {fieldErrors.usuario && <small className="field-error">{fieldErrors.usuario}</small>}
              </label>
              <label>
                Email
                <input className={registerInputClass('email')} type="email" value={registerForm.email} onChange={(event) => updateRegister('email', event.target.value)} />
                {fieldErrors.email && <small className="field-error">{fieldErrors.email}</small>}
              </label>
              <label>
                Contraseña
                <PasswordField
                  className={registerInputClass('contrasena')}
                  value={registerForm.contrasena}
                  onChange={(event) => updateRegister('contrasena', event.target.value)}
                  autoComplete="new-password"
                />
                <small>Mínimo 8 caracteres, una mayúscula, un número y un símbolo seguro.</small>
                {fieldErrors.contrasena && <small className="field-error">{fieldErrors.contrasena}</small>}
              </label>
              <label>
                Documento
                <input className={registerInputClass('numeroDocumento')} value={registerForm.numeroDocumento} onChange={(event) => updateRegister('numeroDocumento', event.target.value)} />
                {fieldErrors.numeroDocumento && <small className="field-error">{fieldErrors.numeroDocumento}</small>}
              </label>
              <label>
                Tipo documento
                <select className={registerInputClass('tipoDocumento')} value={registerForm.tipoDocumento} onChange={(event) => updateRegister('tipoDocumento', event.target.value)}>
                  {DOCUMENT_TYPES.map((type) => <option value={type} key={type}>{type}</option>)}
                </select>
                {fieldErrors.tipoDocumento && <small className="field-error">{fieldErrors.tipoDocumento}</small>}
              </label>
              <label>
                Primer nombre
                <input className={registerInputClass('primerNombre')} value={registerForm.primerNombre} onChange={(event) => updateRegister('primerNombre', event.target.value)} />
                {fieldErrors.primerNombre && <small className="field-error">{fieldErrors.primerNombre}</small>}
              </label>
              <label>
                Segundo nombre
                <input className={registerInputClass('segundoNombre')} value={registerForm.segundoNombre} onChange={(event) => updateRegister('segundoNombre', event.target.value)} />
                {fieldErrors.segundoNombre && <small className="field-error">{fieldErrors.segundoNombre}</small>}
              </label>
              <label>
                Primer apellido
                <input className={registerInputClass('primerApellido')} value={registerForm.primerApellido} onChange={(event) => updateRegister('primerApellido', event.target.value)} />
                {fieldErrors.primerApellido && <small className="field-error">{fieldErrors.primerApellido}</small>}
              </label>
              <label>
                Segundo apellido
                <input className={registerInputClass('segundoApellido')} value={registerForm.segundoApellido} onChange={(event) => updateRegister('segundoApellido', event.target.value)} />
                {fieldErrors.segundoApellido && <small className="field-error">{fieldErrors.segundoApellido}</small>}
              </label>
              <label>
                Nacimiento
                <input className={registerInputClass('fechaNacimiento')} type="date" value={registerForm.fechaNacimiento} onChange={(event) => updateRegister('fechaNacimiento', event.target.value)} />
                {fieldErrors.fechaNacimiento && <small className="field-error">{fieldErrors.fechaNacimiento}</small>}
              </label>
              <label>
                Nacionalidad
                <select className={registerInputClass('nacionalidad')} value={registerForm.nacionalidad} onChange={(event) => updateRegister('nacionalidad', event.target.value)}>
                  <option value="">Selecciona una opción</option>
                  {NATIONALITIES.map((nationality) => <option value={nationality} key={nationality}>{nationality}</option>)}
                </select>
                {fieldErrors.nacionalidad && <small className="field-error">{fieldErrors.nacionalidad}</small>}
              </label>
              <label>
                Sexo
                <select className={registerInputClass('sexo')} value={registerForm.sexo} onChange={(event) => updateRegister('sexo', event.target.value)}>
                  <option value="">Selecciona una opción</option>
                  {SEX_OPTIONS.map((option) => <option value={option.value} key={option.value}>{option.label}</option>)}
                </select>
                {fieldErrors.sexo && <small className="field-error">{fieldErrors.sexo}</small>}
              </label>
              <label>
                Teléfono
                <input className={registerInputClass('telefono')} value={registerForm.telefono} onChange={(event) => updateRegister('telefono', event.target.value)} />
                {fieldErrors.telefono && <small className="field-error">{fieldErrors.telefono}</small>}
              </label>
            </div>
            {error && <div className="form-error">{error}</div>}
            <button className="btn btn-primary" type="submit" disabled={submitting}>
              {submitting ? 'Creando' : 'Crear cuenta'}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}

function FlightBoard({ flights, loading, user, onRequireLogin, onBuyFlight, buyingFlightId, searchTerm, onSearchChange }) {
  return (
    <section className="section" id="rastreo">
      <div className="section-label">Tablero de vuelos</div>
      <h2 className="section-title">Rastrea el estado de un vuelo</h2>
      <div className="flight-search">
        <input
          value={searchTerm}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder="Buscar por vuelo, destino, origen o aerolínea"
        />
      </div>

      <div className="board">
        <div className="board-header">
          <span>Vuelo</span>
          <span>Destino</span>
          <span>Hora</span>
          <span>Avión</span>
          <span>Estado</span>
          <span>Detalle</span>
        </div>

        {loading && (
          <div className="board-empty">
            <span className="loader"></span>
            Cargando vuelos desde el API
          </div>
        )}

        {!loading && !searchTerm.trim() && (
          <div className="board-empty board-empty-rich">
            <span className="board-empty-icon">✈</span>
            <div>
              <strong>Comienza una búsqueda</strong>
              <p>Ingresa destino, vuelo, origen o aerolínea para rastrear vuelos en abordaje, vuelo o cancelados.</p>
            </div>
          </div>
        )}

        {!loading && searchTerm.trim() && flights.length === 0 && (
          <div className="board-empty board-empty-rich">
            <span className="board-empty-icon">◌</span>
            <div>
              <strong>Sin resultados operativos</strong>
              <p>No encontramos vuelos operativos con esa búsqueda.</p>
            </div>
          </div>
        )}

        {!loading && searchTerm.trim() && flights.map((flight) => (
          <div className={`board-row ${!canPurchaseFlight(flight.estado) ? 'unavailable-row' : ''}`} key={flight.id}>
            <span className="flight-num">{flight.numeroVuelo}</span>
            <span className="dest">
              {flight.destino}
              <small>{flight.origen} · {flight.aerolinea}</small>
            </span>
            <span className="time">{formatTime(flight.fechaVuelo)}</span>
            <span className="time">{flight.matriculaAvion || 'Sin asignar'}</span>
            <span>
              <span className={`status ${statusClassName(flight.estado)}`}>{flight.estado}</span>
            </span>
            <span>
              <button
                className="buy-button"
                type="button"
                onClick={() => (user ? onBuyFlight(flight) : onRequireLogin())}
                disabled
                title="Este tablero solo sirve para rastrear vuelos"
              >
                Rastreo
              </button>
            </span>
          </div>
        ))}
      </div>
    </section>
  );
}

function CheckoutView({ flight, user, onBack, onConfirm, submitting, error }) {
  const [step, setStep] = useState('passengers');
  const [form, setForm] = useState({
    metodoPagoId: 1,
    titularNombre: '',
    titularEmail: '',
    pasajeros: [],
    numeroTarjeta: '',
    mesTarjeta: '',
    anioTarjeta: ''
  });
  const [formError, setFormError] = useState('');
  const [touched, setTouched] = useState({});
  const [selectedServices, setSelectedServices] = useState([]);

  useEffect(() => {
    if (flight) {
      const passengerCount = passengerCountFromCriteria(flight.criteria);
      setStep('passengers');
      setTouched({});
      setSelectedServices([]);
      setForm({
        metodoPagoId: 1,
        titularNombre: user?.nombreCompleto || '',
        titularEmail: user?.email || '',
        pasajeros: Array.from({ length: passengerCount }, (_, index) => ({
          primerNombre: '',
          segundoNombre: '',
          primerApellido: '',
          segundoApellido: '',
          tipoDocumento: 'DPI',
          numeroDocumento: index === 0 ? user?.numeroDocumento || '' : ''
        })),
        numeroTarjeta: '',
        mesTarjeta: '',
        anioTarjeta: ''
      });
      setFormError('');
    }
  }, [flight?.cartId, flight?.id, flight?.checkoutId, user]);

  if (!flight) return null;

  const checkoutFlights = Array.isArray(flight.flights) && flight.flights.length > 0 ? flight.flights : [flight];
  const isMultiFlightCheckout = checkoutFlights.length > 1;
  const selectedPayment = PAYMENT_METHODS.find((method) => method.id === Number(form.metodoPagoId)) ?? PAYMENT_METHODS[0];
  const className = checkoutFlights[0]?.selectedClass || flight.selectedClass || 'economica';
  const selectedTariff = tariffByClassName(className);
  const currency = flight.criteria?.currency || 'GTQ';
  const passengerCount = passengerCountFromCriteria(flight.criteria);
  const fare = checkoutFlights.reduce(
    (sum, item) => sum + calculateFlightFare(item.selectedClass || className, passengerCount),
    0
  );
  const servicesTotal = selectedServices.reduce((sum, serviceId) => {
    const service = ANCILLARY_SERVICES.find((item) => item.id === serviceId);
    return sum + (service?.price || 0) * passengerCount;
  }, 0);
  const subtotal = fare + servicesTotal;
  const taxes = Math.round(subtotal * 0.12 * 100) / 100;
  const total = subtotal + taxes;

  const emailValid = !form.titularEmail || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.titularEmail);
  const cardNumberDigits = form.numeroTarjeta.replace(/\D/g, '');
  const expiredCard = selectedPayment.requiresCard && isExpiredCard(form.mesTarjeta, form.anioTarjeta);
  const fieldErrors = {
    titularNombre: form.titularNombre.trim() ? '' : 'Nombre obligatorio.',
    titularEmail: !form.titularEmail.trim() ? 'Email obligatorio.' : emailValid ? '' : 'Formato de email inválido.',
    numeroTarjeta: selectedPayment.requiresCard && cardNumberDigits.length < 13 ? 'Número de tarjeta incompleto.' : '',
    mesTarjeta: selectedPayment.requiresCard && !form.mesTarjeta ? 'Selecciona mes.' : '',
    anioTarjeta: selectedPayment.requiresCard && !form.anioTarjeta ? 'Selecciona año.' : expiredCard ? 'La tarjeta está vencida.' : ''
  };
  const passengerErrors = form.pasajeros.map((passenger, index) => ({
    primerNombre: index === 0 || passenger.primerNombre.trim() ? '' : 'Nombre obligatorio.',
    primerApellido: index === 0 || passenger.primerApellido.trim() ? '' : 'Apellido obligatorio.',
    numeroDocumento: index === 0 || passenger.numeroDocumento.trim() ? '' : 'Documento obligatorio.'
  }));

  const updatePassenger = (index, field, value) => {
    setForm((current) => ({
      ...current,
      pasajeros: current.pasajeros.map((passenger, passengerIndex) =>
        passengerIndex === index ? { ...passenger, [field]: value } : passenger
      )
    }));
  };

  const markTouched = (field) => {
    setTouched((current) => ({ ...current, [field]: true }));
  };

  const passengerStepValid = () =>
    passengerErrors.every((passenger) => !passenger.primerNombre && !passenger.primerApellido && !passenger.numeroDocumento) &&
    !fieldErrors.titularNombre &&
    !fieldErrors.titularEmail;

  const paymentStepValid = () =>
    !fieldErrors.numeroTarjeta &&
    !fieldErrors.mesTarjeta &&
    !fieldErrors.anioTarjeta;

  const continueToServices = () => {
    setTouched((current) => ({
      ...current,
      titularNombre: true,
      titularEmail: true,
      ...Object.fromEntries(form.pasajeros.flatMap((_, index) => [
        [`pasajeros.${index}.primerNombre`, true],
        [`pasajeros.${index}.primerApellido`, true],
        [`pasajeros.${index}.numeroDocumento`, true]
      ]))
    }));

    if (!passengerStepValid()) {
      setFormError('Revisa los datos marcados en rojo antes de continuar.');
      return;
    }

    setFormError('');
    setStep('services');
  };

  const toggleService = (serviceId) => {
    setSelectedServices((current) =>
      current.includes(serviceId) ? current.filter((item) => item !== serviceId) : [...current, serviceId]
    );
  };

  const continueToPayment = () => {
    setFormError('');
    setStep('payment');
  };

  const submit = (event) => {
    event.preventDefault();
    setFormError('');

    setTouched((current) => ({
      ...current,
      numeroTarjeta: true,
      mesTarjeta: true,
      anioTarjeta: true
    }));

    if (!paymentStepValid()) {
      setFormError('Completa correctamente los datos de pago.');
      return;
    }

    onConfirm({
      clase: className,
      equipajeFacturado: 0,
      pesoEquipaje: null,
      tarifaPagada: subtotal,
      metodoPagoId: Number(form.metodoPagoId),
      emailConfirmacion: form.titularEmail.trim(),
      total,
      legs: checkoutFlights.map((item) => ({
        vueloId: item.id,
        clase: item.selectedClass || className,
        tarifaPagada: calculateFlightFare(item.selectedClass || className, passengerCount)
      })),
      pasajerosAdicionales: passengerCount > 1
        ? form.pasajeros.slice(1).map((p) => ({
            primerNombre: p.primerNombre.trim(),
            segundoNombre: p.segundoNombre.trim() || null,
            primerApellido: p.primerApellido.trim(),
            segundoApellido: p.segundoApellido.trim() || null,
            tipoDocumento: p.tipoDocumento || 'DPI',
            numeroDocumento: p.numeroDocumento.trim(),
            fechaNacimiento: null,
            nacionalidad: null,
            sexo: null,
            telefono: null,
            email: null
          }))
        : undefined
    });
  };

  return (
    <section className="checkout-view">
      <div className="checkout-shell">
        <button className="back-button" type="button" onClick={onBack}>Volver a vuelos</button>
        <div className="section-label">Checkout</div>
        <h2 id="purchase-title">{isMultiFlightCheckout ? 'Ida y vuelta' : flight.numeroVuelo}</h2>
        <p>
          {isMultiFlightCheckout
            ? `${checkoutFlights[0].origen} a ${checkoutFlights[0].destino} y regreso`
            : `${flight.origen} a ${flight.destino} - ${formatDate(flight.fechaVuelo)} - ${flight.aerolinea}`}
        </p>

        <form onSubmit={submit}>
          <div className="checkout-steps">
            <span className={step === 'passengers' ? 'active' : ''}>Pasajeros</span>
            <span className={step === 'services' ? 'active' : ''}>Extras</span>
            <span className={step === 'payment' ? 'active' : ''}>Pago</span>
          </div>

          <div className="checkout-card">
            <h3>{isMultiFlightCheckout ? 'Vuelos seleccionados' : 'Vuelo seleccionado'}</h3>
            <div className="checkout-flight-summary">
              {checkoutFlights.map((item, index) => (
                <div key={`${item.id}-${index}`}>
                  <span>{isMultiFlightCheckout ? (index === 0 ? 'Ida' : 'Vuelta') : 'Salida'}</span>
                  <strong>{item.numeroVuelo} - {formatTime(item.fechaVuelo)}</strong>
                  <small>{item.origen} - {item.destino} · {tariffByClassName(item.selectedClass || className).name}</small>
                </div>
              ))}
              <div><span>Viajan</span><strong>{passengerCount} pasajero(s)</strong></div>
              <div><span>Tipo</span><strong>{isMultiFlightCheckout ? 'Ida y vuelta' : 'Solo ida'}</strong></div>
            </div>
          </div>

          {step === 'passengers' && (
            <>
              <div className="checkout-card">
                <h3>Informacion de pasajeros</h3>
                <div className="passenger-list">
                  {form.pasajeros.map((passenger, index) => (
                    <div className="passenger-form-row" key={`passenger-${index}`}>
                      {index === 0 ? (
                        <div className="passenger-account-summary">
                          <strong>Pasajero principal</strong>
                          <span>{user?.nombreCompleto || user?.usuario || 'Tu cuenta'}</span>
                          <small>Usaremos los datos guardados en tu cuenta.</small>
                        </div>
                      ) : (
                        <div className="passenger-additional">
                          <p className="passenger-label">Pasajero {index + 1}</p>
                          <div className="form-grid">
                            <label>
                              Primer nombre
                              <input
                                className={touched[`pasajeros.${index}.primerNombre`] && passengerErrors[index]?.primerNombre ? 'field-invalid' : ''}
                                value={passenger.primerNombre}
                                onBlur={() => markTouched(`pasajeros.${index}.primerNombre`)}
                                onChange={(event) => updatePassenger(index, 'primerNombre', event.target.value)}
                                placeholder="Primer nombre"
                              />
                              {touched[`pasajeros.${index}.primerNombre`] && passengerErrors[index]?.primerNombre && <small className="field-error">{passengerErrors[index].primerNombre}</small>}
                            </label>
                            <label>
                              Segundo nombre <small>(opcional)</small>
                              <input
                                value={passenger.segundoNombre}
                                onChange={(event) => updatePassenger(index, 'segundoNombre', event.target.value)}
                                placeholder="Segundo nombre"
                              />
                            </label>
                            <label>
                              Primer apellido
                              <input
                                className={touched[`pasajeros.${index}.primerApellido`] && passengerErrors[index]?.primerApellido ? 'field-invalid' : ''}
                                value={passenger.primerApellido}
                                onBlur={() => markTouched(`pasajeros.${index}.primerApellido`)}
                                onChange={(event) => updatePassenger(index, 'primerApellido', event.target.value)}
                                placeholder="Primer apellido"
                              />
                              {touched[`pasajeros.${index}.primerApellido`] && passengerErrors[index]?.primerApellido && <small className="field-error">{passengerErrors[index].primerApellido}</small>}
                            </label>
                            <label>
                              Segundo apellido <small>(opcional)</small>
                              <input
                                value={passenger.segundoApellido}
                                onChange={(event) => updatePassenger(index, 'segundoApellido', event.target.value)}
                                placeholder="Segundo apellido"
                              />
                            </label>
                            <label>
                              Tipo de documento
                              <select
                                value={passenger.tipoDocumento}
                                onChange={(event) => updatePassenger(index, 'tipoDocumento', event.target.value)}
                              >
                                <option value="DPI">DPI</option>
                                <option value="PASAPORTE">Pasaporte</option>
                              </select>
                            </label>
                            <label>
                              Número de documento
                              <input
                                className={touched[`pasajeros.${index}.numeroDocumento`] && passengerErrors[index]?.numeroDocumento ? 'field-invalid' : ''}
                                value={passenger.numeroDocumento}
                                onBlur={() => markTouched(`pasajeros.${index}.numeroDocumento`)}
                                onChange={(event) => updatePassenger(index, 'numeroDocumento', event.target.value)}
                                placeholder="Número de documento"
                              />
                              {touched[`pasajeros.${index}.numeroDocumento`] && passengerErrors[index]?.numeroDocumento && <small className="field-error">{passengerErrors[index].numeroDocumento}</small>}
                            </label>
                          </div>
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </div>

              <div className="checkout-card">
                <h3>Titular de reserva</h3>
                <div className="form-grid">
                  <label>
                    Nombre completo
                    <input
                      className={touched.titularNombre && fieldErrors.titularNombre ? 'field-invalid' : ''}
                      value={form.titularNombre}
                      onBlur={() => markTouched('titularNombre')}
                      onChange={(event) => setForm((current) => ({ ...current, titularNombre: event.target.value }))}
                    />
                    {touched.titularNombre && fieldErrors.titularNombre && <small className="field-error">{fieldErrors.titularNombre}</small>}
                  </label>
                  <label>
                    Email
                    <input
                      className={touched.titularEmail && fieldErrors.titularEmail ? 'field-invalid' : ''}
                      value={form.titularEmail}
                      onBlur={() => markTouched('titularEmail')}
                      onChange={(event) => setForm((current) => ({ ...current, titularEmail: event.target.value }))}
                    />
                    {touched.titularEmail && fieldErrors.titularEmail && <small className="field-error">{fieldErrors.titularEmail}</small>}
                  </label>
                </div>
              </div>

              {formError && <div className="form-error">{formError}</div>}
              <button className="btn btn-primary" type="button" onClick={continueToServices}>Continuar</button>
            </>
          )}

          {step === 'services' && (
            <div className="checkout-card">
              {checkoutFlights.some((item) => tariffByClassName(item.selectedClass || className).code === 'turista') && (
                <div className="ancillary-warning">Tu tarifa Turista no incluye equipaje de bodega. Puedes agregar extras ahora o continuar sin cambios.</div>
              )}
              <h3>Servicios adicionales</h3>
              <div className="ancillary-grid">
                {ANCILLARY_SERVICES.map((service) => {
                  const selected = selectedServices.includes(service.id);
                  return (
                    <button
                      className={`ancillary-card ${selected ? 'selected' : ''}`}
                      type="button"
                      key={service.id}
                      onClick={() => toggleService(service.id)}
                    >
                      <span>{service.icon}</span>
                      <strong>{service.title}</strong>
                      <small>{service.description}</small>
                      <b>{formatMoney(service.price, currency)} por pasajero</b>
                    </button>
                  );
                })}
              </div>
              <div className="checkout-actions">
                <button className="btn btn-outline" type="button" onClick={() => setStep('passengers')}>Volver</button>
                <button className="btn btn-primary" type="button" onClick={continueToPayment}>Continuar</button>
              </div>
            </div>
          )}

          {step === 'payment' && (
            <div className="payment-layout">
              <div className="checkout-card">
                <h3>Pago</h3>
                <div className="form-grid">
                  <label>
                    Método de pago
                    <select value={form.metodoPagoId} onChange={(event) => setForm((current) => ({ ...current, metodoPagoId: event.target.value }))}>
                      {PAYMENT_METHODS.map((method) => (
                        <option value={method.id} key={method.id}>{method.name}</option>
                      ))}
                    </select>
                  </label>
                  <label>
                    Plazas disponibles
                    <input value={flight.plazasDisponibles} disabled readOnly />
                  </label>
                </div>

                {selectedPayment.requiresCard && (
                  <div className="card-details">
                    <h3>Datos de tarjeta</h3>
                    <div className="form-grid">
                      <label>
                        Número de tarjeta
                        <input
                          className={touched.numeroTarjeta && fieldErrors.numeroTarjeta ? 'field-invalid' : ''}
                          value={form.numeroTarjeta}
                          onBlur={() => markTouched('numeroTarjeta')}
                          onChange={(event) => setForm((current) => ({ ...current, numeroTarjeta: formatCardNumber(event.target.value) }))}
                          placeholder="4111 1111 1111 1111"
                          autoComplete="cc-number"
                          inputMode="numeric"
                        />
                        {touched.numeroTarjeta && fieldErrors.numeroTarjeta && <small className="field-error">{fieldErrors.numeroTarjeta}</small>}
                      </label>
                      <div className="expiration-grid">
                        <label>
                          Mes
                          <select
                            className={touched.mesTarjeta && fieldErrors.mesTarjeta ? 'field-invalid' : ''}
                            value={form.mesTarjeta}
                            onBlur={() => markTouched('mesTarjeta')}
                            onChange={(event) => setForm((current) => ({ ...current, mesTarjeta: event.target.value }))}
                            autoComplete="cc-exp-month"
                          >
                            <option value="">Mes</option>
                            {CARD_MONTHS.map((month) => (
                              <option value={month.value} key={month.value}>{month.label}</option>
                            ))}
                          </select>
                          {touched.mesTarjeta && fieldErrors.mesTarjeta && <small className="field-error">{fieldErrors.mesTarjeta}</small>}
                        </label>
                        <label>
                          Año
                          <select
                            className={touched.anioTarjeta && fieldErrors.anioTarjeta ? 'field-invalid' : ''}
                            value={form.anioTarjeta}
                            onBlur={() => markTouched('anioTarjeta')}
                            onChange={(event) => setForm((current) => ({ ...current, anioTarjeta: event.target.value }))}
                            autoComplete="cc-exp-year"
                          >
                            <option value="">Año</option>
                            {CARD_YEARS.map((year) => (
                              <option value={year} key={year}>{year}</option>
                            ))}
                          </select>
                          {touched.anioTarjeta && fieldErrors.anioTarjeta && <small className="field-error">{fieldErrors.anioTarjeta}</small>}
                        </label>
                      </div>
                    </div>
                  </div>
                )}

                {!selectedPayment.requiresCard && (
                  <div className="card-details transfer-details">
                    <h3>Datos para transferencia</h3>
                    <div className="transfer-detail-row">
                      <span>Titular</span>
                      <strong>{TRANSFER_ACCOUNT.holder}</strong>
                    </div>
                    <div className="transfer-detail-row">
                      <span>Número de cuenta</span>
                      <strong>{TRANSFER_ACCOUNT.number}</strong>
                    </div>
                  </div>
                )}

                {formError && <div className="form-error">{formError}</div>}
                {error && <div className="form-error">{error}</div>}
                <div className="checkout-actions">
                  <button className="btn btn-outline" type="button" onClick={() => setStep('services')}>Volver</button>
                  <button className="btn btn-primary" type="submit" disabled={submitting}>
                    {submitting ? 'Confirmando' : 'Confirmar compra'}
                  </button>
                </div>
              </div>

              <aside className="purchase-summary sticky-summary">
                <h3>Resumen de compra</h3>
                {checkoutFlights.map((item, index) => (
                  <div key={`summary-${item.id}-${index}`}>
                    <span>{isMultiFlightCheckout ? (index === 0 ? 'Ida' : 'Vuelta') : `${item.origen} - ${item.destino}`}</span>
                    <strong>{item.numeroVuelo}</strong>
                  </div>
                ))}
                <div><span>Moneda</span><strong>{currency}</strong></div>
                <div><span>{isMultiFlightCheckout ? 'Tarifas seleccionadas' : `Tarifa ${selectedTariff.name} x ${passengerCount}`}</span><strong>{formatMoney(fare, currency)}</strong></div>
                <div><span>Servicios adicionales</span><strong>{formatMoney(servicesTotal, currency)}</strong></div>
                <div><span>Impuestos</span><strong>{formatMoney(taxes, currency)}</strong></div>
                <div className="total-row"><span>Total</span><strong>{formatMoney(total, currency)}</strong></div>
              </aside>
            </div>
          )}
        </form>
      </div>
    </section>
  );
}

function PurchaseSuccessView({ summary, onGoHome, onExplore }) {
  if (!summary) return null;

  return (
    <main className="purchase-success-page">
      <section className="purchase-success-card">
        <div className="success-mark">✓</div>
        <div className="section-label">Pago exitoso</div>
        <h1>Tu reserva esta confirmada</h1>
        <p>Guarda el número de reserva para check-in, rastreo y cualquier consulta del viaje.</p>

        <div className="reservation-code-box">
          <span>Número de reserva</span>
          <strong>{summary.reservationCodes.join(' / ') || '-'}</strong>
        </div>

        <div className="purchase-success-grid">
          <div>
            <span>Total pagado</span>
            <strong>{formatMoney(summary.total, summary.currency)}</strong>
          </div>
          <div>
            <span>Pasajeros</span>
            <strong>{summary.passengerCount} pasajero(s)</strong>
          </div>
          <div>
            <span>Estado</span>
            <strong>Confirmada</strong>
          </div>
        </div>

        <div className="success-flight-list">
          {summary.flights.map((item, index) => (
            <article key={`${item.numeroVuelo}-${index}`}>
              <span>{item.label}</span>
              <strong>{item.numeroVuelo}</strong>
              <small>{item.route}</small>
              <small>{item.className} · quedan {item.plazasDisponibles} plazas</small>
            </article>
          ))}
        </div>

        <div className="checkout-actions">
          <button className="btn btn-outline" type="button" onClick={onGoHome}>Ir al inicio</button>
          <button className="btn btn-primary" type="button" onClick={onExplore}>Buscar otro vuelo</button>
        </div>
      </section>
    </main>
  );
}

function getTravelResults(flights, criteria, direction = 'departure') {
  if (!criteria) return [];

  const origin = direction === 'return' ? criteria.destination : criteria.origin;
  const destination = direction === 'return' ? criteria.origin : criteria.destination;
  const dateValue = direction === 'return' ? criteria.returnDate : criteria.departureDate;

  if (!origin || !destination) return [];

  return flights
    .filter((flight) => canPurchaseFlight(flight.estado))
    .filter((flight) => flightMatchesRoute(flight, origin, destination))
    .filter((flight) => !dateValue || toDateInputValue(new Date(flight.fechaVuelo)) === dateValue)
    .sort((first, second) => new Date(first.fechaVuelo) - new Date(second.fechaVuelo));
}

function DateFarePicker({ open, tripType, departureDate, returnDate, flights = [], origin = '', destination = '', passengerCount = 1, currency = 'GTQ', onClose, onApply }) {
  const [draftDeparture, setDraftDeparture] = useState(departureDate);
  const [draftReturn, setDraftReturn] = useState(returnDate);
  const [selecting, setSelecting] = useState(departureDate && tripType === 'roundtrip' ? 'return' : 'departure');
  const [visibleStart, setVisibleStart] = useState(0);
  const [monthsPerView, setMonthsPerView] = useState(() => (window.innerWidth <= 680 ? 1 : 2));

  useEffect(() => {
    if (open) {
      setDraftDeparture(departureDate);
      setDraftReturn(returnDate);
      setSelecting(departureDate && tripType === 'roundtrip' ? 'return' : 'departure');
      setVisibleStart(0);
    }
  }, [departureDate, open, returnDate, tripType]);

  useEffect(() => {
    const updateViewport = () => {
      setMonthsPerView(window.innerWidth <= 680 ? 1 : 2);
    };

    updateViewport();
    window.addEventListener('resize', updateViewport);
    return () => window.removeEventListener('resize', updateViewport);
  }, []);

  if (!open) return null;

  const currentDate = new Date();
  const todayStart = new Date(currentDate.getFullYear(), currentDate.getMonth(), currentDate.getDate());
  const firstMonth = currentDate.getFullYear() === 2026 ? currentDate.getMonth() : 0;
  const months = Array.from({ length: 12 - firstMonth }, (_, index) => new Date(2026, firstMonth + index, 1));
  const visibleMonths = months.slice(visibleStart, visibleStart + monthsPerView);
  const canGoPrev = visibleStart > 0;
  const canGoNext = visibleStart + monthsPerView < months.length;

  const selectDate = (date) => {
    const value = toDateInputValue(date);

    if (tripType === 'oneway') {
      setDraftDeparture(value);
      setDraftReturn('');
      onApply({ departureDate: value, returnDate: '' });
      onClose();
      return;
    }

    if (selecting === 'departure') {
      setDraftDeparture(value);
      if (draftReturn && !isAfterDateValue(draftReturn, value)) {
        setDraftReturn('');
      }
      setSelecting('return');
      return;
    }

    if (draftDeparture && !isAfterDateValue(value, draftDeparture)) {
      return;
    }

    setDraftReturn(value);
    onApply({ departureDate: draftDeparture, returnDate: value });
    onClose();
  };

  const reset = () => {
    setDraftDeparture('');
    setDraftReturn('');
    setSelecting('departure');
  };

  const apply = () => {
    if (tripType === 'roundtrip' && (!draftDeparture || !isAfterDateValue(draftReturn, draftDeparture))) {
      setDraftReturn('');
      setSelecting('return');
      return;
    }
    onApply({ departureDate: draftDeparture, returnDate: tripType === 'roundtrip' ? draftReturn : '' });
    onClose();
  };

  const goPrev = () => {
    setVisibleStart((current) => Math.max(0, current - monthsPerView));
  };

  const goNext = () => {
    setVisibleStart((current) => Math.min(months.length - monthsPerView, current + monthsPerView));
  };

  const renderMonth = (monthDate) => {
    const monthName = new Intl.DateTimeFormat('es-GT', { month: 'long' }).format(monthDate);
    const year = monthDate.getFullYear();
    const month = monthDate.getMonth();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const startDay = new Date(year, month, 1).getDay();
    const blanks = Array.from({ length: startDay }, (_, index) => `blank-${index}`);
    const days = Array.from({ length: daysInMonth }, (_, index) => new Date(year, month, index + 1));

    return (
      <div className="fare-month" key={`${year}-${month}`}>
        <h3>{monthName}</h3>
        <div className="fare-weekdays">
          {['D', 'L', 'M', 'X', 'J', 'V', 'S'].map((day) => <span key={day}>{day}</span>)}
        </div>
        <div className="fare-days">
          {blanks.map((blank) => <span className="fare-blank" key={blank}></span>)}
          {days.map((date) => {
            const value = toDateInputValue(date);
            const isPastDate = date < todayStart;
            const isBeforeDeparture = selecting === 'return' && draftDeparture && !isAfterDateValue(value, draftDeparture);
            const selected = sameDateValue(date, draftDeparture) || sameDateValue(date, draftReturn);
            const inRange = draftDeparture && draftReturn && date > dateFromInputValue(draftDeparture) && date < dateFromInputValue(draftReturn);
            const fareDirection = selecting === 'return' ? 'return' : 'departure';
            const fare = lowestRouteFareForDate({
              date,
              flights,
              origin,
              destination,
              direction: fareDirection,
              passengerCount
            });
            const bestFareLimit = 820 * Math.max(1, Number(passengerCount || 1));
            const disabled = isPastDate || isBeforeDeparture || fare === null;

            return (
              <button
                className={`fare-day ${selected ? 'selected' : ''} ${inRange ? 'in-range' : ''} ${fare !== null && fare <= bestFareLimit ? 'best-fare' : ''}`}
                type="button"
                key={value}
                onClick={() => selectDate(date)}
                disabled={disabled}
              >
                <strong>{date.getDate()}</strong>
                <small>{fare === null ? 'Sin vuelos' : formatCompactFare(fare, currency)}</small>
              </button>
            );
          })}
        </div>
      </div>
    );
  };

  return (
    <div className="modal-backdrop" role="presentation" onMouseDown={onClose}>
      <div className="fare-picker" role="dialog" aria-modal="true" onMouseDown={(event) => event.stopPropagation()}>
        <div className="fare-picker-top">
          <div className="fare-picker-mode">
            <button className="fare-trip-button" type="button">{tripType === 'roundtrip' ? 'Ida y vuelta' : 'Solo ida'}</button>
            <button className="fare-reset" type="button" onClick={reset}>Restablecer</button>
          </div>
          <div className="fare-date-summary">
            <span className={selecting === 'departure' ? 'active' : ''}>Salida {draftDeparture && <strong>{formatShortDate(draftDeparture)}</strong>}</span>
            {tripType === 'roundtrip' && <span className={selecting === 'return' ? 'active' : ''}>Vuelta {draftReturn && <strong>{formatShortDate(draftReturn)}</strong>}</span>}
          </div>
          <div className="fare-month-nav">
            <button className="fare-month-arrow" type="button" onClick={goPrev} disabled={!canGoPrev} aria-label="Mes anterior">←</button>
            <button className="fare-month-arrow" type="button" onClick={goNext} disabled={!canGoNext} aria-label="Mes siguiente">→</button>
          </div>
        </div>
        <div className="fare-picker-body">
          {visibleMonths.map(renderMonth)}
        </div>
        <div className="fare-picker-footer">
          <div className="fare-picker-legend">
            <span>{selecting === 'return' ? 'Precios de vuelta' : 'Precios de ida'} para la ruta seleccionada</span>
            <span className="fare-legend-low">Los precios en verde son los mas bajos</span>
          </div>
          <button className="fare-done" type="button" onClick={apply} disabled={!draftDeparture || (tripType === 'roundtrip' && !isAfterDateValue(draftReturn, draftDeparture))}>Hecho</button>
        </div>
      </div>
    </div>
  );
}

function AirportCombobox({ label, value, airportOptions, placeholder, onChange }) {
  const datalistId = `${label.toLowerCase()}-airports`;

  return (
    <label>
      <span className="field-title"><span className="field-icon">{label === 'Origen' ? '↗' : '⌖'}</span>{label}</span>
      <input
        list={datalistId}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        placeholder={placeholder}
      />
      <datalist id={datalistId}>
        {airportOptions.map((airport) => (
          <option value={airportLabel(airport)} key={`${label}-${airport.name}`} />
        ))}
      </datalist>
    </label>
  );
}

function PassengerPicker({ open, groups, onClose, onApply }) {
  const [draftGroups, setDraftGroups] = useState(groups);

  useEffect(() => {
    if (open) setDraftGroups(groups);
  }, [groups, open]);

  if (!open) return null;

  const updateGroup = (key, delta) => {
    setDraftGroups((current) => {
      const minimum = key === 'adults' ? 1 : 0;
      return { ...current, [key]: Math.max(minimum, Number(current[key] || 0) + delta) };
    });
  };

  return (
    <div className="modal-backdrop" role="presentation" onMouseDown={onClose}>
      <div className="passenger-picker" role="dialog" aria-modal="true" onMouseDown={(event) => event.stopPropagation()}>
        <h2>¿Quiénes vuelan?</h2>
        {PASSENGER_GROUPS.map((group) => (
          <div className="passenger-counter-row" key={group.key}>
            <div>
              <strong>{group.label}</strong>
              <small>{group.hint}</small>
            </div>
            <button type="button" onClick={() => updateGroup(group.key, -1)} disabled={group.key === 'adults' && draftGroups[group.key] <= 1}>
              <span aria-hidden="true">−</span>
            </button>
            <span>{draftGroups[group.key]}</span>
            <button type="button" onClick={() => updateGroup(group.key, 1)}>
              <span aria-hidden="true">+</span>
            </button>
          </div>
        ))}
        <button className="passenger-confirm" type="button" onClick={() => { onApply(draftGroups); onClose(); }}>Confirmar</button>
      </div>
    </div>
  );
}

function TravelSearchSection({ flights, airports = [], currency, onExplore }) {
  const airportOptions = useMemo(() => {
    const values = new Map();
    airports.forEach((airport) => {
      const option = toAirportOption(airport);
      if (option.id && option.name) values.set(normalize(option.name), option);
    });

    KNOWN_AIRPORTS.forEach((airport) => {
      if (!values.has(normalize(airport.name))) values.set(normalize(airport.name), airport);
    });

    flights.forEach((flight) => {
      if (flight.origen && !values.has(normalize(flight.origen))) {
        values.set(normalize(flight.origen), { name: flight.origen, country: airportCountryFallback(flight.origen), aliases: [] });
      }
      if (flight.destino && !values.has(normalize(flight.destino))) {
        values.set(normalize(flight.destino), { name: flight.destino, country: airportCountryFallback(flight.destino), aliases: [] });
      }
    });

    return Array.from(values.values()).sort((first, second) => airportLabel(first).localeCompare(airportLabel(second)));
  }, [airports, flights]);

  const [criteria, setCriteria] = useState({
    tripType: 'roundtrip',
    passengers: 1,
    passengerGroups: DEFAULT_PASSENGER_GROUPS,
    origin: '',
    destination: '',
    departureDate: '',
    returnDate: ''
  });
  const [datePickerOpen, setDatePickerOpen] = useState(false);
  const [passengerPickerOpen, setPassengerPickerOpen] = useState(false);

  const updateCriteria = (field, value) => {
    setCriteria((current) => {
      if (field === 'tripType' && value === 'oneway') {
        return { ...current, tripType: value, returnDate: '' };
      }

      if (field === 'departureDate') {
        return {
          ...current,
          departureDate: value,
          returnDate: isAfterDateValue(current.returnDate, value) ? current.returnDate : ''
        };
      }

      if (field === 'returnDate') {
        return { ...current, returnDate: isAfterDateValue(value, current.departureDate) ? value : '' };
      }

      return { ...current, [field]: value };
    });
  };

  const updatePassengerGroups = (groups) => {
    setCriteria((current) => ({
      ...current,
      passengerGroups: groups,
      passengers: passengerCountFromGroups(groups)
    }));
  };

  const submit = (event) => {
    event.preventDefault();
    onExplore({
      ...criteria,
      origin: resolveAirportQuery(criteria.origin, airportOptions),
      destination: resolveAirportQuery(criteria.destination, airportOptions),
      currency
    });
  };

  return (
    <section className="travel-search-section" id="explorar">
      <div className="travel-shell">
        <div className="section-label">Explorar vuelos</div>
        <h2 className="section-title">Encuentra tu siguiente destino</h2>
        <form className="travel-search-card" onSubmit={submit}>
          <div className="travel-topbar">
            <label>
              <span className="field-title"><span className="field-icon">⇄</span>Tipo</span>
              <select value={criteria.tripType} onChange={(event) => updateCriteria('tripType', event.target.value)}>
                <option value="roundtrip">Ida y vuelta</option>
                <option value="oneway">Solo ida</option>
              </select>
            </label>
            <label>
              <span className="field-title"><span className="field-icon">☉</span>Personas</span>
              <button className="date-field-button" type="button" onClick={() => setPassengerPickerOpen(true)}>
                {passengerCountFromGroups(criteria.passengerGroups)} persona(s)
              </button>
            </label>
          </div>

          <div className="travel-fields">
            <AirportCombobox
              label="Origen"
              value={criteria.origin}
              airportOptions={airportOptions}
              placeholder="Guatemala, Miami, Colombia..."
              onChange={(value) => updateCriteria('origin', value)}
            />
            <AirportCombobox
              label="Destino"
              value={criteria.destination}
              airportOptions={airportOptions}
              placeholder="Guatemala, Miami, Colombia..."
              onChange={(value) => updateCriteria('destination', value)}
            />
            <label>
              <span className="field-title"><span className="field-icon">▣</span>Salida</span>
              <button className="date-field-button" type="button" onClick={() => setDatePickerOpen(true)}>
                {criteria.departureDate ? formatShortDate(criteria.departureDate) : 'Salida'}
              </button>
            </label>
            {criteria.tripType === 'roundtrip' && (
              <label>
                <span className="field-title"><span className="field-icon">↩</span>Vuelta</span>
                <button className="date-field-button" type="button" onClick={() => setDatePickerOpen(true)}>
                  {criteria.returnDate ? formatShortDate(criteria.returnDate) : 'Vuelta'}
                </button>
              </label>
            )}
          </div>

          <button className="explore-button" type="submit">Explorar</button>
        </form>
        <PassengerPicker
          open={passengerPickerOpen}
          groups={criteria.passengerGroups}
          onClose={() => setPassengerPickerOpen(false)}
          onApply={updatePassengerGroups}
        />
        <DateFarePicker
          open={datePickerOpen}
          tripType={criteria.tripType}
          departureDate={criteria.departureDate}
          returnDate={criteria.returnDate}
          flights={flights}
          origin={resolveAirportQuery(criteria.origin, airportOptions)}
          destination={resolveAirportQuery(criteria.destination, airportOptions)}
          passengerCount={passengerCountFromGroups(criteria.passengerGroups)}
          currency={currency}
          onClose={() => setDatePickerOpen(false)}
          onApply={({ departureDate, returnDate }) => {
            updateCriteria('departureDate', departureDate);
            updateCriteria('returnDate', isAfterDateValue(returnDate, departureDate) ? returnDate : '');
          }}
        />
      </div>
    </section>
  );
}

function TravelResultsView({ criteria, flights, user, onBack, onRequireLogin, onBuyFlight, buyingFlightId }) {
  const outboundResults = useMemo(() => getTravelResults(flights, criteria, 'departure'), [criteria, flights]);
  const returnResults = useMemo(() => getTravelResults(flights, criteria, 'return'), [criteria, flights]);
  const [expandedFlightId, setExpandedFlightId] = useState(null);
  const [pendingUpgrade, setPendingUpgrade] = useState(null);
  const [selectedDeparture, setSelectedDeparture] = useState(null);
  const passengerCount = passengerCountFromCriteria(criteria);
  const currency = criteria.currency || 'GTQ';
  const isRoundtrip = criteria.tripType === 'roundtrip';
  const activeDirection = isRoundtrip && selectedDeparture ? 'return' : 'departure';
  const results = activeDirection === 'return' ? returnResults : outboundResults;
  const activeDate = activeDirection === 'return' ? criteria.returnDate : criteria.departureDate;
  const activeTitle = activeDirection === 'return' ? 'Elige tu vuelo de vuelta' : 'Elige tu vuelo de ida';

  useEffect(() => {
    setExpandedFlightId(results[0]?.id || null);
  }, [results]);

  useEffect(() => {
    setSelectedDeparture(null);
    setPendingUpgrade(null);
  }, [criteria]);

  const completeFareSelection = (flight, family) => {
    if (isRoundtrip && activeDirection === 'departure') {
      setSelectedDeparture({ flight, selectedClass: family.className, family });
      setPendingUpgrade(null);
      setExpandedFlightId(null);
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    if (isRoundtrip && selectedDeparture) {
      onBuyFlight([
        {
          flight: selectedDeparture.flight,
          selectedClass: selectedDeparture.selectedClass
        },
        {
          flight,
          selectedClass: family.className
        }
      ], null, criteria);
      setPendingUpgrade(null);
      return;
    }

    onBuyFlight(flight, family.className, criteria);
    setPendingUpgrade(null);
  };

  const chooseFare = (flight, family) => {
    if (!user) {
      onRequireLogin();
      return;
    }

    const nextFamily = nextTariffFamily(family);
    if (nextFamily) {
      setPendingUpgrade({ flight, family, nextFamily, direction: activeDirection });
      return;
    }

    completeFareSelection(flight, family);
  };

  return (
    <section className="travel-results-section">
      <div className="travel-results">
        <button className="back-button" type="button" onClick={onBack}>Volver al buscador</button>
        <div className="travel-results-header">
          <div>
            <div className="section-label">Resultados</div>
            <h2>{activeTitle}</h2>
          </div>
          <span>{results.length} opciones - {isRoundtrip ? (activeDirection === 'return' ? 'vuelta' : 'ida') : 'solo ida'}</span>
        </div>

        {selectedDeparture && (
          <div className="selected-leg-banner">
            <div>
              <span>Ida seleccionada</span>
              <strong>{selectedDeparture.flight.numeroVuelo} - {selectedDeparture.flight.origen} a {selectedDeparture.flight.destino}</strong>
              <small>{formatShortDate(criteria.departureDate)} - {tariffByClassName(selectedDeparture.selectedClass).name}</small>
            </div>
            <button type="button" onClick={() => setSelectedDeparture(null)}>Cambiar ida</button>
          </div>
        )}

        {results.length === 0 && (
          <div className="board-empty">No hay vuelos programados para esta ruta y fecha.</div>
        )}

        {results.map((flight) => {
          const duration = estimateDurationMinutes(flight);
          const arrival = addMinutesToDate(flight.fechaVuelo, duration);
          const expanded = expandedFlightId === flight.id;
          return (
            <article className="flight-result-card" key={`${activeDirection}-${flight.id}`}>
              <button className="flight-result-summary" type="button" onClick={() => setExpandedFlightId(expanded ? null : flight.id)}>
                <span className="airline-mark">{flight.aerolinea?.slice(0, 2).toUpperCase() || 'AV'}</span>
                <span>
                  <strong>{formatTime(flight.fechaVuelo)} - {formatTime(arrival)}</strong>
                  <small>{flight.numeroVuelo} - {flight.aerolinea}</small>
                </span>
                <span>
                  <strong>{flight.origen} - {flight.destino}</strong>
                  <small>{activeDate ? formatShortDate(activeDate) : formatShortDate(toDateInputValue(new Date(flight.fechaVuelo)))}</small>
                </span>
                <span>
                  <strong>{hasTechnicalStop(flight) ? 'Con parada' : 'Directo'}</strong>
                  <small>{duration} min - {passengerCount} pasajero(s)</small>
                </span>
                <span className="result-price">
                  <strong>{formatMoney(calculateFlightFare('economica', passengerCount), currency)}</strong>
                  <small>desde</small>
                </span>
                <span className="expand-indicator">{expanded ? 'Ocultar' : 'Ver tarifas'}</span>
              </button>

              {expanded && (
                <div className="tariff-grid">
                  {TARIFF_FAMILIES.map((family) => {
                    const fare = calculateFlightFare(family.className, passengerCount);
                    return (
                      <section className={`tariff-card ${family.code === 'ejecutiva' ? 'recommended' : ''}`} key={family.code}>
                        {family.code === 'ejecutiva' && <span className="tariff-badge">Recomendada</span>}
                        <h3>{family.name}</h3>
                        <p>{family.tagline}</p>
                        <strong className="tariff-price">{formatMoney(fare, currency)}</strong>
                        <ul>
                          {family.benefits.map(([state, benefit]) => (
                            <li className={state === 'check' ? 'included' : 'excluded'} key={benefit}>
                              <span className="tariff-icon" aria-hidden="true">{state === 'check' ? '✓' : '×'}</span>
                              {benefit}
                            </li>
                          ))}
                        </ul>
                        <button
                          className={family.code === 'ejecutiva' ? 'btn btn-primary' : 'btn btn-outline'}
                          type="button"
                          onClick={() => chooseFare(flight, family)}
                          disabled={buyingFlightId === flight.id || buyingFlightId === 'bundle'}
                        >
                          {buyingFlightId === flight.id || buyingFlightId === 'bundle' ? 'Procesando' : `Elegir ${family.name}`}
                        </button>
                      </section>
                    );
                  })}
                </div>
              )}
            </article>
          );
        })}
      </div>

      {pendingUpgrade && (
        <div className="modal-backdrop" role="presentation">
          <div className="upgrade-modal" role="dialog" aria-modal="true">
            <button className="modal-close" type="button" onClick={() => setPendingUpgrade(null)}>×</button>
            <div className="section-label">Mejora tu tarifa</div>
            <h2>Quieres subir a {pendingUpgrade.nextFamily.name}?</h2>
            <p>{pendingUpgrade.nextFamily.name} agrega mas beneficios que {pendingUpgrade.family.name} para este viaje.</p>
            <div className="upgrade-comparison">
              <span>{pendingUpgrade.family.name}</span>
              <strong>{formatMoney(calculateFlightFare(pendingUpgrade.family.className, passengerCount), currency)}</strong>
              <span>{pendingUpgrade.nextFamily.name}</span>
              <strong>{formatMoney(calculateFlightFare(pendingUpgrade.nextFamily.className, passengerCount), currency)}</strong>
            </div>
            <button
              className="btn btn-primary"
              type="button"
              onClick={() => {
                completeFareSelection(pendingUpgrade.flight, pendingUpgrade.nextFamily);
              }}
            >
              Mejorar a {pendingUpgrade.nextFamily.name}
            </button>
            <button
              className="continue-restricted"
              type="button"
              onClick={() => {
                completeFareSelection(pendingUpgrade.flight, pendingUpgrade.family);
              }}
            >
              Continuar con {pendingUpgrade.family.name}
            </button>
          </div>
        </div>
      )}
    </section>
  );
}

function CartView({ items, user, onBack, onRequireLogin, onCheckoutItem, onRemoveItem }) {
  const visibleItems = items.filter(Boolean);
  const cartTotal = visibleItems.reduce((sum, item) => (
    sum + calculateFlightFare(item.selectedClass || 'economica', passengerCountFromCriteria(item.criteria))
  ), 0);
  const currency = visibleItems[0]?.criteria?.currency || 'GTQ';

  return (
    <main className="cart-page">
      <section className="cart-shell">
        <button className="back-button" type="button" onClick={onBack}>Volver</button>
        <div className="section-label">Carrito de compras</div>
        <h1>Vuelos seleccionados</h1>
        {visibleItems.length === 0 && <div className="board-empty">Tu carrito esta vacio.</div>}

        <div className="cart-list">
          {visibleItems.map((item, index) => {
            const passengerCount = passengerCountFromCriteria(item.criteria);
            const selectedTariff = tariffByClassName(item.selectedClass || 'economica');
            const fare = calculateFlightFare(item.selectedClass || 'economica', passengerCount);
            const arrival = addMinutesToDate(item.fechaVuelo, estimateDurationMinutes(item));
            const tripLabel = item.criteria?.tripType === 'roundtrip' ? 'Ida y vuelta' : 'Solo ida';
            const routeLabel = item.origen && item.destino ? `${item.origen} - ${item.destino}` : 'Ruta pendiente';
            const timeLabel = item.fechaVuelo ? `${formatTime(item.fechaVuelo)} - ${formatTime(arrival)}` : 'Horario pendiente';

            return (
              <article className="cart-flight-card" key={item.cartId || `${item.id || item.vueloId || 'cart'}-${index}`}>
                <div className="cart-flight-icon">✈</div>
                <div className="cart-flight-main">
                  <div className="cart-flight-title">
                    <strong>{item.numeroVuelo || 'Vuelo seleccionado'} - {item.aerolinea || 'Aerolínea pendiente'}</strong>
                    <span>{formatMoney(fare, item.criteria?.currency || 'GTQ')}</span>
                  </div>
                  <div className="cart-flight-route">
                    <span>{routeLabel}</span>
                    <b>{timeLabel}</b>
                  </div>
                  <div className="cart-flight-meta">
                    <span><i>◷</i>{tripLabel}</span>
                    <span><i>☉</i>{passengerCount} pasajero(s)</span>
                    <span><i>▣</i>{selectedTariff.name}</span>
                    <span><i>$</i>{item.criteria?.currency || 'GTQ'}</span>
                  </div>
                </div>
                <div className="cart-flight-actions">
                  <button className="btn btn-primary" type="button" onClick={() => (user ? onCheckoutItem(item) : onRequireLogin())}>Comprar este vuelo</button>
                  <button className="btn btn-outline" type="button" onClick={() => onRemoveItem(item.cartId)}>Eliminar</button>
                </div>
              </article>
            );
          })}
        </div>

        {visibleItems.length > 0 && (
          <aside className="cart-total-box">
            <span>Subtotal del carrito</span>
            <strong>{formatMoney(cartTotal, currency)}</strong>
          </aside>
        )}
      </section>
    </main>
  );
}

function DestinationSection({ destinations, onDestinationClick }) {
  const rankedDestinations = [...destinations]
    .sort((first, second) => {
      const firstScore = Number(first.totalBusquedas || 0) + Number(first.totalClicks || 0);
      const secondScore = Number(second.totalBusquedas || 0) + Number(second.totalClicks || 0);
      return secondScore - firstScore || destinationReportName(first).localeCompare(destinationReportName(second));
    })
    .slice(0, 5);

  return (
    <section className="section compact-section" id="destinos">
      <div className="section-label">Interes de pasajeros</div>
      <h2 className="section-title">Destinos mas buscados</h2>
      <div className="destination-grid">
        {rankedDestinations.length === 0 && <p className="muted-text">Los destinos apareceran cuando el reporte tenga datos.</p>}
        {rankedDestinations.map((destination, index) => (
          <button className="destination-card destination-button" type="button" key={destinationReportId(destination)} onClick={() => onDestinationClick(destination)}>
            <div className="destination-visual">
              <span>{String(index + 1).padStart(2, '0')}</span>
              <small>{destinationReportName(destination).slice(0, 3).toUpperCase()}</small>
            </div>
            <strong>{destinationReportName(destination)}</strong>
            <div className="destination-metrics">
              <small>{Number(destination.totalBusquedas || 0) + Number(destination.totalClicks || 0)} puntos de interes</small>
              <small>{destination.totalBusquedas} búsquedas</small>
              <small>{destination.totalClicks} clicks</small>
            </div>
          </button>
        ))}
      </div>
    </section>
  );
}

function ServicesSection() {
  return (
    <section className="section" id="servicios">
      <div className="section-label">Para viajeros</div>
      <h2 className="section-title">Servicios del aeropuerto</h2>
      <div className="services-grid">
        {services.map((service) => (
          <article className="service-card" key={service.title}>
            <div className="service-icon">{service.code}</div>
            <h3>{service.title}</h3>
            <p>{service.text}</p>
          </article>
        ))}
      </div>
    </section>
  );
}

function OperationsSection({ baggage, incidents }) {
  return (
    <section className="section" id="operaciones">
      <div className="section-label">Operacion diaria</div>
      <h2 className="section-title">Equipaje e incidentes</h2>
      <div className="operations-grid">
        <div className="operation-panel">
          <h3>Equipaje reciente</h3>
          {baggage.length === 0 && <p className="muted-text">Sin movimientos de equipaje para mostrar.</p>}
          {baggage.map((item) => (
            <div className="operation-row" key={item.id}>
              <div>
                <strong>{item.codigoBarras}</strong>
                <small>{item.numeroVuelo} - {formatDate(item.fechaRegistro)}</small>
              </div>
              <span className={`status ${statusClassName(item.estado)}`}>{item.estado}</span>
            </div>
          ))}
        </div>

        <div className="operation-panel">
          <h3>Incidentes</h3>
          {incidents.length === 0 && <p className="muted-text">Sin incidentes para mostrar.</p>}
          {incidents.map((incident) => (
            <div className="operation-row" key={incident.id}>
              <div>
                <strong>{incident.tipoIncidente}</strong>
                <small>{incident.ubicacion} · {formatDate(incident.fechaIncidente)}</small>
              </div>
              <span className={`status ${statusClassName(incident.estado)}`}>{incident.severidad}</span>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

function LocationSection() {
  return (
    <section className="location-section" id="ubicacion">
      <div className="location-inner">
        <div>
          <div className="section-label">Como llegar</div>
          <h2 className="section-title">Ubicación</h2>
          <div className="location-detail">
            <span>⌖</span>
            <p>7a Avenida 11-03, Zona 13, Ciudad de Guatemala.</p>
          </div>
          <div className="location-detail">
            <span>⇄</span>
            <p>Acceso a taxis autorizados, parqueo y transporte hacia puntos principales de la ciudad.</p>
          </div>
          <div className="location-detail">
            <span>◔</span>
            <p>Operación aeroportuaria disponible todos los días del año.</p>
          </div>
        </div>
        <div className="map-box">
          <iframe
            src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3861.0!2d-90.527775!3d14.583272!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x8589a3f8a0e3e405%3A0x5e3c8e66a7ef44e!2sAeropuerto%20Internacional%20La%20Aurora!5e0!3m2!1ses!2sgt!4v1700000000000"
            loading="lazy"
            referrerPolicy="no-referrer-when-downgrade"
            title="Mapa Aeropuerto La Aurora"
          />
        </div>
      </div>
    </section>
  );
}

function MyTripsSection({ user, flights, onRequireLogin }) {
  const [data, setData] = useState({ reservations: [], checkIns: [], passes: [] });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  useEffect(() => {
    if (!user?.pasajeroId) return undefined;

    let active = true;
    setLoading(true);
    setMessage('');

    Promise.allSettled([
      api.reservations(user.pasajeroId, 100),
      api.checkIns(200),
      api.boardingPasses(200)
    ]).then((results) => {
      if (!active) return;
      setData({
        reservations: results[0].status === 'fulfilled' ? results[0].value : [],
        checkIns: results[1].status === 'fulfilled' ? results[1].value : [],
        passes: results[2].status === 'fulfilled' ? results[2].value : []
      });
      if (results.some((result) => result.status === 'rejected')) {
        setMessage('No se pudo cargar todo el historial, pero mostramos lo disponible.');
      }
    }).finally(() => {
      if (active) setLoading(false);
    });

    return () => {
      active = false;
    };
  }, [user?.pasajeroId]);

  if (!user) {
    return (
      <main className="tab-page">
        <section className="section passenger-tool">
          <div className="section-label">Mis viajes</div>
          <h1>Entra para ver tus reservas</h1>
          <button className="btn btn-primary" type="button" onClick={onRequireLogin}>Iniciar sesión</button>
        </section>
      </main>
    );
  }

  const flightMap = new Map(flights.map((flight) => [Number(flight.id), flight]));
  const checkInByReservation = new Map(data.checkIns.map((checkIn) => [Number(checkIn.reservaId), checkIn]));
  const passByCheckIn = new Map(data.passes.map((pass) => [Number(pass.checkInId), pass]));

  return (
    <main className="tab-page">
      <section className="section passenger-tool">
        <div className="section-label">Mis viajes</div>
        <h1>Reservas y tarjetas</h1>
        {message && <div className="connection-alert">{message}</div>}
        {loading && <div className="board-empty"><span className="loader"></span>Cargando reservas</div>}
        {!loading && data.reservations.length === 0 && <div className="board-empty">Aun no tienes reservas registradas.</div>}
        <div className="passenger-card-grid">
          {data.reservations.map((reservation) => {
            const flight = flightMap.get(Number(reservation.vueloId));
            const checkIn = checkInByReservation.get(Number(reservation.id));
            const boardingPass = checkIn ? passByCheckIn.get(Number(checkIn.id)) : null;

            return (
              <article className="passenger-card" key={reservation.id}>
                <div>
                  <span className={`status ${statusClassName(reservation.estado)}`}>{reservation.estado}</span>
                  <h2>{reservation.numeroVuelo}</h2>
                  <p>{flight ? `${flight.origen} - ${flight.destino}` : 'Ruta pendiente'}</p>
                </div>
                <dl>
                  <div><dt>Reserva</dt><dd>{reservation.codigo}</dd></div>
                  <div><dt>Fecha</dt><dd>{formatDate(flight?.fechaVuelo || reservation.fechaReserva)}</dd></div>
                  <div><dt>Clase</dt><dd>{reservation.clase}</dd></div>
                  <div><dt>Check-in</dt><dd>{checkIn ? checkIn.estado : 'Pendiente'}</dd></div>
                </dl>
                {boardingPass && (
                  <div className="boarding-pass-mini">
                    <strong>{boardingPass.codigoQr}</strong>
                    <span>Grupo {boardingPass.grupoAbordaje || 'A'} - Zona {boardingPass.zona || '1'}</span>
                  </div>
                )}
              </article>
            );
          })}
        </div>
      </section>
    </main>
  );
}

function CheckInSection({ user, flights, onRequireLogin }) {
  const [reservations, setReservations] = useState([]);
  const [checkIns, setCheckIns] = useState([]);
  const [selectedReservationId, setSelectedReservationId] = useState('');
  const [checkInQuery, setCheckInQuery] = useState('');
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState('');

  const loadData = useCallback(async () => {
    if (!user?.pasajeroId) return;

    setLoading(true);
    setMessage('');
    try {
      const [reservationRows, checkInRows] = await Promise.all([
        api.reservations(user.pasajeroId, 100),
        api.checkIns(200)
      ]);
      setReservations(reservationRows);
      setCheckIns(checkInRows);
      setSelectedReservationId((current) => current || reservationRows[0]?.id?.toString() || '');
    } catch (requestError) {
      setMessage(`No se pudieron cargar tus reservas: ${requestError.message}`);
    } finally {
      setLoading(false);
    }
  }, [user?.pasajeroId]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  if (!user) {
    return (
      <main className="tab-page">
        <section className="section passenger-tool">
          <div className="section-label">Check-in</div>
          <h1>Confirma tu vuelo en línea</h1>
          <button className="btn btn-primary" type="button" onClick={onRequireLogin}>Iniciar sesión</button>
        </section>
      </main>
    );
  }

  const flightMap = new Map(flights.map((flight) => [Number(flight.id), flight]));
  const normalizedCheckInQuery = normalize(checkInQuery);
  const matchingReservations = reservations.filter((reservation) => {
    if (!normalizedCheckInQuery) return true;

    return [reservation.codigo, reservation.numeroVuelo]
      .some((value) => normalize(value).includes(normalizedCheckInQuery));
  });
  const typedReservation = normalizedCheckInQuery
    ? matchingReservations.find((reservation) =>
      normalize(reservation.codigo) === normalizedCheckInQuery ||
      normalize(reservation.numeroVuelo) === normalizedCheckInQuery) || matchingReservations[0]
    : null;
  const selectedReservation = typedReservation || reservations.find((reservation) => String(reservation.id) === String(selectedReservationId));
  const existingCheckIn = selectedReservation
    ? checkIns.find((checkIn) => Number(checkIn.reservaId) === Number(selectedReservation.id))
    : null;
  const selectedFlight = selectedReservation ? flightMap.get(Number(selectedReservation.vueloId)) : null;

  const submitCheckIn = async () => {
    if (!selectedReservation || existingCheckIn) return;

    setSaving(true);
    setMessage('');
    try {
      const checkIn = await api.createCheckIn({
        reservaId: selectedReservation.id,
        pasajeroId: user.pasajeroId,
        vueloId: selectedReservation.vueloId,
        fechaHora: new Date().toISOString(),
        metodo: 'web',
        estado: 'COMPLETADO'
      });

      await api.createBoardingPass({
        checkInId: checkIn.id,
        codigoQr: boardingCode(selectedReservation.id, user.pasajeroId),
        grupoAbordaje: 'A',
        zona: '1',
        fechaEmision: new Date().toISOString()
      });

      setMessage('Check-in completado y tarjeta de embarque emitida.');
      await loadData();
    } catch (requestError) {
      setMessage(`No se pudo completar el check-in: ${requestError.message}`);
    } finally {
      setSaving(false);
    }
  };

  return (
    <main className="tab-page">
      <section className="section passenger-tool">
        <div className="section-label">Check-in</div>
          <h1>Check-in en línea</h1>
        {message && <div className="connection-alert">{message}</div>}
        <div className="tool-grid">
          <div className="tool-panel">
            <label className="field">
              <span>Vuelo o reserva</span>
              <input
                value={checkInQuery}
                onChange={(event) => setCheckInQuery(event.target.value)}
                placeholder="Ej. AV2200X o R051..."
                list="checkin-reservations"
                disabled={loading || reservations.length === 0}
              />
              <datalist id="checkin-reservations">
                {reservations.map((reservation) => (
                  <option value={reservation.numeroVuelo} key={`flight-${reservation.id}`} />
                ))}
                {reservations.map((reservation) => (
                  <option value={reservation.codigo} key={`reservation-${reservation.id}`} />
                ))}
              </datalist>
            </label>
            <label className="field">
              <span>Coincidencias</span>
              <select
                value={selectedReservation?.id || selectedReservationId}
                onChange={(event) => {
                  setSelectedReservationId(event.target.value);
                  setCheckInQuery('');
                }}
                disabled={loading || matchingReservations.length === 0}
              >
                {matchingReservations.length === 0 && <option value="">Sin coincidencias</option>}
                {matchingReservations.slice(0, 25).map((reservation) => (
                  <option value={reservation.id} key={reservation.id}>{reservation.numeroVuelo} - {reservation.codigo}</option>
                ))}
              </select>
            </label>
            <button className="btn btn-primary" type="button" onClick={submitCheckIn} disabled={!selectedReservation || Boolean(existingCheckIn) || saving}>
              {saving ? 'Procesando' : existingCheckIn ? 'Check-in completado' : 'Completar check-in'}
            </button>
          </div>
          <div className="tool-panel">
            {!selectedReservation && <p className="muted-text">Selecciona una reserva para continuar.</p>}
            {selectedReservation && (
              <dl className="detail-list">
                <div><dt>Ruta</dt><dd>{selectedFlight ? `${selectedFlight.origen} - ${selectedFlight.destino}` : 'Pendiente'}</dd></div>
                <div><dt>Salida</dt><dd>{formatDate(selectedFlight?.fechaVuelo)}</dd></div>
                <div><dt>Reserva</dt><dd>{selectedReservation.codigo}</dd></div>
                <div><dt>Vuelo</dt><dd>{selectedReservation.numeroVuelo}</dd></div>
                <div><dt>Estado</dt><dd>{existingCheckIn ? existingCheckIn.estado : 'Pendiente'}</dd></div>
              </dl>
            )}
          </div>
        </div>
      </section>
    </main>
  );
}

function BaggageTrackerSection() {
  const [baggage, setBaggage] = useState([]);
  const [movements, setMovements] = useState([]);
  const [query, setQuery] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    setLoading(true);
    Promise.allSettled([
      api.baggage(80),
      api.baggageMovements(200)
    ]).then((results) => {
      setBaggage(results[0].status === 'fulfilled' ? results[0].value : []);
      setMovements(results[1].status === 'fulfilled' ? results[1].value : []);
    }).finally(() => setLoading(false));
  }, []);

  const visible = baggage.filter((item) =>
    [item.codigoBarras, item.numeroVuelo, item.estado]
      .some((value) => normalize(value).includes(normalize(query)))
  );
  const selected = visible[0];
  const selectedMovements = selected
    ? movements.filter((movement) => Number(movement.equipajeId) === Number(selected.id))
    : [];

  return (
    <main className="tab-page">
      <section className="section passenger-tool">
        <div className="section-label">Equipaje</div>
        <h1>Rastreo de maletas</h1>
        <div className="tool-grid">
          <div className="tool-panel">
            <input value={query} onChange={(event) => setQuery(event.target.value)} placeholder="Codigo de maleta o vuelo" />
            {loading && <div className="board-empty"><span className="loader"></span>Cargando equipaje</div>}
            {!loading && visible.slice(0, 8).map((item) => (
              <button className="tool-list-button" type="button" key={item.id} onClick={() => setQuery(item.codigoBarras)}>
                <strong>{item.codigoBarras}</strong>
                <span>{item.numeroVuelo} - {item.estado}</span>
              </button>
            ))}
          </div>
          <div className="tool-panel">
            {!selected && <p className="muted-text">Busca una maleta para ver su recorrido.</p>}
            {selected && (
              <>
                <h2>{selected.codigoBarras}</h2>
                <p>{selected.numeroVuelo} - {formatDate(selected.fechaRegistro)}</p>
                <span className={`status ${statusClassName(selected.estado)}`}>{selected.estado}</span>
                <div className="timeline">
                  {selectedMovements.length === 0 && <p className="muted-text">Sin movimientos registrados.</p>}
                  {selectedMovements.map((movement) => (
                    <div className="timeline-item" key={movement.id}>
                      <strong>{movement.ubicacion}</strong>
                      <span>{movement.estado} - {formatDate(movement.fechaHora)}</span>
                      {movement.observacion && <small>{movement.observacion}</small>}
                    </div>
                  ))}
                </div>
              </>
            )}
          </div>
        </div>
      </section>
    </main>
  );
}

const LOST_OBJECT_ESTADOS = ['REPORTADO', 'ENCONTRADO', 'ENTREGADO', 'NO_RECLAMADO'];

function AdminLostObjectPanel({ airports, onCreated }) {
  const [flights, setFlights] = useState([]);
  const [form, setForm] = useState({
    vueloId: '',
    descripcion: '',
    ubicacionEncontrado: '',
    estado: 'ENCONTRADO',
    reportantePrimerNombre: '',
    reportantePrimerApellido: '',
    contactoReportante: ''
  });
  const [message, setMessage] = useState('');
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    api.flights(200).then(setFlights).catch(() => setFlights([]));
  }, []);

  const update = (field, value) => setForm((f) => ({ ...f, [field]: value }));

  const submit = async (event) => {
    event.preventDefault();
    if (!form.descripcion.trim() || !form.ubicacionEncontrado.trim()) {
      setMessage('Descripción y ubicación son obligatorias.');
      return;
    }
    setSaving(true);
    setMessage('');
    try {
      await api.createLostObject({
        vueloId: form.vueloId ? Number(form.vueloId) : null,
        aeropuertoId: airports[0]?.id || 1,
        descripcion: form.descripcion.trim(),
        fechaReporte: new Date().toISOString(),
        ubicacionEncontrado: form.ubicacionEncontrado.trim(),
        estado: form.estado,
        reportantePrimerNombre: form.reportantePrimerNombre.trim() || null,
        reportanteSegundoNombre: null,
        reportantePrimerApellido: form.reportantePrimerApellido.trim() || null,
        reportanteSegundoApellido: null,
        contactoReportante: form.contactoReportante.trim() || null,
        fechaEntrega: null,
        reclamantePrimerNombre: null,
        reclamanteSegundoNombre: null,
        reclamantePrimerApellido: null,
        reclamanteSegundoApellido: null
      });
      setForm({ vueloId: '', descripcion: '', ubicacionEncontrado: '', estado: 'ENCONTRADO', reportantePrimerNombre: '', reportantePrimerApellido: '', contactoReportante: '' });
      setMessage('Objeto registrado correctamente.');
      onCreated();
    } catch (err) {
      setMessage(`Error al registrar: ${err.message}`);
    } finally {
      setSaving(false);
    }
  };

  return (
    <form className="tool-panel lost-object-form" onSubmit={submit}>
      <h2>Registrar objeto encontrado</h2>
      {message && <div className="connection-alert">{message}</div>}
      <label className="field">
        <span>Vuelo relacionado <small>(opcional)</small></span>
        <select value={form.vueloId} onChange={(e) => update('vueloId', e.target.value)}>
          <option value="">— Sin vuelo —</option>
          {flights.map((f) => (
            <option key={f.id} value={f.id}>{f.numeroVuelo} · {f.origen} → {f.destino} · {formatDate(f.fechaVuelo)}</option>
          ))}
        </select>
      </label>
      <label className="field"><span>Descripción del objeto</span><textarea value={form.descripcion} onChange={(e) => update('descripcion', e.target.value)} placeholder="Ej. mochila negra con etiqueta roja, laptop plateada..." required /></label>
      <label className="field"><span>Ubicación donde fue encontrado</span><input value={form.ubicacionEncontrado} onChange={(e) => update('ubicacionEncontrado', e.target.value)} placeholder="Ej. Puerta 12, Banda de equipaje 3" required /></label>
      <label className="field">
        <span>Estado</span>
        <select value={form.estado} onChange={(e) => update('estado', e.target.value)}>
          {LOST_OBJECT_ESTADOS.map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
      </label>
      <label className="field"><span>Nombre quien reporta <small>(opcional)</small></span><input value={form.reportantePrimerNombre} onChange={(e) => update('reportantePrimerNombre', e.target.value)} placeholder="Primer nombre" /></label>
      <label className="field"><span>Apellido quien reporta <small>(opcional)</small></span><input value={form.reportantePrimerApellido} onChange={(e) => update('reportantePrimerApellido', e.target.value)} placeholder="Primer apellido" /></label>
      <label className="field"><span>Contacto interno <small>(opcional)</small></span><input value={form.contactoReportante} onChange={(e) => update('contactoReportante', e.target.value)} placeholder="teléfono o email del personal" /></label>
      <button className="btn btn-primary" type="submit" disabled={saving}>{saving ? 'Guardando...' : 'Registrar objeto'}</button>
    </form>
  );
}

function LostObjectsSection({ airports, isAdmin }) {
  const [items, setItems] = useState([]);
  const [form, setForm] = useState({
    descripcion: '',
    reportanteNombre: '',
    contactoReportante: ''
  });
  const [message, setMessage] = useState('');
  const [saving, setSaving] = useState(false);

  const loadItems = useCallback(async () => {
    try {
      setItems(await api.lostObjects(50));
    } catch {
      setItems([]);
    }
  }, []);

  useEffect(() => {
    loadItems();
  }, [loadItems]);

  const updateForm = (field, value) => setForm((current) => ({ ...current, [field]: value }));

  const submit = async (event) => {
    event.preventDefault();
    setSaving(true);
    setMessage('');
    const nameParts = form.reportanteNombre.trim().split(/\s+/).filter(Boolean);
    const firstName = nameParts[0] || null;
    const lastName = nameParts.length > 1 ? nameParts.slice(1).join(' ') : null;

    try {
      await api.createLostObject({
        vueloId: null,
        aeropuertoId: airports[0]?.id || 1,
        descripcion: form.descripcion,
        fechaReporte: new Date().toISOString(),
        ubicacionEncontrado: 'Pendiente de revisión',
        estado: 'REPORTADO',
        reportantePrimerNombre: firstName,
        reportanteSegundoNombre: null,
        reportantePrimerApellido: lastName,
        reportanteSegundoApellido: null,
        contactoReportante: form.contactoReportante,
        fechaEntrega: null,
        reclamantePrimerNombre: null,
        reclamanteSegundoNombre: null,
        reclamantePrimerApellido: null,
        reclamanteSegundoApellido: null
      });
      setForm({ descripcion: '', reportanteNombre: '', contactoReportante: '' });
      setMessage('Reporte recibido. El equipo de atencion lo revisara y usara el contacto solo para seguimiento interno.');
      await loadItems();
    } catch (requestError) {
      setMessage(`No se pudo registrar: ${requestError.message}`);
    } finally {
      setSaving(false);
    }
  };

  return (
    <main className="tab-page">
      <section className="section passenger-tool">
        <div className="section-label">Objetos perdidos</div>
        <h1>Objetos encontrados y reportes</h1>
        {message && <div className="connection-alert">{message}</div>}
        {!isAdmin && (
          <p className="tool-intro">
            Esta pantalla sirve para dos cosas: ver objetos encontrados recientemente y dejar un reporte si perdiste algo.
            Los datos de contacto no se publican.
          </p>
        )}
        <div className="tool-grid lost-object-grid">
          {isAdmin ? (
            <AdminLostObjectPanel airports={airports} onCreated={loadItems} />
          ) : (
            <form className="tool-panel lost-object-form" onSubmit={submit}>
              <h2>Reportar pérdida</h2>
              <label className="field"><span>¿Qué perdiste?</span><textarea value={form.descripcion} onChange={(event) => updateForm('descripcion', event.target.value)} placeholder="Ej. mochila negra con etiqueta roja" required /></label>
              <label className="field"><span>Tu nombre</span><input value={form.reportanteNombre} onChange={(event) => updateForm('reportanteNombre', event.target.value)} placeholder="nombre y apellido" required /></label>
              <label className="field"><span>Contacto privado</span><input value={form.contactoReportante} onChange={(event) => updateForm('contactoReportante', event.target.value)} placeholder="teléfono o email" required /></label>
              <button className="btn btn-primary" type="submit" disabled={saving}>{saving ? 'Guardando' : 'Enviar reporte'}</button>
            </form>
          )}
          <div className="tool-panel">
            <h2>{isAdmin ? 'Todos los registros' : 'Encontrados recientes'}</h2>
            {items.length === 0 && <div className="board-empty">Sin registros.</div>}
            {items.slice(0, isAdmin ? 50 : 8).map((item) => (
              <div className="operation-row" key={item.id}>
                <div>
                  <strong>{item.descripcion}</strong>
                  <small>
                    {item.ubicacionEncontrado || 'Ubicación pendiente'}
                    {item.vueloId ? ` · Vuelo #${item.vueloId}` : ''}
                    {' · '}{formatDateOnly(item.fechaReporte)}
                  </small>
                </div>
                <span className={`status ${statusClassName(item.estado)}`}>{item.estado}</span>
              </div>
            ))}
          </div>
        </div>
      </section>
    </main>
  );
}

function PromotionsSection() {
  const [promotions, setPromotions] = useState([]);

  useEffect(() => {
    api.promotions(50).then(setPromotions).catch(() => setPromotions([]));
  }, []);

  const activePromotions = promotions.filter((promotion) => normalize(promotion.estado).includes('activ'));

  return (
    <main className="tab-page">
      <section className="section passenger-tool">
        <div className="section-label">Promociones</div>
        <h1>Beneficios disponibles</h1>
        <div className="passenger-card-grid">
          {activePromotions.length === 0 && <div className="board-empty">No hay promociones activas en este momento.</div>}
          {activePromotions.map((promotion) => (
            <article className="passenger-card promo-card" key={promotion.id}>
              <span>{promotion.codigo}</span>
              <h2>{promotion.tipoDescuento}</h2>
              <strong>{promotion.valorDescuento}{normalize(promotion.tipoDescuento).includes('porcent') ? '%' : ' GTQ'} OFF</strong>
              <p>{promotion.descripcion || 'Promoción disponible para compras seleccionadas.'}</p>
              <small>Válida del {formatDateOnly(promotion.fechaInicio)} al {formatDateOnly(promotion.fechaFin)}</small>
            </article>
          ))}
        </div>
      </section>
    </main>
  );
}

const ADMIN_MODULES = [
  { title: 'Operaciones', tables: ['vuelo', 'asignacion_puerta', 'tripulacion', 'checkin', 'tarjeta_embarque'] },
  { title: 'Equipaje y seguridad', tables: ['equipaje', 'movimiento_equipaje', 'control_seguridad', 'control_migratorio', 'arresto'] },
  { title: 'Mantenimiento', tables: ['mantenimientoavion', 'hangar', 'asignacionhangar', 'repuesto', 'repuestoutilizado'] },
  { title: 'Inventario', tables: ['categoriarepuesto', 'proveedor', 'ordencomprarepuesto', 'detalleordencompra', 'movimientorepuesto'] },
  { title: 'RRHH', tables: ['empleado', 'departamento', 'puesto', 'licenciaempleado', 'asistencia', 'planilla'] },
  { title: 'Ventas', tables: ['reserva', 'ventaboleto', 'detalleventaboleto', 'transaccionpago', 'usopromocion'] },
  { title: 'Auditoria', tables: ['auditoria'] }
];

function AdminSection({ tables, selectedTable, onSelectTable }) {
  const [metadata, setMetadata] = useState(null);
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState('');
  const [editingRow, setEditingRow] = useState(null);
  const [formValues, setFormValues] = useState({});

  const editableColumns = useMemo(
    () => (metadata?.columnas || []).filter((column) => !column.esIdentidad && (!editingRow || !column.esLlavePrimaria)),
    [metadata, editingRow]
  );

  const visibleColumns = useMemo(
    () => (metadata?.columnas || []).slice(0, 12),
    [metadata]
  );

  const loadTable = useCallback(async () => {
    if (!selectedTable) {
      setMetadata(null);
      setRows([]);
      return;
    }

    setLoading(true);
    setMessage('');

    try {
      const [tableMetadata, tableRows] = await Promise.all([
        api.tableMetadata(selectedTable),
        api.tableRows(selectedTable, 100)
      ]);
      setMetadata(tableMetadata);
      setRows(tableRows.filas || []);
      setEditingRow(null);
      setFormValues({});
    } catch (requestError) {
      setMetadata(null);
      setRows([]);
      setMessage(`No se pudo cargar la tabla: ${requestError.message}`);
    } finally {
      setLoading(false);
    }
  }, [selectedTable]);

  useEffect(() => {
    loadTable();
  }, [loadTable]);

  const startCreate = () => {
    setEditingRow(null);
    setMessage('');
    setFormValues(editableColumns.reduce((values, column) => ({ ...values, [column.nombre]: '' }), {}));
  };

  const startEdit = (row) => {
    setEditingRow(row);
    setMessage('');
    setFormValues(editableColumns.reduce((values, column) => ({ ...values, [column.nombre]: stringifyValue(row[column.nombre]) }), {}));
  };

  const saveRow = async (event) => {
    event.preventDefault();
    if (!metadata || editableColumns.length === 0) return;

    setSaving(true);
    setMessage('');

    try {
      if (editingRow) {
        await api.updateRow(selectedTable, editingRow[metadata.llavePrimaria], formValues);
        setMessage('Registro actualizado.');
      } else {
        await api.createRow(selectedTable, formValues);
        setMessage('Registro creado.');
      }

      await loadTable();
    } catch (requestError) {
      setMessage(`No se pudo guardar: ${requestError.message}`);
    } finally {
      setSaving(false);
    }
  };

  const deleteRow = async (row) => {
    if (!metadata?.llavePrimaria) {
      setMessage('Esta tabla no tiene llave primaria para borrar registros desde el panel.');
      return;
    }

    const keyValue = row[metadata.llavePrimaria];
    if (!window.confirm(`Eliminar registro ${keyValue}?`)) return;

    setSaving(true);
    setMessage('');

    try {
      await api.deleteRow(selectedTable, keyValue);
      setMessage('Registro eliminado.');
      await loadTable();
    } catch (requestError) {
      setMessage(`No se pudo eliminar: ${requestError.message}`);
    } finally {
      setSaving(false);
    }
  };

  const renderInput = (column) => {
    const type = column.tipoDato.toUpperCase();
    const value = formValues[column.nombre] ?? '';
    const onChange = (event) => setFormValues((values) => ({ ...values, [column.nombre]: event.target.value }));

    if (type.includes('DATE') || type.includes('TIMESTAMP')) {
      return <input type="datetime-local" value={value} onChange={onChange} />;
    }

    if (type.includes('NUMBER') || type.includes('FLOAT') || type.includes('INTEGER')) {
      return <input type="number" step="any" value={value} onChange={onChange} />;
    }

    if ((column.longitud || 0) > 180) {
      return <textarea value={value} onChange={onChange} rows="3" />;
    }

    return <input value={value} onChange={onChange} maxLength={column.longitud || undefined} />;
  };

  return (
    <main className="admin-page">
      <section className="admin-header">
        <div>
          <div className="section-label">Panel administrativo</div>
          <h1>{prettifyName(metadata?.tabla || selectedTable || 'Admin')}</h1>
        </div>
        <div className="admin-actions">
          <select value={selectedTable} onChange={(event) => onSelectTable(event.target.value)} disabled={loading || tables.length === 0}>
            {tables.length === 0 && <option value="">Sin tablas cargadas</option>}
            {tables.map((table) => <option key={table.nombre} value={table.alias}>{prettifyName(table.nombre)}</option>)}
          </select>
          <button className="btn btn-primary" type="button" onClick={startCreate} disabled={!metadata || loading || editableColumns.length === 0}>Nuevo</button>
          <button className="btn btn-outline" type="button" onClick={loadTable} disabled={loading || !selectedTable}>Actualizar</button>
        </div>
      </section>

      {message && <div className="connection-alert">{message}</div>}

      <section className="admin-module-grid">
        {ADMIN_MODULES.map((module) => (
          <article className="admin-module" key={module.title}>
            <strong>{module.title}</strong>
            <div>
              {module.tables.map((alias) => (
                <button
                  type="button"
                  key={`${module.title}-${alias}`}
                  onClick={() => onSelectTable(alias)}
                  className={selectedTable === alias ? 'active' : ''}
                >
                  {prettifyName(alias)}
                </button>
              ))}
            </div>
          </article>
        ))}
      </section>

      <section className="admin-workspace">
        <aside className="admin-form-panel">
          <h2>{editingRow ? 'Editar registro' : 'Crear registro'}</h2>
          {!metadata && <p className="muted-text">Selecciona una tabla para empezar.</p>}
          {metadata && editableColumns.length === 0 && <p className="muted-text">Esta tabla no tiene columnas editables desde el panel.</p>}
          {metadata && editableColumns.length > 0 && (
            <form className="admin-form-grid" onSubmit={saveRow}>
              {editableColumns.map((column) => (
                <label className="field" key={column.nombre}>
                  <span>{column.nombre}{!column.esNullable && ' *'}</span>
                  {renderInput(column)}
                </label>
              ))}
              <div className="form-actions">
                <button className="btn btn-primary" disabled={saving || Boolean(editingRow && !metadata.llavePrimaria)}>{saving ? 'Guardando' : 'Guardar'}</button>
                <button className="btn btn-outline" type="button" onClick={startCreate} disabled={saving}>Limpiar</button>
              </div>
            </form>
          )}
        </aside>

        <div className="admin-table-shell">
          {loading && <div className="board-empty"><span className="loader"></span>Cargando tabla</div>}
          {!loading && !selectedTable && <div className="board-empty">Selecciona una tabla para ver datos.</div>}
          {!loading && selectedTable && rows.length === 0 && <div className="board-empty">No hay registros para mostrar.</div>}
          {!loading && rows.length > 0 && (
            <div className="admin-table-scroll">
              <table className="admin-table">
                <thead>
                  <tr>
                    <th>Acciones</th>
                    {visibleColumns.map((column) => <th key={column.nombre}>{column.nombre}</th>)}
                  </tr>
                </thead>
                <tbody>
                  {rows.map((row, index) => (
                    <tr key={`${row[metadata?.llavePrimaria] || index}`}>
                      <td>
                        <div className="row-actions">
                          <button className="action-edit" type="button" onClick={() => startEdit(row)} disabled={saving || editableColumns.length === 0}>✎ Editar</button>
                          <button className="action-delete" type="button" onClick={() => deleteRow(row)} disabled={saving || !metadata?.llavePrimaria}>🗑 Borrar</button>
                        </div>
                      </td>
                      {visibleColumns.map((column) => <td key={column.nombre}>{stringifyValue(row[column.nombre]) || '-'}</td>)}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </section>
    </main>
  );
}

function ReporteriaSection() {
  const [reports, setReports] = useState({
    sales: [],
    occupancy: [],
    payments: [],
    severities: [],
    maintenance: [],
    security: [],
    audit: []
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const loadReports = useCallback(async () => {
    setLoading(true);
    setMessage('');
    const results = await Promise.allSettled([
      api.salesByDate(),
      api.occupancy(12),
      api.paymentMethodsReport(),
      api.incidentsBySeverity(),
      api.maintenance(8),
      api.securityControls(8),
      api.audit(12)
    ]);

    setReports({
      sales: results[0].status === 'fulfilled' ? results[0].value : [],
      occupancy: results[1].status === 'fulfilled' ? results[1].value : [],
      payments: results[2].status === 'fulfilled' ? results[2].value : [],
      severities: results[3].status === 'fulfilled' ? results[3].value : [],
      maintenance: results[4].status === 'fulfilled' ? results[4].value : [],
      security: results[5].status === 'fulfilled' ? results[5].value : [],
      audit: results[6].status === 'fulfilled' ? results[6].value : []
    });

    if (results.some((result) => result.status === 'rejected')) {
      setMessage('Algunos reportes no respondieron; se muestra la información disponible.');
    }
    setLoading(false);
  }, []);

  useEffect(() => {
    loadReports();
  }, [loadReports]);

  const salesTotal = reports.sales.reduce((sum, item) => sum + Number(item.montoTotal || 0), 0);
  const occupiedSeats = reports.occupancy.reduce((sum, item) => sum + Number(item.plazasOcupadas || 0), 0);
  const openIncidents = reports.severities.reduce((sum, item) => sum + Number(item.abiertos || 0), 0);

  return (
    <main className="admin-page">
      <section className="admin-header">
        <div>
          <div className="section-label">Reporteria</div>
          <h1>Reporteria</h1>
        </div>
        <button className="btn btn-outline" type="button" onClick={loadReports} disabled={loading}>
          {loading ? 'Actualizando' : 'Actualizar'}
        </button>
      </section>
      {message && <div className="connection-alert">{message}</div>}
      <section className="report-summary-grid">
        <article><span>Ventas</span><strong>{formatCurrency(salesTotal)}</strong></article>
        <article><span>Plazas ocupadas</span><strong>{occupiedSeats}</strong></article>
        <article><span>Incidentes abiertos</span><strong>{openIncidents}</strong></article>
        <article><span>Auditorias recientes</span><strong>{reports.audit.length}</strong></article>
      </section>
      <section className="report-grid">
        <article className="report-panel">
          <h2>Ventas por fecha</h2>
          {reports.sales.slice(0, 8).map((row) => (
            <div className="report-row" key={row.fecha}>
              <span>{formatDateOnly(row.fecha)}</span>
              <strong>{formatCurrency(row.montoTotal)}</strong>
            </div>
          ))}
        </article>
        <article className="report-panel">
          <h2>Ocupacion de vuelos</h2>
          {reports.occupancy.slice(0, 8).map((row) => (
            <div className="report-row" key={row.vueloId}>
              <span>{row.numeroVuelo}</span>
              <strong>{Number(row.porcentajeOcupacion || 0).toFixed(1)}%</strong>
            </div>
          ))}
        </article>
        <article className="report-panel">
          <h2>Metodos de pago</h2>
          {reports.payments.map((row) => (
            <div className="report-row" key={row.metodoPagoId}>
              <span>{row.metodoPago}</span>
              <strong>{formatCurrency(row.montoTotal)}</strong>
            </div>
          ))}
        </article>
        <article className="report-panel">
          <h2>Incidentes</h2>
          {reports.severities.map((row) => (
            <div className="report-row" key={row.severidad}>
              <span>{row.severidad}</span>
              <strong>{row.totalIncidentes}</strong>
            </div>
          ))}
        </article>
        <article className="report-panel">
          <h2>Mantenimientos</h2>
          {reports.maintenance.map((row) => (
            <div className="report-row" key={row.id}>
              <span>{row.matriculaAvion || row.avion || `Mantenimiento ${row.id}`}</span>
              <strong>{row.estado}</strong>
            </div>
          ))}
        </article>
        <article className="report-panel">
          <h2>Controles de seguridad</h2>
          {reports.security.map((row) => (
            <div className="report-row" key={row.id}>
              <span>{row.pasajero || `Control ${row.id}`}</span>
              <strong>{row.resultado || row.estado}</strong>
            </div>
          ))}
        </article>
        <article className="report-panel report-panel-wide">
          <h2>Auditoria reciente</h2>
          {reports.audit.map((row) => (
            <div className="report-row" key={row.id}>
              <span>{row.tablaAfectada} - {row.operacion}</span>
              <strong>{row.usuario}</strong>
            </div>
          ))}
        </article>
      </section>
    </main>
  );
}
function App() {
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');
  const [loginOpen, setLoginOpen] = useState(false);
  const [flightSearch, setFlightSearch] = useState('');
  const [travelCriteria, setTravelCriteria] = useState(null);
  const [cartOpen, setCartOpen] = useState(false);
  const [activeView, setActiveView] = useState('inicio');
  const [currency, setCurrency] = useState('GTQ');
  const [user, setUser] = useState(() => {
    const stored = window.localStorage.getItem(SESSION_KEY);
    return stored ? JSON.parse(stored) : null;
  });
  const [adminView, setAdminView] = useState('');
  const [tables, setTables] = useState([]);
  const [selectedTable, setSelectedTable] = useState('');
  const isAdmin = isAdminUser(user);
  const [buyingFlightId, setBuyingFlightId] = useState(null);
  const [selectedFlight, setSelectedFlight] = useState(null);
  const [cartItems, setCartItems] = useState(() => {
    const stored = window.localStorage.getItem(CART_KEY);
    if (!stored) return [];
    const parsed = JSON.parse(stored);
    return Array.isArray(parsed) ? parsed : [parsed];
  });
  const [pendingSection, setPendingSection] = useState('');
  const [purchaseError, setPurchaseError] = useState('');
  const [purchaseMessage, setPurchaseMessage] = useState('');
  const [purchaseSuccess, setPurchaseSuccess] = useState(null);
  const [dashboard, setDashboard] = useState({
    health: null,
    flights: [],
    airports: [],
    destinations: [],
    severities: [],
    baggage: [],
    incidents: []
  });

  const loadDashboard = useCallback(async () => {
    setRefreshing(true);
    setError('');

    const requests = await Promise.allSettled([
      api.health(),
      api.flights(1000),
      api.airports(500),
      api.topDestinations(5),
      api.incidentsBySeverity(),
      api.baggage(5),
      api.incidents(5)
    ]);

    const firstFailure = requests.find((item) => item.status === 'rejected');

    setDashboard({
      health: requests[0].status === 'fulfilled' ? requests[0].value : null,
      flights: requests[1].status === 'fulfilled' ? requests[1].value : [],
      airports: requests[2].status === 'fulfilled' ? requests[2].value : [],
      destinations: requests[3].status === 'fulfilled' ? requests[3].value : [],
      severities: requests[4].status === 'fulfilled' ? requests[4].value : [],
      baggage: requests[5].status === 'fulfilled' ? requests[5].value : [],
      incidents: requests[6].status === 'fulfilled' ? requests[6].value : []
    });

    if (firstFailure) {
      setError(`No se pudieron cargar todos los datos: ${firstFailure.reason.message}`);
    }

    setLoading(false);
    setRefreshing(false);
  }, []);

  useEffect(() => {
    loadDashboard();
  }, [loadDashboard]);

  useEffect(() => {
    if (!pendingSection) return undefined;

    const timeout = window.setTimeout(() => {
      document.getElementById(pendingSection)?.scrollIntoView({ behavior: 'smooth', block: 'start' });
      setPendingSection('');
    }, 60);

    return () => window.clearTimeout(timeout);
  }, [pendingSection, selectedFlight, travelCriteria, adminView]);

  useEffect(() => {
    if (!isAdmin) {
      setTables([]);
      setSelectedTable('');
      setAdminView('');
      return undefined;
    }

    let active = true;
    api.tables().then((items) => {
      if (!active) return;
      setTables(items);
      setSelectedTable((current) => current || items[0]?.alias || '');
    }).catch(() => {
      if (active) setTables([]);
    });

    return () => {
      active = false;
    };
  }, [isAdmin]);

  useEffect(() => {
    if (!user?.pasajeroId) return undefined;

    let active = true;
    api.cartItems(user.pasajeroId).then((items) => {
      if (!active) return;
      const normalizedItems = items.map(serverCartItemToFlight);
      setCartItems(normalizedItems);
      window.localStorage.setItem(CART_KEY, JSON.stringify(normalizedItems));
    }).catch(() => {
      // Si el carrito compartido falla, mantenemos el carrito local para no bloquear la compra.
    });

    return () => {
      active = false;
    };
  }, [user?.pasajeroId]);

  useEffect(() => {
    if (!error || dashboard.health) return undefined;

    const retry = window.setTimeout(() => {
      loadDashboard();
    }, 3500);

    return () => window.clearTimeout(retry);
  }, [dashboard.health, error, loadDashboard]);

  useEffect(() => {
    if (activeView !== 'inicio') return undefined;

    const interval = window.setInterval(() => {
      refreshTopDestinations();
    }, 5000);

    return () => window.clearInterval(interval);
  }, [activeView]);

  const openIncidents = useMemo(
    () => dashboard.severities.reduce((sum, severity) => sum + Number(severity.abiertos || 0), 0),
    [dashboard.severities]
  );

  const handleLogin = async (credentials) => {
    const sessionUser = await api.login(credentials);
    const pendingLocalItems = cartItems.filter((item) => !item.itemCarritoId);

    for (const item of pendingLocalItems) {
      for (const payload of cartItemPayloads(item)) {
        await api.addCartItem(sessionUser.pasajeroId, payload);
      }
    }

    const serverItems = await api.cartItems(sessionUser.pasajeroId);
    const normalizedItems = serverItems.map(serverCartItemToFlight);
    window.localStorage.setItem(SESSION_KEY, JSON.stringify(sessionUser));
    window.localStorage.setItem(CART_KEY, JSON.stringify(normalizedItems));
    setUser(sessionUser);
    setCartItems(normalizedItems);
    setLoginOpen(false);
  };

  const handleRegister = async (payload) => {
    const sessionUser = await api.register(payload);
    const pendingLocalItems = cartItems.filter((item) => !item.itemCarritoId);

    for (const item of pendingLocalItems) {
      for (const payload of cartItemPayloads(item)) {
        await api.addCartItem(sessionUser.pasajeroId, payload);
      }
    }

    const serverItems = await api.cartItems(sessionUser.pasajeroId);
    const normalizedItems = serverItems.map(serverCartItemToFlight);
    window.localStorage.setItem(SESSION_KEY, JSON.stringify(sessionUser));
    window.localStorage.setItem(CART_KEY, JSON.stringify(normalizedItems));
    setUser(sessionUser);
    setCartItems(normalizedItems);
    setLoginOpen(false);
  };

  const handleLogout = () => {
    window.localStorage.removeItem(SESSION_KEY);
    setUser(null);
    setAdminView('');
    setSelectedFlight(null);
    setPurchaseMessage('');
    setPurchaseSuccess(null);
  };

  const buildCartItem = (flight, selectedClass = 'economica', criteria = travelCriteria) => ({
    ...flight,
    cartId: `${flight.id}-${Date.now()}-${Math.random().toString(16).slice(2)}`,
    selectedClass,
    criteria: {
      tripType: criteria?.tripType || 'oneway',
      passengers: passengerCountFromCriteria(criteria),
      origin: criteria?.origin || flight.origen || '',
      destination: criteria?.destination || flight.destino || '',
      departureDate: criteria?.departureDate || '',
      returnDate: criteria?.returnDate || '',
      currency: criteria?.currency || currency
    }
  });

  const buildCheckoutSelection = (items) => {
    const flights = items.map((item) => buildCartItem(item.flight, item.selectedClass || 'economica', item.criteria || travelCriteria));

    if (flights.length === 1) {
      return flights[0];
    }

    const plazasDisponibles = Math.min(...flights.map((item) => Number(item.plazasDisponibles || 0)).filter((value) => value > 0));
    const firstFlight = flights[0];
    const lastFlight = flights[flights.length - 1];

    return {
      ...firstFlight,
      id: `bundle-${Date.now()}`,
      checkoutId: `checkout-${Date.now()}-${Math.random().toString(16).slice(2)}`,
      cartId: `checkout-${Date.now()}-${Math.random().toString(16).slice(2)}`,
      numeroVuelo: flights.map((item) => item.numeroVuelo).join(' / '),
      aerolinea: flights.map((item) => item.aerolinea).filter(Boolean).join(' / '),
      origen: firstFlight.origen,
      destino: lastFlight.destino,
      fechaVuelo: firstFlight.fechaVuelo,
      plazasDisponibles: Number.isFinite(plazasDisponibles) ? plazasDisponibles : firstFlight.plazasDisponibles,
      flights,
      criteria: {
        ...firstFlight.criteria,
        tripType: flights.length > 1 ? 'roundtrip' : firstFlight.criteria?.tripType || 'oneway'
      }
    };
  };

  const saveCartItems = (items) => {
    setCartItems(items);
    window.localStorage.setItem(CART_KEY, JSON.stringify(items));
  };

  const addCheckoutToCart = (checkoutSelection) => {
    const checkoutCartIds = new Set([
      checkoutSelection.cartId,
      ...(Array.isArray(checkoutSelection.flights) ? checkoutSelection.flights.map((item) => item.cartId) : [])
    ].filter(Boolean));

    const nextItems = [
      ...cartItems.filter((item) => !checkoutCartIds.has(item.cartId)),
      checkoutSelection
    ];
    saveCartItems(nextItems);
    return checkoutSelection;
  };

  const handleBuyFlight = async (flight, selectedClass = 'economica', criteria = travelCriteria) => {
    if (Array.isArray(flight)) {
      const selections = flight;
      const invalidSelection = selections.find((selection) => !canPurchaseFlight(selection.flight?.estado));

      if (invalidSelection) {
        setPurchaseMessage('Solo se pueden comprar vuelos programados.');
        return;
      }

      const checkoutSelection = buildCheckoutSelection(selections.map((selection) => ({ ...selection, criteria })));
      setSelectedFlight(checkoutSelection);
      setCartOpen(false);
      setActiveView('explorar');
      setPurchaseError('');
      setPurchaseMessage('');
      setPurchaseSuccess(null);
      window.scrollTo({ top: 0, behavior: 'smooth' });
      return;
    }

    if (!canPurchaseFlight(flight.estado)) {
      setPurchaseMessage('Solo se pueden comprar vuelos programados.');
      return;
    }

    const checkoutSelection = buildCheckoutSelection([{ flight, selectedClass, criteria }]);
    setSelectedFlight(checkoutSelection);
    setCartOpen(false);
    setActiveView('explorar');
    setPurchaseError('');
    setPurchaseMessage('');
    setPurchaseSuccess(null);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const refreshTopDestinations = async () => {
    try {
      const destinations = await api.topDestinations(5);
      setDashboard((current) => ({ ...current, destinations }));
    } catch {
      // El ranking no debe bloquear el flujo de compra.
    }
  };

  const registerFlightSearch = async (criteria) => {
    const origin = matchAirportRecord(dashboard.airports, criteria.origin || 'La Aurora');
    const destination = matchAirportRecord(dashboard.airports, criteria.destination);
    const originId = airportId(origin);
    const destinationId = airportId(destination);

    const departureDate = criteria.departureDate || toDateInputValue(new Date());

    if (!originId || !destinationId) return;

    console.info('Registrando busqueda en AER_BUSQUEDAVUELO', { originId, destinationId, criteria });
    setDashboard((current) => ({
      ...current,
      destinations: incrementDestinationScore(current.destinations, destinationId, 'totalBusquedas', airportName(destination))
    }));

    try {
      await api.createFlightSearch({
        sesionId: null,
        aeropuertoOrigenId: originId,
        aeropuertoDestinoId: destinationId,
        fechaIda: `${departureDate}T00:00:00`,
        fechaVuelta: criteria.returnDate ? `${criteria.returnDate}T00:00:00` : null,
        numeroPasajeros: passengerCountFromCriteria(criteria),
        clase: null,
        fechaBusqueda: new Date().toISOString(),
        seConvirtioCompra: 'N'
      });
      await refreshTopDestinations();
    } catch (analyticsError) {
      console.error('No se pudo guardar la busqueda en AER_BUSQUEDAVUELO.', analyticsError);
    }
  };

  const handleExploreFlights = (criteria) => {
    setTravelCriteria(criteria);
    setSelectedFlight(null);
    setCartOpen(false);
    setPurchaseSuccess(null);
    setActiveView('explorar');
    registerFlightSearch(criteria);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleConfirmPurchase = async (purchaseOptions) => {
    if (!selectedFlight) return;

    const checkoutFlights = Array.isArray(selectedFlight.flights) && selectedFlight.flights.length > 0 ? selectedFlight.flights : [selectedFlight];
    const passengerCount = passengerCountFromCriteria(selectedFlight.criteria);
    const purchaseLegs = purchaseOptions.legs?.length ? purchaseOptions.legs : checkoutFlights.map((item) => ({
      vueloId: item.id,
      clase: item.selectedClass || purchaseOptions.clase,
      tarifaPagada: calculateFlightFare(item.selectedClass || purchaseOptions.clase, passengerCount)
    }));
    const extraPerLeg = purchaseLegs.length > 0
      ? Math.round(Math.max(0, Number(purchaseOptions.tarifaPagada || 0) - purchaseLegs.reduce((sum, leg) => sum + Number(leg.tarifaPagada || 0), 0)) / purchaseLegs.length * 100) / 100
      : 0;

    setBuyingFlightId(checkoutFlights.length > 1 ? 'bundle' : selectedFlight.id);
    setPurchaseError('');

    try {
      const purchases = [];
      const shouldSendIndividualEmail = purchaseLegs.length === 1;
      for (const leg of purchaseLegs) {
        purchases.push(await api.buyFlight({
          usuarioId: user.usuarioId,
          pasajeroId: user.pasajeroId,
          vueloId: leg.vueloId,
          clase: leg.clase,
          numeroPasajeros: passengerCount,
          equipajeFacturado: purchaseOptions.equipajeFacturado,
          pesoEquipaje: purchaseOptions.pesoEquipaje,
          tarifaPagada: Number(leg.tarifaPagada || 0) + extraPerLeg,
          metodoPagoId: purchaseOptions.metodoPagoId,
          emailConfirmacion: shouldSendIndividualEmail ? purchaseOptions.emailConfirmacion : null,
          enviarCorreoConfirmacion: shouldSendIndividualEmail,
          pasajerosAdicionales: purchaseOptions.pasajerosAdicionales
        }));
      }

      const reservationCodes = purchases.map((purchase) => purchase.codigoReserva).filter(Boolean);
      const boughtFlights = checkoutFlights.map((item) => item.numeroVuelo).join(' / ');
      const totalPaid = purchases.reduce((sum, purchase) => sum + Number(purchase.total || 0), 0);
      if (!shouldSendIndividualEmail && purchaseOptions.emailConfirmacion) {
        try {
          await api.sendPurchaseConfirmation({
            emailConfirmacion: purchaseOptions.emailConfirmacion,
            pasajeroNombre: user.nombreCompleto || '',
            total: totalPaid,
            reservas: purchases.map((purchase, index) => {
              const flight = checkoutFlights[index] || checkoutFlights[0];
              return {
                codigoReserva: purchase.codigoReserva,
                numeroVenta: purchase.numeroVenta,
                numeroVuelo: flight.numeroVuelo,
                aerolinea: flight.aerolinea,
                origen: flight.origen,
                destino: flight.destino,
                fechaVuelo: flight.fechaVuelo,
                clase: tariffByClassName(flight.selectedClass || purchaseOptions.clase).name,
                total: purchase.total
              };
            })
          });
        } catch (emailError) {
          console.warn('No se pudo enviar el correo de confirmacion. La compra ya fue registrada.', emailError);
        }
      }
      const successCurrency = selectedFlight.criteria?.currency || currency;
      setPurchaseSuccess({
        reservationCodes,
        total: totalPaid,
        currency: successCurrency,
        passengerCount,
        flights: checkoutFlights.map((item, index) => ({
          label: checkoutFlights.length > 1 ? (index === 0 ? 'Vuelo de ida' : 'Vuelo de vuelta') : 'Vuelo',
          numeroVuelo: item.numeroVuelo,
          route: `${item.origen} - ${item.destino}`,
          className: tariffByClassName(item.selectedClass || purchaseOptions.clase).name,
          plazasDisponibles: purchases[index]?.plazasDisponibles ?? Math.max(0, Number(item.plazasDisponibles || 0) - passengerCount)
        }))
      });
      setPurchaseMessage(`Compra confirmada para ${boughtFlights}. Reserva${purchases.length > 1 ? 's' : ''} ${reservationCodes.join(', ')}.`);
      setSelectedFlight(null);
      setTravelCriteria(null);
      setCartOpen(false);
      setActiveView('success');
      if (selectedFlight.itemCarritoId && user?.pasajeroId) {
        await api.deleteCartItem(user.pasajeroId, selectedFlight.itemCarritoId);
      }
      const checkoutCartIds = new Set(checkoutFlights.map((item) => item.cartId));
      const nextCartItems = cartItems.filter((item) => item.cartId !== selectedFlight.cartId && !checkoutCartIds.has(item.cartId));
      saveCartItems(nextCartItems);
      await loadDashboard();
    } catch (purchaseError) {
      setPurchaseError(purchaseError.message);
    } finally {
      setBuyingFlightId(null);
    }
  };

  const filteredFlights = useMemo(() => {
    const term = normalize(flightSearch);
    const operationalFlights = dashboard.flights.filter((flight) => !canPurchaseFlight(flight.estado));
    if (!term) return [];

    return operationalFlights.filter((flight) =>
      [flight.numeroVuelo, flight.aerolinea, flight.origen, flight.destino, flight.estado, flight.matriculaAvion]
        .some((value) => normalize(value).includes(term))
    );
  }, [dashboard.flights, flightSearch]);

  const handleNavigate = (event, section) => {
    event.preventDefault();
    setAdminView('');
    setSelectedFlight(null);
    setTravelCriteria(null);
    setCartOpen(false);
    setPurchaseSuccess(null);
    setActiveView(section);
    setPendingSection(section === 'inicio' ? 'inicio' : '');
  };

  const handleFooterNavigate = (event, view, section = '') => {
    event.preventDefault();
    setAdminView('');
    setSelectedFlight(null);
    setTravelCriteria(null);
    setCartOpen(false);
    setPurchaseSuccess(null);
    setActiveView(view);
    setPendingSection(section || (view === 'inicio' ? 'inicio' : ''));
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const continueCartItem = (item = cartItems[0]) => {
    if (!item) return;
    if (!user) {
      setLoginOpen(true);
      return;
    }

    setSelectedFlight(item);
    setCartOpen(false);
    setPurchaseSuccess(null);
    setPurchaseError('');
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const openCart = () => {
    setAdminView('');
    setSelectedFlight(null);
    setTravelCriteria(null);
    setPurchaseSuccess(null);
    setActiveView('carrito');
    setCartOpen(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const removeCartItem = async (cartId) => {
    const removedItem = cartItems.find((item) => item.cartId === cartId);

    if (removedItem?.itemCarritoId && user?.pasajeroId) {
      try {
        await api.deleteCartItem(user.pasajeroId, removedItem.itemCarritoId);
      } catch (cartError) {
        setPurchaseMessage(`No se pudo quitar del carrito compartido: ${cartError.message}`);
        return;
      }
    }

    const nextCartItems = cartItems.filter((item) => item.cartId !== cartId);
    saveCartItems(nextCartItems);
  };

  const handleDestinationClick = async (destination) => {
    const destinationId = destinationReportId(destination);
    const destinationName = destinationReportName(destination);

    if (!destinationId) {
      console.error('No se puede registrar click sin aeropuertoDestinoId.', destination);
      return;
    }

    setDashboard((current) => ({
      ...current,
      destinations: incrementDestinationScore(current.destinations, destinationId, 'totalClicks', destinationName)
    }));

    try {
      console.info('Registrando click en AER_CLICKDESTINO', destination);
      await api.createDestinationClick({
        sesionId: null,
        aeropuertoDestinoId: destinationId,
        fechaClick: new Date().toISOString(),
        origenBusqueda: 'home',
        fechaViajeBuscada: null,
        numeroPasajeros: null,
        claseBuscada: null
      });
      await refreshTopDestinations();
    } catch (analyticsError) {
      console.error('No se pudo guardar el click en AER_CLICKDESTINO.', analyticsError);
    }

    setTravelCriteria({
      tripType: 'roundtrip',
      passengers: 1,
      passengerGroups: DEFAULT_PASSENGER_GROUPS,
      origin: '',
      destination: destinationName,
      departureDate: '',
      returnDate: '',
      currency
    });
    setPurchaseSuccess(null);
    setActiveView('explorar');
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  return (
    <>
      <NavBar
        user={user}
        adminView={adminView}
        isAdmin={isAdmin}
        activeView={activeView}
        onAdminView={setAdminView}
        onNavigate={handleNavigate}
        onLoginClick={() => setLoginOpen(true)}
        onLogout={handleLogout}
      />
      <AuthModal open={loginOpen} onClose={() => setLoginOpen(false)} onLogin={handleLogin} onRegister={handleRegister} />
      {adminView === 'admin' && isAdmin ? (
        <AdminSection tables={tables} selectedTable={selectedTable} onSelectTable={setSelectedTable} />
      ) : adminView === 'reporteria' && isAdmin ? (
        <ReporteriaSection />
      ) : activeView === 'success' && purchaseSuccess ? (
        <PurchaseSuccessView
          summary={purchaseSuccess}
          onGoHome={() => {
            setPurchaseSuccess(null);
            setPurchaseMessage('');
            setActiveView('inicio');
            window.scrollTo({ top: 0, behavior: 'smooth' });
          }}
          onExplore={() => {
            setPurchaseSuccess(null);
            setPurchaseMessage('');
            setActiveView('explorar');
            window.scrollTo({ top: 0, behavior: 'smooth' });
          }}
        />
      ) : selectedFlight ? (
        <main>
          <CheckoutView
            flight={selectedFlight}
            user={user}
            onBack={() => setSelectedFlight(null)}
            onConfirm={handleConfirmPurchase}
            submitting={Boolean(buyingFlightId)}
            error={purchaseError}
          />
        </main>
      ) : activeView === 'mis-viajes' ? (
        <MyTripsSection user={user} flights={dashboard.flights} onRequireLogin={() => setLoginOpen(true)} />
      ) : activeView === 'checkin' ? (
        <CheckInSection user={user} flights={dashboard.flights} onRequireLogin={() => setLoginOpen(true)} />
      ) : activeView === 'objetos' ? (
        <LostObjectsSection airports={dashboard.airports} isAdmin={isAdmin} />
      ) : activeView === 'promos' ? (
        <PromotionsSection />
      ) : activeView === 'explorar' && travelCriteria ? (
        <main>
          <TravelResultsView
            criteria={travelCriteria}
            flights={dashboard.flights}
            user={user}
            onBack={() => setTravelCriteria(null)}
            onRequireLogin={() => setLoginOpen(true)}
            onBuyFlight={handleBuyFlight}
            buyingFlightId={buyingFlightId}
          />
        </main>
      ) : activeView === 'explorar' ? (
        <main className="tab-page">
          <TravelSearchSection
            flights={dashboard.flights}
            airports={dashboard.airports}
            currency={currency}
            onExplore={handleExploreFlights}
          />
        </main>
      ) : activeView === 'rastreo' ? (
        <main className="tab-page">
          <FlightBoard
            flights={filteredFlights}
            loading={loading}
            user={user}
            onRequireLogin={() => setLoginOpen(true)}
            onBuyFlight={handleBuyFlight}
            buyingFlightId={buyingFlightId}
            searchTerm={flightSearch}
            onSearchChange={setFlightSearch}
          />
        </main>
      ) : activeView === 'ubicacion' ? (
        <main className="tab-page">
          <LocationSection />
        </main>
      ) : (
        <main>
          <Hero
            onExploreClick={() => {
              setAdminView('');
              setSelectedFlight(null);
              setTravelCriteria(null);
              setCartOpen(false);
              setPurchaseSuccess(null);
              setActiveView('explorar');
              setPendingSection('');
              window.scrollTo({ top: 0, behavior: 'smooth' });
            }}
          />
          {purchaseMessage && <div className="connection-alert success-alert">{purchaseMessage}</div>}
          <DestinationSection destinations={dashboard.destinations} onDestinationClick={handleDestinationClick} />
        </main>
      )}
      <footer>
        <div className="footer-shell">
          <div className="footer-brand">
            <div className="footer-logo">Aeropuerto Internacional La Aurora</div>
            <p>Conectando Ciudad de Guatemala con operaciones, rastreo y experiencia digital de viaje en tiempo real.</p>
          </div>
          <div className="footer-columns">
            <div className="footer-column">
              <h3>Navegacion</h3>
              <a href="#inicio" onClick={(event) => handleFooterNavigate(event, 'inicio', 'inicio')}>Inicio</a>
              <a href="#explorar" onClick={(event) => handleFooterNavigate(event, 'explorar')}>Explorar vuelos</a>
              <a href="#rastreo" onClick={(event) => handleFooterNavigate(event, 'rastreo')}>Rastreo</a>
              <a href="#ubicacion" onClick={(event) => handleFooterNavigate(event, 'ubicacion')}>Ubicación</a>
            </div>
            <div className="footer-column">
              <h3>Aeropuerto</h3>
              <p>Ciudad de Guatemala</p>
              <p>Codigo IATA: GUA</p>
              <p>Codigo ICAO: MGGT</p>
            </div>
            <div className="footer-column">
              <h3>Soporte</h3>
              <a href="#destinos" onClick={(event) => handleFooterNavigate(event, 'inicio', 'destinos')}>Destinos</a>
              <a href="#explorar" onClick={(event) => handleFooterNavigate(event, 'explorar')}>Reservas</a>
              <a href="#ubicacion" onClick={(event) => handleFooterNavigate(event, 'ubicacion')}>Transporte</a>
            </div>
            <div className="footer-column footer-column-currency">
              <h3>Moneda</h3>
              <label className="footer-currency">
                <span>Seleccion actual</span>
                <select value={currency} onChange={(event) => setCurrency(event.target.value)}>
                  {CURRENCIES.map((item) => <option value={item} key={item}>{item}</option>)}
                </select>
              </label>
            </div>
          </div>
        </div>
      </footer>
    </>
  );
}

export default App;







