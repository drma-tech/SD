using AzureStaticWebApps.Blazor.Authentication;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.List.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Provider.Core;
using SD.WEB.Modules.Support.Core;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

await builder.Build().RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress)
{
    collection
        .AddBlazorise(options => options.Immediate = true)
        .AddBootstrapProviders()
        .AddFontAwesomeIcons();

    collection.AddPWAUpdater();
    collection.AddMediaQueryService();
    collection.AddMemoryCache();

    collection
        .AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) })
        .AddStaticWebAppsAuthentication();

    collection.AddScoped<PrincipalApi>();
    collection.AddScoped<WatchedListApi>();
    collection.AddScoped<WishListApi>();
    collection.AddScoped<MyProvidersApi>();
    collection.AddScoped<AllProvidersApi>();
    collection.AddScoped<TicketApi>();
    collection.AddScoped<TicketVoteApi>();
    collection.AddScoped<AnnouncementApi>();

    collection.AddScoped<ExternalIdApi>();

    collection.AddScoped<ImdbApi>();
    collection.AddScoped<TmdbApi>();

    collection.AddLogging(logging =>
    {
        logging.AddProvider(new CosmosLoggerProvider());
    });
}