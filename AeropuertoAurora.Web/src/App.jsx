import { useCallback, useEffect, useMemo, useState } from 'react';
import { api } from './services/api';

const services = [
  {
    code: 'DF',
    title: 'Tiendas y Duty Free',
    text: 'Compras, recuerdos, tecnologia y productos de viaje antes de abordar.'
  },
  {
    code: 'RS',
    title: 'Restaurantes',
    text: 'Opciones rapidas y espacios comodos para esperar el vuelo.'
  },
  {
    code: 'TR',
    title: 'Parqueo y Transporte',
    text: 'Acceso a taxis autorizados, parqueo y movilidad hacia la ciudad.'
  },
  {
    code: 'VIP',
    title: 'Sala VIP',
    text: 'Areas privadas para descanso, trabajo y espera prioritaria.'
  },
  {
    code: 'WF',
    title: 'WiFi Gratuito',
    text: 'Conexion disponible para pasajeros dentro de la terminal.'
  },
  {
    code: 'MD',
    title: 'Servicio Medico',
    text: 'Atencion de primeros auxilios y apoyo ante emergencias.'
  }
];

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

const normalize = (value = '') => value.toString().trim().toLowerCase();

const statusClassName = (status = '') => {
  const value = normalize(status);

  if (value.includes('cancel') || value.includes('demor') || value.includes('retras')) return 'delayed';
  if (value.includes('abord') || value.includes('proceso') || value.includes('program')) return 'boarding';
  if (value.includes('final') || value.includes('complet') || value.includes('activo')) return 'on-time';

  return 'neutral';
};

const SESSION_KEY = 'aeropuertoAurora.user';
const BASE_FARE = 1250;
const CLASS_MULTIPLIERS = {
  economica: 1,
  ejecutiva: 1.55,
  primera: 2.2
};
const PAYMENT_METHODS = [
  { id: 1, name: 'Tarjeta de credito', requiresCard: true },
  { id: 2, name: 'Tarjeta de debito', requiresCard: true },
  { id: 3, name: 'Transferencia', requiresCard: false }
];

const DOCUMENT_TYPES = ['DPI', 'Pasaporte', 'Licencia'];
const NATIONALITIES = ['Guatemala', 'El Salvador', 'Honduras', 'Nicaragua', 'Costa Rica', 'Panama', 'Mexico', 'Estados Unidos', 'Otra'];
const SEX_OPTIONS = [
  { value: 'M', label: 'Masculino' },
  { value: 'F', label: 'Femenino' }
];

const canPurchaseFlight = (status = '') => normalize(status) === 'programado';

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

function NavBar({ user, adminView, isAdmin, onAdminView, onLoginClick, onLogout }) {
  return (
    <nav className="site-nav">
      <a className="nav-logo" href="#inicio" onClick={() => onAdminView('')}>
        La <span>Aurora</span>
      </a>
      <div className="nav-links">
        <a href="#explorar" onClick={() => onAdminView('')}>Explorar</a>
        <a href="#rastreo" onClick={() => onAdminView('')}>Rastreo</a>
        <a href="#ubicacion" onClick={() => onAdminView('')}>Ubicacion</a>
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
          <>
            <span className="nav-user">{user.nombreCompleto || user.usuario}</span>
            <button className="nav-session-button" type="button" onClick={onLogout}>Salir</button>
          </>
        ) : (
          <button className="nav-session-button" type="button" onClick={onLoginClick}>Iniciar sesion</button>
        )}
      </div>
    </nav>
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

