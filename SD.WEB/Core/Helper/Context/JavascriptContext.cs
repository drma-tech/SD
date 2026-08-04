using SD.Shared.Models.List.Tmdb;
using System.Text.Json.Serialization;

namespace SD.WEB.Core.Api
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(bool?))]
    [JsonSerializable(typeof(string))]
    [JsonSerializable(typeof(Platform?))]
    [JsonSerializable(typeof(AppLanguage?))]
    [JsonSerializable(typeof(ContentLanguage?))]
    [JsonSerializable(typeof(Country?))]
    [JsonSerializable(typeof(AuthProvider))]
    [JsonSerializable(typeof(AllProviders))]
    [JsonSerializable(typeof(TMDB_AllProviders))]
    [JsonSerializable(typeof(HashSet<DateTime>))]
    internal sealed partial class JavascriptContext : JsonSerializerContext
    {
    }
}
