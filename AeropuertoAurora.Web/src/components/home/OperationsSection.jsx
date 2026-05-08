import {
  statusClassName,
  formatDate
} from '../../utils/formatters';

export default function OperationsSection({
  baggage,
  incidents
}) {
  return (
    <section className="section" id="operaciones">
      <div className="section-label">
        Operacion diaria
      </div>

      <h2 className="section-title">
        Equipaje e incidentes
      </h2>

      <div className="operations-grid">
        <div className="operation-panel">
          <h3>Equipaje reciente</h3>

          {baggage.length === 0 && (
            <p className="muted-text">
              Sin movimientos de equipaje para mostrar.
            </p>
          )}

          {baggage.map((item) => (
            <div
              className="operation-row"
              key={item.id}
            >
              <div>
                <strong>
                  {item.codigoBarras}
                </strong>

                <small>
                  {item.pasajero} · {item.numeroVuelo}
                </small>
              </div>

              <span
                className={`status ${statusClassName(item.estado)}`}
              >
                {item.estado}
              </span>
            </div>
          ))}
        </div>

        <div className="operation-panel">
          <h3>Incidentes</h3>

          {incidents.length === 0 && (
            <p className="muted-text">
              Sin incidentes para mostrar.
            </p>
          )}

          {incidents.map((incident) => (
            <div
              className="operation-row"
              key={incident.id}
            >
              <div>
                <strong>
                  {incident.tipoIncidente}
                </strong>

                <small>
                  {incident.ubicacion} ·{' '}
                  {formatDate(incident.fechaIncidente)}
                </small>
              </div>

              <span
                className={`status ${statusClassName(incident.estado)}`}
              >
                {incident.severidad}
              </span>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}