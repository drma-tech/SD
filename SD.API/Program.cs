using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.StartupLogging;
using System.Diagnostics;
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

        var swBind = Stopwatch.StartNew();

        var cfg = new Configurations();
        config.Build().Bind(cfg);
        ApiStartup.Configurations = cfg;

        swBind.Stop();
        StartupLogBuffer.Enqueue("Configurations", swBind.Elapsed);

        var swFirebase = Stopwatch.StartNew();

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

        swFirebase.Stop();
        StartupLogBuffer.Enqueue("FirebaseApp", swFirebase.Elapsed);
    })
    .ConfigureServices(ConfigureServices) //125
    .Build();

// measure host start (this is the Azure Functions host initialization you mentioned)
var swHostStart = Stopwatch.StartNew();
try
{
    await app.StartAsync();
    swHostStart.Stop();

    // prefer ILoggerFactory so we can create a logger even if ILogger<Program> isn't registered
    var loggerFactory = app.Services.GetService<ILoggerFactory>();
    if (loggerFactory != null)
    {
        var logger = loggerFactory.CreateLogger("HostStartup");
        logger.LogWarning("Host started in {Elapsed}", swHostStart.Elapsed);
    }
    else
    {
        // fall back to buffered startup log
        StartupLogBuffer.Enqueue($"Host started", swHostStart.Elapsed);
    }
}
catch (Exception ex)
{
    swHostStart.Stop();
    throw;
}

await app.WaitForShutdownAsync();

static void ConfigureServices(IServiceCollection services)
{
    // ensure logging services are available so ILoggerFactory/ILogger<T> will be resolved
    services.AddLogging();

    //http clients
    var swHttp = Stopwatch.StartNew();

    services.AddHttpClient("tmdb", client => { client.Timeout = TimeSpan.FromSeconds(60); }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { MaxConnectionsPerServer = 20 });
    services.AddHttpClient("paddle");
    services.AddHttpClient("apple");
    services.AddHttpClient("auth", client => { client.Timeout = TimeSpan.FromSeconds(60); });
    services.AddHttpClient("rapidapi");
    services.AddHttpClient("ipinfo");
    services.AddHttpClient("rapidapi-gzip").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

    swHttp.Stop();
    StartupLogBuffer.Enqueue($"http clients", swHttp.Elapsed);

    //repositories
    var swRepo = Stopwatch.StartNew();

    services.AddSingleton(provider =>
    {
        var swCosmos = Stopwatch.StartNew();

        var client = new CosmosClient(ApiStartup.Configurations.CosmosDB?.ConnectionString, new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });

        swCosmos.Stop();
        StartupLogBuffer.Enqueue($"new CosmosClient", swCosmos.Elapsed);

        return client;
    });

    services.AddSingleton<CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
    services.AddSingleton<CosmosLogRepository>();

    services.AddSingleton<ILoggerProvider>(provider =>
    {
        var repo = provider.GetRequiredService<CosmosLogRepository>();
        return new CosmosLoggerProvider(repo);
    });

    swRepo.Stop();
    StartupLogBuffer.Enqueue($"repositories", swRepo.Elapsed);

    //general services
    var swGeneral = Stopwatch.StartNew();

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

    swGeneral.Stop();
    StartupLogBuffer.Enqueue($"general services", swGeneral.Elapsed);

    services.AddHostedService<StartupLogFlusher>();
}