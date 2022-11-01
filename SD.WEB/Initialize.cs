using Blazored.SessionStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using BlazorPro.BlazorSize;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SD.WEB.Modules.List.Core.TMDB;
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

            collection.AddPWAUpdater();
            collection.AddMediaQueryService();

            collection.AddScoped<Settings>();
        }

        public static void ConfigureServices(this IServiceCollection collection)
        {
            collection.AddScoped<ListService>();
            collection.AddScoped<MediaDetailService>();
        }

        public static void ConfigureCulture(this WebAssemblyHost host)
        {
            CultureInfo culture;
            var session = host.Services.GetRequiredService<ISyncSessionStorageService>();
            var sett = session.GetItem<Settings>("Settings");

            if (sett != null)
            {
                culture = new CultureInfo(sett.Language.GetName(false) ?? "en-US");
            }
            else
            {
                culture = CultureInfo.CurrentCulture;

                //save the new settings
                sett = new Settings(session);
                session.SetItem("Settings", sett);
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}