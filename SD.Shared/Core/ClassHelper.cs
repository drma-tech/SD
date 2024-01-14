using SD.Shared.Helper;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SD.Shared.Core
{
    public static class ClassHelper
    {
        public static T? GetClonedInstance<T>(this T? instance) where T : class
        {
            if (instance == null) return null;
            var json = JsonSerializer.Serialize(instance, options: new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            if (string.IsNullOrEmpty(json)) throw new UnhandledException("invalid instance for GetClonedInstance");
            var result = JsonSerializer.Deserialize<T>(json);
            return result ?? throw new UnhandledException("invalid instance for GetClonedInstance");
        }
    }
}