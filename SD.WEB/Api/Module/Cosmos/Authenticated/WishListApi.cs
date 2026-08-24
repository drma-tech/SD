using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.Cosmos.Authenticated;

public class WishListApi(IHttpClientFactory factory) : ApiCosmos<WishList>(factory, ApiType.Authenticated, "wishlist", [], ApiContext.Default.WishList)
{
    public async Task<WishList?> Get(RenderControlState<WishList?>[] states, CancellationToken cancellationToken)
    {
        return await GetAsync("wishlist/get", setNewVersion: false, states, cancellationToken);
    }

    public async Task<WishList?> Add(MediaType? mediaType, WishList? obj, WishListItem item, AccountProduct? product, CancellationToken cancellationToken)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }

        ArgumentNullException.ThrowIfNull(item);
        SubscriptionHelper.ValidateWishList(product, (obj?.Items(mediaType).Count ?? 0) + 1);

        return await PostAsync($"wishlist/add/{mediaType}", item, ApiContext.Default.WishListItem, states: [], cancellationToken);
    }

    public async Task<WishList?> Remove(MediaType? mediaType, string? id, CancellationToken cancellationToken)
    {
        if (!mediaType.HasValue)
        {
            throw new ArgumentNullException(nameof(mediaType));
        }
        ArgumentNullException.ThrowIfNull(id);

        return await PostAsync($"wishlist/remove/{mediaType}/{id}", null, ApiContext.Default.WishList, states: [], cancellationToken);
    }
}