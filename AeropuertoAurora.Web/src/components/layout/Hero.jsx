import Stars from '../shared/Stars';
import Plane from '../shared/Plane';

export default function Hero() {
  return (
    <section className="hero" id="inicio">
      <Stars />

      <Plane />

      <div className="hero-content">
        <div className="hero-badge">
          Guatemala City · GUA
        </div>

        <h1>
          Aeropuerto Internacional
          <span> La Aurora</span>
        </h1>

        <p>
          Puerta de entrada al corazon de Centroamerica,
          conectada en tiempo real con la operacion aeroportuaria.
        </p>

        <div className="hero-ctas">
          <a
            href="#explorar"
            className="btn btn-primary"
          >
            Explorar vuelos
          </a>
        </div>
      </div>
    </section>
  );
}