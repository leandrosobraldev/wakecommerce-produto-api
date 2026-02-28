using Microsoft.AspNetCore.Builder;
using WakeCommerce.IoC.DependencyInjection;

namespace WakeCommerce.API.Extensions;

public static class DatabaseStartupExtensions
{
    private const int MaxRetries = 15;
    private const int DelayMs = 2000;

    public static async Task EnsureDatabaseWithRetryAsync(this WebApplication app)
    {
        if (app.Environment.IsEnvironment("Testing"))
            return;

        var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseStartup");

        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                logger.LogInformation("Garantir que o banco de dados esteja pronto (attempt {Attempt}/{Max})", attempt, MaxRetries);
                await app.Services.EnsureDatabaseSeededAsync();
                logger.LogInformation("O banco está pronto.");
                return;
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (attempt < MaxRetries)
            {
                logger.LogWarning(ex, "Banco de dados ainda não está pronto. Tentando novamente em {Delay}ms...", DelayMs);
                await Task.Delay(DelayMs);
            }
        }
    }
}
