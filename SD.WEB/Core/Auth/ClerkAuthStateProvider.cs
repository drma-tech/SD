using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SD.WEB.Core.Auth
{
    public class ClerkAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public void OnAuthChanged(string? token)
        {
            GenerateClaimsIdentity(token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void GenerateClaimsIdentity(string? token)
        {
            AppStateStatic.ClerkToken = null;

            if (token == null)
            {
                _currentUser = new(new ClaimsIdentity());
            }
            else
            {
                var claims = JwtParser.ParseClaimsFromJwt(token).ToList();

                var userId = claims.Single(c => string.Equals(c.Type, "sub", StringComparison.OrdinalIgnoreCase)).Value;
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));

                var email = claims.SingleOrDefault(c => string.Equals(c.Type, "email", StringComparison.OrdinalIgnoreCase))?.Value;
                claims.Add(new Claim(ClaimTypes.Email, email ?? ""));

                var name = claims.SingleOrDefault(c => string.Equals(c.Type, "name", StringComparison.OrdinalIgnoreCase))?.Value;
                claims.Add(new Claim("name", name ?? ""));

                var avatar = claims.SingleOrDefault(c => string.Equals(c.Type, "avatar", StringComparison.OrdinalIgnoreCase))?.Value;
                claims.Add(new Claim("avatar", avatar ?? ""));

                _currentUser = new(new ClaimsIdentity(claims, "clerk"));
            }

            AppStateStatic.ClerkToken = token;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));
    }
}