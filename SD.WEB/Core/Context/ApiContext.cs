using SD.Common.Models.Auth;
using SD.Common.Models.Franchise;
using SD.Common.Models.List;
using SD.Common.Models.List.Imdb;
using SD.Common.Models.List.Tmdb;
using SD.Common.Models.News;
using SD.Common.Models.Reviews;
using SD.Common.Models.Subscription;
using SD.Common.Models.Trailers;
using System.Text.Json.Serialization;

namespace SD.WEB.Core.Context
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
    [JsonSerializable(typeof(CustomList))]
    internal sealed partial class ApiContext : JsonSerializerContext
    {
    }
}