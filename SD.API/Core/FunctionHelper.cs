using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Core
{
    public static class FunctionHelper
    {
        public static async Task<I> GetParameterObject<I>(this HttpRequest req, CancellationToken cancellationToken) where I : DocumentBase
        {
            var obj = await JsonSerializer.DeserializeAsync<I>(req.Body, new JsonSerializerOptions(), cancellationToken);

            var userId = req.GetUserId();
            if (obj != null && !string.IsNullOrEmpty(userId)) obj.SetIds(userId);

            return obj ?? default!;
        }

        public static async Task<I> GetParameterObjectPublic<I>(this HttpRequest req, CancellationToken cancellationToken)
        {
            var obj = await JsonSerializer.DeserializeAsync<I>(req.Body, new JsonSerializerOptions(), cancellationToken);

            return obj ?? default!;
        }
    }
}