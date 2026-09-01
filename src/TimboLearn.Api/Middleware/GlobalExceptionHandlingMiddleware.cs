using System.Net;
using System.Text.Json;
using TimboLearn.Infrastructure;

namespace TimboLearn.Api.Middleware;

/// <summary>
/// MIDDLEWARE: GlobalExceptionHandlingMiddleware
/// 
/// PURPOSE: Catch all unhandled exceptions and return RFC 7807 Problem Details
/// 
/// HANDLES:
/// - BusinessException (and subclasses): Returns structured error with appropriate status code
/// - Other exceptions: Returns 500 Internal Server Error with sanitized details
/// 
/// POSITION IN PIPELINE: After authentication/authorization, wraps endpoint execution
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IHostEnvironment env
    )
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException ex)
        {
            await HandleBusinessExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnhandledExceptionAsync(context, ex);
        }
    }

    private async Task HandleBusinessExceptionAsync(HttpContext context, BusinessException ex)
    {
        _logger.LogWarning(
            ex,
            "Business error: {Type} - {Title}",
            ex.Type,
            ex.Title
        );

        var problemDetails = ProblemDetails.Create(
            type: ex.Type,
            title: ex.Title,
            status: ex.StatusCode,
            detail: ex.Message,
            httpContext: context,
            errors: ex.Errors
        );

        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private async Task HandleUnhandledExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(
            ex,
            "Unhandled exception: {Message}",
            ex.Message
        );

        var problemDetails = ProblemDetails.Create(
            type: "https://api.timbolearn.com/errors/internal-server-error",
            title: "Internal Server Error",
            status: 500,
            detail: _env.IsDevelopment() ? ex.Message : "An unexpected error occurred",
            httpContext: context
        );

        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
