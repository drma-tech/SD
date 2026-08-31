using Clerk.BackendAPI.Helpers.Jwks;
using Microsoft.Azure.Functions.Worker.Http;
using System.Globalization;
using System.Security.Claims;

namespace SD.API.Core.Auth;

public static class AuthUsersHelper
{
    public static async Task<string> GetUserIdAsync(this HttpRequestData req)
    {
        var principal = await req.ParseAndValidateJwtAsync();

        var id = principal?.Claims.FirstOrDefault(w => string.Equals(w.Type, "user_id", StringComparison.OrdinalIgnoreCase))?.Value;

        return id ?? throw new UnhandledException("unauthenticated user");
    }

    public static string? GetUserIP(this HttpRequestData req, bool includePort)
    {
        if (req.Headers.TryGetValues("X-Forwarded-For", out var values))
        {
            if (includePort)
                return values.FirstOrDefault()?.Split(',')[0];

            return values.FirstOrDefault()?.Split(',')[0].Split(':')[0];
        }

        if (string.Equals(Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT"), "Development", StringComparison.OrdinalIgnoreCase))
        {
            return "127.0.0.1";
        }

        return null;
    }

    public static CultureInfo? GetUserCulture(this HttpRequestData req)
    {
        var language = "en";

        if (req.Headers.TryGetValues("Referer", out var referers))
        {
            var referer = referers.FirstOrDefault();

            if (Uri.TryCreate(referer, UriKind.Absolute, out var uri))
            {
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (segments.Length > 0 && ConfigurationsStatic.SupportedLanguages.Contains(segments[0], StringComparer.OrdinalIgnoreCase))
                {
                    language = segments[0];
                }
            }
        }

        return CultureInfo.GetCultureInfo(language);
    }

    private static async Task<ClaimsPrincipal?> ParseAndValidateJwtAsync(this HttpRequestData req)
    {
        if (req.Headers.TryGetValues("X-Clerk-Token", out var headerClerk))
        {
            var authHeader = headerClerk.LastOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length);

                var options = new VerifyTokenOptions(
                    secretKey: ApiStartup.Configurations.ClerkAuth?.SecretKey,
                    authorizedParties: [
                        "https://localhost:7272",
                        "https://streamingdiscovery.com",
                    ],
                    clockSkewInMs: 10_000
                );

                var result = await VerifyToken.VerifyTokenAsync(token, options);

                var claims = new List<Claim>
                {
                    new("user_id", result.Claims.FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase))?.Value ?? ""),
                };

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "clerk"));
            }
        }

        return null;
    }
}