using AzureStaticWebApps.Blazor.Authentication;
using Blazored.SessionStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB;
using SD.WEB.Modules.List.Core.TMDB;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ConfigureComponents(builder.Services);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

builder.Services.AddLogging(logging =>
{
    logging.AddProvider(new CosmosLoggerProvider());
});

var hostBuilder = builder.Build();

ConfigureCulture(hostBuilder);

await hostBuilder.RunAsync();

static void ConfigureComponents(IServiceCollection collection)
{
    collection
        .AddBlazorise(options => options.Immediate = true)
        .AddBootstrapProviders()
        .AddFontAwesomeIcons();

    collection.AddBlazoredSessionStorage(config => config.JsonSerializerOptions.WriteIndented = true);

    collection.AddPWAUpdater();
    collection.AddMediaQueryService();

    collection.AddScoped<Settings>();
}

static void ConfigureServices(IServiceCollection collection, string baseAddress)
{
    collection
        .AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) })
        .AddStaticWebAppsAuthentication();

    collection.AddScoped<ListService>();
    collection.AddScoped<MediaDetailService>();
}

static void ConfigureCulture(WebAssemblyHost host)
{
    CultureInfo culture;
    var session = host.Services.GetRequiredService<ISyncSessionStorageService>();
    var sett = session.GetItem<Settings>("Settings");

    if (sett != null)
    {
        culture = new CultureInfo(sett.Language.GetName(false) ?? "en-US");
    }
    else
    {
        culture = CultureInfo.CurrentCulture;

        //save the new settings
        sett = new Settings(session);
        session.SetItem("Settings", sett);
    }

    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}