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

        for (var i = 0; i < MaxRetries; i++)
        {
            try
            {
                await app.Services.EnsureDatabaseSeededAsync();
                return;
            }
            catch (Microsoft.Data.SqlClient.SqlException) when (i < MaxRetries - 1)
            {
                await Task.Delay(DelayMs);
            }
        }
    }
}
