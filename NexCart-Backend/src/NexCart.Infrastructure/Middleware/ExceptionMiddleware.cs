using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NexCart.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            // Each exception type is logged at its own level in HandleExceptionAsync
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionMiddleware> logger)
    {
        context.Response.ContentType = "application/json";

        var response = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An error occurred while processing your request.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        switch (exception)
        {
            case ValidationException validationException:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Validation failed";
                response.Detail = "One or more validation errors occurred.";
                // Keys are camelCased so the client can match them to its form fields
                response.Errors = validationException.Errors
                    .GroupBy(e => JsonNamingPolicy.CamelCase.ConvertName(e.PropertyName))
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());
                logger.LogWarning("Validation failed for {Path}: {Errors}", context.Request.Path, response.Errors.Keys);
                break;

            case ArgumentNullException:
            case ArgumentException:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Invalid argument";
                logger.LogWarning("Bad request: {Message}", exception.Message);
                break;

            case KeyNotFoundException:
                response.Status = StatusCodes.Status404NotFound;
                response.Title = "Resource not found";
                logger.LogWarning("Resource not found: {Message}", exception.Message);
                break;

            case UnauthorizedAccessException:
                response.Status = StatusCodes.Status401Unauthorized;
                response.Title = "Unauthorized";
                logger.LogWarning("Unauthorized access attempted: {Message}", exception.Message);
                break;

            case InvalidOperationException:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Invalid operation";
                logger.LogWarning("Invalid operation: {Message}", exception.Message);
                break;

            default:
                response.Status = StatusCodes.Status500InternalServerError;
                response.Title = "Internal server error";
                logger.LogError(exception, "Internal server error");
                break;
        }

        context.Response.StatusCode = response.Status.Value;
        return context.Response.WriteAsJsonAsync(response);
    }
}

/// <summary>
/// ProblemDetails response model following RFC 7807 standard
/// </summary>
public class ProblemDetails
{
    public string? Type { get; set; }
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
    public string? Instance { get; set; }
    public IDictionary<string, string[]>? Errors { get; set; }
}