function Hero({ health, refreshing, onRefresh }) {
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
          <button className="btn btn-outline" onClick={onRefresh} disabled={refreshing}>
            {refreshing ? 'Actualizando' : 'Actualizar datos'}
          </button>
        </div>
        <div className="api-status">
          <span>API</span>
          <strong>{health ? 'Conectada' : 'Pendiente'}</strong>
          <small>{api.baseUrl}</small>
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
          <div className="board-empty">Ingresa destino, vuelo, origen o aerolinea para rastrear vuelos en abordaje, vuelo o cancelados.</div>
        )}

        {!loading && searchTerm.trim() && flights.length === 0 && (
          <div className="board-empty">No encontramos vuelos operativos con esa busqueda.</div>
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

function CheckoutView({ flight, onBack, onConfirm, submitting, error }) {
  const [form, setForm] = useState({
    clase: 'economica',
    equipajeFacturado: true,
    pesoEquipaje: 18,
    metodoPagoId: 1,
    nombreTarjeta: '',
    numeroTarjeta: '',
    vencimientoTarjeta: '',
    cvvTarjeta: ''
  });
  const [formError, setFormError] = useState('');

  useEffect(() => {
    if (flight) {
      setForm({
        clase: 'economica',
        equipajeFacturado: true,
        pesoEquipaje: 18,
        metodoPagoId: 1,
        nombreTarjeta: '',
        numeroTarjeta: '',
        vencimientoTarjeta: '',
        cvvTarjeta: ''
      });
      setFormError('');
    }
  }, [flight?.id]);

  if (!flight) return null;

  const selectedPayment = PAYMENT_METHODS.find((method) => method.id === Number(form.metodoPagoId)) ?? PAYMENT_METHODS[0];
  const fare = Math.round(BASE_FARE * CLASS_MULTIPLIERS[form.clase] * 100) / 100;
  const baggageFee = form.equipajeFacturado ? Math.max(0, Number(form.pesoEquipaje || 0) - 23) * 35 : 0;
  const subtotal = fare + baggageFee;
  const taxes = Math.round(subtotal * 0.12 * 100) / 100;
  const total = subtotal + taxes;

  const submit = (event) => {
    event.preventDefault();
    setFormError('');

    if (selectedPayment.requiresCard) {
      const cardNumber = form.numeroTarjeta.replace(/\s/g, '');
      if (!form.nombreTarjeta.trim() || cardNumber.length < 13 || !form.vencimientoTarjeta.trim() || form.cvvTarjeta.length < 3) {
        setFormError('Ingresa nombre, numero, vencimiento y CVV de la tarjeta.');
        return;
      }
    }

    onConfirm({
      clase: form.clase,
      equipajeFacturado: form.equipajeFacturado ? 1 : 0,
      pesoEquipaje: form.equipajeFacturado ? Number(form.pesoEquipaje || 0) : null,
      tarifaPagada: subtotal,
      metodoPagoId: Number(form.metodoPagoId),
      total
    });
  };

  return (
    <section className="checkout-view">
      <div className="checkout-shell">
        <button className="back-button" type="button" onClick={onBack}>Volver a vuelos</button>
        <div className="section-label">Confirmar compra</div>
        <h2 id="purchase-title">{flight.numeroVuelo}</h2>
        <p>{flight.origen} a {flight.destino} - {formatDate(flight.fechaVuelo)} - {flight.aerolinea}</p>

        <form onSubmit={submit}>
          <div className="form-grid">
            <label>
              Clase
              <select value={form.clase} onChange={(event) => setForm((current) => ({ ...current, clase: event.target.value }))}>
                <option value="economica">Economica</option>
                <option value="ejecutiva">Ejecutiva</option>
                <option value="primera">Primera</option>
              </select>
            </label>
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
            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={form.equipajeFacturado}
                onChange={(event) => setForm((current) => ({ ...current, equipajeFacturado: event.target.checked }))}
              />
              Equipaje facturado
            </label>
            <label>
              Peso equipaje kg
              <input
                type="number"
                min="0"
                max="60"
                value={form.pesoEquipaje}
                disabled={!form.equipajeFacturado}
                onChange={(event) => setForm((current) => ({ ...current, pesoEquipaje: event.target.value }))}
              />
            </label>
          </div>

          {selectedPayment.requiresCard && (
            <div className="card-details">
              <h3>Datos de tarjeta</h3>
              <div className="form-grid">
                <label>
                  Nombre en tarjeta
                  <input
                    value={form.nombreTarjeta}
                    onChange={(event) => setForm((current) => ({ ...current, nombreTarjeta: event.target.value }))}
                    placeholder="Nombre completo"
                  />
                </label>
                <label>
                  Numero de tarjeta
                  <input
                    value={form.numeroTarjeta}
                    onChange={(event) => setForm((current) => ({ ...current, numeroTarjeta: event.target.value }))}
                    placeholder="4111 1111 1111 1111"
                    inputMode="numeric"
                  />
                </label>
                <label>
                  Vencimiento
                  <input
                    value={form.vencimientoTarjeta}
                    onChange={(event) => setForm((current) => ({ ...current, vencimientoTarjeta: event.target.value }))}
                    placeholder="MM/AA"
                  />
                </label>
                <label>
                  CVV
                  <input
                    value={form.cvvTarjeta}
                    onChange={(event) => setForm((current) => ({ ...current, cvvTarjeta: event.target.value }))}
                    placeholder="123"
                    inputMode="numeric"
                    maxLength="4"
                  />
                </label>
              </div>
            </div>
          )}

          <div className="purchase-summary">
            <div><span>Tarifa</span><strong>{formatCurrency(fare)}</strong></div>
            <div><span>Equipaje extra</span><strong>{formatCurrency(baggageFee)}</strong></div>
            <div><span>Impuestos</span><strong>{formatCurrency(taxes)}</strong></div>
            <div className="total-row"><span>Total</span><strong>{formatCurrency(total)}</strong></div>
          </div>

          {formError && <div className="form-error">{formError}</div>}
          {error && <div className="form-error">{error}</div>}
          <button className="btn btn-primary" type="submit" disabled={submitting}>
            {submitting ? 'Confirmando' : 'Confirmar compra'}
          </button>
        </form>
      </div>
    </section>
  );
}

function getTravelResults(flights, criteria) {
  if (!criteria) return [];

  const destination = normalize(criteria.destination);
  const origin = normalize(criteria.origin);

  return flights
    .filter((flight) => canPurchaseFlight(flight.estado))
    .filter((flight) => {
      const destinationMatch = !destination || normalize(flight.destino) === destination;
      const originMatch = !origin || normalize(flight.origen) === origin;
      return destinationMatch && originMatch;
    })
    .slice(0, 6);
}

function DateFarePicker({ open, tripType, departureDate, returnDate, onClose, onApply }) {
  const [draftDeparture, setDraftDeparture] = useState(departureDate);
  const [draftReturn, setDraftReturn] = useState(returnDate);
  const [selecting, setSelecting] = useState(departureDate && tripType === 'roundtrip' ? 'return' : 'departure');

  useEffect(() => {
    if (open) {
      setDraftDeparture(departureDate);
      setDraftReturn(returnDate);
      setSelecting(departureDate && tripType === 'roundtrip' ? 'return' : 'departure');
    }
  }, [departureDate, open, returnDate, tripType]);

  if (!open) return null;

  const today = new Date();
  const firstMonth = new Date(today.getFullYear(), today.getMonth() + 1, 1);
  const secondMonth = new Date(firstMonth.getFullYear(), firstMonth.getMonth() + 1, 1);

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
    <div className="modal-backdrop" role="presentation">
      <div className="fare-picker" role="dialog" aria-modal="true">
        <div className="fare-picker-top">
          <button className="fare-trip-button" type="button">{tripType === 'roundtrip' ? 'Ida y vuelta' : 'Solo ida'}</button>
          <button className="fare-reset" type="button" onClick={reset}>Restablecer</button>
          <div className="fare-date-summary">
            <span className={selecting === 'departure' ? 'active' : ''}>Salida {draftDeparture && <strong>{formatShortDate(draftDeparture)}</strong>}</span>
            {tripType === 'roundtrip' && <span className={selecting === 'return' ? 'active' : ''}>Vuelta {draftReturn && <strong>{formatShortDate(draftReturn)}</strong>}</span>}
          </div>
        </div>
        <div className="fare-picker-body">
          {renderMonth(firstMonth)}
          {renderMonth(secondMonth)}
        </div>
        <div className="fare-picker-footer">
          <span>Los precios de los viajes se muestran en GTQ</span>
          <button className="fare-done" type="button" onClick={apply} disabled={!draftDeparture || (tripType === 'roundtrip' && !draftReturn)}>Hecho</button>
        </div>
      </div>
    </div>
  );
}

function TravelSearchSection({ flights, onExplore }) {
  const airportOptions = useMemo(() => {
    const values = new Set();
    flights.forEach((flight) => {
      if (flight.origen) values.add(flight.origen);
      if (flight.destino) values.add(flight.destino);
    });

    return Array.from(values).sort((first, second) => first.localeCompare(second));
  }, [flights]);

  const [criteria, setCriteria] = useState({
    tripType: 'roundtrip',
    passengers: 1,
    className: 'economica',
    origin: '',
    destination: '',
    departureDate: '',
    returnDate: ''
  });
  const [datePickerOpen, setDatePickerOpen] = useState(false);

  const updateCriteria = (field, value) => {
    setCriteria((current) => ({ ...current, [field]: value }));
  };

  const submit = (event) => {
    event.preventDefault();
    onExplore(criteria);
  };

  return (
    <section className="travel-search-section" id="explorar">
      <div className="travel-shell">
        <div className="section-label">Explorar vuelos</div>
        <h2 className="section-title">Encuentra tu siguiente destino</h2>
        <form className="travel-search-card" onSubmit={submit}>
          <div className="travel-topbar">
            <label>
              Tipo
              <select value={criteria.tripType} onChange={(event) => updateCriteria('tripType', event.target.value)}>
                <option value="roundtrip">Ida y vuelta</option>
                <option value="oneway">Solo ida</option>
              </select>
            </label>
            <label>
              Personas
              <select value={criteria.passengers} onChange={(event) => updateCriteria('passengers', event.target.value)}>
                {[1, 2, 3, 4, 5, 6].map((count) => <option value={count} key={count}>{count}</option>)}
              </select>
            </label>
            <label>
              Clase
              <select value={criteria.className} onChange={(event) => updateCriteria('className', event.target.value)}>
                <option value="economica">Turista</option>
                <option value="ejecutiva">Ejecutiva</option>
                <option value="primera">Primera</option>
              </select>
            </label>
          </div>

          <div className="travel-fields">
            <label>
              Origen
              <select value={criteria.origin} onChange={(event) => updateCriteria('origin', event.target.value)}>
                <option value="">Cualquier origen</option>
                {airportOptions.map((airport) => <option value={airport} key={airport}>{airport}</option>)}
              </select>
            </label>
            <label>
              Destino
              <select value={criteria.destination} onChange={(event) => updateCriteria('destination', event.target.value)}>
                <option value="">Cualquier destino</option>
                {airportOptions.map((airport) => <option value={airport} key={airport}>{airport}</option>)}
              </select>
            </label>
            <label>
              Salida
              <button className="date-field-button" type="button" onClick={() => setDatePickerOpen(true)}>
                {criteria.departureDate ? formatShortDate(criteria.departureDate) : 'Salida'}
              </button>
            </label>
            {criteria.tripType === 'roundtrip' && (
              <label>
                Vuelta
                <button className="date-field-button" type="button" onClick={() => setDatePickerOpen(true)}>
                  {criteria.returnDate ? formatShortDate(criteria.returnDate) : 'Vuelta'}
                </button>
              </label>
            )}
          </div>

          <button className="explore-button" type="submit">Explorar</button>
        </form>
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

  return (
    <section className="travel-results-section">
      <div className="travel-results">
        <button className="back-button" type="button" onClick={onBack}>Volver al buscador</button>
        <div className="travel-results-header">
          <div>
            <div className="section-label">Resultados</div>
            <h2>Vuelos de ida destacados</h2>
          </div>
          <span>{results.length} opciones</span>
        </div>

        {results.length === 0 && (
          <div className="board-empty">No hay vuelos programados que coincidan con tu busqueda.</div>
        )}

        {results.map((flight) => {
          const duration = flight.retrasoMinutos > 0 ? `${flight.retrasoMinutos} min retraso` : 'Salida programada';
          return (
            <article className="travel-result-row" key={flight.id}>
              <div className="airline-mark">{flight.aerolinea?.slice(0, 2).toUpperCase() || 'AV'}</div>
              <div>
                <strong>{formatTime(flight.fechaVuelo)} - {flight.numeroVuelo}</strong>
                <small>{flight.aerolinea}</small>
              </div>
              <div>
                <strong>{flight.origen} - {flight.destino}</strong>
                <small>{flight.matriculaAvion}</small>
              </div>
              <div>
                <strong>{duration}</strong>
                <small>{criteria.tripType === 'roundtrip' ? 'ida y vuelta' : 'solo ida'} Â· {criteria.passengers} pasajero(s)</small>
              </div>
              <div className="result-price">
                <strong>{formatCurrency(BASE_FARE * CLASS_MULTIPLIERS[criteria.className])}</strong>
                <small>desde</small>
              </div>
              <button
                className="buy-button"
                type="button"
                onClick={() => (user ? onBuyFlight(flight) : onRequireLogin())}
                disabled={buyingFlightId === flight.id}
              >
                {buyingFlightId === flight.id ? 'Comprando' : 'Comprar'}
              </button>
            </article>
          );
        })}
      </div>
    </section>
  );
}

function DestinationSection({ destinations }) {
  return (
    <section className="section compact-section">
      <div className="section-label">Interes de pasajeros</div>
      <h2 className="section-title">Destinos mas buscados</h2>
      <div className="destination-grid">
        {destinations.length === 0 && <p className="muted-text">Los destinos apareceran cuando el reporte tenga datos.</p>}
        {destinations.map((destination, index) => (
          <article className="destination-card" key={destination.aeropuertoId}>
            <span>{String(index + 1).padStart(2, '0')}</span>
            <strong>{destination.aeropuerto}</strong>
            <small>{destination.totalBusquedas} busquedas Â· {destination.totalClicks} clicks Â· {destination.totalPasajeros} pasajeros</small>
          </article>
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
            <span>01</span>
            <p>7a Avenida 11-03, Zona 13, Ciudad de Guatemala.</p>
          </div>
          <div className="location-detail">
            <span>02</span>
            <p>Acceso a taxis autorizados, parqueo y transporte hacia puntos principales de la ciudad.</p>
          </div>
          <div className="location-detail">
            <span>03</span>
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
      return <input type="number" step="any" value={value} onChange={onChange} placeholder={column.tipoDato} />;
    }

    if ((column.longitud || 0) > 180) {
      return <textarea value={value} onChange={onChange} placeholder={column.tipoDato} rows="3" />;
    }

    return <input value={value} onChange={onChange} placeholder={column.tipoDato} maxLength={column.longitud || undefined} />;
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
            <form onSubmit={saveRow}>
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
                          <button type="button" onClick={() => startEdit(row)} disabled={saving || editableColumns.length === 0}>Editar</button>
                          <button type="button" onClick={() => deleteRow(row)} disabled={saving || !metadata?.llavePrimaria}>Borrar</button>
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
  const [purchaseError, setPurchaseError] = useState('');
  const [purchaseMessage, setPurchaseMessage] = useState('');
  const [dashboard, setDashboard] = useState({
    health: null,
    flights: [],
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
      api.flights(100),
      api.topDestinations(6),
      api.incidentsBySeverity(),
      api.baggage(5),
      api.incidents(5)
    ]);

    const firstFailure = requests.find((item) => item.status === 'rejected');

    setDashboard({
      health: requests[0].status === 'fulfilled' ? requests[0].value : null,
      flights: requests[1].status === 'fulfilled' ? requests[1].value : [],
      destinations: requests[2].status === 'fulfilled' ? requests[2].value : [],
      severities: requests[3].status === 'fulfilled' ? requests[3].value : [],
      baggage: requests[4].status === 'fulfilled' ? requests[4].value : [],
      incidents: requests[5].status === 'fulfilled' ? requests[5].value : []
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
    setPurchaseMessage('');
  };

  const handleBuyFlight = (flight) => {
    if (!canPurchaseFlight(flight.estado)) {
      setPurchaseMessage('Solo se pueden comprar vuelos programados.');
      return;
    }

    setSelectedFlight(flight);
    setPurchaseError('');
    setPurchaseMessage('');
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleExploreFlights = (criteria) => {
    setTravelCriteria(criteria);
    setSelectedFlight(null);
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

  return (
    <>
      <NavBar user={user} adminView={adminView} isAdmin={isAdmin} onAdminView={setAdminView} onLoginClick={() => setLoginOpen(true)} onLogout={handleLogout} />
      <AuthModal open={loginOpen} onClose={() => setLoginOpen(false)} onLogin={handleLogin} onRegister={handleRegister} />
      {adminView === 'admin' && isAdmin ? (
        <AdminSection tables={tables} selectedTable={selectedTable} onSelectTable={setSelectedTable} />
      ) : adminView === 'reporteria' && isAdmin ? (
        <ReporteriaSection />
      ) : selectedFlight ? (
        <main>
          <CheckoutView
            flight={selectedFlight}
            onBack={() => setSelectedFlight(null)}
            onConfirm={handleConfirmPurchase}
            submitting={Boolean(buyingFlightId)}
            error={purchaseError}
          />
        </main>
      ) : travelCriteria ? (
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
      ) : (
        <main>
          <Hero health={dashboard.health} refreshing={refreshing} onRefresh={loadDashboard} />
          {error && <div className="connection-alert">{error}</div>}
          {purchaseMessage && <div className="connection-alert success-alert">{purchaseMessage}</div>}
          <TravelSearchSection
            flights={dashboard.flights}
            onExplore={handleExploreFlights}
          />
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
          <LocationSection />
        </main>
      )}
      <footer>
        <div className="footer-logo">Aeropuerto Internacional La Aurora</div>
        <p>Ciudad de Guatemala Â· Codigo IATA: GUA Â· Codigo ICAO: MGGT</p>
        <p>Frontend React conectado al API del proyecto.</p>
      </footer>
    </>
  );
}

export default App;







