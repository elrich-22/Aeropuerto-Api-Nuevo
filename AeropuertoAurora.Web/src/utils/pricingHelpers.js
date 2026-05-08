import {
  BASE_FARE,
  CURRENCY_RATES,
  TARIFF_FAMILIES,
  PASSENGER_GROUPS
} from '../constants/appConstants';

const DEFAULT_PASSENGER_GROUPS = {
  adultos: 1,
  ninios: 0,
  bebes: 0
};

export const tariffByClassName = (
  className = 'economica'
) =>
  TARIFF_FAMILIES.find(
    (family) => family.className === className
  ) || TARIFF_FAMILIES[0];

export const passengerCountFromGroups = (
  groups = DEFAULT_PASSENGER_GROUPS
) =>
  Math.max(
    1,
    PASSENGER_GROUPS.reduce(
      (sum, group) =>
        sum + Number(groups[group.key] || 0),
      0
    )
  );

export const passengerCountFromCriteria = (
  criteria
) =>
  criteria?.passengerGroups
    ? passengerCountFromGroups(
        criteria.passengerGroups
      )
    : Math.max(
        1,
        Number(criteria?.passengers || 1)
      );

export const calculateFlightFare = (
  className = 'economica',
  passengerCount = 1
) => {
  const tariff = tariffByClassName(className);

  return (
    Math.round(
      BASE_FARE *
        tariff.multiplier *
        passengerCount *
        100
    ) / 100
  );
};

export const formatMoney = (
  value,
  currency = 'GTQ'
) =>
  new Intl.NumberFormat('es-GT', {
    style: 'currency',
    currency: CURRENCY_RATES[currency]
      ? currency
      : 'GTQ'
  }).format(
    Number(value || 0) *
      (CURRENCY_RATES[currency] || 1)
  );