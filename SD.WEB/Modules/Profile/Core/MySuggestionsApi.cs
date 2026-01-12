namespace SD.WEB.Modules.Profile.Core;

public class MySuggestionsApi(IHttpClientFactory factory) : ApiCosmos<MySuggestions>(factory, ApiType.Authenticated, "my-suggestions")
{
    public async Task<MySuggestions?> Get(AccountProduct? product, bool isUserAuthenticated)
    {
        if (product is null or AccountProduct.Basic) return new MySuggestions();

        if (isUserAuthenticated) return await GetAsync(Endpoint.Get);

        return new MySuggestions();
    }

    public async Task<MySuggestions?> Sync(MediaType? mediaType, MySuggestions obj)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Sync(mediaType), obj);
    }

    public async Task<MySuggestions?> Add(MySuggestions obj)
    {
        return await PostAsync(Endpoint.Add, obj);
    }

    private struct Endpoint
    {
        public const string Get = "my-suggestions/get";
        public const string Add = "my-suggestions/add";

        public static string Sync(MediaType? type)
        {
            return $"my-suggestions/sync/{type}";
        }
    }
}