using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Profile.Core;

public class WishListApi(IHttpClientFactory factory) : ApiCosmos<WishList>(factory, ApiType.Authenticated, "wishlist")
{
    public async Task<WishList?> Get(bool isUserAuthenticated)
    {
        if (isUserAuthenticated) return await GetAsync(Endpoint.Get);

        return new WishList();
    }

    public async Task<WishList?> Add(MediaType? mediaType, WishList? obj, WishListItem item, AuthSubscription? subs)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWishList(subs?.ActiveProduct, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync(Endpoint.Add(mediaType), item);
    }

    public async Task<WishList?> Remove(MediaType? mediaType, string? id)
    {
        ArgumentNullException.ThrowIfNull(mediaType);
        ArgumentNullException.ThrowIfNull(id);

        return await PostAsync(Endpoint.Remove(mediaType, id), null);
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