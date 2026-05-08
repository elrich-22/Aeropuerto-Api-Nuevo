import { SERVICES } from '../../constants/appConstants';

export default function ServicesSection() {
  return (
    <section className="section" id="servicios">
      <div className="section-label">
        Para viajeros
      </div>

      <h2 className="section-title">
        Servicios del aeropuerto
      </h2>

      <div className="services-grid">
        {SERVICES.map((service) => (
          <article
            className="service-card"
            key={service.title}
          >
            <div className="service-icon">
              {service.code}
            </div>

            <h3>{service.title}</h3>

            <p>{service.text}</p>
          </article>
        ))}
      </div>
    </section>
  );
}