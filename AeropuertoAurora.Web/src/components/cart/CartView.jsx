import {
  calculateFlightFare,
  passengerCountFromCriteria,
  tariffByClassName,
  formatMoney
} from '../../utils/pricingHelpers';

import {
  addMinutesToDate,
  estimateDurationMinutes
} from '../../utils/flightHelpers';

import { formatTime } from '../../utils/formatters';

export default function CartView({
  items,
  user,
  onBack,
  onRequireLogin,
  onCheckoutItem,
  onRemoveItem
}) {
  const cartTotal = items.reduce(
    (sum, item) =>
      sum +
      calculateFlightFare(
        item.selectedClass || 'economica',
        passengerCountFromCriteria(item.criteria)
      ),
    0
  );

  const currency =
    items[0]?.criteria?.currency || 'GTQ';

  return (
    <main className="cart-page">
      <section className="cart-shell">
        <button
          className="back-button"
          type="button"
          onClick={onBack}
        >
          Volver
        </button>

        <div className="section-label">
          Carrito de compras
        </div>

        <h1>Vuelos seleccionados</h1>

        {items.length === 0 && (
          <div className="board-empty">
            Tu carrito esta vacio.
          </div>
        )}

        <div className="cart-list">
          {items.map((item) => {
            const passengerCount =
              passengerCountFromCriteria(
                item.criteria
              );

            const selectedTariff =
              tariffByClassName(
                item.selectedClass ||
                  'economica'
              );

            const fare =
              calculateFlightFare(
                item.selectedClass ||
                  'economica',
                passengerCount
              );

            const arrival =
              addMinutesToDate(
                item.fechaVuelo,
                estimateDurationMinutes(item)
              );

            return (
              <article
                className="cart-flight-card"
                key={item.cartId}
              >
                <div className="cart-flight-icon">
                  ✈
                </div>

                <div className="cart-flight-main">
                  <div className="cart-flight-title">
                    <strong>
                      {item.numeroVuelo} -{' '}
                      {item.aerolinea}
                    </strong>

                    <span>
                      {formatMoney(
                        fare,
                        item.criteria?.currency ||
                          'GTQ'
                      )}
                    </span>
                  </div>

                  <div className="cart-flight-route">
                    <span>{item.origen}</span>

                    <b>
                      {formatTime(
                        item.fechaVuelo
                      )}{' '}
                      -{' '}
                      {formatTime(arrival)}
                    </b>

                    <span>{item.destino}</span>
                  </div>

                  <div className="cart-flight-meta">
                    <span>
                      <i>◷</i>

                      {item.criteria?.tripType ===
                      'roundtrip'
                        ? 'Ida y vuelta'
                        : 'Solo ida'}
                    </span>

                    <span>
                      <i>☉</i>
                      {passengerCount}{' '}
                      pasajero(s)
                    </span>

                    <span>
                      <i>▣</i>
                      {selectedTariff.name}
                    </span>

                    <span>
                      <i>$</i>
                      {item.criteria?.currency ||
                        'GTQ'}
                    </span>
                  </div>
                </div>

                <div className="cart-flight-actions">
                  <button
                    className="btn btn-primary"
                    type="button"
                    onClick={() =>
                      user
                        ? onCheckoutItem(item)
                        : onRequireLogin()
                    }
                  >
                    Comprar este vuelo
                  </button>

                  <button
                    className="btn btn-outline"
                    type="button"
                    onClick={() =>
                      onRemoveItem(item.cartId)
                    }
                  >
                    Eliminar
                  </button>
                </div>
              </article>
            );
          })}
        </div>

        {items.length > 0 && (
          <aside className="cart-total-box">
            <span>
              Subtotal del carrito
            </span>

            <strong>
              {formatMoney(
                cartTotal,
                currency
              )}
            </strong>
          </aside>
        )}
      </section>
    </main>
  );
}