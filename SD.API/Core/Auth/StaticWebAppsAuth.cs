using FirebaseAdmin.Auth;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

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
        if (req.Headers.TryGetValues("X-Firebase-Token", out var header1))
        {
            var authHeader = header1.LastOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token, cancellationToken);

                var claims = decodedToken.Claims.Select(kv => new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty)).ToList();

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "firebase"));
            }
        }
        else if (req.Headers.TryGetValues("X-Supabase-Token", out var header2))
        {
            var authHeader = header2.LastOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);

                var projectRef = "mlsztbywzbbqqbwgplky";
                var audience = "authenticated";

                var principal = await VerifyTokenAsync(token, projectRef, audience, cancellationToken);

                var claims = principal.Claims.ToList();

                claims.Add(new Claim("user_id", principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? ""));

                return new ClaimsPrincipal(new ClaimsIdentity(claims, "supabase"));
            }
        }
        else if (req.Headers.TryGetValues("X-Auth-Token", out var header)) //todo: old header name, to be removed in the future
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

    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static JsonWebKeySet? _jwksCache;
    private static DateTime _jwksCacheExpiry = DateTime.MinValue;

    public static async Task<ClaimsPrincipal> VerifyTokenAsync(string token, string projectRef, string audience = "authenticated", CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        // Lê JWT
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Baixa JWKS se cache expirou ou não existe
        if (_jwksCache == null || _jwksCacheExpiry < DateTime.UtcNow)
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                if (_jwksCache == null || _jwksCacheExpiry < DateTime.UtcNow)
                {
                    using var http = new HttpClient();
                    var jwksUrl = $"https://{projectRef}.supabase.co/auth/v1/.well-known/jwks.json";
                    var jwksJson = await http.GetStringAsync(jwksUrl, cancellationToken);
                    _jwksCache = new JsonWebKeySet(jwksJson);
                    _jwksCacheExpiry = DateTime.UtcNow.AddHours(1); // cache por 1h
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // Encontrar a chave que bate com o kid do token
        var jwk = _jwksCache.Keys.FirstOrDefault(k => k.Kid == jwt.Header.Kid) ?? throw new SecurityTokenException("Supabase signing key not found for kid: " + jwt.Header.Kid);

        // Criar ECDsaSecurityKey a partir do JWK (ES256 / P-256)
        var ecdsa = ECDsa.Create(new ECParameters
        {
            Curve = ECCurve.NamedCurves.nistP256,
            Q = new ECPoint
            {
                X = Base64UrlEncoder.DecodeBytes(jwk.X),
                Y = Base64UrlEncoder.DecodeBytes(jwk.Y)
            }
        });

        var signingKey = new ECDsaSecurityKey(ecdsa) { KeyId = jwk.Kid };

        // Validação do token
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://{projectRef}.supabase.co/auth/v1",

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidAlgorithms = [SecurityAlgorithms.EcdsaSha256]
        };

        var principal = handler.ValidateToken(token, validationParameters, out _);

        return principal;
    }
}