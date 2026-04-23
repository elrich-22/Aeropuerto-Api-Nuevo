using AeropuertoAurora.Api.Configuration;
using Microsoft.Extensions.Options;

namespace AeropuertoAurora.Api.Middleware;

public sealed class ApiKeyMiddleware(RequestDelegate next, IOptions<ApiSecurityOptions> options, IWebHostEnvironment environment)
{
    private readonly ApiSecurityOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (IsPublicEndpoint(context) || string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(_options.HeaderName, out var apiKey) ||
            !string.Equals(apiKey, _options.ApiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "API key invalida o ausente." });
            return;
        }

        await next(context);
    }

    private bool IsPublicEndpoint(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        return environment.IsDevelopment() && (path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase) ||
                                               path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) ||
                                               path.Equals("/api/health", StringComparison.OrdinalIgnoreCase));
    }
}
