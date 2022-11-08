using AzureStaticWebApps.Blazor.Authentication;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.ConfigureComponents();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.ConfigureServices(builder.HostEnvironment.BaseAddress);

builder.Services.AddLogging(logging =>
{
    logging.AddProvider(new CosmosLoggerProvider());
});

var hostBuilder = builder.Build();

hostBuilder.ConfigureCulture();

await hostBuilder.RunAsync();