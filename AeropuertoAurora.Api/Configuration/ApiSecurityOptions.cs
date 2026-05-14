namespace AeropuertoAurora.Api.Configuration;

public sealed class ApiSecurityOptions
{
    public const string SectionName = "ApiSecurity";

    public bool Enabled { get; init; } = false;
    public string? ApiKey { get; init; }
    public string HeaderName { get; init; } = "X-Api-Key";
}
