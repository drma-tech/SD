using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
    public static async Task<string?> GetUserIdAsync(this HttpRequestData req, bool required = true)
    {
        var principal = await req.ParseAndValidateJwtAsync();

        var id = principal?.Claims.FirstOrDefault(w => w.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier" || w.Type == "oid")?.Value;

        if (required)
            return id ?? throw new UnhandledException("user id not available");
        else
            return id;
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

    private static async Task<ClaimsPrincipal?> ParseAndValidateJwtAsync(this HttpRequestData req)
    {
        if (req.Headers.TryGetValues("Authorization", out var header))
        {
            var authHeader = header.FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                // get config from env
                var issuer = ApiStartup.Configurations.AzureAd?.Issuer ?? throw new UnhandledException("issuer is null");
                var clientId = ApiStartup.Configurations.AzureAd?.ClientId ?? throw new UnhandledException("clientId is null");

                try
                {
                    return await ValidateTokenAsync(token, issuer, clientId);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        return null;
    }

    private static async Task<ClaimsPrincipal> ValidateTokenAsync(string token, string issuer, string audience)
    {
        System.Collections.Concurrent.ConcurrentDictionary<string, Microsoft.IdentityModel.Protocols.ConfigurationManager<OpenIdConnectConfiguration>> _configManagers = new();
        var mgr = _configManagers.GetOrAdd(issuer, key => new Microsoft.IdentityModel.Protocols.ConfigurationManager<OpenIdConnectConfiguration>($"{key}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever()));

        var oidc = await mgr.GetConfigurationAsync(CancellationToken.None);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer.TrimEnd('/'),
            ValidAudience = audience,
            IssuerSigningKeys = oidc.SigningKeys,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };

        var handler = new JwtSecurityTokenHandler();
        return handler.ValidateToken(token, validationParameters, out var _);
    }
}