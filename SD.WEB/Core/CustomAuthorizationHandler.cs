using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SD.WEB.Core
{
    public class CustomAuthorizationHandler(IAccessTokenProvider tokenProvider) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await tokenProvider.RequestAccessToken();

            if (result.TryGetToken(out var token))
            {
                request.Headers.Remove("X-Auth-Token");
                request.Headers.Add("X-Auth-Token", $"Bearer {token.Value}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}