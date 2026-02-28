using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using WakeCommerce.Application.Common.Exceptions;
using WakeCommerce.Domain.Validation;

namespace WakeCommerce.API.Extensions
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var activity = httpContext.Features.Get<IHttpActivityFeature>()?.Activity;

            var problemDetails = exception switch
            {
                NotFoundException nf => CreateProblemDetails(
                    title: "Not Found",
                    detail: nf.Message,
                    status: StatusCodes.Status404NotFound,
                    type: "https://httpstatuses.com/404",
                    code: "NOT_FOUND",
                    httpContext, activity),

                BadRequestException br => CreateProblemDetails(
                    title: "Bad Request",
                    detail: br.Message,
                    status: StatusCodes.Status400BadRequest,
                    type: "https://httpstatuses.com/400",
                    code: "BAD_REQUEST",
                    httpContext, activity),

                DomainExceptionValidation dev => CreateProblemDetails(
                    title: "Validation Error",
                    detail: dev.Message,
                    status: StatusCodes.Status422UnprocessableEntity,
                    type: "https://httpstatuses.com/422",
                    code: "VALIDATION_ERROR",
                    httpContext, activity),

                _ => CreateProblemDetails(
                    title: "Server Error",
                    detail: "Ocorreu um erro inesperado.",
                    status: StatusCodes.Status500InternalServerError,
                    type: "https://httpstatuses.com/500",
                    code: "SERVER_ERROR",
                    httpContext, activity)
            };

            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = problemDetails.Status!.Value;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }

        private static ProblemDetails CreateProblemDetails(
            string title,
            string detail,
            int status,
            string type,
            string code,
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
                Extensions =
                {
                    ["code"] = code,
                    ["requestId"] = context.TraceIdentifier,
                    ["traceId"] = activity?.Id
                }
            };
        }
    }
}