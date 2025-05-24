using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Core.Middleware;

var app = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker => { worker.UseMiddleware<ExceptionHandlingMiddleware>(); })
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
    .ConfigureServices(ConfigureServices)
    .ConfigureLogging(ConfigureLogging)
    .Build();

await app.RunAsync();

return;

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton<CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();
}

static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
{
    builder.AddProvider(new CosmosLoggerProvider(new CosmosLogRepository()));
}