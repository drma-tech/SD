using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace SD.WEB.Core
{
    public class CustomAuthorizationHandler(IAccessTokenProvider tokenProvider, NavigationManager navigation, IConfiguration configuration) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = await tokenProvider.RequestAccessToken();

            if (!result.TryGetToken(out var token))
            {
                result = await tokenProvider.RequestAccessToken(new AccessTokenRequestOptions
                {
                    Scopes = ["openid", "email", configuration["DownstreamApi:Scopes"] ?? throw new UnhandledException("Scopes null")],
                    ReturnUrl = "/"
                });

                if (!result.TryGetToken(out token))
                {
                    navigation.NavigateToLogin("/authentication/login");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                    {
                        ReasonPhrase = "Invalid or expired token"
                    };
                }
            }

            request.Headers.Remove("X-Auth-Token");
            request.Headers.Add("X-Auth-Token", $"Bearer {token!.Value}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}