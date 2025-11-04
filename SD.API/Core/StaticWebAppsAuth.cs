using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SD.API.Core;

public static class StaticWebAppsAuth
{
    public static async Task<string?> GetUserIdAsync(this HttpRequestData req, IHttpClientFactory factory, CancellationToken cancellationToken, bool required = true)
    {
        var principal = await req.ParseAndValidateJwtAsync(factory, required, cancellationToken);

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

    private static async Task<ClaimsPrincipal?> ParseAndValidateJwtAsync(this HttpRequestData req, IHttpClientFactory factory, bool required, CancellationToken cancellationToken)
    {
        if (req.Headers.TryGetValues("X-Auth-Token", out var header))
        {
            var authHeader = header.LastOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                // get config from env
                var issuer = ApiStartup.Configurations.AzureAd?.Issuer ?? throw new UnhandledException("issuer is null");
                var clientId = ApiStartup.Configurations.AzureAd?.ClientId ?? throw new UnhandledException("clientId is null");

                try
                {
                    return await ValidateTokenAsync(req, factory, token, issuer, clientId, cancellationToken);
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    JwksCache.Invalidate();
                    return await ValidateTokenAsync(req, factory, token, issuer, clientId, cancellationToken);
                }
            }
        }
        else
        {
            if (required)
                throw new UnhandledException("Authorization header not found");
        }

        return null;
    }

    private static async Task<ClaimsPrincipal> ValidateTokenAsync(this HttpRequestData req, IHttpClientFactory factory, string token, string issuer, string audience, CancellationToken cancellationToken)
    {
        var sw1 = Stopwatch.StartNew();
        var jwksUri = issuer.TrimEnd('/').Replace("v2.0", "discovery/v2.0/keys");
        var keys = await JwksCache.GetKeysAsync(factory, jwksUri, cancellationToken);
        sw1.Stop(); if (sw1.ElapsedMilliseconds > 1000) req.LogWarning($"GetKeysAsync: {sw1.Elapsed}");

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = issuer.TrimEnd('/'),
            ValidAudience = audience,
            IssuerSigningKeys = keys,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true
        };

        var sw2 = Stopwatch.StartNew();
        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParameters, out var _);
        sw2.Stop(); if (sw2.ElapsedMilliseconds > 1000) req.LogWarning($"ValidateToken: {sw2.Elapsed}");

        return principal;
    }
}