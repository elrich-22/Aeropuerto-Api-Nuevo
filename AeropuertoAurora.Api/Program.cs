using AeropuertoAurora.Api.Configuration;
using AeropuertoAurora.Api.Data;
using AeropuertoAurora.Api.Middleware;
using AeropuertoAurora.Api.Repositories;
using AeropuertoAurora.Api.Services;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection(DatabaseOptions.SectionName));
builder.Services.Configure<ApiSecurityOptions>(builder.Configuration.GetSection(ApiSecurityOptions.SectionName));

builder.Services.AddSingleton<IOracleConnectionFactory, OracleConnectionFactory>();
builder.Services.AddScoped<IAeropuertoReadRepository, AeropuertoReadRepository>();
builder.Services.AddScoped<ITableReadRepository, TableReadRepository>();
builder.Services.AddScoped<IOracleCrudRepository, OracleCrudRepository>();
builder.Services.AddScoped<IAeropuertoQueryService, AeropuertoQueryService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Aeropuerto Aurora API",
        Version = "v1",
        Description = "API para consultar datos operativos del Aeropuerto Aurora."
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Ingresa la API key configurada. Ejemplo: clave-local-segura",
        Name = "X-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("ApiKey", document, null),
            []
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        if (origins.Length == 0)
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(origins);
        }

        policy.AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSecurityHeaders();
app.UseOracleExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Aeropuerto Aurora API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Aeropuerto Aurora API";
    });
}

app.UseCors("Frontend");
app.UseApiKeyProtection();
app.UseAuthorization();

app.MapControllers();

app.Run();
