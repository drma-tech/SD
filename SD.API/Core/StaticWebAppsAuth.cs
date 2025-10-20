using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
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
    public static async Task<string?> GetUserIdAsync(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, CancellationToken cancellationToken, bool required = true)
    {
        var principal = await req.ParseAndValidateJwtAsync(required, cancellationToken);

        var id = principal?.Claims.FirstOrDefault(w => w.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier" || w.Type == "oid")?.Value;

        req.LogWarning("Claims: " + string.Join(";", principal?.Claims ?? []));

        if (required)
            return id ?? throw new UnhandledException("user id not available");
        else
            return id;
    }

    public static string? GetUserIP(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, bool includePort = true)
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

    private static async Task<ClaimsPrincipal?> ParseAndValidateJwtAsync(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, bool required, CancellationToken cancellationToken)
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

                return await ValidateTokenAsync(token, issuer, clientId, cancellationToken);
            }
        }
        else
        {
            if (required)
                throw new UnhandledException("Authorization header not found");
        }

        return null;
    }

    private static readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> _configManagers = new();

    private static async Task<ClaimsPrincipal> ValidateTokenAsync(string token, string issuer, string audience, CancellationToken cancellationToken)
    {
        var mgr = _configManagers.GetOrAdd(issuer, key => new ConfigurationManager<OpenIdConnectConfiguration>($"{key.TrimEnd('/')}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever()));

        var oidc = await mgr.GetConfigurationAsync(cancellationToken);

        if (oidc.SigningKeys == null || oidc.SigningKeys.Count == 0)
            throw new UnhandledException($"No signing keys found for issuer {issuer}");

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