using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

var app = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ApiMiddleware>();
    })
    .ConfigureAppConfiguration((hostContext, config) => //736
    {
        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile("local.settings.json");
            config.AddUserSecrets<Program>();
        }

        var cfg = new Configurations();
        config.Build().Bind(cfg);
        ApiStartup.Configurations = cfg;

        var firebaseTemplate = File.ReadAllText("firebase.json");
        var firebaseJson = firebaseTemplate
            .Replace("@ID", ApiStartup.Configurations.Firebase?.ClientId)
            .Replace("@KEY", ApiStartup.Configurations.Firebase?.PrivateKey)
            .Replace("@KEY-ID", ApiStartup.Configurations.Firebase?.PrivateKeyId)
            .Replace("@EMAIL", ApiStartup.Configurations.Firebase?.ClientEmail)
            .Replace("@CERT", ApiStartup.Configurations.Firebase?.CertUrl);

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseJson)
        });
    })
    .ConfigureServices(ConfigureServices) //125
    .Build();

await app.RunAsync(); //1442

static void ConfigureServices(IServiceCollection services)
{
    //http clients

    services.AddHttpClient("tmdb", client => { client.Timeout = TimeSpan.FromSeconds(60); }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { MaxConnectionsPerServer = 20 });
    services.AddHttpClient("paddle");
    services.AddHttpClient("apple");
    services.AddHttpClient("auth", client => { client.Timeout = TimeSpan.FromSeconds(60); });
    services.AddHttpClient("rapidapi");
    services.AddHttpClient("ipinfo");
    services.AddHttpClient("rapidapi-gzip").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

    //repositories

    services.AddSingleton(provider =>
    {
        return new CosmosClient(ApiStartup.Configurations.CosmosDB?.ConnectionString, new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Direct,
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
    });

    services.AddSingleton<CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
    services.AddSingleton<CosmosLogRepository>();

    services.AddSingleton<ILoggerProvider>(provider =>
    {
        var repo = provider.GetRequiredService<CosmosLogRepository>();
        return new CosmosLoggerProvider(repo);
    });

    //general services

    services.AddDistributedMemoryCache();

    services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = "https://securetoken.google.com/streaming-discovery-4c483";
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "https://securetoken.google.com/streaming-discovery-4c483",
                ValidateAudience = true,
                ValidAudience = "streaming-discovery-4c483",
                ValidateLifetime = true
            };
        });
}