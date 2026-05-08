export const destinationReportName = (destination) =>
  destination?.aeropuerto ?? destination?.Aeropuerto ?? '';

export const destinationReportId = (destination) =>
  destination?.aeropuertoDestinoId ?? destination?.AeropuertoDestinoId ?? '';

export const incrementDestinationScore = (
  nextDestinations,
  destinationId,
  field,
  destinationName
) => {
  return nextDestinations
    .map((destination) =>
      destinationReportId(destination) === destinationId
        ? {
            ...destination,
            [field]: Number(destination[field] || 0) + 1
          }
        : destination
    )
    .sort((first, second) => {
      const firstScore =
        Number(first.totalBusquedas || 0) +
        Number(first.totalClicks || 0);

      const secondScore =
        Number(second.totalBusquedas || 0) +
        Number(second.totalClicks || 0);

      return (
        secondScore - firstScore ||
        destinationReportName(first).localeCompare(
          destinationReportName(second)
        )
      );
    });
};