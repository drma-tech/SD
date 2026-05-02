using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using SD.Shared.Models.List.Tmdb;
using SD.WEB.Core.Auth;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.Collections.Core;
using SD.WEB.Modules.Platform.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Subscription.Core;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.UseSentry(options =>
{
    options.Dsn = "https://94ae67eb3fb0bc82327607ddd9d6aebb@o4510938040041472.ingest.us.sentry.io/4510938043711488";
    options.DiagnosticLevel = SentryLevel.Warning;
    options.Environment = builder.HostEnvironment.Environment;

    options.TracePropagationTargets = []; //Disable tracing because it breaks communication with external APIs.

    options.SetBeforeSend(evt =>
    {
        const string error = "error";

        evt.Release = $"sd-blazor@{AppStateStatic.Version ?? error}";

        evt.SetTag("custom.version", AppStateStatic.Version ?? error);
        evt.SetTag("custom.platform", AppStateStatic.GetSavedPlatform()?.ToString() ?? error);

        evt.SetExtra("browser_name", AppStateStatic.BrowserName ?? error);
        evt.SetExtra("browser_version", AppStateStatic.BrowserVersion ?? error);
        evt.SetExtra("operation_system", AppStateStatic.OperatingSystem ?? error);

        return evt;
    });
});

builder.Logging.SetMinimumLevel(LogLevel.Warning);

if (builder.RootComponents.Empty())
{
    builder.RootComponents.Add<SD.WEB.App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress, builder.Configuration);

var app = builder.Build();

var nav = app.Services.GetService<NavigationManager>();
var js = app.Services.GetRequiredService<IJSRuntime>();

await ConfigureCulture(nav, js);

AppStateStatic.Version = await AppStateStatic.GetAppVersion(js);
AppStateStatic.BrowserName = await js.Utils().GetBrowserName();
AppStateStatic.BrowserVersion = await js.Utils().GetBrowserVersion();
AppStateStatic.OperatingSystem = await js.Utils().GetOperatingSystem();

await js.Utils().SetStorage("app-version", AppStateStatic.Version);
_ = await AppStateStatic.GetPlatform(js);
await js.Services().InitGoogleAnalytics(AppStateStatic.Version);
await js.Services().InitUserBack(AppStateStatic.Version);

await app.RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress, IConfiguration configuration)
{
    ConfigurePrerendering();

    collection.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.VisibleStateDuration = 10000;
    });

    collection.AddPWAUpdater();
    collection.AddScoped<AppVersionHandler>();

    collection.AddHttpClient("Local", c => { c.BaseAddress = new Uri(baseAddress); }); //json files and other assets, not the API.

    var apiOrigin = configuration["DownstreamApi:BaseUrl"] ??
        (baseAddress.Contains("localhost") || baseAddress.Contains("127.0.0.1") ? throw new UnhandledException($"DownstreamApi:BaseUrl is null.") : $"{baseAddress}api/");

    collection.AddHttpClient("Anonymous", (service, options) => { options.BaseAddress = new Uri(apiOrigin); options.Timeout = TimeSpan.FromSeconds(30); })
        .AddHttpMessageHandler<AppVersionHandler>()
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddScoped<AuthenticationStateProvider, CompositeAuthStateProvider>();
    collection.AddScoped<SupabaseAuthStateProvider>();

    collection.AddScoped<CustomAuthorizationHandler>();
    collection.AddHttpClient("Authenticated", (service, options) => { options.BaseAddress = new Uri(apiOrigin); options.Timeout = TimeSpan.FromSeconds(30); })
        .AddHttpMessageHandler<CustomAuthorizationHandler>()
        .AddHttpMessageHandler<AppVersionHandler>()
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddAuthorizationCore();

    ConfigureApi(collection);
}

static void ConfigurePrerendering()
{
    const string loading = "loading";

    AppStateStatic.Version = loading;
    AppStateStatic.BrowserName = loading;
    AppStateStatic.BrowserVersion = loading;
    AppStateStatic.OperatingSystem = loading;
}

static void ConfigureApi(IServiceCollection collection)
{
    collection.AddScoped<PrincipalApi>();
    collection.AddScoped<PrincipalImportApi>();
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
    collection.AddScoped<TmdbCreditApi>();
    collection.AddScoped<TmdbSearchMultiApi>();
    collection.AddScoped<TmdbSearchKeywordApi>();
    collection.AddScoped<TmdbRecommendationsApi>();

    collection.AddScoped<ImdbPopularApi>();

    collection.AddScoped<PaymentConfigurationApi>();
    collection.AddScoped<PaymentAuthApi>();
    collection.AddScoped<PaymentPublicApi>();
    collection.AddScoped<IpInfoApi>();
}

static async Task ConfigureCulture(NavigationManager? nav, IJSRuntime js)
{
    //app language

    var uri = new Uri(nav!.Uri);

    var appLanguage = await ExtensionMethodsWeb.GetRouteLanguage(js, uri.AbsolutePath);

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

//https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 408,5xx
        .WaitAndRetryAsync([TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)]);
}