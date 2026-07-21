namespace SD.WEB.Core.Api
{
    public sealed class AppVersionHandler() : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove("X-App-Version");

            request.Headers.Add("X-App-Version", AppStateStatic.Version);

            if (request.RequestUri?.Host.StartsWith("www.", StringComparison.OrdinalIgnoreCase) == true)
            {
                throw new NotificationException("It looks like you are using an older version of the app. To continue using it normally, please update the app to the latest version from your app store (Microsoft Store, Google Play, Apple App Store, etc.).");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}