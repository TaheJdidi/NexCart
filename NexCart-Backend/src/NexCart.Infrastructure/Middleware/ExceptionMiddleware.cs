using System.Text.Json;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
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
            case ArgumentNullException:
            case ArgumentException:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Invalid argument";
                break;

            case KeyNotFoundException:
                response.Status = StatusCodes.Status404NotFound;
                response.Title = "Resource not found";
                break;

            case UnauthorizedAccessException:
                response.Status = StatusCodes.Status401Unauthorized;
                response.Title = "Unauthorized";
                break;

            case InvalidOperationException:
                response.Status = StatusCodes.Status400BadRequest;
                response.Title = "Invalid operation";
                break;

            default:
                response.Status = StatusCodes.Status500InternalServerError;
                response.Title = "Internal server error";
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
}
