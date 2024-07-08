using Microsoft.Azure.Cosmos;

namespace SD.API.Core
{
    public static class ApiStartup
    {
        private static readonly HttpClient _httpClient = new();
        private static readonly HttpClient _httpClientPaddle = new();

        private static CosmosClient _cosmosClient = default!;

        public static HttpClient HttpClient => _httpClient;
        public static HttpClient HttpClientPaddle => _httpClientPaddle;
        public static CosmosClient CosmosClient => _cosmosClient;

        public static void Startup(string conn)
        {
            _cosmosClient = new(conn, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },

                //https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-connection-modes
                //ConnectionMode = ConnectionMode.Gateway // ConnectionMode.Direct is the default
            });
        }
    }
}