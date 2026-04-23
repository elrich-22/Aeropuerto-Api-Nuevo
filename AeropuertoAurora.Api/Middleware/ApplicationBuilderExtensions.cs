namespace AeropuertoAurora.Api.Middleware;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiKeyProtection(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiKeyMiddleware>();
    }

    public static IApplicationBuilder UseOracleExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<OracleExceptionMiddleware>();
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
            context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
            context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
            context.Response.Headers.TryAdd("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            await next();
        });
    }
}
