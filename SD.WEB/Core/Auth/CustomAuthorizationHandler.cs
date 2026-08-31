namespace SD.WEB.Core.Auth
{
    public class CustomAuthorizationHandler() : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AppStateStatic.ClerkToken.Empty())
            {
                throw new InvalidOperationException("unauthenticated user");
            }

            if (AppStateStatic.ClerkToken.NotEmpty())
            {
                request.Headers.Remove("X-Clerk-Token");
                request.Headers.Add("X-Clerk-Token", $"Bearer {AppStateStatic.ClerkToken}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}