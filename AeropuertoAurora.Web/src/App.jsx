import { useCallback, useEffect, useMemo, useState } from 'react';
import { api } from './services/api';
import NavBar from './components/layout/NavBar';
import Stars from './components/shared/Stars';
import Plane from './components/shared/Plane';
import Footer from './components/layout/Footer';
import {
  SESSION_KEY,
  CART_KEY,
  BASE_FARE,
  CURRENCY_RATES,
  CURRENCIES,
  CLASS_LABELS,
  DOCUMENT_TYPES,
  NATIONALITIES,
  SEX_OPTIONS,
  SERVICES,
  PASSENGER_GROUPS,
  KNOWN_AIRPORTS,
  TARIFF_FAMILIES
} from './constants/appConstants';
import AlertMessage from './components/shared/AlertMessage';





const formatDate = (value) => {
  if (!value) return 'Pendiente';

  return new Intl.DateTimeFormat('es-GT', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value));
};

const formatTime = (value) => {
  if (!value) return '--:--';

  return new Intl.DateTimeFormat('es-GT', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value));
};

const formatShortDate = (value) => {
  if (!value) return '';

  return new Intl.DateTimeFormat('es-GT', {
    weekday: 'short',
    day: '2-digit',
    month: 'short'
  }).format(new Date(`${value}T00:00:00`));
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

const normalize = (value = '') => value.toString().trim().toLowerCase();

const statusClassName = (status = '') => {
  const value = normalize(status);

  if (value.includes('cancel') || value.includes('demor') || value.includes('retras')) return 'delayed';
  if (value.includes('abord') || value.includes('proceso') || value.includes('program')) return 'boarding';
  if (value.includes('final') || value.includes('complet') || value.includes('activo')) return 'on-time';

  return 'neutral';
};









const DEFAULT_PASSENGER_GROUPS = PASSENGER_GROUPS.reduce(
  (groups, group) => ({ ...groups, [group.key]: group.defaultValue }),
  {}
);

const PAYMENT_METHODS = [
  { id: 1, name: 'Tarjeta de credito', requiresCard: true },
  { id: 2, name: 'Tarjeta de debito', requiresCard: true },
  { id: 3, name: 'Transferencia', requiresCard: false }
];
const ANCILLARY_SERVICES = [
  { id: 'seat', title: 'Elige tu asiento', description: 'Selecciona ventana, pasillo o salida rapida.', price: 120, icon: 'AS' },
  { id: 'bag', title: 'Equipaje adicional', description: 'Agrega una maleta documentada al viaje.', price: 280, icon: 'EQ' },
  { id: 'vip', title: 'Sala VIP', description: 'Acceso a sala preferencial antes del vuelo.', price: 360, icon: 'VP' },
  { id: 'priority', title: 'Prioridad de abordaje', description: 'Aborda primero y ahorra tiempo en puerta.', price: 95, icon: 'PR' }
];





const canPurchaseFlight = (status = '') => normalize(status) === 'programado';

const passengerCountFromGroups = (groups = DEFAULT_PASSENGER_GROUPS) =>
  Math.max(1, PASSENGER_GROUPS.reduce((sum, group) => sum + Number(groups[group.key] || 0), 0));

const passengerCountFromCriteria = (criteria) =>
  criteria?.passengerGroups ? passengerCountFromGroups(criteria.passengerGroups) : Math.max(1, Number(criteria?.passengers || 1));

const passengerAgesFromCriteria = (criteria) => {
  const count = passengerCountFromCriteria(criteria);
  const ages = Array.isArray(criteria?.passengerAges) ? criteria.passengerAges : [];
  if (ages.length > 0) return Array.from({ length: count }, (_, index) => ages[index] ?? '');

  if (criteria?.passengerGroups) {
    return PASSENGER_GROUPS.flatMap((group) =>
      Array.from({ length: Number(criteria.passengerGroups[group.key] || 0) }, () => group.age)
    );
  }

  return Array.from({ length: count }, (_, index) => ages[index] ?? '');
};

const tariffByClassName = (className = 'economica') =>
  TARIFF_FAMILIES.find((family) => family.className === className) || TARIFF_FAMILIES[0];

const calculateFlightFare = (className = 'economica', passengerCount = 1) => {
  const tariff = tariffByClassName(className);
  return Math.round(BASE_FARE * tariff.multiplier * passengerCount * 100) / 100;
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

const formatCompactFare = (value) => `Q${value >= 1000 ? `${(value / 1000).toFixed(value % 1000 === 0 ? 0 : 1)} mil` : value}`;

const sameDateValue = (date, value) => toDateInputValue(date) === value;

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
    ['contrasena', 'Ingresa una contrasena.'],
    ['numeroDocumento', 'Ingresa tu documento.'],
    ['tipoDocumento', 'Selecciona tipo de documento.'],
    ['primerNombre', 'Ingresa tu primer nombre.'],
    ['segundoNombre', 'Ingresa tu segundo nombre.'],
    ['primerApellido', 'Ingresa tu primer apellido.'],
    ['segundoApellido', 'Ingresa tu segundo apellido.'],
    ['fechaNacimiento', 'Selecciona tu fecha de nacimiento.'],
    ['nacionalidad', 'Selecciona tu nacionalidad.'],
    ['sexo', 'Selecciona tu sexo.'],
    ['telefono', 'Ingresa tu telefono.']
  ];

  requiredFields.forEach(([field, message]) => {
    if (!form[field]?.toString().trim()) {
      errors[field] = message;
    }
  });

  if (form.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) {
    errors.email = 'Usa un formato de email valido.';
  }

  if (form.contrasena && form.contrasena.length < 4) {
    errors.contrasena = 'Usa al menos 4 caracteres.';
  }

  return errors;
};




