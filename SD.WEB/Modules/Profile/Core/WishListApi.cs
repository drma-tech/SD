namespace SD.WEB.Modules.Profile.Core;

public class WishListApi(IHttpClientFactory factory) : ApiCosmos<WishList>(factory, ApiType.Authenticated, "wishlist", [], ApiContext.Default.WishList)
{
    public async Task<WishList?> Get(RenderControlState<WishList>? actions, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.Get, setNewVersion: false, actions, cancellationToken);
    }

    public async Task<WishList?> Add(MediaType? mediaType, WishList? obj, WishListItem item, AccountProduct? product, CancellationToken cancellationToken)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWishList(product, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync(Endpoint.Add(mediaType), item, ApiContext.Default.WishListItem, cancellationToken);
    }

    public async Task<WishList?> Remove(MediaType? mediaType, string? id, CancellationToken cancellationToken)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }
        ArgumentNullException.ThrowIfNull(id);

        return await PostAsync(Endpoint.Remove(mediaType, id), null, ApiContext.Default.WishList, cancellationToken);
    }

    private struct Endpoint
    {
        public const string Get = "wishlist/get";

        public static string Add(MediaType? type)
        {
            return $"wishlist/add/{type}";
        }

        public static string Remove(MediaType? type, string id)
        {
            return $"wishlist/remove/{type}/{id}";
        }
    }
}