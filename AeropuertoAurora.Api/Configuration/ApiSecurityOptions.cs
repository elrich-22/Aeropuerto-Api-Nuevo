namespace AeropuertoAurora.Api.Configuration;

public sealed class ApiSecurityOptions
{
    public const string SectionName = "ApiSecurity";

    public string? ApiKey { get; init; }
    public string HeaderName { get; init; } = "X-Api-Key";
}
