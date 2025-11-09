using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace SD.WEB.Core.Auth
{
    public class FirebaseAuthStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private string? _token;

        public void SetToken(string? token)
        {
            _token = token;
            if (token == null)
            {
                _currentUser = new(new ClaimsIdentity());
            }
            else
            {
                var claims = JwtParser.ParseClaimsFromJwt(token);
                _currentUser = new(new ClaimsIdentity(claims, "firebase"));
            }
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => Task.FromResult(new AuthenticationState(_currentUser));

        public string? GetToken() => _token;
    }
}