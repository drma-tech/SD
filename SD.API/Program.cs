using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
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

            options.MinimumEventLevel = LogLevel.Warning;
            options.MinimumBreadcrumbLevel = LogLevel.Warning;
            options.DiagnosticLevel = SentryLevel.Warning;

            options.Release = $"sd-api@{DateTime.UtcNow:yyyy.MM.dd}";
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

                    options.MinimumEventLevel = LogLevel.Warning;
                    options.MinimumBreadcrumbLevel = LogLevel.Warning;
                    options.DiagnosticLevel = SentryLevel.Warning;

                    options.Release = $"sd-api@{DateTime.UtcNow:yyyy.MM.dd}";
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

        services.AddHttpClient("apple");
        services.AddHttpClient("auth", client => { client.Timeout = TimeSpan.FromSeconds(30); });

        services.AddHttpClient("ipinfo")
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

        services.AddHttpClient("tmdb", client => { client.Timeout = TimeSpan.FromSeconds(30); })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { MaxConnectionsPerServer = 20, AutomaticDecompression = DecompressionMethods.GZip })
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

        services.AddHttpClient("rapidapi")
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());
        services.AddHttpClient("rapidapi-gzip")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip })
            .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

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
        services.AddHostedService<CosmosWarmupService>();

        services.AddSingleton<CosmosRepository>();
        services.AddSingleton<CosmosCacheRepository>();
        services.AddSingleton<CosmosJobRepository>();

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

                options.MinimumEventLevel = LogLevel.Warning;
                options.MinimumBreadcrumbLevel = LogLevel.Warning;
                options.DiagnosticLevel = SentryLevel.Warning;

                options.Release = $"sd-api@{DateTime.UtcNow:yyyy.MM.dd}";
                //options.Environment = context.HostingEnvironment.EnvironmentName;

                options.TracePropagationTargets = []; //Disable tracing because it breaks communication with external APIs.
            });
        });

        var logger = loggerFactory.CreateLogger("StartupConfig");

        logger.LogError(ex, "ConfigureAppConfiguration");

        throw;
    }
}

//https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 408,5xx
        .WaitAndRetryAsync([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)]);
}