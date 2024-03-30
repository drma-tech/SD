using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Core.Middleware;
using SD.API.Repository.Core;

var host = new HostBuilder()
     .ConfigureFunctionsWorkerDefaults(worker =>
     {
         worker.UseMiddleware<ExceptionHandlingMiddleware>();
         //https://github.com/Azure/azure-functions-openapi-extension/blob/main/docs/enable-open-api-endpoints-out-of-proc.md
         //worker.UseNewtonsoftJson();
     })
    .ConfigureAppConfiguration((hostContext, config) =>
    {
        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            config.AddJsonFile("local.settings.json");
            config.AddUserSecrets<Program>();
        }
    })
    .ConfigureServices(ConfigureServices)
    .ConfigureLogging(ConfigureLogging)
    .Build();

host.Run();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    services.AddSingleton<IRepository, CosmosRepository>();
    services.AddSingleton<CosmosCacheRepository>();
    services.AddSingleton<CosmosEmailRepository>();
    services.AddApplicationInsightsTelemetryWorkerService();
    services.ConfigureFunctionsApplicationInsights();
}

static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
{
    builder.AddProvider(new CosmosLoggerProvider(new CosmosLogRepository(context.Configuration)));
}