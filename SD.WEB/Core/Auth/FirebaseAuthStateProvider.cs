using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace SD.WEB.Core.Auth
{
    public class FirebaseAuthStateProvider : AuthenticationStateProvider
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

                var firebase = claims.Single(c => c.Type == "firebase").Value;
                using var doc = JsonDocument.Parse(firebase);
                if (doc.RootElement.TryGetProperty("sign_in_provider", out var providerProp))
                {
                    claims.Add(new Claim("idp", providerProp.GetString()!));
                }

                var user_id = claims.Single(c => c.Type == "user_id").Value;
                claims.Add(new Claim(ClaimTypes.NameIdentifier, user_id));

                _currentUser = new(new ClaimsIdentity(claims, "firebase"));
            }

            AppStateStatic.FirebaseToken = token;

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));
    }
}