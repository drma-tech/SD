using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        ApiStartup.Startup(ApiStartup.Configurations.CosmosDB?.ConnectionString); //650
    })
    .ConfigureLogging(ConfigureLogging) //12
    .ConfigureServices(ConfigureServices) //125
    .Build();

await app.RunAsync(); //1442

return;

static void ConfigureLogging(ILoggingBuilder builder)
{
    builder.AddProvider(new CosmosLoggerProvider(new CosmosLogRepository()));
}

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddHttpClient("tmdb", client => { client.Timeout = TimeSpan.FromSeconds(10); }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { MaxConnectionsPerServer = 20 });
    services.AddHttpClient("paddle");
    services.AddHttpClient("apple");
    services.AddHttpClient("auth", client => { client.Timeout = TimeSpan.FromSeconds(10); });
    services.AddHttpClient("rapidapi");
    services.AddHttpClient("ipinfo");
    services.AddHttpClient("rapidapi-gzip").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

    services.AddSingleton<CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
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