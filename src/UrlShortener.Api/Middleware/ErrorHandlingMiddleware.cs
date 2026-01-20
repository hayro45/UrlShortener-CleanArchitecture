using System.Net;
using System.Text.Json;
using FluentValidation;
using UrlShortener.Application.Common.Exceptions;

namespace UrlShortener.Api.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches all unhandled exceptions and returns appropriate HTTP responses.
/// </summary>
public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var (statusCode, title, detail) = exception switch
        {
            NotFoundException notFound => (
                HttpStatusCode.NotFound,
                "Resource Not Found",
                notFound.Message
            ),
            ValidationException validation => (
                HttpStatusCode.BadRequest,
                "Validation Error",
                string.Join("; ", validation.Errors.Select(e => e.ErrorMessage))
            ),
            ConflictException conflict => (
                HttpStatusCode.Conflict,
                "Conflict",
                conflict.Message
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Internal Server Error",
                _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An error occurred while processing your request."
            )
        };

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status = (int)statusCode,
            detail,
            instance = context.Request.Path.ToString(),
            traceId = context.TraceIdentifier,
            // Stack trace only in development
            stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
        };

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}
