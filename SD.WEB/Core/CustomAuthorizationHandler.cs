using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SD.WEB.Core
{
    public class CustomAuthorizationHandler(CachedTokenProvider cachedTokenProvider, NavigationManager navigation) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await cachedTokenProvider.GetTokenAsync();

            if (token == null)
            {
                navigation.NavigateToLogin("/authentication/login");
                return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                {
                    ReasonPhrase = "Invalid or expired token"
                };
            }

            request.Headers.Remove("X-Auth-Token");
            request.Headers.Add("X-Auth-Token", $"Bearer {token.Value}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}