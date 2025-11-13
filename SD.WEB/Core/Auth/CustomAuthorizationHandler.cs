namespace SD.WEB.Core.Auth
{
    public class CustomAuthorizationHandler() : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AppStateStatic.Token.Empty())
            {
                throw new InvalidOperationException("empty token");
            }

            request.Headers.Remove("X-Auth-Token");
            request.Headers.Add("X-Auth-Token", $"Bearer {AppStateStatic.Token}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}