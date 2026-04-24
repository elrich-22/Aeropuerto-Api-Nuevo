import { useEffect, useMemo, useState } from 'react';
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

const normalize = (value = '') => value.toString().trim().toLowerCase();

const statusClassName = (status = '') => {
  const value = normalize(status);

  if (value.includes('cancel') || value.includes('demor') || value.includes('retras')) return 'delayed';
  if (value.includes('abord') || value.includes('proceso') || value.includes('program')) return 'boarding';
  if (value.includes('final') || value.includes('complet') || value.includes('activo')) return 'on-time';

  return 'neutral';
};

function NavBar() {
  return (
    <nav className="site-nav">
      <a className="nav-logo" href="#inicio">
        La <span>Aurora</span>
      </a>
      <div className="nav-links">
        <a href="#vuelos">Vuelos</a>
        <a href="#servicios">Servicios</a>
        <a href="#operaciones">Operaciones</a>
        <a href="#ubicacion">Ubicacion</a>
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
        <div className="hero-badge">Guatemala City · GUA</div>
        <h1>
          Aeropuerto Internacional
          <span> La Aurora</span>
        </h1>
        <p>Puerta de entrada al corazon de Centroamerica, conectada en tiempo real con la operacion aeroportuaria.</p>
        <div className="hero-ctas">
          <a href="#vuelos" className="btn btn-primary">Ver vuelos</a>
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

function FlightBoard({ flights, loading }) {
  return (
    <section className="section" id="vuelos">
      <div className="section-label">Tablero de vuelos</div>
      <h2 className="section-title">Salidas de hoy</h2>

      <div className="board">
        <div className="board-header">
          <span>Vuelo</span>
          <span>Destino</span>
          <span>Hora</span>
          <span>Avion</span>
          <span>Estado</span>
        </div>

        {loading && (
          <div className="board-empty">
            <span className="loader"></span>
            Cargando vuelos desde el API
          </div>
        )}

        {!loading && flights.length === 0 && (
          <div className="board-empty">No hay vuelos disponibles. Verifica que el backend y la base de datos esten activos.</div>
        )}

        {!loading && flights.map((flight) => (
          <div className="board-row" key={flight.id}>
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
          </div>
        ))}
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
            <small>{destination.totalBusquedas} busquedas · {destination.totalClicks} clicks · {destination.totalPasajeros} pasajeros</small>
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
                <small>{item.pasajero} · {item.numeroVuelo}</small>
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
            <p>Operacion aeroportuaria disponible todos los dias del año.</p>
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

function App() {
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [error, setError] = useState('');
  const [dashboard, setDashboard] = useState({
    health: null,
    flights: [],
    destinations: [],
    severities: [],
    baggage: [],
    incidents: []
  });

  const loadDashboard = async () => {
    setRefreshing(true);
    setError('');

    const requests = await Promise.allSettled([
      api.health(),
      api.flights(8),
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
  };

  useEffect(() => {
    loadDashboard();
  }, []);

  const openIncidents = useMemo(
    () => dashboard.severities.reduce((sum, severity) => sum + Number(severity.abiertos || 0), 0),
    [dashboard.severities]
  );

  return (
    <>
      <NavBar />
      <main>
        <Hero health={dashboard.health} refreshing={refreshing} onRefresh={loadDashboard} />
        {error && <div className="connection-alert">{error}</div>}
        <StatsStrip
          flights={dashboard.flights}
          destinations={dashboard.destinations}
          baggage={dashboard.baggage}
          openIncidents={openIncidents}
        />
        <FlightBoard flights={dashboard.flights} loading={loading} />
        <DestinationSection destinations={dashboard.destinations} />
        <ServicesSection />
        <OperationsSection baggage={dashboard.baggage} incidents={dashboard.incidents} />
        <LocationSection />
      </main>
      <footer>
        <div className="footer-logo">Aeropuerto Internacional La Aurora</div>
        <p>Ciudad de Guatemala · Codigo IATA: GUA · Codigo ICAO: MGGT</p>
        <p>Frontend React conectado al API del proyecto.</p>
      </footer>
    </>
  );
}

export default App;
