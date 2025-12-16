using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using SD.WEB;
using SD.WEB.Core.Auth;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.Collections.Core;
using SD.WEB.Modules.Platform.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Subscription.Core;
using SD.WEB.Modules.Support.Core;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

if (builder.RootComponents.Empty())
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress, builder.Configuration);

builder.Services.AddSingleton<ILoggerProvider, CosmosLoggerProvider>();

var app = builder.Build();

var js = app.Services.GetRequiredService<IJSRuntime>();

await ConfigureCulture(app, js);

var version = SD.WEB.Layout.MainLayout.GetAppVersion();
await js.Utils().SetLocalStorage("app-version", version);
await AppStateStatic.GetPlatform(js);
await js.Services().InitGoogleAnalytics(version);
await js.Services().InitUserBack(version);

await app.RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress, IConfiguration configuration)
{
    collection.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.VisibleStateDuration = 10000;
    });

    collection.AddPWAUpdater();

    collection.AddHttpClient("Local", c => { c.BaseAddress = new Uri(baseAddress); });

    var apiOrigin = configuration["DownstreamApi:BaseUrl"] ??
        (baseAddress.Contains("localhost") || baseAddress.Contains("127.0.0.1") ? throw new UnhandledException($"DownstreamApi:BaseUrl is null.") : $"{baseAddress}api/");

    collection.AddHttpClient("Anonymous", (service, options) => { options.BaseAddress = new Uri(apiOrigin); options.Timeout = TimeSpan.FromSeconds(60); })
       .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddScoped<AuthenticationStateProvider, FirebaseAuthStateProvider>();
    collection.AddScoped<CustomAuthorizationHandler>();
    collection.AddHttpClient("Authenticated", (service, options) => { options.BaseAddress = new Uri(apiOrigin); options.Timeout = TimeSpan.FromSeconds(60); })
        .AddHttpMessageHandler<CustomAuthorizationHandler>()
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddHttpClient("External", (service, options) => { options.Timeout = TimeSpan.FromSeconds(60); })
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

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
    collection.AddScoped<TmdbPopularApi>();
    collection.AddScoped<TmdbTopRatedApi>();
    collection.AddScoped<TmdbUpcomingApi>();
    collection.AddScoped<TmdbCreditApi>();
    collection.AddScoped<TmdbSearchApi>();
    collection.AddScoped<TmdbRecommendationsApi>();

    collection.AddScoped<ImdbPopularApi>();
    collection.AddScoped<ImdbTopRatedApi>();

    collection.AddScoped<PaymentConfigurationApi>();
    collection.AddScoped<PaymentSubscriptionApi>();
    collection.AddScoped<PaymentAuthApi>();
    collection.AddScoped<IpInfoApi>();
    collection.AddScoped<IpInfoServerApi>();
    collection.AddScoped<LoggerApi>();
    collection.AddScoped<EnergyApi>();
    collection.AddScoped<EnergyAuthApi>();
    collection.AddScoped<FirebaseApi>();
}

static async Task ConfigureCulture(WebAssemblyHost? app, IJSRuntime js)
{
    if (app != null)
    {
        //app language

        var nav = app.Services.GetService<NavigationManager>();
        var appLanguage = nav?.QueryString("app-language");

        if (appLanguage.Empty())
        {
            appLanguage = (await AppStateStatic.GetAppLanguage(js)).ToString();
        }

        if (appLanguage.NotEmpty())
        {
            CultureInfo cultureInfo;

            try
            {
                cultureInfo = new CultureInfo(appLanguage);
            }
            catch (Exception)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        //content language

        _ = await AppStateStatic.GetContentLanguage(js); //force read previously saved
    }
}

//https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 408,5xx
        .WaitAndRetryAsync([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4)]);
}