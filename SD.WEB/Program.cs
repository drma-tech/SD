using System.Globalization;
using AzureStaticWebApps.Blazor.Authentication;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using Polly;
using Polly.Extensions.Http;
using SD.WEB;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Provider.Core;
using SD.WEB.Modules.Subscription.Core;
using SD.WEB.Modules.Suggestions.Core;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress);

if (builder.RootComponents.Empty())
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

var app = builder.Build();

await ConfigureCulture(app);

await app.RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress)
{
    collection
        .AddBlazorise(options => options.Immediate = true)
        .AddBootstrap5Providers()
        .AddFontAwesomeIcons();

    collection.AddPWAUpdater();

    collection.AddHttpClient("RetryHttpClient", c => { c.BaseAddress = new Uri(baseAddress); })
        .AddPolicyHandler(request =>
            request.Method == HttpMethod.Get
                ? GetRetryPolicy()
                : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddStaticWebAppsAuthentication();
    collection.AddCascadingAuthenticationState();
    collection.AddOptions();
    collection.AddAuthorizationCore();

    collection.AddScoped<PrincipalApi>();
    collection.AddScoped<LoginApi>();
    collection.AddScoped<WatchedListApi>();
    collection.AddScoped<WishListApi>();
    collection.AddScoped<WatchingListApi>();
    collection.AddScoped<MyProvidersApi>();
    collection.AddScoped<MySuggestionsApi>();
    collection.AddScoped<AllProvidersApi>();

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

    collection.AddScoped<CacheSettingsApi>();
    collection.AddScoped<PaddleConfigurationApi>();
    collection.AddScoped<PaddleSubscriptionApi>();

    collection.AddLogging(logging => { logging.AddProvider(new CosmosLoggerProvider()); });
}

static async Task ConfigureCulture(WebAssemblyHost? app)
{
    if (app != null)
    {
        var nav = app.Services.GetService<NavigationManager>();
        var language = nav?.QueryString("language");

        if (language.Empty())
        {
            var jsRuntime = app.Services.GetRequiredService<IJSRuntime>();
            language = await jsRuntime.InvokeAsync<string>("GetLocalStorage", "language");
        }

        if (language.NotEmpty())
        {
            CultureInfo cultureInfo;

            try
            {
                cultureInfo = new CultureInfo(language!);
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

//https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 408,5xx
        .WaitAndRetryAsync([TimeSpan.FromSeconds(2)]);
}