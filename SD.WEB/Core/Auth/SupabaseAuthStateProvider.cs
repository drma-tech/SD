using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace SD.WEB.Core.Auth
{
    public class SupabaseAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public void OnAuthChanged(string? token)
        {
            GenerateClaimsIdentity(token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void GenerateClaimsIdentity(string? token)
        {
            AppStateStatic.SupabaseToken = null;

            if (token == null)
            {
                _currentUser = new(new ClaimsIdentity());
            }
            else
            {
                var claims = JwtParser.ParseClaimsFromJwt(token).ToList();

                var userMetadataClaim = claims.SingleOrDefault(c => string.Equals(c.Type, "user_metadata", StringComparison.OrdinalIgnoreCase));
                if (userMetadataClaim != null)
                {
                    using var doc = JsonDocument.Parse(userMetadataClaim.Value);
                    if (doc.RootElement.TryGetProperty("iss", out var iss))
                    {
                        if (iss.GetString()?.Contains("apple", StringComparison.OrdinalIgnoreCase) ?? false)
                            claims.Add(new Claim("idp", "apple"));
                        else if (iss.GetString()?.Contains("google", StringComparison.OrdinalIgnoreCase) ?? false)
                            claims.Add(new Claim("idp", "google"));
                        else if (iss.GetString()?.Contains("microsoft", StringComparison.OrdinalIgnoreCase) ?? false)
                            claims.Add(new Claim("idp", "microsoft"));
                        else
                            claims.Add(new Claim("idp", iss.GetString() ?? "email"));
                    }
                    else
                    {
                        claims.Add(new Claim("idp", "email"));
                    }

                    if (doc.RootElement.TryGetProperty("full_name", out var full_name))
                    {
                        claims.Add(new Claim("name", full_name.GetString() ?? ""));
                    }
                    else if (doc.RootElement.TryGetProperty("name", out var name))
                    {
                        claims.Add(new Claim("name", name.GetString() ?? ""));
                    }

                    if (doc.RootElement.TryGetProperty("avatar_url", out var avatar_url))
                    {
                        claims.Add(new Claim("avatar", avatar_url.GetString() ?? ""));
                    }
                }

                var userId = claims.Single(c => string.Equals(c.Type, "sub", StringComparison.OrdinalIgnoreCase)).Value;
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

                var emailClaim = claims.SingleOrDefault(c => string.Equals(c.Type, "email", StringComparison.OrdinalIgnoreCase));
                if (emailClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
                }

                _currentUser = new(new ClaimsIdentity(claims, "supabase"));
            }

            AppStateStatic.SupabaseToken = token;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));
    }
}