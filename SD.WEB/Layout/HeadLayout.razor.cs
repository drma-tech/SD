using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace SD.WEB.Layout
{
    public partial class HeadLayout : IDisposable
    {
        [Parameter][SupplyParameterFromQuery(Name = "stripe_session_id")] public string? stripe_session_id { get; set; }
        [Parameter][SupplyParameterFromQuery(Name = "printscreen")] public bool Printscreen { get; set; } = false;

        private string Culture => Navigation.GetCulture();

        private bool _openMenu;
        private bool _openApps;

        private int _processingCount;
        private bool _processing => _processingCount > 0;

        private bool _blockedActions => Navigation.Uri.Contains("register-user", StringComparison.OrdinalIgnoreCase) || Navigation.Uri.Contains("ask-consent", StringComparison.OrdinalIgnoreCase);
        private static string menuClass => AppStateStatic.IsMobile ? "icon-text-button" : "px-3";

        protected CancellationTokenSource Cts { get; } = new();

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += delegate { StateHasChanged(); };

            AppStateStatic.BreakpointChanged.Subscribe(breakpoint => StateHasChanged(), Cts.Token);

            //avoid - Object reference not set to an instance of an object.
            //commit: breakpoint refactorer / ActionDispatcher and TaskDispatcher (2026-05-25)
            if (!Navigation.IsPrerendering())
            {
                AppStateStatic.ProcessingStarted.Subscribe(async () =>
                {
                    Interlocked.Increment(ref _processingCount);
                    await InvokeAsync(StateHasChanged);
                }, Cts.Token);

                AppStateStatic.ProcessingFinished.Subscribe(async () =>
                {
                    Interlocked.Decrement(ref _processingCount);
                    await Task.Delay(200, Cts.Token);
                    await InvokeAsync(StateHasChanged);
                }, Cts.Token);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender && stripe_session_id.NotEmpty())
                {
                    var success = await PaymentPublicApi.StripeValidateSession(stripe_session_id, Cts.Token);

                    if (success)
                        await ShowSuccess("Subscription successful!");
                    else
                        await ShowError("Something didn’t go as expected. If your payment wasn’t completed, please try again. You can also contact support for assistance.");
                }

                await base.OnAfterRenderAsync(firstRender);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to process the Stripe checkout result.");
            }
        }

        private async Task Login()
        {
            try
            {
    //_openMenu = false;
                //Navigation.NavigateTo($"/{Culture}/auth/login?returnUrl={Uri.EscapeDataString(Navigation.Uri.Split('#')[0])}");
                _openMenu = false;
                await JsRuntime.Clerk().SignInAsync(Cts.Token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute Login.");
            }
        }

        private async Task Logout()
        {
            try
            {
    _openMenu = false;
                await JsRuntime.Clerk().SignOutAsync(Cts.Token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute Logout.");
            }
        }

        private async Task OpenConfigurations()
        {
            try
            {
    _openMenu = false;
                await DialogService.SettingsPopup();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute OpenConfigurations.");
            }
        }

        private async Task MyAccount()
        {
            try
            {
    _openMenu = false;
                await JsRuntime.Clerk().AccountPopup(Cts.Token);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute MyAccount.");
            }
        }

        private Color GetColor(string endpoint)
        {
            return Focused(endpoint) ? Color.Primary : Color.Inherit;
        }

        private Variant GetVariant(string endpoint)
        {
            return Focused(endpoint) ? Variant.Filled : Variant.Text;
        }

        private bool Focused(string endpoint)
        {
            var uri = new Uri(Navigation.Uri);

            return string.Equals(uri.AbsolutePath, endpoint, StringComparison.OrdinalIgnoreCase);
        }

        private async Task OpenSubscription()
        {
            try
            {
    _openMenu = false;
                await DialogService.SubscriptionPopup();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute OpenSubscription.");
            }
        }

        protected async Task ShowSuccess(string message)
        {
            Snackbar.Add(message, Severity.Success);

            await JsRuntime.Utils().PlayBeep(880, 100, "sine", Cts.Token);
            await JsRuntime.Utils().Vibrate([40], Cts.Token);
        }

        protected async Task ShowError(string message)
        {
            Snackbar.Add(message, Severity.Error);

            await JsRuntime.Utils().PlayBeep(220, 400, "square", Cts.Token);
            await JsRuntime.Utils().Vibrate([200, 100, 200], Cts.Token);
        }

        private void AppsClick()
        {
            _openApps = true;
        }

        private void MenuClick()
        {
            _openMenu = true;
        }

        private bool isDisposed;

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                Cts.Cancel();
                Cts.Dispose();
            }

            isDisposed = true;
        }
    }
}