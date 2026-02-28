using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using WakeCommerce.API.Extensions;
using WakeCommerce.Infrastructure.Context;
using WakeCommerce.IoC.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructureAPI(builder.Configuration);

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

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(name: "database");

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
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

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Rate Limiting
builder.Services.AddWakeCommerceRateLimiting();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    // Em Docker o SQL Server pode levar alguns segundos para ficar pronto
    const int maxRetries = 15;
    const int delayMs = 2000;
    for (var i = 0; i < maxRetries; i++)
    {
        try
        {
            await app.Services.EnsureDatabaseSeededAsync();
            break;
        }
        catch (Microsoft.Data.SqlClient.SqlException) when (i < maxRetries - 1)
        {
            await Task.Delay(delayMs);
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
