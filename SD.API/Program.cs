using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SD.API.Core.Middleware;
using System.Diagnostics;
using System.Net;

var totalSw = Stopwatch.StartNew();

long configureAppConfigurationMs = 0;
long apiStartupMs = 0;
long configureLoggingMs = 0;
long configureServicesMs = 0;
long buildMs = 0;

var app = new HostBuilder()
 .ConfigureFunctionsWorkerDefaults(worker =>
 {
     worker.UseMiddleware<ExceptionHandlingMiddleware>();
 })
 .ConfigureAppConfiguration((hostContext, config) =>
 {
     var sw = Stopwatch.StartNew();

     if (hostContext.HostingEnvironment.IsDevelopment())
     {
         config.AddJsonFile("local.settings.json");
         config.AddUserSecrets<Program>();
     }

     var cfg = new Configurations();
     config.Build().Bind(cfg);
     ApiStartup.Configurations = cfg;

     var startupSw = Stopwatch.StartNew();
     ApiStartup.Startup(ApiStartup.Configurations.CosmosDB?.ConnectionString);
     startupSw.Stop();
     apiStartupMs = startupSw.ElapsedMilliseconds;

     sw.Stop();
     configureAppConfigurationMs = sw.ElapsedMilliseconds;
 })
 .ConfigureLogging((context, builder) =>
 {
     var sw = Stopwatch.StartNew();
     ConfigureLogging(builder);
     sw.Stop();
     configureLoggingMs = sw.ElapsedMilliseconds;
 })
 .ConfigureServices((context, services) =>
 {
     var sw = Stopwatch.StartNew();
     ConfigureServices(context, services);
     sw.Stop();
     configureServicesMs = sw.ElapsedMilliseconds;
 });

var buildSw = Stopwatch.StartNew();
var host = app.Build();
buildSw.Stop();
buildMs = buildSw.ElapsedMilliseconds;

// Use the host's logger to emit startup timing so logs are available on the server (no console required)
var loggerFactory = host.Services.GetService<ILoggerFactory>();
var logger = loggerFactory?.CreateLogger("StartupTiming");
logger?.LogWarning("Startup timings (ms) - ConfigureAppConfiguration: {ConfigureAppConfigurationMs}, ApiStartup.Startup: {ApiStartupMs}, ConfigureLogging: {ConfigureLoggingMs}, ConfigureServices: {ConfigureServicesMs}, HostBuild: {BuildMs}", configureAppConfigurationMs, apiStartupMs, configureLoggingMs, configureServicesMs, buildMs);

var runSw = Stopwatch.StartNew();
await host.RunAsync();
runSw.Stop();
var runMs = runSw.ElapsedMilliseconds;

totalSw.Stop();
var totalMs = totalSw.ElapsedMilliseconds;
logger?.LogWarning("Host RunAsync returned after {RunMs} ms. Total program execution time (to after RunAsync): {TotalMs} ms", runMs, totalMs);

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