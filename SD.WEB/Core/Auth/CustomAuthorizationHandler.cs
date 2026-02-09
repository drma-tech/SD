namespace SD.WEB.Core.Auth
{
    public class CustomAuthorizationHandler() : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (AppStateStatic.FirebaseToken.Empty() && AppStateStatic.SupabaseToken.Empty())
            {
                throw new InvalidOperationException("empty token");
            }

            request.Headers.Remove("X-Firebase-Token");
            request.Headers.Remove("X-Supabase-Token");

            if (AppStateStatic.FirebaseToken.NotEmpty())
            {
                request.Headers.Add("X-Firebase-Token", $"Bearer {AppStateStatic.FirebaseToken}");
            }
            else
            {
                request.Headers.Add("X-Supabase-Token", $"Bearer {AppStateStatic.SupabaseToken}");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}