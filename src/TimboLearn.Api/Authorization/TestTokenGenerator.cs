using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TimboLearn.Api.Authorization;

public static class TestTokenGenerator
{
    public static string GenerateToken(string email, string firstName, string lastName, string role = "TeamAdmin")
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TimboLearnDemoSigningKey2026!WhichIsLongEnough"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, $"test-{email}"),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, $"{firstName} {lastName}"),
            new Claim("name", firstName),
            new Claim("email", email),
            new Claim("role", role),
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
