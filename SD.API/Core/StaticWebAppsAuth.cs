using Microsoft.Azure.Functions.Worker.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace SD.API.Core;

public class ClientPrincipal
{
    public string? IdentityProvider { get; set; }
    public string? UserId { get; set; }
    public string? UserDetails { get; set; }
    public IEnumerable<string> UserRoles { get; set; } = [];
}

public static class StaticWebAppsAuth
{
    public static string? GetUserId(this HttpRequestData req, bool required = true)
    {
        var principal = req.ParseJwt();

        if (required)
            return principal?.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new UnhandledException("user id not available");
        else
            return principal?.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    public static string? GetUserIP(this HttpRequestData req, bool includePort = true)
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

    private static ClaimsPrincipal? ParseJwt(this HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Authorization", out var header))
            return null;

        var authHeader = header.FirstOrDefault();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return null;

        var token = authHeader.Substring("Bearer ".Length);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var idp = jwtToken.Payload.TryGetValue("idp", out var idpValue) ? idpValue?.ToString() : null;
        var identity = new ClaimsIdentity(idp);

        foreach (var kv in jwtToken.Payload)
        {
            if (kv.Value is JsonElement je && je.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in je.EnumerateArray())
                    identity.AddClaim(new Claim(kv.Key, item.ToString() ?? ""));
            }
            else
            {
                identity.AddClaim(new Claim(kv.Key, kv.Value?.ToString() ?? ""));
            }
        }

        //add claims not recognized by default
        var oid = jwtToken.Payload.TryGetValue("oid", out var oidValue) ? oidValue?.ToString() : null;
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, oid ?? throw new UnhandledException("invalid oid")));

        identity.AddClaim(new Claim(ClaimTypes.Name, jwtToken.Payload.TryGetValue("name", out var name) ? name?.ToString() ?? "" : ""));

        if (jwtToken.Payload.TryGetValue("roles", out var rolesObj) && rolesObj is JsonElement rolesElement && rolesElement.ValueKind == JsonValueKind.Array)
        {
            foreach (var role in rolesElement.EnumerateArray())
                identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
        }

        return new ClaimsPrincipal(identity);
    }
}