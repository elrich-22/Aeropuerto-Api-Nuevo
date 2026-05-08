import {
  statusClassName,
  formatTime,
  canPurchaseFlight
} from '../../utils/formatters';

export default function FlightBoard({
  flights,
  loading,
  user,
  onRequireLogin,
  onBuyFlight,
  buyingFlightId,
  searchTerm,
  onSearchChange
}) {
  return (
    <section className="section" id="rastreo">
      <div className="section-label">
        Tablero de vuelos
      </div>

      <h2 className="section-title">
        Rastrea el estado de un vuelo
      </h2>

      <div className="flight-search">
        <input
          value={searchTerm}
          onChange={(event) =>
            onSearchChange(event.target.value)
          }
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
            <span className="board-empty-icon">
              ✈
            </span>

            <div>
              <strong>
                Comienza una busqueda
              </strong>

              <p>
                Ingresa destino, vuelo, origen o aerolinea
                para rastrear vuelos en abordaje,
                vuelo o cancelados.
              </p>
            </div>
          </div>
        )}

        {!loading &&
          searchTerm.trim() &&
          flights.length === 0 && (
            <div className="board-empty board-empty-rich">
              <span className="board-empty-icon">
                ◌
              </span>

              <div>
                <strong>
                  Sin resultados operativos
                </strong>

                <p>
                  No encontramos vuelos operativos
                  con esa busqueda.
                </p>
              </div>
            </div>
          )}

        {!loading &&
          searchTerm.trim() &&
          flights.map((flight) => (
            <div
              className={`board-row ${
                !canPurchaseFlight(flight.estado)
                  ? 'unavailable-row'
                  : ''
              }`}
              key={flight.id}
            >
              <span className="flight-num">
                {flight.numeroVuelo}
              </span>

              <span className="dest">
                {flight.destino}

                <small>
                  {flight.origen} · {flight.aerolinea}
                </small>
              </span>

              <span className="time">
                {formatTime(flight.fechaVuelo)}
              </span>

              <span className="time">
                {flight.matriculaAvion || 'Sin asignar'}
              </span>

              <span>
                <span
                  className={`status ${statusClassName(flight.estado)}`}
                >
                  {flight.estado}
                </span>
              </span>

              <span>
                <button
                  className="buy-button"
                  type="button"
                  onClick={() =>
                    user
                      ? onBuyFlight(flight)
                      : onRequireLogin()
                  }
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