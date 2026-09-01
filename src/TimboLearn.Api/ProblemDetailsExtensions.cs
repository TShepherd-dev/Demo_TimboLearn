using FastEndpoints;
using TimboLearn.Api;

namespace TimboLearn.Api;

/// <summary>
/// Extension methods for returning RFC 7807 Problem Details from FastEndpoints
/// </summary>
public static class ProblemDetailsExtensions
{
    public static async Task SendProblemAsync(
        this IEndpoint endpoint,
        ProblemDetails problem,
        CancellationToken ct = default
    )
    {
        await endpoint.HttpContext.Response.WriteAsJsonAsync(problem, cancellationToken: ct);
    }

    public static Task SendNotFoundAsync(
        this IEndpoint endpoint,
        string? detail = null,
        CancellationToken ct = default
    )
    {
        var problem = ProblemDetails.Create(
            type: "https://api.timbolearn.com/errors/not-found",
            title: "Not Found",
            status: 404,
            detail: detail ?? "The requested resource was not found",
            httpContext: endpoint.HttpContext
        );
        return endpoint.SendProblemAsync(problem, ct);
    }

    public static Task SendValidationProblemAsync(
        this IEndpoint endpoint,
        IDictionary<string, object?> errors,
        string? title = null,
        CancellationToken ct = default
    )
    {
        var problem = ProblemDetails.Create(
            type: "https://api.timbolearn.com/errors/validation-error",
            title: title ?? "Validation Error",
            status: 400,
            detail: "One or more validation errors occurred",
            httpContext: endpoint.HttpContext,
            errors: errors
        );
        return endpoint.SendProblemAsync(problem, ct);
    }

    public static Task SendConflictAsync(
        this IEndpoint endpoint,
        string? detail = null,
        CancellationToken ct = default
    )
    {
        var problem = ProblemDetails.Create(
            type: "https://api.timbolearn.com/errors/conflict",
            title: "Conflict",
            status: 409,
            detail: detail ?? "A conflict occurred",
            httpContext: endpoint.HttpContext
        );
        return endpoint.SendProblemAsync(problem, ct);
    }

    public static Task SendForbiddenAsync(
        this IEndpoint endpoint,
        string? reason = null,
        CancellationToken ct = default
    )
    {
        var problem = ProblemDetails.Create(
            type: "https://api.timbolearn.com/errors/forbidden",
            title: "Forbidden",
            status: 403,
            detail: reason ?? "Access denied",
            httpContext: endpoint.HttpContext
        );
        return endpoint.SendProblemAsync(problem, ct);
    }
}
