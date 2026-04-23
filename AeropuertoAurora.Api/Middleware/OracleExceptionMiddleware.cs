using AeropuertoAurora.Api.DTOs;
using Oracle.ManagedDataAccess.Client;

namespace AeropuertoAurora.Api.Middleware;

public sealed class OracleExceptionMiddleware(RequestDelegate next, ILogger<OracleExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OracleException exception) when (exception.Number == 1)
        {
            logger.LogWarning(exception, "Restriccion unica violada en Oracle.");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ErrorApiDto("Ya existe un registro con esos datos o la identity de Oracle no esta sincronizada."));
        }
        catch (OracleException exception) when (exception.Number == 2291)
        {
            logger.LogWarning(exception, "Llave foranea padre no encontrada en Oracle.");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ErrorApiDto("No existe el registro padre requerido por una llave foranea."));
        }
        catch (OracleException exception) when (exception.Number == 2292)
        {
            logger.LogWarning(exception, "Registro con hijos no puede eliminarse en Oracle.");
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new ErrorApiDto("No se puede eliminar porque existen registros relacionados."));
        }
    }
}
