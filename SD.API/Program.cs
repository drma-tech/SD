using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Core.Middleware;
using System.Net;

var app = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile("local.settings.json");
            config.AddUserSecrets<Program>();
        }

        var cfg = new Configurations();
        config.Build().Bind(cfg);
        ApiStartup.Configurations = cfg;

        ApiStartup.Startup(ApiStartup.Configurations.CosmosDB?.ConnectionString);
    })
    .ConfigureLogging(ConfigureLogging)
    .ConfigureServices(ConfigureServices)
    .Build();

await app.RunAsync();

return;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddHttpClient("tmdb", client => { client.Timeout = TimeSpan.FromSeconds(90); })
      .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { MaxConnectionsPerServer = 20 });
    services.AddHttpClient("paddle");
    services.AddHttpClient("rapidapi");
    services.AddHttpClient("rapidapi-gzip")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

    services.AddSingleton<CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();
    services.AddDistributedMemoryCache();
}

static void ConfigureLogging(ILoggingBuilder builder)
{
    builder.AddProvider(new CosmosLoggerProvider(new CosmosLogRepository()));
}