using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.Shared.Core;
using SD.WEB.Core;
using SD.WEB.Services;
using System.Globalization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace SD.WEB
{
    public static class Initialize
    {
        public static void ConfigureComponents(this IServiceCollection collection)
        {
            collection
                .AddBlazorise(options => options.Immediate = true)
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            collection.AddBlazoredSessionStorage(config => config.JsonSerializerOptions.WriteIndented = true);
            collection.AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);

            collection.AddPWAUpdater();
            collection.AddMediaQueryService();

            collection.AddScoped<IStorageService, StorageService>();
            collection.AddScoped<ProviderServide>();
            collection.AddScoped<Settings>();
        }

        public static void ConfigureServices(this IServiceCollection collection)
        {
            collection.AddScoped<Services.TMDB.DiscoverService>();
            collection.AddScoped<Services.TMDB.ListService>();
            collection.AddScoped<Services.TMDB.MediaDetailService>();
            collection.AddScoped<Services.TMDB.PopularService>();
            collection.AddScoped<Services.TMDB.TopRatedService>();
            collection.AddScoped<Services.TMDB.UpcomingService>();
            collection.AddScoped<Services.TmdbExternalIdTvService>();

            collection.AddScoped<Services.IMDB.PopularService>();
            collection.AddScoped<Services.IMDB.TopRatedService>();
            collection.AddScoped<Services.IMDB.UpcomingService>();
        }

        public static void ConfigureCulture(this WebAssemblyHost host)
        {
            CultureInfo culture;
            var StorageService = host.Services.GetRequiredService<IStorageService>();
            var sett = StorageService.Local.GetItem<Settings>("Settings");

            if (sett != null)
            {
                culture = new CultureInfo(sett.Language.GetName(false) ?? "en-US");
            }
            else
            {
                culture = CultureInfo.CurrentCulture;

                //save the new settings
                sett = new Settings(StorageService);
                StorageService.Local.SetItem("Settings", sett);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}