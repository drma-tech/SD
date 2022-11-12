namespace SD.WEB.Modules.Profile.Core
{
    public static class MyProvidersApi
    {
        private struct Endpoint
        {
            public const string Get = "MyProviders/Get";
            public const string Post = "MyProviders/Post";
        }

        public static async Task<MyProviders?> MyProviders_Get(this HttpClient http)
        {
            if (ComponenteUtils.IsAuthenticated)
            {
                return await http.Get<MyProviders>(Endpoint.Get, false);
            }
            else
            {
                return new();
            }
        }

        public static async Task<HttpResponseMessage> MyProviders_Post(this HttpClient http, MyProviders? obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return await http.Post(Endpoint.Post, false, obj, Endpoint.Get);
        }
    }
}