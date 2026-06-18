using System.Net;
using Bookstore.Application.Exceptions;
using Bookstore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            DomainException => HttpStatusCode.BadRequest,
            ConflictException => HttpStatusCode.Conflict,
            NotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception processing request.");
        else
            _logger.LogWarning(exception, "Request failed with expected application error.");

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = statusCode == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : exception.Message,
            Detail = statusCode == HttpStatusCode.InternalServerError
                ? "Check application logs for details."
                : exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem);
    }
}
