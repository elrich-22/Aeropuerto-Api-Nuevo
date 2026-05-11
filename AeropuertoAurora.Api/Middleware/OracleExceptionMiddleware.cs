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
        catch (OracleException exception) when (exception.Number == 12899)
        {
            logger.LogWarning(exception, "Valor demasiado largo para una columna Oracle.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorApiDto("Un valor excede el tamano permitido por la base de datos."));
        }
        catch (OracleException exception) when (exception.Number == 904)
        {
            logger.LogError(exception, "Columna o identificador invalido en Oracle.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorApiDto("Hay una columna mal configurada en el API para esta operacion."));
        }
    }
}
