using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using WakeCommerce.API.Logging;
using WakeCommerce.Infrastructure.Context;
using WakeCommerce.IoC.DependencyInjection;

namespace WakeCommerce.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static WebApplicationBuilder AddWakeCommerceServices(this WebApplicationBuilder builder)
    {
        // ---- Observabilidade / Logs ----
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IMemoryLogStore, MemoryLogStore>();
        builder.Services.AddSingleton<ILoggerProvider, MemoryLoggerProvider>();

        builder.Logging.AddSimpleConsole(o =>
        {
            o.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
            o.IncludeScopes = true;
        });

        // ----Controllers ----
        builder.Services
            .AddControllers()
            .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // ---- INFRA  ----
        builder.Services.AddInfrastructureAPI(builder.Configuration);

        // ---- Swagger ----
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "WAKE Commerce Produto.API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                },
                Array.Empty<string>()
            }
        });
        });

        // ---- Exception ----
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // ---- Health ----
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database");

        // ---- CORS ----
        ConfigureCors(builder);

        // ---- Auth / Rate limit ----
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddWakeCommerceRateLimiting();

        return builder;
    }

    private static void ConfigureCors(WebApplicationBuilder builder)
    {
        var allowedOrigins =
            builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigins.Length > 0)
                    policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader();
                else
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });
    }
}
