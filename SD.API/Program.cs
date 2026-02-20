using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Core.Auth;
using Stripe;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

var app = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ApiMiddleware>();
    })
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        try
        {
            if (hostContext.HostingEnvironment.IsDevelopment())
            {
                config.AddJsonFile("local.settings.json");
                config.AddUserSecrets<Program>();
            }

            var cfg = new Configurations();
            config.Build().Bind(cfg);
            ApiStartup.Configurations = cfg;

            var key = ApiStartup.Configurations.Firebase?.PrivateKey ?? throw new UnhandledException("PrivateKey null");

            var firebaseConfig = new FirebaseConfig
            {
                project_id = "streaming-discovery-4c483",
                private_key_id = ApiStartup.Configurations.Firebase?.PrivateKeyId ?? throw new UnhandledException("PrivateKeyId null"),
                private_key = Regex.Unescape(key),
                client_email = ApiStartup.Configurations.Firebase?.ClientEmail ?? throw new UnhandledException("ClientEmail null"),
                client_id = ApiStartup.Configurations.Firebase?.ClientId ?? throw new UnhandledException("ClientId null"),
                client_x509_cert_url = ApiStartup.Configurations.Firebase?.CertUrl ?? throw new UnhandledException("Firebase null")
            };

            var firebaseJson = JsonSerializer.Serialize(firebaseConfig);

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromJson(firebaseJson)
                });
            }

            StripeConfiguration.ApiKey = ApiStartup.Configurations.Stripe?.ApiKey;
            StripeConfiguration.AddBetaVersion("managed_payments_preview", "v1");
        }
        catch (Exception ex)
        {
            var tempClient = new CosmosClient(ApiStartup.Configurations.CosmosDB?.ConnectionString, new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
            var tempRepo = new CosmosLogRepository(tempClient);
            var provider = new CosmosLoggerProvider(tempRepo);
            var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(provider));
            var logger = loggerFactory.CreateLogger("ConfigureAppConfiguration");

            logger.LogError(ex, "ConfigureAppConfiguration");
        }
    })
    .ConfigureServices(ConfigureServices)
    .Build();

await app.RunAsync();

static void ConfigureServices(IServiceCollection services)
{
    try
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
                ConnectionMode = ConnectionMode.Gateway,
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
    catch (Exception ex)
    {
        var tempClient = new CosmosClient(ApiStartup.Configurations.CosmosDB?.ConnectionString, new CosmosClientOptions()
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            }
        });
        var tempRepo = new CosmosLogRepository(tempClient);
        var provider = new CosmosLoggerProvider(tempRepo);
        var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(provider));
        var logger = loggerFactory.CreateLogger("ConfigureServices");

        logger.LogError(ex, "ConfigureServices");
    }
}
