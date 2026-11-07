using SD.WEB.Modules.Auth.Core;
using System.Globalization;

namespace SD.WEB.Modules.Help
{
    public partial class HelpCenterPage
    {
        private SD.Shared.Enums.Platform? CurrentPlatform;
        private bool DarkMode;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CurrentPlatform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);
                DarkMode = await AppStateStatic.GetDarkMode(JsRuntime, Cts.Token) ?? false;
            }
        }

        private async Task DownloadData()
        {
            try
            {
                if (AppStateStatic.Principal == null) return;

                var data = new SuperData
                {
                    Principal = AppStateStatic.Principal,
                    Login = await LoginApi.Get(Cts.Token),
                    Providers = await MyProvidersApi.Get(states: [], Cts.Token),
                    WatchingList = await WatchingListApi.Get(states: [], Cts.Token),
                    WishList = await WishListApi.Get(states: [], Cts.Token),
                };

                var fileName = string.Create(CultureInfo.InvariantCulture, $"{AppInfo.Title.ToSlug()}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
                await JsRuntime.Utils().DownloadFile(fileName, "application/json", data.ConvertFromObjectToBytes(), Cts.Token);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task DeleteAccount()
        {
            try
            {
                if (AppStateStatic.Principal == null) return;

                // TODO: Remove this active-subscription check. The deletion request must be accepted independently of subscription cancellation.
                // TODO: Audit the full deletion lifecycle, including all first-party stores, authentication, third-party processors, billing records, logs, support data, and backups. Retain only records supported by a documented legal basis and retention period.
                if (AppStateStatic.Principal.GetActiveSubscription() != null)
                {
                    await DialogService.ShowMessageBoxAsync("You still have an active subscription", "Please cancel your subscription before deleting your profile.");
                    return;
                }

                if (await DialogService.ShowMessageBoxAsync(AppInfo.Title, Translations.Module.Auth.SureDeleteAccount, Translations.Button.Ok, Translations.Button.Cancel) ?? false)
                {
                    //remove data from cosmos db and authentication provider
                    await PrincipalApi.Remove(Cts.Token);

                    //close current login session
                    await Logout();
                }
                else
                {
                    await ShowInfo(Translations.Notification.OperationCanceled);
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task FeedbackClick()
        {
            await JsRuntime.Window().InvokeVoidAsync("eval", Cts.Token, "uj && uj.open();");
        }

        private async Task ShowCacheClick()
        {
            await JsRuntime.Utils().ShowCache(Cts.Token);
        }

        private void ShowConsole()
        {
            Navigation.NavigateTo("w2n://console-screen");
        }

        private async Task ClearCacheClick()
        {
            await JsRuntime.Utils().ClearAllStorage();
        }

        private void AppLanguageClick(AppLanguage lang)
        {
            Navigation.NavigateTo($"/{lang}/help", forceLoad: true);
        }

        private async Task Logout()
        {
            await JsRuntime.Clerk().SignOutAsync(Cts.Token);
        }

        private async Task IsBetaValueChanged(bool value)
        {
            if (value)
            {
                var confirmed = await DialogService.ShowMessageBoxAsync(AppInfo.Title, "Are you sure you want to switch to the beta version? Some system features or features of the app may be unstable.", Translations.Button.Ok, Translations.Button.Cancel) ?? false;

                if (!confirmed) return;
            }

            await JsRuntime.Utils().SetStorage("beta", value, JavascriptContext.Default.Boolean, Cts.Token);
            Navigation.NavigateTo($"/{Culture}/help", forceLoad: true);
        }
    }
}