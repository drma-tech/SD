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
            req.LogWarning("header count: " + header.Count());
            var authHeader = header.LastOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                // get config from env
                var issuer = ApiStartup.Configurations.AzureAd?.Issuer ?? throw new UnhandledException("issuer is null");
                var clientId = ApiStartup.Configurations.AzureAd?.ClientId ?? throw new UnhandledException("clientId is null");

                return await req.ValidateTokenAsync(token, issuer, clientId, cancellationToken);
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

    private static async Task<ClaimsPrincipal> ValidateTokenAsync(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, string token, string issuer, string audience, CancellationToken cancellationToken)
    {
        // Debug: Inspect the token header
        try
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            if (jwtHandler.CanReadToken(token))
            {
                var jwtToken = jwtHandler.ReadJwtToken(token);
                req.LogWarning(token);
                req.LogWarning($"Token Header: {System.Text.Json.JsonSerializer.Serialize(jwtToken.Header)}");
                req.LogWarning($"Token has kid: {jwtToken.Header.ContainsKey("kid")}");
            }
        }
        catch (Exception ex)
        {
            req.LogWarning($"Token inspection failed: {ex.Message}");
        }

        //---------------------
        var mgr = _configManagers.GetOrAdd(issuer, key => new ConfigurationManager<OpenIdConnectConfiguration>($"{key.TrimEnd('/')}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever()));

        var oidc = await mgr.GetConfigurationAsync(cancellationToken);

        if (oidc.SigningKeys == null || oidc.SigningKeys.Count == 0)
        {
            mgr.RequestRefresh();
            oidc = await mgr.GetConfigurationAsync(cancellationToken);

            if (oidc.SigningKeys == null || oidc.SigningKeys.Count == 0)
            {
                try
                {
                    var keysJson = await new HttpClient().GetStringAsync($"{issuer.TrimEnd('/')}/discovery/v2.0/keys", cancellationToken);
                    req.LogWarning($"JWKS download OK: {keysJson.Length} bytes");
                }
                catch (Exception ex)
                {
                    req.LogWarning($"JWKS fetch failed: {ex.Message}");
                }

                throw new UnhandledException($"No signing keys found for issuer {issuer}");
            }
        }

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

        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.InboundClaimTypeMap.Clear(); // Clear the claim type map for better compatibility

            return handler.ValidateToken(token, validationParameters, out var _);
        }
        catch (SecurityTokenInvalidSigningKeyException)
        {
            // If validation fails due to signing key issues, refresh config and retry
            mgr.RequestRefresh();
            oidc = await mgr.GetConfigurationAsync(cancellationToken);

            validationParameters.IssuerSigningKeys = oidc.SigningKeys;

            var handler = new JwtSecurityTokenHandler();
            return handler.ValidateToken(token, validationParameters, out var _);
        }
    }
}