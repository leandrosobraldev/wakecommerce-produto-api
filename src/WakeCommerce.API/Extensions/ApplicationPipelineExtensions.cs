using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WakeCommerce.API.Logging;

namespace WakeCommerce.API.Extensions;

public static class ApplicationPipelineExtensions
{
    public static WebApplication UseWakeCommercePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Use(RequestScopeMiddleware);
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseCors();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");
        app.MapLogsPage();

        return app;
    }

    private static async Task RequestScopeMiddleware(HttpContext context, RequestDelegate next)
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Request");
        using (logger.BeginScope("Method={Method} Path={Path}", context.Request.Method, context.Request.Path))
        {
            await next(context);
        }
    }

    private static void MapLogsPage(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/logs", (IMemoryLogStore store) =>
        {
            var html = LogsPageHtml.Content;
            return Results.Content(html, "text/html; charset=utf-8");
        }).AllowAnonymous();
    }
}
