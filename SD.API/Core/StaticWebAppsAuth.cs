using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SD.API.Core;

public static class StaticWebAppsAuth
{
    public static async Task<string?> GetUserIdAsync(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, CancellationToken cancellationToken, bool required = true)
    {
        var principal = await req.ParseAndValidateJwtAsync(required, cancellationToken);

        var id = principal?.Claims.FirstOrDefault(w => w.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier" || w.Type == "oid")?.Value;

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
        if (req.Headers.TryGetValues("X-Auth-Token", out var header))
        {
            var authHeader = header.LastOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                // get config from env
                var issuer = ApiStartup.Configurations.AzureAd?.Issuer ?? throw new UnhandledException("issuer is null");
                var clientId = ApiStartup.Configurations.AzureAd?.ClientId ?? throw new UnhandledException("clientId is null");

                return await ValidateTokenAsync(req, token, issuer, clientId, cancellationToken);
            }
        }
        else
        {
            if (required)
                throw new UnhandledException("Authorization header not found");
        }

        return null;
    }

    private static async Task<ClaimsPrincipal> ValidateTokenAsync(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, string token, string issuer, string audience, CancellationToken cancellationToken)
    {
        var sw1 = Stopwatch.StartNew();
        var oidc = await LoadConfigurationAsync(req, issuer, cancellationToken); //sometimes takes long time here (20 seconds or more)
        sw1.Stop(); if (sw1.ElapsedMilliseconds > 2000) req.LogWarning($"GetConfigurationAsync: {sw1.Elapsed}");

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

        var sw2 = Stopwatch.StartNew();
        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParameters, out var _);
        sw2.Stop(); if (sw2.ElapsedMilliseconds > 2000) req.LogWarning($"ValidateToken: {sw2.Elapsed}");

        return principal;
    }

    private static async Task<OpenIdConnectConfiguration> LoadConfigurationAsync(this Microsoft.Azure.Functions.Worker.Http.HttpRequestData req, string issuer, CancellationToken cancellationToken)
    {
        var mgr = new ConfigurationManager<OpenIdConnectConfiguration>($"{issuer.TrimEnd('/')}/.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever()) {  };

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        try
        {
            return await mgr.GetConfigurationAsync(linkedCts.Token);
        }
        catch (Exception ex)
        {
            req.LogWarning($"LoadConfigurationAsync: {ex.Message}");
            return await mgr.GetConfigurationAsync(cancellationToken);
        }
        finally
        {
            cts.Dispose();
            linkedCts.Dispose();
        }
    }
}