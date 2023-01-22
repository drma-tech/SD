using AzureStaticWebApps.Blazor.Authentication;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.News.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Provider.Core;
using SD.WEB.Modules.Suggestions.Core;
using SD.WEB.Modules.Support.Core;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

if (!builder.RootComponents.Any())
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

var host = builder.Build();

ConfigureCulture(host);

await host.RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress)
{
    collection
        .AddBlazorise(options => options.Immediate = true)
        .AddBootstrapProviders()
        .AddFontAwesomeIcons();

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
    collection.AddScoped<TrailersApi>();

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

static void ConfigureCulture(WebAssemblyHost? host)
{
    if (host != null)
    {
        var nav = host.Services.GetService<NavigationManager>();
        var language = nav?.QueryString("language");

        if (!string.IsNullOrEmpty(language))
        {
            CultureInfo cultureInfo;

            try
            {
                cultureInfo = new CultureInfo(language);
            }
            catch (Exception)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}