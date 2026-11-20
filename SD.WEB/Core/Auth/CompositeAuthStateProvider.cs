using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace SD.WEB.Core.Auth
{
    public class CompositeAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public void OnClerkAuthChanged(string? token)
        {
            GenerateClerkClaimsIdentity(token);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void GenerateClerkClaimsIdentity(string? token)
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

                var metadata = claims.SingleOrDefault(c => string.Equals(c.Type, "metadata", StringComparison.OrdinalIgnoreCase))?.Value;

                if (metadata is not null)
                {
                    using var document = JsonDocument.Parse(metadata);

                    if (document.RootElement.TryGetProperty("role", out var role))
                    {
                        var roleValue = role.GetString();

                        if (!string.IsNullOrWhiteSpace(roleValue))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roleValue));
                        }
                    }
                }

                _currentUser = new(new ClaimsIdentity(claims, "clerk"));
            }

            AppStateStatic.ClerkToken = token;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));
    }
}