using AzureStaticWebApps.Blazor.Authentication;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB;
using SD.WEB.Core;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.ConfigureComponents();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.ConfigureServices();

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddStaticWebAppsAuthentication();

builder.Services.AddLogging(logging =>
{
    logging.AddProvider(new CosmosLoggerProvider());
});

await builder.Build().RunAsync();