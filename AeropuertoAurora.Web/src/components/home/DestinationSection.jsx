import {
  destinationReportName,
  destinationReportId
} from '../../utils/destinationHelpers';

export default function DestinationSection({
  destinations,
  onDestinationClick
}) {
  const rankedDestinations = [...destinations]
    .sort((first, second) => {
      const firstScore =
        Number(first.totalBusquedas || 0) +
        Number(first.totalClicks || 0);

      const secondScore =
        Number(second.totalBusquedas || 0) +
        Number(second.totalClicks || 0);

      return (
        secondScore - firstScore ||
        destinationReportName(first).localeCompare(
          destinationReportName(second)
        )
      );
    })
    .slice(0, 5);

  return (
    <section className="section compact-section" id="destinos">
      <div className="section-label">
        Interes de pasajeros
      </div>

      <h2 className="section-title">
        Destinos mas buscados
      </h2>

      <div className="destination-grid">
        {rankedDestinations.length === 0 && (
          <p className="muted-text">
            Los destinos apareceran cuando el reporte tenga datos.
          </p>
        )}

        {rankedDestinations.map((destination, index) => (
          <button
            className="destination-card destination-button"
            type="button"
            key={destinationReportId(destination)}
            onClick={() => onDestinationClick(destination)}
          >
            <div className="destination-visual">
              <span>
                {String(index + 1).padStart(2, '0')}
              </span>

              <small>
                {destinationReportName(destination)
                  .slice(0, 3)
                  .toUpperCase()}
              </small>
            </div>

            <strong>
              {destinationReportName(destination)}
            </strong>

            <div className="destination-metrics">
              <small>
                {Number(destination.totalBusquedas || 0) +
                  Number(destination.totalClicks || 0)}
                {' '}puntos de interes
              </small>

              <small>
                {destination.totalBusquedas} busquedas
              </small>

              <small>
                {destination.totalClicks} clicks
              </small>
            </div>
          </button>
        ))}
      </div>
    </section>
  );
}