using System.Text.Json;

namespace TimboLearn.Api;

/// <summary>
/// RFC 7807 Problem Details for HTTP APIs
/// Standard format for error responses
/// </summary>
public record ProblemDetails(
    string Type,
    string Title,
    int Status,
    string Detail,
    string? Instance = null,
    string? TraceId = null,
    IDictionary<string, object?>? Errors = null
)
{
    public static ProblemDetails Create(
        string type,
        string title,
        int status,
        string detail,
        HttpContext? httpContext = null,
        IDictionary<string, object?>? errors = null
    )
    {
        return new ProblemDetails(
            Type: type,
            Title: title,
            Status: status,
            Detail: detail,
            Instance: httpContext?.Request.Path,
            TraceId: httpContext?.TraceIdentifier,
            Errors: errors
        );
    }
}
