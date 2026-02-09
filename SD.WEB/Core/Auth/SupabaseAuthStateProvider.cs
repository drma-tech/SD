using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace SD.WEB.Core.Auth
{
    public class SupabaseAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public void NotifyAuthenticationStateChanged(string? token)
        {
            AppStateStatic.FirebaseToken = null;
            AppStateStatic.SupabaseToken = null;

            if (token == null)
            {
                _currentUser = new(new ClaimsIdentity());
            }
            else
            {
                var claims = JwtParser.ParseClaimsFromJwt(token).ToList();

                var userMetadataClaim = claims.SingleOrDefault(c => c.Type == "user_metadata");
                if (userMetadataClaim != null)
                {
                    using var doc = JsonDocument.Parse(userMetadataClaim.Value);
                    if (doc.RootElement.TryGetProperty("iss", out var iss))
                    {
                        if (iss.GetString()?.Contains("apple") ?? false)
                            claims.Add(new Claim("idp", "apple"));
                        if (iss.GetString()?.Contains("google") ?? false)
                            claims.Add(new Claim("idp", "google"));
                        if (iss.GetString()?.Contains("microsoft") ?? false)
                            claims.Add(new Claim("idp", "microsoft"));
                        else
                            claims.Add(new Claim("idp", iss.GetString() ?? "email"));
                    }
                    else
                    {
                        claims.Add(new Claim("idp", "email"));
                    }
                }

                var userId = claims.Single(c => c.Type == "sub").Value;
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

                var emailClaim = claims.SingleOrDefault(c => c.Type == "email");
                if (emailClaim != null)
                {
                    claims.Add(new Claim(ClaimTypes.Email, emailClaim.Value));
                }

                _currentUser = new(new ClaimsIdentity(claims, "supabase"));
            }

            AppStateStatic.SupabaseToken = token;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));
    }
}