using AzureStaticWebApps.Blazor.Authentication;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB;
using SD.WEB.Modules.Administrator.Core;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Provider.Core;
using SD.WEB.Modules.Suggestions.Core;
using SD.WEB.Modules.Support.Core;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

if (builder.RootComponents.Empty())
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
        .AddBootstrap5Providers()
        .AddFontAwesomeIcons();

    collection.AddPWAUpdater();
    collection.AddMediaQueryService();
    collection.AddMemoryCache();

    collection.AddHttpClient("RetryHttpClient", c => { c.BaseAddress = new Uri(baseAddress); });

    collection.AddStaticWebAppsAuthentication();
    collection.AddCascadingAuthenticationState();
    collection.AddOptions();
    collection.AddAuthorizationCore();

    collection.AddScoped<AdministratorApi>();
    collection.AddScoped<PrincipalApi>();
    collection.AddScoped<GravatarApi>();
    collection.AddScoped<LoginApi>();
    collection.AddScoped<WatchedListApi>();
    collection.AddScoped<WishListApi>();
    collection.AddScoped<WatchingListApi>();
    collection.AddScoped<MyProvidersApi>();
    collection.AddScoped<MySuggestionsApi>();
    collection.AddScoped<AllProvidersApi>();
    collection.AddScoped<TicketApi>();
    collection.AddScoped<UpdateApi>();

    collection.AddScoped<ExternalIdApi>();
    collection.AddScoped<CacheFlixsterApi>();
    collection.AddScoped<CacheYoutubeApi>();
    collection.AddScoped<CacheRatingsApi>();
    collection.AddScoped<CacheMetaCriticApi>();

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

    collection.AddScoped<PaddleConfigurationApi>();
    collection.AddScoped<PaddleSubscriptionApi>();

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