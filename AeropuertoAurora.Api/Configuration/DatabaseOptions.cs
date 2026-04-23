namespace AeropuertoAurora.Api.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string? ConnectionString { get; init; }
    public string Schema { get; init; } = "AEROPUERTO_AURORA";
}
