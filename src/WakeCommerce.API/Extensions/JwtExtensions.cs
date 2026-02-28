using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WakeCommerce.API.Extensions;

public static class JwtExtensions
{
    private const int MinSecretLength = 32;

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        
        var secretKey = configuration["Jwt:SecretKey"];
        var issuer = configuration["Jwt:Issuer"] ?? "WakeCommerce.API";
        var audience = configuration["Jwt:Audience"] ?? "WakeCommerce.Client";

        
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("Jwt:SecretKey não configurado. Configure no appsettings ou variável de ambiente.");
        }

        
        if (secretKey.Length < MinSecretLength)
        {
            throw new InvalidOperationException($"Jwt:SecretKey precisa ter pelo menos {MinSecretLength} caracteres.");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Assinatura
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    // Emissor / Audiência
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,

                    // Expiração
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        
                         var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                             .CreateLogger("Jwt");
                         logger.LogWarning(context.Exception, "JWT inválido.");

                        return Task.CompletedTask;
                    },

                    OnChallenge = async context =>
                    {
                        
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json; charset=utf-8";

                        var problem = new ProblemDetails
                        {
                            Title = "Unauthorized",
                            Status = StatusCodes.Status401Unauthorized,
                            Type = "https://httpstatuses.com/401",
                            Detail = "Token JWT ausente, inválido ou expirado."
                        };

                        
                        problem.Extensions["code"] = "USER_UNAUTHORIZED";
                        problem.Extensions["requestId"] = context.HttpContext.TraceIdentifier;

                        await context.Response.WriteAsJsonAsync(problem);
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}