function Hero() {
  return (
    <section className="hero" id="inicio">
      <Stars />
      <Plane />
      <div className="hero-content">
        <div className="hero-badge">Guatemala City Â· GUA</div>
        <h1>
          Aeropuerto Internacional
          <span> La Aurora</span>
        </h1>
        <p>Puerta de entrada al corazon de Centroamerica, conectada en tiempo real con la operacion aeroportuaria.</p>
        <div className="hero-ctas">
          <a href="#explorar" className="btn btn-primary">Explorar vuelos</a>
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
    setRegisterForm((current) => ({ ...current, [field]: value }));
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
        <h2 id="login-title">{mode === 'login' ? 'Iniciar sesion' : 'Crear usuario'}</h2>
        <p>{mode === 'login' ? 'Entra para comprar vuelos y mantener tu sesion activa.' : 'Crea tu pasajero y usuario para comprar boletos en linea.'}</p>
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
              Contrasena
              <input
                value={form.contrasena}
                onChange={(event) => setForm((current) => ({ ...current, contrasena: event.target.value }))}
                placeholder="demo"
                type="password"
                autoComplete="current-password"
                required
              />
            </label>
            {error && <div className="form-error">{error}</div>}
            <button className="btn btn-primary" type="submit" disabled={submitting}>
              {submitting ? 'Entrando' : 'Entrar'}
            </button>
            <small>Usuarios seed: luis.perez, maria.ruiz o daniel.soto. Clave: demo.</small>
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
                Contrasena
                <input className={registerInputClass('contrasena')} type="password" value={registerForm.contrasena} onChange={(event) => updateRegister('contrasena', event.target.value)} />
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
                  <option value="">Selecciona una opcion</option>
                  {NATIONALITIES.map((nationality) => <option value={nationality} key={nationality}>{nationality}</option>)}
                </select>
                {fieldErrors.nacionalidad && <small className="field-error">{fieldErrors.nacionalidad}</small>}
              </label>
              <label>
                Sexo
                <select className={registerInputClass('sexo')} value={registerForm.sexo} onChange={(event) => updateRegister('sexo', event.target.value)}>
                  <option value="">Selecciona una opcion</option>
                  {SEX_OPTIONS.map((option) => <option value={option.value} key={option.value}>{option.label}</option>)}
                </select>
                {fieldErrors.sexo && <small className="field-error">{fieldErrors.sexo}</small>}
              </label>
              <label>
                Telefono
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
          placeholder="Buscar por vuelo, destino, origen o aerolinea"
        />
      </div>

      <div className="board">
        <div className="board-header">
          <span>Vuelo</span>
          <span>Destino</span>
          <span>Hora</span>
          <span>Avion</span>
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
              <strong>Comienza una busqueda</strong>
              <p>Ingresa destino, vuelo, origen o aerolinea para rastrear vuelos en abordaje, vuelo o cancelados.</p>
            </div>
          </div>
        )}

        {!loading && searchTerm.trim() && flights.length === 0 && (
          <div className="board-empty board-empty-rich">
            <span className="board-empty-icon">◌</span>
            <div>
              <strong>Sin resultados operativos</strong>
              <p>No encontramos vuelos operativos con esa busqueda.</p>
            </div>
          </div>
        )}

        {!loading && searchTerm.trim() && flights.map((flight) => (
          <div className={`board-row ${!canPurchaseFlight(flight.estado) ? 'unavailable-row' : ''}`} key={flight.id}>
            <span className="flight-num">{flight.numeroVuelo}</span>
            <span className="dest">
              {flight.destino}
              <small>{flight.origen} Â· {flight.aerolinea}</small>
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
    titularTelefono: '',
    titularDocumento: '',
    pasajeros: [],
    nombreTarjeta: '',
    numeroTarjeta: '',
    vencimientoTarjeta: '',
    cvvTarjeta: ''
  });
  const [formError, setFormError] = useState('');
  const [touched, setTouched] = useState({});
  const [selectedServices, setSelectedServices] = useState([]);

  useEffect(() => {
    if (flight) {
      const passengerAges = passengerAgesFromCriteria(flight.criteria);
      setStep('passengers');
      setTouched({});
      setSelectedServices([]);
      setForm({
        metodoPagoId: 1,
        titularNombre: user?.nombreCompleto || '',
        titularEmail: user?.email || '',
        titularTelefono: user?.telefono || '',
        titularDocumento: user?.numeroDocumento || '',
        pasajeros: passengerAges.map((age, index) => ({
          nombre: index === 0 ? user?.nombreCompleto || '' : '',
          documento: index === 0 ? user?.numeroDocumento || '' : '',
          edad: age
        })),
        nombreTarjeta: '',
        numeroTarjeta: '',
        vencimientoTarjeta: '',
        cvvTarjeta: ''
      });
      setFormError('');
    }
  }, [flight?.cartId, flight?.id, user]);

  if (!flight) return null;

  const selectedPayment = PAYMENT_METHODS.find((method) => method.id === Number(form.metodoPagoId)) ?? PAYMENT_METHODS[0];
  const className = flight.selectedClass || 'economica';
  const selectedTariff = tariffByClassName(className);
  const currency = flight.criteria?.currency || 'GTQ';
  const passengerCount = passengerCountFromCriteria(flight.criteria);
  const fare = calculateFlightFare(className, passengerCount);
  const servicesTotal = selectedServices.reduce((sum, serviceId) => {
    const service = ANCILLARY_SERVICES.find((item) => item.id === serviceId);
    return sum + (service?.price || 0) * passengerCount;
  }, 0);
  const subtotal = fare + servicesTotal;
  const taxes = Math.round(subtotal * 0.12 * 100) / 100;
  const total = subtotal + taxes;

  const emailValid = !form.titularEmail || /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.titularEmail);
  const phoneValid = !form.titularTelefono || form.titularTelefono.replace(/\D/g, '').length >= 8;
  const fieldErrors = {
    titularNombre: form.titularNombre.trim() ? '' : 'Nombre obligatorio.',
    titularEmail: !form.titularEmail.trim() ? 'Email obligatorio.' : emailValid ? '' : 'Formato de email invalido.',
    titularTelefono: !form.titularTelefono.trim() ? 'Telefono obligatorio.' : phoneValid ? '' : 'Usa al menos 8 digitos.',
    nombreTarjeta: selectedPayment.requiresCard && !form.nombreTarjeta.trim() ? 'Nombre obligatorio.' : '',
    numeroTarjeta: selectedPayment.requiresCard && form.numeroTarjeta.replace(/\s/g, '').length < 13 ? 'Numero de tarjeta incompleto.' : '',
    vencimientoTarjeta: selectedPayment.requiresCard && !/^\d{2}\/\d{2}$/.test(form.vencimientoTarjeta) ? 'Usa formato MM/AA.' : '',
    cvvTarjeta: selectedPayment.requiresCard && form.cvvTarjeta.length < 3 ? 'CVV incompleto.' : ''
  };
  const passengerErrors = form.pasajeros.map((passenger) => ({
    nombre: passenger.nombre.trim() ? '' : 'Nombre obligatorio.',
    documento: passenger.documento.trim() ? '' : 'Documento obligatorio.',
    edad: passenger.edad !== '' && Number(passenger.edad) >= 0 ? '' : 'Edad obligatoria.'
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
    passengerErrors.every((passenger) => !passenger.nombre && !passenger.documento && !passenger.edad) &&
    !fieldErrors.titularNombre &&
    !fieldErrors.titularEmail &&
    !fieldErrors.titularTelefono;

  const paymentStepValid = () =>
    !fieldErrors.nombreTarjeta &&
    !fieldErrors.numeroTarjeta &&
    !fieldErrors.vencimientoTarjeta &&
    !fieldErrors.cvvTarjeta;

  const continueToServices = () => {
    setTouched((current) => ({
      ...current,
      titularNombre: true,
      titularEmail: true,
      titularTelefono: true,
      ...Object.fromEntries(form.pasajeros.flatMap((_, index) => [
        [`pasajeros.${index}.nombre`, true],
        [`pasajeros.${index}.documento`, true],
        [`pasajeros.${index}.edad`, true]
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
      nombreTarjeta: true,
      numeroTarjeta: true,
      vencimientoTarjeta: true,
      cvvTarjeta: true
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
      total
    });
  };

  return (
    <section className="checkout-view">
      <div className="checkout-shell">
        <button className="back-button" type="button" onClick={onBack}>Volver a vuelos</button>
        <div className="section-label">Carrito de compras</div>
        <h2 id="purchase-title">{flight.numeroVuelo}</h2>
        <p>{flight.origen} a {flight.destino} - {formatDate(flight.fechaVuelo)} - {flight.aerolinea}</p>

        <form onSubmit={submit}>
          <div className="checkout-steps">
            <span className={step === 'passengers' ? 'active' : ''}>Pasajeros</span>
            <span className={step === 'services' ? 'active' : ''}>Extras</span>
            <span className={step === 'payment' ? 'active' : ''}>Pago</span>
          </div>

          <div className="checkout-card">
            <h3>Vuelo seleccionado</h3>
            <div className="checkout-flight-summary">
              <div><span>Salida</span><strong>{formatTime(flight.fechaVuelo)}</strong></div>
              <div><span>Llegada</span><strong>{formatTime(addMinutesToDate(flight.fechaVuelo, estimateDurationMinutes(flight)))}</strong></div>
              <div><span>Tarifa</span><strong>{selectedTariff.name}</strong></div>
              <div><span>Viajan</span><strong>{passengerCount} pasajero(s)</strong></div>
              <div><span>Parada</span><strong>{hasTechnicalStop(flight) ? 'Con parada' : 'Directo'}</strong></div>
            </div>
          </div>

          {step === 'passengers' && (
            <>
              <div className="checkout-card">
                <h3>Informacion de pasajeros</h3>
                <div className="passenger-list">
                  {form.pasajeros.map((passenger, index) => (
                    <div className="passenger-form-row" key={`passenger-${index}`}>
                      <label>
                        Pasajero {index + 1}
                        <input
                          className={touched[`pasajeros.${index}.nombre`] && passengerErrors[index]?.nombre ? 'field-invalid' : ''}
                          value={passenger.nombre}
                          onBlur={() => markTouched(`pasajeros.${index}.nombre`)}
                          onChange={(event) => updatePassenger(index, 'nombre', event.target.value)}
                          placeholder="Nombre completo"
                        />
                        {touched[`pasajeros.${index}.nombre`] && passengerErrors[index]?.nombre && <small className="field-error">{passengerErrors[index].nombre}</small>}
                      </label>
                      <label>
                        Documento
                        <input
                          className={touched[`pasajeros.${index}.documento`] && passengerErrors[index]?.documento ? 'field-invalid' : ''}
                          value={passenger.documento}
                          onBlur={() => markTouched(`pasajeros.${index}.documento`)}
                          onChange={(event) => updatePassenger(index, 'documento', event.target.value)}
                          placeholder="DPI o pasaporte"
                        />
                        {touched[`pasajeros.${index}.documento`] && passengerErrors[index]?.documento && <small className="field-error">{passengerErrors[index].documento}</small>}
                      </label>
                      <label>
                        Edad
                        <input
                          className={touched[`pasajeros.${index}.edad`] && passengerErrors[index]?.edad ? 'field-invalid' : ''}
                          type="number"
                          min="0"
                          max="120"
                          value={passenger.edad}
                          onBlur={() => markTouched(`pasajeros.${index}.edad`)}
                          onChange={(event) => updatePassenger(index, 'edad', event.target.value)}
                        />
                        {touched[`pasajeros.${index}.edad`] && passengerErrors[index]?.edad && <small className="field-error">{passengerErrors[index].edad}</small>}
                      </label>
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
                  <label>
                    Telefono
                    <input
                      className={touched.titularTelefono && fieldErrors.titularTelefono ? 'field-invalid' : ''}
                      value={form.titularTelefono}
                      onBlur={() => markTouched('titularTelefono')}
                      onChange={(event) => setForm((current) => ({ ...current, titularTelefono: event.target.value }))}
                    />
                    {touched.titularTelefono && fieldErrors.titularTelefono && <small className="field-error">{fieldErrors.titularTelefono}</small>}
                  </label>
                  <label>
                    Documento
                    <input value={form.titularDocumento} onChange={(event) => setForm((current) => ({ ...current, titularDocumento: event.target.value }))} />
                  </label>
                </div>
              </div>

              {formError && <div className="form-error">{formError}</div>}
              <button className="btn btn-primary" type="button" onClick={continueToServices}>Continuar</button>
            </>
          )}

          {step === 'services' && (
            <div className="checkout-card">
              {selectedTariff.code === 'turista' && (
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
                    Metodo de pago
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
                        Nombre en tarjeta
                        <input
                          className={touched.nombreTarjeta && fieldErrors.nombreTarjeta ? 'field-invalid' : ''}
                          value={form.nombreTarjeta}
                          onBlur={() => markTouched('nombreTarjeta')}
                          onChange={(event) => setForm((current) => ({ ...current, nombreTarjeta: event.target.value }))}
                          placeholder="Nombre completo"
                        />
                        {touched.nombreTarjeta && fieldErrors.nombreTarjeta && <small className="field-error">{fieldErrors.nombreTarjeta}</small>}
                      </label>
                      <label>
                        Numero de tarjeta
                        <input
                          className={touched.numeroTarjeta && fieldErrors.numeroTarjeta ? 'field-invalid' : ''}
                          value={form.numeroTarjeta}
                          onBlur={() => markTouched('numeroTarjeta')}
                          onChange={(event) => setForm((current) => ({ ...current, numeroTarjeta: event.target.value }))}
                          placeholder="4111 1111 1111 1111"
                          inputMode="numeric"
                        />
                        {touched.numeroTarjeta && fieldErrors.numeroTarjeta && <small className="field-error">{fieldErrors.numeroTarjeta}</small>}
                      </label>
                      <label>
                        Vencimiento
                        <input
                          className={touched.vencimientoTarjeta && fieldErrors.vencimientoTarjeta ? 'field-invalid' : ''}
                          value={form.vencimientoTarjeta}
                          onBlur={() => markTouched('vencimientoTarjeta')}
                          onChange={(event) => setForm((current) => ({ ...current, vencimientoTarjeta: event.target.value }))}
                          placeholder="MM/AA"
                        />
                        {touched.vencimientoTarjeta && fieldErrors.vencimientoTarjeta && <small className="field-error">{fieldErrors.vencimientoTarjeta}</small>}
                      </label>
                      <label>
                        CVV
                        <input
                          className={touched.cvvTarjeta && fieldErrors.cvvTarjeta ? 'field-invalid' : ''}
                          value={form.cvvTarjeta}
                          onBlur={() => markTouched('cvvTarjeta')}
                          onChange={(event) => setForm((current) => ({ ...current, cvvTarjeta: event.target.value }))}
                          placeholder="123"
                          inputMode="numeric"
                          maxLength="4"
                        />
                        {touched.cvvTarjeta && fieldErrors.cvvTarjeta && <small className="field-error">{fieldErrors.cvvTarjeta}</small>}
                      </label>
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
                <div><span>{flight.origen} - {flight.destino}</span><strong>{formatShortDate(flight.criteria?.departureDate) || formatDate(flight.fechaVuelo)}</strong></div>
                {flight.criteria?.tripType === 'roundtrip' && <div><span>Regreso</span><strong>{formatShortDate(flight.criteria?.returnDate)}</strong></div>}
                <div><span>Moneda</span><strong>{currency}</strong></div>
                <div><span>Tarifa {selectedTariff.name} x {passengerCount}</span><strong>{formatMoney(fare, currency)}</strong></div>
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

function getTravelResults(flights, criteria) {
  if (!criteria) return [];

  const destination = normalize(criteria.destination);
  const origin = normalize(criteria.origin);
  const routeMatches = flights
    .filter((flight) => canPurchaseFlight(flight.estado))
    .filter((flight) => {
      const flightDestination = normalize(flight.destino);
      const flightOrigin = normalize(flight.origen);
      const destinationMatch = !destination || flightDestination === destination || flightDestination.includes(destination) || destination.includes(flightDestination);
      const originMatch = !origin || flightOrigin === origin || flightOrigin.includes(origin) || origin.includes(flightOrigin);
      return destinationMatch && originMatch;
    });

  const dateMatches = criteria.departureDate
    ? routeMatches.filter((flight) => toDateInputValue(new Date(flight.fechaVuelo)) === criteria.departureDate)
    : routeMatches;

  return (dateMatches.length > 0 ? dateMatches : routeMatches)
    .sort((first, second) => new Date(first.fechaVuelo) - new Date(second.fechaVuelo));
}

function DateFarePicker({ open, tripType, departureDate, returnDate, onClose, onApply }) {
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

  const months = Array.from({ length: 8 }, (_, index) => new Date(2026, 4 + index, 1));
  const visibleMonths = months.slice(visibleStart, visibleStart + monthsPerView);
  const canGoPrev = visibleStart > 0;
  const canGoNext = visibleStart + monthsPerView < months.length;

  const selectDate = (date) => {
    const value = toDateInputValue(date);

    if (tripType === 'oneway') {
      setDraftDeparture(value);
      setDraftReturn('');
      return;
    }

    if (selecting === 'departure') {
      setDraftDeparture(value);
      if (draftReturn && dateFromInputValue(draftReturn) < date) {
        setDraftReturn('');
      }
      setSelecting('return');
      return;
    }

    if (draftDeparture && date < dateFromInputValue(draftDeparture)) {
      setDraftDeparture(value);
      setDraftReturn('');
      setSelecting('return');
      return;
    }

    setDraftReturn(value);
  };

  const reset = () => {
    setDraftDeparture('');
    setDraftReturn('');
    setSelecting('departure');
  };

  const apply = () => {
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
            const selected = sameDateValue(date, draftDeparture) || sameDateValue(date, draftReturn);
            const inRange = draftDeparture && draftReturn && date > dateFromInputValue(draftDeparture) && date < dateFromInputValue(draftReturn);
            const fare = fareForDate(date);

            return (
              <button
                className={`fare-day ${selected ? 'selected' : ''} ${inRange ? 'in-range' : ''} ${fare <= 820 ? 'best-fare' : ''}`}
                type="button"
                key={value}
                onClick={() => selectDate(date)}
              >
                <strong>{date.getDate()}</strong>
                <small>{formatCompactFare(fare)}</small>
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
            <span>Los precios de los viajes se muestran en GTQ</span>
            <span className="fare-legend-low">Los precios en verde son los mas bajos</span>
          </div>
          <button className="fare-done" type="button" onClick={apply} disabled={!draftDeparture || (tripType === 'roundtrip' && !draftReturn)}>Hecho</button>
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
        <h2>Quienes vuelan?</h2>
        {PASSENGER_GROUPS.map((group) => (
          <div className="passenger-counter-row" key={group.key}>
            <div>
              <strong>{group.label}</strong>
              <small>{group.hint}</small>
            </div>
            <button type="button" onClick={() => updateGroup(group.key, -1)} disabled={group.key === 'adults' && draftGroups[group.key] <= 1}>−</button>
            <span>{draftGroups[group.key]}</span>
            <button type="button" onClick={() => updateGroup(group.key, 1)}>+</button>
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
    passengerAges: passengerAgesFromCriteria({ passengerGroups: DEFAULT_PASSENGER_GROUPS }),
    origin: '',
    destination: '',
    departureDate: '',
    returnDate: ''
  });
  const [datePickerOpen, setDatePickerOpen] = useState(false);
  const [passengerPickerOpen, setPassengerPickerOpen] = useState(false);

  const updateCriteria = (field, value) => {
    setCriteria((current) => {
      return { ...current, [field]: value };
    });
  };

  const updatePassengerGroups = (groups) => {
    setCriteria((current) => ({
      ...current,
      passengerGroups: groups,
      passengers: passengerCountFromGroups(groups),
      passengerAges: passengerAgesFromCriteria({ passengerGroups: groups })
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
          onClose={() => setDatePickerOpen(false)}
          onApply={({ departureDate, returnDate }) => {
            updateCriteria('departureDate', departureDate);
            updateCriteria('returnDate', returnDate);
          }}
        />
      </div>
    </section>
  );
}

function TravelResultsView({ criteria, flights, user, onBack, onRequireLogin, onBuyFlight, buyingFlightId }) {
  const results = useMemo(() => getTravelResults(flights, criteria), [criteria, flights]);
  const [expandedFlightId, setExpandedFlightId] = useState(null);
  const [pendingUpgrade, setPendingUpgrade] = useState(null);
  const passengerCount = passengerCountFromCriteria(criteria);
  const currency = criteria.currency || 'GTQ';

  useEffect(() => {
    setExpandedFlightId(results[0]?.id || null);
  }, [results]);

  const chooseFare = (flight, family) => {
    if (!user) {
      onRequireLogin();
      return;
    }

    const nextFamily = nextTariffFamily(family);
    if (nextFamily) {
      setPendingUpgrade({ flight, family, nextFamily });
      return;
    }

    onBuyFlight(flight, family.className, criteria);
  };

  return (
    <section className="travel-results-section">
      <div className="travel-results">
        <button className="back-button" type="button" onClick={onBack}>Volver al buscador</button>
        <div className="travel-results-header">
          <div>
            <div className="section-label">Resultados</div>
            <h2>Vuelos disponibles</h2>
          </div>
          <span>{results.length} opciones - {criteria.tripType === 'roundtrip' ? 'ida y vuelta' : 'solo ida'}</span>
        </div>

        {results.length === 0 && (
          <div className="board-empty">No hay vuelos programados que coincidan con tu busqueda.</div>
        )}

        {results.map((flight) => {
          const duration = estimateDurationMinutes(flight);
          const arrival = addMinutesToDate(flight.fechaVuelo, duration);
          const expanded = expandedFlightId === flight.id;
          return (
            <article className="flight-result-card" key={flight.id}>
              <button className="flight-result-summary" type="button" onClick={() => setExpandedFlightId(expanded ? null : flight.id)}>
                <span className="airline-mark">{flight.aerolinea?.slice(0, 2).toUpperCase() || 'AV'}</span>
                <span>
                  <strong>{formatTime(flight.fechaVuelo)} - {formatTime(arrival)}</strong>
                  <small>{flight.numeroVuelo} - {flight.aerolinea}</small>
                </span>
                <span>
                  <strong>{flight.origen} - {flight.destino}</strong>
                  <small>{criteria.departureDate ? formatShortDate(criteria.departureDate) : formatShortDate(toDateInputValue(new Date(flight.fechaVuelo)))}</small>
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
                              <span>{state === 'check' ? '✓' : '×'}</span>
                              {benefit}
                            </li>
                          ))}
                        </ul>
                        <button
                          className={family.code === 'ejecutiva' ? 'btn btn-primary' : 'btn btn-outline'}
                          type="button"
                          onClick={() => chooseFare(flight, family)}
                          disabled={buyingFlightId === flight.id}
                        >
                          {buyingFlightId === flight.id ? 'Procesando' : `Elegir ${family.name}`}
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
                onBuyFlight(pendingUpgrade.flight, pendingUpgrade.nextFamily.className, criteria);
                setPendingUpgrade(null);
              }}
            >
              Mejorar a {pendingUpgrade.nextFamily.name}
            </button>
            <button
              className="continue-restricted"
              type="button"
              onClick={() => {
                onBuyFlight(pendingUpgrade.flight, pendingUpgrade.family.className, criteria);
                setPendingUpgrade(null);
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
  const cartTotal = items.reduce((sum, item) => (
    sum + calculateFlightFare(item.selectedClass || 'economica', passengerCountFromCriteria(item.criteria))
  ), 0);
  const currency = items[0]?.criteria?.currency || 'GTQ';

  return (
    <main className="cart-page">
      <section className="cart-shell">
        <button className="back-button" type="button" onClick={onBack}>Volver</button>
        <div className="section-label">Carrito de compras</div>
        <h1>Vuelos seleccionados</h1>
        {items.length === 0 && <div className="board-empty">Tu carrito esta vacio.</div>}

        <div className="cart-list">
          {items.map((item) => {
            const passengerCount = passengerCountFromCriteria(item.criteria);
            const selectedTariff = tariffByClassName(item.selectedClass || 'economica');
            const fare = calculateFlightFare(item.selectedClass || 'economica', passengerCount);
            const arrival = addMinutesToDate(item.fechaVuelo, estimateDurationMinutes(item));

            return (
              <article className="cart-flight-card" key={item.cartId}>
                <div className="cart-flight-icon">✈</div>
                <div className="cart-flight-main">
                  <div className="cart-flight-title">
                    <strong>{item.numeroVuelo} - {item.aerolinea}</strong>
                    <span>{formatMoney(fare, item.criteria?.currency || 'GTQ')}</span>
                  </div>
                  <div className="cart-flight-route">
                    <span>{item.origen}</span>
                    <b>{formatTime(item.fechaVuelo)} - {formatTime(arrival)}</b>
                    <span>{item.destino}</span>
                  </div>
                  <div className="cart-flight-meta">
                    <span><i>◷</i>{item.criteria?.tripType === 'roundtrip' ? 'Ida y vuelta' : 'Solo ida'}</span>
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

        {items.length > 0 && (
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
              <small>{destination.totalBusquedas} busquedas</small>
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
                <small>{item.pasajero} Â· {item.numeroVuelo}</small>
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
                <small>{incident.ubicacion} Â· {formatDate(incident.fechaIncidente)}</small>
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
          <h2 className="section-title">Ubicacion</h2>
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
            <p>Operacion aeroportuaria disponible todos los dias del aÃ±o.</p>
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
  return (
    <main className="admin-page">
      <section className="admin-header">
        <div>
          <div className="section-label">Reporteria</div>
          <h1>Reporteria</h1>
        </div>
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
    window.localStorage.setItem(SESSION_KEY, JSON.stringify(sessionUser));
    setUser(sessionUser);
    setLoginOpen(false);
  };

  const handleRegister = async (payload) => {
    const sessionUser = await api.register(payload);
    window.localStorage.setItem(SESSION_KEY, JSON.stringify(sessionUser));
    setUser(sessionUser);
    setLoginOpen(false);
  };

  const handleLogout = () => {
    window.localStorage.removeItem(SESSION_KEY);
    setUser(null);
    setAdminView('');
    setSelectedFlight(null);
    setPurchaseMessage('');
  };

  const handleBuyFlight = (flight, selectedClass = 'economica', criteria = travelCriteria) => {
    if (!canPurchaseFlight(flight.estado)) {
      setPurchaseMessage('Solo se pueden comprar vuelos programados.');
      return;
    }

    const nextCartItem = {
      ...flight,
      cartId: `${flight.id}-${Date.now()}`,
      selectedClass,
      criteria: {
        tripType: criteria?.tripType || 'oneway',
        passengers: passengerCountFromCriteria(criteria),
        passengerAges: passengerAgesFromCriteria(criteria),
        origin: criteria?.origin || flight.origen || '',
        destination: criteria?.destination || flight.destino || '',
        departureDate: criteria?.departureDate || '',
        returnDate: criteria?.returnDate || '',
        currency: criteria?.currency || currency
      }
    };

    const nextCartItems = [...cartItems, nextCartItem];
    setCartItems(nextCartItems);
    window.localStorage.setItem(CART_KEY, JSON.stringify(nextCartItems));
    setSelectedFlight(nextCartItem);
    setPurchaseError('');
    setPurchaseMessage('');
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
    setActiveView('explorar');
    registerFlightSearch(criteria);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleConfirmPurchase = async (purchaseOptions) => {
    if (!selectedFlight) return;

    setBuyingFlightId(selectedFlight.id);
    setPurchaseError('');

    try {
      const purchase = await api.buyFlight({
        usuarioId: user.usuarioId,
        pasajeroId: user.pasajeroId,
        vueloId: selectedFlight.id,
        clase: purchaseOptions.clase,
        equipajeFacturado: purchaseOptions.equipajeFacturado,
        pesoEquipaje: purchaseOptions.pesoEquipaje,
        tarifaPagada: purchaseOptions.tarifaPagada,
        metodoPagoId: purchaseOptions.metodoPagoId
      });

      setPurchaseMessage(`Compra confirmada para ${selectedFlight.numeroVuelo}. Reserva ${purchase.codigoReserva}. Total ${formatCurrency(purchase.total)}.`);
      setSelectedFlight(null);
      const nextCartItems = cartItems.filter((item) => item.cartId !== selectedFlight.cartId);
      setCartItems(nextCartItems);
      window.localStorage.setItem(CART_KEY, JSON.stringify(nextCartItems));
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
    setActiveView(section);
    setPendingSection(section === 'inicio' ? 'inicio' : '');
  };

  const handleFooterNavigate = (event, view, section = '') => {
    event.preventDefault();
    setAdminView('');
    setSelectedFlight(null);
    setTravelCriteria(null);
    setCartOpen(false);
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
    setPurchaseError('');
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const openCart = () => {
    setAdminView('');
    setSelectedFlight(null);
    setTravelCriteria(null);
    setActiveView('carrito');
    setCartOpen(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const removeCartItem = (cartId) => {
    const nextCartItems = cartItems.filter((item) => item.cartId !== cartId);
    setCartItems(nextCartItems);
    window.localStorage.setItem(CART_KEY, JSON.stringify(nextCartItems));
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
      passengerAges: passengerAgesFromCriteria({ passengerGroups: DEFAULT_PASSENGER_GROUPS }),
      origin: '',
      destination: destinationName,
      departureDate: '',
      returnDate: '',
      currency
    });
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
        cartCount={cartItems.length}
        onAdminView={setAdminView}
        onNavigate={handleNavigate}
        onCartClick={openCart}
        onLoginClick={() => setLoginOpen(true)}
        onLogout={handleLogout}
      />
      <AuthModal open={loginOpen} onClose={() => setLoginOpen(false)} onLogin={handleLogin} onRegister={handleRegister} />
      {adminView === 'admin' && isAdmin ? (
        <AdminSection tables={tables} selectedTable={selectedTable} onSelectTable={setSelectedTable} />
      ) : adminView === 'reporteria' && isAdmin ? (
        <ReporteriaSection />
      ) : cartOpen ? (
        <CartView
          items={cartItems}
          user={user}
          onBack={() => setCartOpen(false)}
          onRequireLogin={() => setLoginOpen(true)}
          onCheckoutItem={continueCartItem}
          onRemoveItem={removeCartItem}
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
          <Hero />
          <AlertMessage
             message={purchaseMessage}
             type="success"
          />
          <DestinationSection destinations={dashboard.destinations} onDestinationClick={handleDestinationClick} />
        </main>
      )}

    </>
  );
}

export default App;







