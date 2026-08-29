namespace SD.WEB.Core.Auth
{
    public class CustomAuthorizationHandler() : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AppStateStatic.SupabaseToken.Empty() && AppStateStatic.ClerkToken.Empty())
            {
                throw new InvalidOperationException("unauthenticated user");
            }

            if (AppStateStatic.SupabaseToken.NotEmpty())
            {
                request.Headers.Remove("X-Supabase-Token");
                request.Headers.Add("X-Supabase-Token", $"Bearer {AppStateStatic.SupabaseToken}");
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