using FirebaseAdmin.Auth;
using Microsoft.Azure.Functions.Worker.Http;
using System.Security.Claims;

namespace SD.API.Core.Auth;

public static class StaticWebAppsAuth
{
    public static async Task<string?> GetUserIdAsync(this HttpRequestData req, CancellationToken cancellationToken, bool required = true)
    {
        var principal = await req.ParseAndValidateJwtAsync(required, cancellationToken);

        var id = principal?.Claims.FirstOrDefault(w => w.Type == "user_id")?.Value;

        if (required)
            return id ?? throw new UnhandledException("user id not available");
        else
            return id;
    }

    public static string? GetUserIP(this HttpRequestData req, bool includePort)
    {
        if (req.Headers.TryGetValues("X-Forwarded-For", out var values))
        {
            if (includePort)
                return values.FirstOrDefault()?.Split(',')[0];
            else
                return values.FirstOrDefault()?.Split(',')[0].Split(':')[0];
        }

        if (Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development")
        {
            return "127.0.0.1";
        }

        return null;
    }

    private static async Task<ClaimsPrincipal?> ParseAndValidateJwtAsync(this HttpRequestData req, bool required, CancellationToken cancellationToken)
    {
        if (req.Headers.TryGetValues("X-Auth-Token", out var header))
        {
            var authHeader = header.LastOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, cancellationToken);

                var claims = decodedToken.Claims.Select(kv => new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty)).ToList();

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "firebase"));
            }
        }
        else
        {
            if (required)
                throw new UnhandledException("Authorization header not found");
        }

        return null;
    }
}