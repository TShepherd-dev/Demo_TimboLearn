using System.Security.Claims;

namespace TimboLearn.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static string? GetSubjectId(this ClaimsPrincipal user)
    {
        // Try multiple claim type variants for the subject/user ID
        var claimTypes = new[]
        {
            "sub",
            ClaimTypes.NameIdentifier,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        };

        foreach (var claimType in claimTypes)
        {
            var claim = user.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
        }

        return null;
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        var claimTypes = new[]
        {
            "email",
            ClaimTypes.Email,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        };

        foreach (var claimType in claimTypes)
        {
            var claim = user.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
        }

        return null;
    }

    public static string? GetName(this ClaimsPrincipal user)
    {
        var claimTypes = new[]
        {
            "name",
            ClaimTypes.Name,
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
        };

        foreach (var claimType in claimTypes)
        {
            var claim = user.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }
        }

        return null;
    }
}
