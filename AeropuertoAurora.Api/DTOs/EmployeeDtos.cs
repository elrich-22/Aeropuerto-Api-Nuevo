namespace AeropuertoAurora.Api.DTOs;

public sealed record EmpleadoDto(
    int Id,
    string NumeroEmpleado,
    string NombreCompleto,
    string Departamento,
    string Puesto,
    string? Email,
    string? Telefono,
    DateTime? FechaContratacion,
    decimal Salario,
    string TipoContrato,
    string Estado);

public sealed record DepartamentoDto(
    int Id,
    string Nombre,
    string? Descripcion,
    int AeropuertoId,
    string Estado);

public sealed record CrearDepartamentoDto(
    string Nombre,
    string? Descripcion,
    int AeropuertoId,
    string Estado);

public sealed record ActualizarDepartamentoDto(
    string Nombre,
    string? Descripcion,
    int AeropuertoId,
    string Estado);

public sealed record PuestoDto(
    int Id,
    string Nombre,
    int DepartamentoId,
    string? Descripcion,
    decimal? SalarioMinimo,
    decimal? SalarioMaximo,
    string RequiereLicencia);

public sealed record CrearPuestoDto(
    string Nombre,
    int DepartamentoId,
    string? Descripcion,
    decimal? SalarioMinimo,
    decimal? SalarioMaximo,
    string RequiereLicencia);

public sealed record ActualizarPuestoDto(
    string Nombre,
    int DepartamentoId,
    string? Descripcion,
    decimal? SalarioMinimo,
    decimal? SalarioMaximo,
    string RequiereLicencia);

public sealed record EmpleadoDetalleDto(
    int Id,
    string NumeroEmpleado,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    DateTime? FechaNacimiento,
    string? Dpi,
    string? Nit,
    string? DireccionCalle,
    string? DireccionZona,
    string? DireccionMunicipio,
    string? DireccionDepartamento,
    string? DireccionPais,
    string? Telefono,
    string Email,
    DateTime FechaContratacion,
    int PuestoId,
    int DepartamentoId,
    decimal? SalarioActual,
    string? TipoContrato,
    string Estado,
    byte[]? Foto);

public sealed record CrearEmpleadoDto(
    string NumeroEmpleado,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    DateTime? FechaNacimiento,
    string? Dpi,
    string? Nit,
    string? DireccionCalle,
    string? DireccionZona,
    string? DireccionMunicipio,
    string? DireccionDepartamento,
    string? DireccionPais,
    string? Telefono,
    string Email,
    DateTime FechaContratacion,
    int PuestoId,
    int DepartamentoId,
    decimal? SalarioActual,
    string? TipoContrato,
    string Estado,
    byte[]? Foto);

public sealed record ActualizarEmpleadoDto(
    string NumeroEmpleado,
    string PrimerNombre,
    string? SegundoNombre,
    string PrimerApellido,
    string? SegundoApellido,
    DateTime? FechaNacimiento,
    string? Dpi,
    string? Nit,
    string? DireccionCalle,
    string? DireccionZona,
    string? DireccionMunicipio,
    string? DireccionDepartamento,
    string? DireccionPais,
    string? Telefono,
    string Email,
    DateTime FechaContratacion,
    int PuestoId,
    int DepartamentoId,
    decimal? SalarioActual,
    string? TipoContrato,
    string Estado,
    byte[]? Foto);

public sealed record LicenciaEmpleadoDto(
    int Id,
    int EmpleadoId,
    string TipoLicencia,
    string NumeroLicencia,
    DateTime FechaEmision,
    DateTime FechaVencimiento,
    string? AutoridadEmisora,
    string Estado);

public sealed record CrearLicenciaEmpleadoDto(
    int EmpleadoId,
    string TipoLicencia,
    string NumeroLicencia,
    DateTime FechaEmision,
    DateTime FechaVencimiento,
    string? AutoridadEmisora,
    string Estado);

public sealed record ActualizarLicenciaEmpleadoDto(
    int EmpleadoId,
    string TipoLicencia,
    string NumeroLicencia,
    DateTime FechaEmision,
    DateTime FechaVencimiento,
    string? AutoridadEmisora,
    string Estado);

public sealed record AsistenciaDto(
    int Id,
    int EmpleadoId,
    DateTime Fecha,
    DateTime? HoraEntrada,
    DateTime? HoraSalida,
    decimal? HorasTrabajadas,
    string Tipo,
    string Estado);

public sealed record CrearAsistenciaDto(
    int EmpleadoId,
    DateTime Fecha,
    DateTime? HoraEntrada,
    DateTime? HoraSalida,
    decimal? HorasTrabajadas,
    string Tipo,
    string Estado);

public sealed record ActualizarAsistenciaDto(
    int EmpleadoId,
    DateTime Fecha,
    DateTime? HoraEntrada,
    DateTime? HoraSalida,
    decimal? HorasTrabajadas,
    string Tipo,
    string Estado);

public sealed record PlanillaDto(
    int Id,
    int EmpleadoId,
    DateTime PeriodoInicio,
    DateTime PeriodoFin,
    decimal SalarioBase,
    decimal? Bonificaciones,
    decimal? HorasExtra,
    decimal? Deducciones,
    decimal? SalarioNeto,
    DateTime? FechaPago,
    string Estado);

public sealed record CrearPlanillaDto(
    int EmpleadoId,
    DateTime PeriodoInicio,
    DateTime PeriodoFin,
    decimal SalarioBase,
    decimal? Bonificaciones,
    decimal? HorasExtra,
    decimal? Deducciones,
    decimal? SalarioNeto,
    DateTime? FechaPago,
    string Estado);

public sealed record ActualizarPlanillaDto(
    int EmpleadoId,
    DateTime PeriodoInicio,
    DateTime PeriodoFin,
    decimal SalarioBase,
    decimal? Bonificaciones,
    decimal? HorasExtra,
    decimal? Deducciones,
    decimal? SalarioNeto,
    DateTime? FechaPago,
    string Estado);
