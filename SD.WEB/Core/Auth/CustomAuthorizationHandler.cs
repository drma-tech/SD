namespace SD.WEB.Core.Auth
{
    public class CustomAuthorizationHandler() : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AppStateStatic.SupabaseToken.Empty())
            {
                throw new InvalidOperationException("unauthenticated user");
            }

            request.Headers.Remove("X-Supabase-Token");
            request.Headers.Add("X-Supabase-Token", $"Bearer {AppStateStatic.SupabaseToken}");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
