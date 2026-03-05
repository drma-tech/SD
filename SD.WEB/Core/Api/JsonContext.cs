using System.Text.Json.Serialization;

namespace SD.WEB.Core.Api
{
    [JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(Platform?))]
    [JsonSerializable(typeof(AppLanguage?))]
    [JsonSerializable(typeof(ContentLanguage?))]
    [JsonSerializable(typeof(Region?))]
    [JsonSerializable(typeof(bool?))]
    [JsonSerializable(typeof(string))]
    internal partial class JavascriptContext : JsonSerializerContext
    {
    }
}