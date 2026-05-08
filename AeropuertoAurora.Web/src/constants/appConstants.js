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

export 
  const SERVICES = [
  {
    code: 'DF',
    title: 'Tiendas y Duty Free',
    text: 'Compras, recuerdos, tecnologia y productos de viaje antes de abordar.'
  },
  {
    code: 'RS',
    title: 'Restaurantes',
    text: 'Opciones rapidas y espacios comodos para esperar el vuelo.'
  },
  {
    code: 'TR',
    title: 'Parqueo y Transporte',
    text: 'Acceso a taxis autorizados, parqueo y movilidad hacia la ciudad.'
  },
  {
    code: 'VIP',
    title: 'Sala VIP',
    text: 'Areas privadas para descanso, trabajo y espera prioritaria.'
  },
  {
    code: 'WF',
    title: 'WiFi Gratuito',
    text: 'Conexion disponible para pasajeros dentro de la terminal.'
  },
  {
    code: 'MD',
    title: 'Servicio Medico',
    text: 'Atencion de primeros auxilios y apoyo ante emergencias.'
  }
];

export const PASSENGER_GROUPS = [
  { key: 'adults', label: 'Adultos', hint: 'Desde 15 anos', defaultValue: 1, age: 30 },
  { key: 'youth', label: 'Jovenes', hint: 'De 12 a 14 anos', defaultValue: 0, age: 13 },
  { key: 'children', label: 'Ninos', hint: 'De 2 a 11 anos', defaultValue: 0, age: 8 },
  { key: 'babies', label: 'Bebes', hint: 'Menores de 2 anos', defaultValue: 0, age: 1 }
];

export const KNOWN_AIRPORTS = [
  { name: 'Aeropuerto Internacional La Aurora', country: 'Guatemala', city: 'Ciudad de Guatemala', aliases: ['guatemala', 'la aurora', 'gua'] },
  { name: 'Aeropuerto Internacional El Dorado', country: 'Colombia', city: 'Bogota', aliases: ['colombia', 'bogota', 'el dorado'] },
  { name: 'Miami International Airport', country: 'Estados Unidos', city: 'Miami', aliases: ['miami', 'mia'] },
  { name: 'John F. Kennedy International Airport', country: 'Estados Unidos', city: 'Nueva York', aliases: ['new york', 'jfk'] },
  { name: 'Los Angeles International Airport', country: 'Estados Unidos', city: 'Los Angeles', aliases: ['los angeles', 'lax'] },
  { name: 'Aeropuerto Internacional Benito Juarez', country: 'Mexico', city: 'Ciudad de Mexico', aliases: ['mexico', 'ciudad de mexico'] },
  { name: 'Aeropuerto Internacional de Cancun', country: 'Mexico', city: 'Cancun', aliases: ['cancun'] },
  { name: 'Aeropuerto Internacional de Tocumen', country: 'Panama', city: 'Panama', aliases: ['panama', 'tocumen'] },
  { name: 'Aeropuerto Internacional Juan Santamaria', country: 'Costa Rica', city: 'San Jose', aliases: ['costa rica', 'san jose'] },
  { name: 'Aeropuerto Internacional de El Salvador San Oscar Arnulfo Romero', country: 'El Salvador', city: 'San Salvador', aliases: ['el salvador', 'san salvador'] },
  { name: 'Aeropuerto Adolfo Suarez Madrid-Barajas', country: 'Espana', city: 'Madrid', aliases: ['madrid', 'espana'] }
];

export const TARIFF_FAMILIES = [
  {
    code: 'turista',
    className: 'economica',
    name: 'Turista',
    tagline: 'Para viajar basico',
    multiplier: 1,
    benefits: [
      ['check', 'Articulo personal'],
      ['x', 'Equipaje de bodega'],
      ['x', 'Cambios sin penalidad']
    ]
  },
  {
    code: 'ejecutiva',
    className: 'ejecutiva',
    name: 'Ejecutiva',
    tagline: 'Mas comodidad',
    multiplier: 1.32,
    benefits: [
      ['check', 'Articulo personal'],
      ['check', 'Equipaje de bodega'],
      ['x', 'Reembolso completo']
    ]
  },
  {
    code: 'primera',
    className: 'primera',
    name: 'Primera clase',
    tagline: 'Mayor tranquilidad',
    multiplier: 1.68,
    benefits: [
      ['check', 'Articulo personal'],
      ['check', 'Equipaje de bodega'],
      ['check', 'Cambios y reembolso']
    ]
  }
];