using WakeCommerce.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddWakeCommerceServices();

var app = builder.Build();

await app.EnsureDatabaseWithRetryAsync();
app.UseWakeCommercePipeline();

app.Run();

public partial class Program { }
