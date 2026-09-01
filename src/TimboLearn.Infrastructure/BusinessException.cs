namespace TimboLearn.Infrastructure;

/// <summary>
/// Base exception for business logic errors that should return RFC 7807 Problem Details
/// </summary>
public class BusinessException : Exception
{
    public string Type { get; }
    public int StatusCode { get; }
    public IDictionary<string, object?>? Errors { get; }

    public BusinessException(
        string type,
        string title,
        int statusCode,
        string? detail = null,
        IDictionary<string, object?>? errors = null
    ) : base(detail ?? title)
    {
        Type = type;
        StatusCode = statusCode;
        Errors = errors;
        Title = title;
    }

    public string Title { get; }
}

public class NotFoundException : BusinessException
{
    public NotFoundException(string resource, string? detail = null)
        : base(
            type: "https://api.timbolearn.com/errors/not-found",
            title: $"{resource} Not Found",
            statusCode: 404,
            detail: detail ?? $"The specified {resource.ToLower()} was not found"
        )
    {
    }
}

public class ValidationException : BusinessException
{
    public ValidationException(
        string title = "Validation Error",
        IDictionary<string, object?>? errors = null
    ) : base(
        type: "https://api.timbolearn.com/errors/validation-error",
        title: title,
        statusCode: 400,
        detail: "One or more validation errors occurred",
        errors: errors
    )
    {
    }
}

public class ConflictException : BusinessException
{
    public ConflictException(string resource, string? detail = null)
        : base(
            type: "https://api.timbolearn.com/errors/conflict",
            title: $"{resource} Conflict",
            statusCode: 409,
            detail: detail ?? $"A conflict occurred with the {resource.ToLower()}"
        )
    {
    }
}

public class ForbiddenException : BusinessException
{
    public ForbiddenException(string reason = "Access denied")
        : base(
            type: "https://api.timbolearn.com/errors/forbidden",
            title: "Forbidden",
            statusCode: 403,
            detail: reason
        )
    {
    }
}
