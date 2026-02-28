using WakeCommerce.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// DI + Cross-cutting (Swagger, Auth, RateLimit, CORS, Logs, Health, Infra)
builder.AddWakeCommerceServices();

var app = builder.Build();

// Ensures DB is ready (useful in Docker); skipped in Testing
await app.EnsureDatabaseWithRetryAsync();

// Middlewares + endpoints
app.UseWakeCommercePipeline();

app.Run();

public partial class Program { }
