using Microsoft.Azure.Cosmos;
using System.Net;

namespace SD.API.Core;

public static class ApiStartup
{
    public static HttpClient HttpClient { get; } = new(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

    public static HttpClient HttpClientPaddle { get; } = new();
    public static CosmosClient CosmosClient { get; private set; } = null!;
    public static Settings Settings { get; set; } = new();

    public static void Startup(string conn)
    {
        CosmosClient = new CosmosClient(conn, new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }

            //https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/sdk-connection-modes
            //ConnectionMode = ConnectionMode.Gateway // ConnectionMode.Direct is the default
        });
    }
}