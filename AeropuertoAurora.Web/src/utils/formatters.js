export const normalize = (value = '') =>
  value.toString().trim().toLowerCase();

export const statusClassName = (status = '') => {
  const value = normalize(status);

  if (
    value.includes('cancel') ||
    value.includes('demor') ||
    value.includes('retras')
  ) {
    return 'delayed';
  }

  if (
    value.includes('abord') ||
    value.includes('proceso') ||
    value.includes('program')
  ) {
    return 'boarding';
  }

  if (
    value.includes('final') ||
    value.includes('complet') ||
    value.includes('activo')
  ) {
    return 'on-time';
  }

  return 'neutral';
};

export const formatDate = (value) => {
  if (!value) return 'Pendiente';

  return new Intl.DateTimeFormat('es-GT', {
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(value));
};