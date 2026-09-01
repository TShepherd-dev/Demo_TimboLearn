using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// UTILITY: TestTokenGenerator
/// 
/// PURPOSE: Generate valid JWT tokens for local development/testing WITHOUT Auth0/Entra ID.
/// 
/// WHY THIS EXISTS:
/// - Calling real OIDC providers during development adds friction (signup, API keys, quotas)
/// - This generates tokens that pass JWT validation with a known signing key
/// - Tokens include realistic claims (sub, email, name, role, permission)
/// 
/// SECURITY NOTE: Only use in Development environment!
/// The signing key is hardcoded and public - NOT for production use.
/// 
/// USAGE IN SWAGGER:
/// 1. POST /api/test-token to get a token
/// 2. Click Authorize button (🔒)
/// 3. Enter: Bearer &lt;token&gt;
/// 4. All protected endpoints now work!
/// </summary>
public static class TestTokenGenerator
{
    /// <summary>
    /// Generate a test JWT token with configurable claims
    /// </summary>
    /// <param name="email">User email (also used in sub claim)</param>
    /// <param name="firstName">User first name</param>
    /// <param name="lastName">User last name</param>
    /// <param name="role">User role (default: TeamAdmin - has all permissions)</param>
    /// <returns>JWT token string valid for 24 hours</returns>
    public static string GenerateToken(string email, string firstName, string lastName, string role = "TeamAdmin")
    {
        // Hardcoded signing key - matches key in Program.cs JWT configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TimboLearnDemoSigningKey2026!WhichIsLongEnough"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Claims that mimic real OIDC provider output
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, $"test-{email}"),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, $"{firstName} {lastName}"),
            new Claim("name", firstName),
            new Claim("email", email),
            new Claim("role", role),
            // Grant all permissions for easy testing
            new Claim("permission", "ContentCourse.Assign"),
            new Claim("permission", "ContentCourse.Manage"),
            new Claim("permission", "Team.Manage")
        };

        var token = new JwtSecurityToken(
            issuer: "https://timbolearn-test",
            audience: "https://timbolearn-api",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
