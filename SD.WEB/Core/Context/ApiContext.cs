using SD.Shared.Models.Auth;
using SD.Shared.Models.Franchise;
using SD.Shared.Models.List;
using SD.Shared.Models.List.Imdb;
using SD.Shared.Models.List.Tmdb;
using SD.Shared.Models.News;
using SD.Shared.Models.Reviews;
using SD.Shared.Models.Subscription;
using SD.Shared.Models.Trailers;
using System.Text.Json.Serialization;

namespace SD.WEB.Core.Api
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(PaymentConfigurations))]
    [JsonSerializable(typeof(AuthPrincipal))]
    [JsonSerializable(typeof(AuthLogin))]
    [JsonSerializable(typeof(AuthSubscription))]
    [JsonSerializable(typeof(MyProviders))]
    [JsonSerializable(typeof(WatchingList))]
    [JsonSerializable(typeof(WishList))]
    [JsonSerializable(typeof(NewsCache))]
    [JsonSerializable(typeof(YoutubeCache))]
    [JsonSerializable(typeof(RatingsCache))]
    [JsonSerializable(typeof(MetaCriticCache))]
    [JsonSerializable(typeof(FranchiseCache))]
    [JsonSerializable(typeof(MostPopularDataCache))]
    [JsonSerializable(typeof(CustomListNew))]
    internal sealed partial class ApiContext : JsonSerializerContext
    {
    }
}