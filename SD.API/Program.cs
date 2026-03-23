using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Net;

var app = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ApiMiddleware>();
    })
    .ConfigureLogging((context, logging) =>
    {
        logging.AddSentry(options =>
        {
            options.Dsn = "https://94ae67eb3fb0bc82327607ddd9d6aebb@o4510938040041472.ingest.us.sentry.io/4510938043711488";
            options.DiagnosticLevel = SentryLevel.Warning;

            options.Release = $"sd-api@{DateTime.Now:yyyy.MM.dd}";
            options.Environment = context.HostingEnvironment.EnvironmentName;

            options.TracePropagationTargets = []; //Disable tracing because it breaks communication with external APIs.
        });
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

            StripeConfiguration.ApiKey = ApiStartup.Configurations.Stripe?.ApiKey;
            StripeConfiguration.AddBetaVersion("managed_payments_preview", "v1");
        }
        catch (Exception ex)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSentry(options =>
                {
                    options.Dsn = "https://94ae67eb3fb0bc82327607ddd9d6aebb@o4510938040041472.ingest.us.sentry.io/4510938043711488";
                    options.DiagnosticLevel = SentryLevel.Warning;

                    options.Release = $"sd-api@{DateTime.Now:yyyy.MM.dd}";
                    //options.Environment = context.HostingEnvironment.EnvironmentName;

                    options.TracePropagationTargets = []; //Disable tracing because it breaks communication with external APIs.
                });
            });

            var logger = loggerFactory.CreateLogger("StartupConfig");

            logger.LogError(ex, "ConfigureAppConfiguration");

            throw;
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

        //general services

        services.AddDistributedMemoryCache();
    }
    catch (Exception ex)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddSentry(options =>
            {
                options.Dsn = "https://94ae67eb3fb0bc82327607ddd9d6aebb@o4510938040041472.ingest.us.sentry.io/4510938043711488";
                options.DiagnosticLevel = SentryLevel.Warning;

                options.Release = $"sd-api@{DateTime.Now:yyyy.MM.dd}";
                //options.Environment = context.HostingEnvironment.EnvironmentName;

                options.TracePropagationTargets = []; //Disable tracing because it breaks communication with external APIs.
            });
        });

        var logger = loggerFactory.CreateLogger("StartupConfig");

        logger.LogError(ex, "ConfigureAppConfiguration");

        throw;
    }
}