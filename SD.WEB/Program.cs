using AzureStaticWebApps.Blazor.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using SD.WEB;
using SD.WEB.Modules.Auth.Core;
using SD.WEB.Modules.Collections.Core;
using SD.WEB.Modules.Platform.Core;
using SD.WEB.Modules.Profile.Core;
using SD.WEB.Modules.Subscription.Core;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

if (builder.RootComponents.Empty())
{
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");
}

ConfigureServices(builder.Services, builder.HostEnvironment.BaseAddress, builder.Configuration);

var app = builder.Build();

await ConfigureCulture(app);

await app.RunAsync();

static void ConfigureServices(IServiceCollection collection, string baseAddress, IConfiguration configuration)
{
    collection.AddMudServices();

    collection.AddPWAUpdater();

    collection.AddHttpClient("LocalHttpClient", c => { c.BaseAddress = new Uri(baseAddress); });

    collection.AddHttpClient("ApiHttpClient", c => { c.BaseAddress = new Uri(configuration.GetValue<string>("ApiBaseAddress") ?? $"{baseAddress}api/"); })
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

    collection.AddHttpClient("ExternalHttpClient")
        .AddPolicyHandler(request => request.Method == HttpMethod.Get ? GetRetryPolicy() : Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>());

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
    collection.AddScoped<TmdbPopularApi>();
    collection.AddScoped<TmdbTopRatedApi>();
    collection.AddScoped<TmdbUpcomingApi>();
    collection.AddScoped<TmdbCreditApi>();
    collection.AddScoped<TmdbSearchApi>();

    collection.AddScoped<ImdbPopularApi>();
    collection.AddScoped<ImdbTopRatedApi>();

    collection.AddScoped<PaddleConfigurationApi>();
    collection.AddScoped<PaddleSubscriptionApi>();
    collection.AddScoped<IpInfoApi>();
}

static async Task ConfigureCulture(WebAssemblyHost? app)
{
    if (app != null)
    {
        var jsRuntime = app.Services.GetRequiredService<IJSRuntime>();

        //app language

        var nav = app.Services.GetService<NavigationManager>();
        var appLanguage = nav?.QueryString("app-language");

        if (appLanguage.Empty())
        {
            appLanguage = await jsRuntime.InvokeAsync<string>("GetLocalStorage", "app-language");
        }

        if (appLanguage.NotEmpty())
        {
            CultureInfo cultureInfo;

            try
            {
                cultureInfo = new CultureInfo(appLanguage!);
            }
            catch (Exception)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        AppStateStatic.AppLanguage = AppStateStatic.GetValidAppLanguage(CultureInfo.DefaultThreadCurrentUICulture);
        await jsRuntime.InvokeAsync<string>("SetLocalStorage", "app-language", AppStateStatic.AppLanguage.ToString());

        //content language

        var contentLanguage = await jsRuntime.InvokeAsync<string>("GetLocalStorage", "content-language");

        if (contentLanguage.Empty())
        {
            AppStateStatic.ContentLanguage = AppStateStatic.GetValidContentLanguage(CultureInfo.DefaultThreadCurrentUICulture);
        }
        else
        {
            if (Enum.TryParse<ContentLanguage>(contentLanguage, true, out var lang) && Enum.IsDefined(lang))
                AppStateStatic.ContentLanguage = lang;
            else
                AppStateStatic.ContentLanguage = ContentLanguage.enUS;
        }

        await jsRuntime.InvokeAsync<string>("SetLocalStorage", "content-language", AppStateStatic.ContentLanguage.ToString());
    }
}

//https://github.com/App-vNext/Polly/wiki/Polly-and-HttpClientFactory
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() // 408,5xx
        .WaitAndRetryAsync([TimeSpan.FromSeconds(2)]);
}