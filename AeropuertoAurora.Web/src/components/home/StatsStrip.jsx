export default function StatsStrip({
  flights,
  destinations,
  baggage,
  openIncidents
}) {
  const routes =
    new Set(
      flights
        .map((flight) => flight.destino)
        .filter(Boolean)
    ).size || destinations.length;

  return (
    <section
      className="info-strip"
      aria-label="Resumen"
    >
      <div className="section stat-grid">
        <div className="stat-item">
          <div className="stat-num">
            {flights.length}
          </div>

          <div className="stat-label">
            Vuelos cargados
          </div>
        </div>

        <div className="stat-item">
          <div className="stat-num">
            {routes}
          </div>

          <div className="stat-label">
            Destinos activos
          </div>
        </div>

        <div className="stat-item">
          <div className="stat-num">
            {baggage.length}
          </div>

          <div className="stat-label">
            Equipajes monitoreados
          </div>
        </div>

        <div className="stat-item">
          <div className="stat-num">
            {openIncidents}
          </div>

          <div className="stat-label">
            Incidentes abiertos
          </div>
        </div>
      </div>
    </section>
  );
}