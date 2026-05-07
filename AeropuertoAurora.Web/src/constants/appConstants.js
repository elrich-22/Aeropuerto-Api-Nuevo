export const SESSION_KEY = 'aeropuertoAurora.user';

export const CART_KEY = 'aeropuertoAurora.cart';

export const BASE_FARE = 1250;

export const CURRENCY_RATES = {
  GTQ: 1,
  USD: 0.128,
  EUR: 0.12,
  COP: 512,
  MXN: 2.35,
  CRC: 65.5,
  HNL: 3.18,
  NIO: 4.72,
  PAB: 0.128,
  CAD: 0.176,
  BRL: 0.66,
  GBP: 0.103,
  JPY: 20.1,
  CHF: 0.117,
  AUD: 0.197
};

export const CURRENCIES = Object.keys(CURRENCY_RATES);

export const CLASS_LABELS = {
  economica: 'Turista',
  ejecutiva: 'Ejecutiva',
  primera: 'Primera clase'
};

export const DOCUMENT_TYPES = [
  'DPI',
  'Pasaporte',
  'Licencia'
];

export const NATIONALITIES = [
  'Guatemala',
  'El Salvador',
  'Honduras',
  'Nicaragua',
  'Costa Rica',
  'Panama',
  'Mexico',
  'Estados Unidos',
  'Otra'
];

export const SEX_OPTIONS = [
  { value: 'M', label: 'Masculino' },
  { value: 'F', label: 'Femenino' }
];