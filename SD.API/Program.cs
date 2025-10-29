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
    services.AddHttpClient("tmdb", client => { client.Timeout = TimeSpan.FromSeconds(180); })
      .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { MaxConnectionsPerServer = 20 });
    services.AddHttpClient("paddle");
    services.AddHttpClient("rapidapi");
    services.AddHttpClient("ipinfo");
    services.AddHttpClient("rapidapi-gzip")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });

    services.AddSingleton<CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
    services.AddDistributedMemoryCache();
}
