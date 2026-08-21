using SD.WEB.Modules.Auth.Core;
using System.Globalization;

namespace SD.WEB.Modules.Help
{
    public partial class HelpCenterPage
    {
        private SD.Shared.Enums.Platform? CurrentPlatform;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                CurrentPlatform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);
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
                    Providers = await MyProvidersApi.Get(actions: null, Cts.Token),
                    WatchingList = await WatchingListApi.Get(actions: null, Cts.Token),
                    WishList = await WishListApi.Get(actions: null, Cts.Token),
                };

                var fileName = string.Create(CultureInfo.InvariantCulture, $"{AppInfo.Title.ToSlug()}_{AppStateStatic.Principal.AuthProviders[0]}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
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

                if (AppStateStatic.Principal.GetActiveSubscription() != null)
                {
                    await DialogService.ShowMessageBoxAsync("You still have an active subscription", "Please cancel your subscription before deleting your profile.");
                    return;
                }

                if (await DialogService.ShowMessageBoxAsync(AppInfo.Title, Translations.Module.Auth.SureDeleteAccount, Translations.Button.Ok, Translations.Button.Cancel) ?? false)
                {
                    //remove data from cosmos db
                    await PrincipalApi.Remove(Cts.Token);

                    //TODO: remove data from supabase table

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
            await JsRuntime.Window().InvokeVoidAsync("eval", "Userback && Userback.openForm('general', 'form');");
        }

        private async Task ShowCacheClick()
        {
            await JsRuntime.Utils().ShowCache(Cts.Token);
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
            await JsRuntime.Supabase().SignOutAsync(Cts.Token);
        }
    }
}