using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WakeCommerce.Application.Common.Exceptions;

namespace WakeCommerce.API.Extensions
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
                                              Exception exception,
                                              CancellationToken cancellationToken)
        {
            Activity? activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

            var problemDetails = exception switch
            {
                NotFoundException nf => CreateProblemDetails("NotFound", nf.Message, 404, "https://tools.ietf.org/html/rfc9110#section-15.5.5", httpContext, activity),
                BadRequestException br => CreateProblemDetails("BadRequest", br.Message, 400, "https://tools.ietf.org/html/rfc9110#section-15.5.1", httpContext, activity),
                _ => CreateProblemDetails("Server error", exception.Message, 500, "https://tools.ietf.org/html/rfc9110#section-15.6.1", httpContext, activity)
            };

            httpContext.Response.StatusCode = problemDetails.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        private static ProblemDetails CreateProblemDetails(string title,
                                                       string detail,
                                                       int status,
                                                       string type,
                                                       HttpContext context,
                                                       Activity? activity)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Type = type,
                Status = status,
                Instance = context.Request.Path,
                Extensions = new Dictionary<string, object?>
            {
                { "requestId", context.TraceIdentifier },
                { "traceId", activity?.Id }
            }
            };
        }
    }
}
