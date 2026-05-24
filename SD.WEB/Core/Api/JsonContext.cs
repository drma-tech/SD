using SD.Shared.Models.Auth;
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
    [JsonSerializable(typeof(bool?))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(Platform?))]
    [JsonSerializable(typeof(AppLanguage?))]
    [JsonSerializable(typeof(ContentLanguage?))]
    [JsonSerializable(typeof(Region?))]
    [JsonSerializable(typeof(AuthProvider))]
    [JsonSerializable(typeof(AllProviders))]
    [JsonSerializable(typeof(TMDB_AllProviders))]
    [JsonSerializable(typeof(HashSet<DateTime>))]
    internal partial class JavascriptContext : JsonSerializerContext
    {
    }

    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(PaymentConfigurations))]
    [JsonSerializable(typeof(AuthPrincipal))]
    [JsonSerializable(typeof(AuthLogin))]
    [JsonSerializable(typeof(AuthSubscription))]
    [JsonSerializable(typeof(MyProviders))]
    [JsonSerializable(typeof(MySuggestions))]
    [JsonSerializable(typeof(WatchingList))]
    [JsonSerializable(typeof(WatchedList))]
    [JsonSerializable(typeof(WishList))]
    [JsonSerializable(typeof(CacheDocument<NewsModel>))]
    [JsonSerializable(typeof(CacheDocument<TrailerModel>))]
    [JsonSerializable(typeof(CacheDocument<Ratings>))]
    [JsonSerializable(typeof(CacheDocument<ReviewModel>))]
    [JsonSerializable(typeof(CacheDocument<MostPopularData>))]
    [JsonSerializable(typeof(CustomListNew))]
    internal partial class ApiContext : JsonSerializerContext
    {
    }
}