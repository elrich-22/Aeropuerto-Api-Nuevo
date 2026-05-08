import {
  normalize,
  canPurchaseFlight
} from './formatters';

export const toDateInputValue = (date) => {
  const year = date.getFullYear();

  const month = String(
    date.getMonth() + 1
  ).padStart(2, '0');

  const day = String(
    date.getDate()
  ).padStart(2, '0');

  return `${year}-${month}-${day}`;
};

export function getTravelResults(
  flights,
  criteria
) {
  if (!criteria) return [];

  const destination = normalize(
    criteria.destination
  );

  const origin = normalize(
    criteria.origin
  );

  const routeMatches = flights
    .filter((flight) =>
      canPurchaseFlight(flight.estado)
    )
    .filter((flight) => {
      const flightDestination = normalize(
        flight.destino
      );

      const flightOrigin = normalize(
        flight.origen
      );

      const destinationMatch =
        !destination ||
        flightDestination === destination ||
        flightDestination.includes(destination) ||
        destination.includes(flightDestination);

      const originMatch =
        !origin ||
        flightOrigin === origin ||
        flightOrigin.includes(origin) ||
        origin.includes(flightOrigin);

      return destinationMatch && originMatch;
    });

  const dateMatches = criteria.departureDate
    ? routeMatches.filter(
        (flight) =>
          toDateInputValue(
            new Date(flight.fechaVuelo)
          ) === criteria.departureDate
      )
    : routeMatches;

  return (
    dateMatches.length > 0
      ? dateMatches
      : routeMatches
  ).sort(
    (first, second) =>
      new Date(first.fechaVuelo) -
      new Date(second.fechaVuelo)
  );
}