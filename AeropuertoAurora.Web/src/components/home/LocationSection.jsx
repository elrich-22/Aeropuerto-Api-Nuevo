export default function LocationSection() {
  return (
    <section className="location-section" id="ubicacion">
      <div className="location-inner">
        <div>
          <div className="section-label">
            Como llegar
          </div>

          <h2 className="section-title">
            Ubicacion
          </h2>

          <div className="location-detail">
            <span>⌖</span>

            <p>
              7a Avenida 11-03, Zona 13, Ciudad de Guatemala.
            </p>
          </div>

          <div className="location-detail">
            <span>⇄</span>

            <p>
              Acceso a taxis autorizados, parqueo y transporte hacia puntos principales de la ciudad.
            </p>
          </div>

          <div className="location-detail">
            <span>◔</span>

            <p>
              Operacion aeroportuaria disponible todos los dias del año.
            </p>
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