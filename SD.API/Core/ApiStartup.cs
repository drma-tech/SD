using Microsoft.Azure.Cosmos;

namespace SD.API.Core
{
    public static class ApiStartup
    {
        private static readonly HttpClient _httpClient = new();

        private static CosmosClient _cosmosClient = default!;

        public static HttpClient HttpClient => _httpClient;
        public static CosmosClient CosmosClient => _cosmosClient;

        public static void Startup(string conn)
        {
            _cosmosClient = new(conn, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
        }
    }
}