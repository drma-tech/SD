using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Repository.Core;
using System.Reflection;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("appsettings.json", true, true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), true);
    })
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices(ConfigureServices)
    .ConfigureLogging(ConfigureLogging)
    .Build();

host.Run();

static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    //builder.Services.AddHttpClient();

    services.AddSingleton<IRepository>((s) =>
    {
        return new CosmosRepository(context.Configuration);
    });

    services.AddSingleton<CosmosCacheRepository>();
}

static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
{
    builder.AddProvider(new CosmosLoggerProvider(new CosmosLogRepository(context.Configuration)));
}