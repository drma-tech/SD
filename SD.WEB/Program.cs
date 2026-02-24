using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
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

    options.TracePropagationTargets = [];

    options.SetBeforeSend(evt =>
    {
        evt.SetTag("custom.version", AppStateStatic.Version ?? "error");
        evt.SetTag("custom.platform", AppStateStatic.GetSavedPlatform()?.ToString() ?? "error");

        evt.SetExtra("browser_name", AppStateStatic.BrowserName);
        evt.SetExtra("browser_version", AppStateStatic.BrowserVersion);
        evt.SetExtra("operation_system", AppStateStatic.OperatingSystem);

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

var js = app.Services.GetRequiredService<IJSRuntime>();

await ConfigureCulture(app, js);

AppStateStatic.Version = await AppStateStatic.GetAppVersion(js);
AppStateStatic.BrowserName = await js.Utils().GetBrowserName();
AppStateStatic.BrowserVersion = await js.Utils().GetBrowserVersion();
AppStateStatic.OperatingSystem = await js.Utils().GetOperatingSystem();
AppStateStatic.UserAgent = await js.Window().InvokeAsync<string>("eval", "navigator.userAgent");

await js.Utils().SetStorage("app-version", AppStateStatic.Version);
await AppStateStatic.GetPlatform(js);
await js.Services().InitGoogleAnalytics(AppStateStatic.Version);
await js.Services().InitUserBack(AppStateStatic.Version);

await app.RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress, IConfiguration configuration)
{
    collection.AddMudServices(config =>
    {
        config.SnackbarConfiguration.PreventDuplicates = false;
        config.SnackbarConfiguration.VisibleStateDuration = 10000;
    });

    collection.AddPWAUpdater();

    collection.AddHttpClient("Local", c => { c.BaseAddress = new Uri(baseAddress); });

    var apiOrigin = configuration["DownstreamApi:BaseUrl"] ??
        (baseAddress.Contains("localhost") || baseAddress.Contains("127.0.0.1") ? throw new UnhandledException($"DownstreamApi:BaseUrl is null.") : $"{baseAddress}api/");

    collection.AddHttpClient("Anonymous", (service, options) => { options.BaseAddress = new Uri(apiOrigin); options.Timeout = TimeSpan.FromSeconds(60); })
       .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddScoped<AuthenticationStateProvider, CompositeAuthStateProvider>();
    collection.AddScoped<FirebaseAuthStateProvider>();
    collection.AddScoped<SupabaseAuthStateProvider>();

    collection.AddScoped<CustomAuthorizationHandler>();
    collection.AddHttpClient("Authenticated", (service, options) => { options.BaseAddress = new Uri(apiOrigin); options.Timeout = TimeSpan.FromSeconds(60); })
        .AddHttpMessageHandler<CustomAuthorizationHandler>()
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddHttpClient("External", (service, options) => { options.Timeout = TimeSpan.FromSeconds(60); })
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddAuthorizationCore();

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
    collection.AddScoped<TmdbSearchApi>();
    collection.AddScoped<TmdbRecommendationsApi>();

    collection.AddScoped<ImdbPopularApi>();

    collection.AddScoped<PaymentConfigurationApi>();
    collection.AddScoped<PaymentAuthApi>();
    collection.AddScoped<IpInfoApi>();
    collection.AddScoped<IpInfoServerApi>();
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