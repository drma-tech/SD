using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SD.WEB.Shared
{
    public partial class SettingsPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        private readonly IEnumerable<EnumFieldObject<Country>> regions = EnumHelper.GetList<Country>();
        private readonly IEnumerable<EnumFieldObject<AppLanguage>> appLanguages = EnumHelper.GetList<AppLanguage>();
        private readonly IEnumerable<EnumFieldObject<ContentLanguage>> contentLanguages = EnumHelper.GetList<ContentLanguage>();

        public AppLanguage Language { get; set; } = AppLanguage.en;
        public Country Region { get; set; } = Country.US;
        public ContentLanguage ContentLanguage { get; set; } = ContentLanguage.enUS;
        public bool DarkMode { get; set; }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            Language = await AppStateStatic.GetAppLanguage(JsRuntime, Cts.Token);
            Region = await AppStateStatic.GetRegion(IpInfoApi, JsRuntime, Cts.Token);
            ContentLanguage = await AppStateStatic.GetContentLanguage(JsRuntime, Cts.Token);
            DarkMode = await AppStateStatic.GetDarkMode(JsRuntime, Cts.Token) ?? false;

            return true;
        }

        protected async Task AppLanguageValueChanged(AppLanguage value)
        {
            Language = value;

            await JsRuntime.Utils().SetStorage("app-language", value, JavascriptContext.Default.AppLanguage, Cts.Token);

            if (value == AppLanguage.en)
            {
                await ContentLanguageValueChanged(ContentLanguage.enUS);
            }
            else if (value == AppLanguage.pt)
            {
                await ContentLanguageValueChanged(ContentLanguage.ptBR);
            }
            else if (value == AppLanguage.es)
            {
                await ContentLanguageValueChanged(ContentLanguage.esES);
            }
            else if (value == AppLanguage.zh)
            {
                await ContentLanguageValueChanged(ContentLanguage.zhCN);
            }
            else if (value == AppLanguage.fr)
            {
                await ContentLanguageValueChanged(ContentLanguage.frFR);
            }
            else if (value == AppLanguage.it)
            {
                await ContentLanguageValueChanged(ContentLanguage.itIT);
            }

            var uri = new Uri(Navigation.Uri);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries).Skip(1);
            var newPath = $"/{value}/{string.Join('/', segments)}";

            Navigation.NavigateTo($"{newPath}{uri.Query}".TrimEnd('/'), forceLoad: true);
        }

        protected async Task DarkModeChanged(bool value)
        {
            DarkMode = value;

            await JsRuntime.Utils().SetStorage("dark-mode", value, JavascriptContext.Default.Boolean, Cts.Token);

            AppStateStatic.ChangeDarkMode(value);
        }

        protected async Task RegionValueChanged(Country value)
        {
            Region = value;

            await JsRuntime.Utils().SetStorage("region", value.GetFieldSettings(translate: false).Name ?? "en-US", JavascriptContext.Default.String, Cts.Token);

            await AppStateStatic.ChangeRegionAsync(value);
        }

        protected async Task ContentLanguageValueChanged(ContentLanguage value)
        {
            ContentLanguage = value;

            await JsRuntime.Utils().SetStorage("content-language", value, JavascriptContext.Default.ContentLanguage, Cts.Token);

            AppStateStatic.ChangeContentLanguage(value);
        }
    }
}