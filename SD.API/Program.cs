using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;

var host = new HostBuilder()
     .ConfigureFunctionsWorkerDefaults(worker =>
     {
         worker.UseMiddleware<ExceptionHandlingMiddleware>();
         //https://github.com/Azure/azure-functions-openapi-extension/blob/main/docs/enable-open-api-endpoints-out-of-proc.md
         //worker.UseNewtonsoftJson();
     })
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", false, true);
        config.AddUserSecrets<Program>();
    })
    .ConfigureServices(ConfigureServices)
    .ConfigureLogging(ConfigureLogging)
    .Build();

host.Run();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton<IRepository>((s) =>
    {
        return new CosmosRepository(context.Configuration);
    });

    services.AddSingleton<CosmosCacheRepository>();
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();
    services.AddMemoryCache();
}

static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
{
    builder.AddProvider(new CosmosLoggerProvider(new CosmosLogRepository(context.Configuration)));
}