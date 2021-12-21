using Microsoft.AspNetCore.Http;
using SD.Shared.Core;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SD.API.Core
{
    public static class FunctionHelper
    {
        public static async Task<I> GetParameterObject<I>(this HttpRequest req, CancellationToken cancellationToken) where I : CosmosBase
        {
            var obj = await JsonSerializer.DeserializeAsync<I>(req.Body, new JsonSerializerOptions(), cancellationToken);

            obj.SetIds(req.GetUserId());

            return obj;
        }
    }
}