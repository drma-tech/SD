namespace SD.WEB.Modules.Profile.Core;

public class MySuggestionsApi(IHttpClientFactory factory) : ApiCosmos<MySuggestions>(factory, ApiType.Authenticated, "my-suggestions", ApiContext.Default.MySuggestions)
{
    public async Task<MySuggestions?> Get(AccountProduct? product, ComponentActions<MySuggestions?>? actions, CancellationToken cancellationToken)
    {
        if (product is null or AccountProduct.Basic) return new MySuggestions();

        return await GetAsync(Endpoint.Get, true, actions, cancellationToken);
    }

    public async Task<MySuggestions?> Sync(MediaType? mediaType, MySuggestions obj, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(obj);

        return await PostAsync(Endpoint.Sync(mediaType), obj, cancellationToken);
    }

    public async Task<MySuggestions?> Add(MySuggestions obj, CancellationToken cancellationToken)
    {
        return await PostAsync(Endpoint.Add, obj, cancellationToken);
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