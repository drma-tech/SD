using AzureStaticWebApps.Blazor.Authentication;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using SD.WEB;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.News.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Provider.Core;
using SD.WEB.Modules.Suggestions.Core;
using SD.WEB.Modules.Support.Core;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var host = builder.Build();

await ConfigureCulture(host);

await host.RunAsync();

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
    collection.AddScoped<WatchingListApi>();
    collection.AddScoped<MyProvidersApi>();
    collection.AddScoped<AllProvidersApi>();
    collection.AddScoped<TicketApi>();
    collection.AddScoped<TicketVoteApi>();
    collection.AddScoped<AnnouncementApi>();

    collection.AddScoped<ExternalIdApi>();
    collection.AddScoped<CacheApi>();
    collection.AddScoped<NewsApi>();

    collection.AddScoped<ImdbApi>();
    collection.AddScoped<TmdbApi>();

    collection.AddScoped<TmdbListApi>();
    collection.AddScoped<TmdbDiscoveryApi>();
    collection.AddScoped<TmdbNowPlayingApi>();
    collection.AddScoped<TmdbPopularApi>();
    collection.AddScoped<TmdbTopRatedApi>();
    collection.AddScoped<TmdbUpcomingApi>();
    collection.AddScoped<TmdbCreditApi>();
    collection.AddScoped<TmdbSearchApi>();

    collection.AddScoped<ImdbPopularApi>();
    collection.AddScoped<ImdbTopRatedApi>();
    collection.AddScoped<ImdbUpcomingApi>();

    collection.AddScoped<AppState>();

    collection.AddResizeListener();

    collection.AddLogging(logging =>
    {
        logging.AddProvider(new CosmosLoggerProvider());
    });
}

static async Task ConfigureCulture(WebAssemblyHost? host)
{
    if (host != null)
    {
        var JS = host.Services.GetRequiredService<IJSRuntime>();

        CultureInfo cultureInfo;
        var language = await JS.InvokeAsync<string>("blazorLanguage.get");
        if (language != null)
        {
            cultureInfo = new CultureInfo(language ?? "en-US");
        }
        else
        {
            cultureInfo = new CultureInfo("en-US");
            await JS.InvokeVoidAsync("blazorLanguage.set", "en-US");
        }
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        var region = await JS.InvokeAsync<string>("blazorRegion.get");
        if (region == null)
            await JS.InvokeVoidAsync("blazorRegion.set", "US");
    }
}