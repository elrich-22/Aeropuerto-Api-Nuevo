namespace AeropuertoAurora.Api.Configuration;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = "AeropuertoAurora";
    public string Audience { get; init; } = "AeropuertoAuroraApp";
    public int ExpirationMinutes { get; init; } = 480;
}
