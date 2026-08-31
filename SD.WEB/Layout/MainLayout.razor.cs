using Microsoft.JSInterop;
using MudBlazor;
using SD.WEB.Core.Auth;
using System.Security.Claims;

namespace SD.WEB.Layout
{
    public partial class MainLayout : IDisposable
    {
        private bool ShowAdvertising { get; set; } = true;

        private MudThemeProvider? _mudThemeProvider;
        private bool _darkMode;
        private string Culture => Navigation.GetCulture();
        protected CancellationTokenSource Cts { get; } = new();

        protected override void OnInitialized()
        {
            try
            {
                // *************************************
                // attention: avoid using asynchronous calls here, as it may affect static html generation (especially for anonymous users)
                // *************************************

                BufferedEvent.Register(nameof(ShowError), async (string msg) => { await ShowNotificationError(msg); });

                AppStateStatic.DarkModeChanged += dark => { _darkMode = dark; StateHasChanged(); };
                AppStateStatic.BreakpointChanged.Subscribe(breakpoint => StateHasChanged(), Cts.Token);
                AppStateStatic.HideAdvertising.Subscribe(hide => { ShowAdvertising = !hide; _ = InvokeAsync(StateHasChanged); }, Cts.Token);

                PrincipalApi.DataChanged += principal => { AppStateStatic.Principal = principal; StateHasChanged(); };

                // 2 = transform claims into useful data (need to register before)
                AuthStateProvider.AuthenticationStateChanged += async (task) =>
                {
                    try
                    {
                        var state = await task;
                        await ProcessAuthClaims(state.User);
                    }
                    catch (Exception ex)
                    {
                        ex.ProcessException(Snackbar, Logger);
                    }
                };

                // 1 = capture the claims
                BufferedEvent.Register(nameof(ClerkAuthChanged), async (string? token) =>
                {
                    try
                    {
                        var provider = (CompositeAuthStateProvider)AuthStateProvider;
                        provider.OnClerkAuthChanged(token);
                    }
                    catch (Exception ex)
                    {
                        ex.ProcessException(Snackbar, Logger);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.ProcessException(Snackbar, Logger);
            }
        }

        /// <summary>
        /// Do not process anything here related to authenticated users. (use UserStateChanged instead)
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                //Get the value at the beginning (NotifyBrowserViewportChangeAsync is too slow)
                AppStateStatic.Breakpoint = await BrowserViewportService.GetCurrentBreakpointAsync();
                AppStateStatic.Size = AppStateStatic.Breakpoint == Breakpoint.Xs ? Size.Small : Size.Medium;
                AppStateStatic.BreakpointChanged.Publish(AppStateStatic.Breakpoint);

                try
                {
                    await ApplyDarkMode(Cts.Token);
                    await RedirectToRequestConsentForChina(Cts.Token);
                    await AskUSerForReview(Cts.Token);
                    await RegisterSessionAccesses(Cts.Token);
                    await ShowOnBoardingPopup(Cts.Token);
                }
                catch (Exception ex)
                {
                    ex.ProcessException(Snackbar, Logger);
                }
            }
        }

        private async Task ProcessAuthClaims(ClaimsPrincipal user)
        {
            AppStateStatic.User = user;
            AppStateStatic.UserId = user?.FindFirst(c => string.Equals(c.Type, ClaimTypes.NameIdentifier, StringComparison.Ordinal))?.Value;
            var authenticated = user?.Identity?.IsAuthenticated ?? false;

            AppStateStatic.IsAuthenticated = authenticated;
            if (!authenticated) AppStateStatic.IsPremiumUser = false;

            //principal to be used for all the app
            AppStateStatic.Principal = await PrincipalApi.Get(setNewVersion: true, Cts.Token);

            var sub = AppStateStatic.Principal?.GetActiveSubscription();
            AppStateStatic.IsPremiumUser = sub?.IsActive() ?? false;
            AppStateStatic.ActiveProduct = sub?.Product ?? AccountProduct.Basic;

            await ProcessUserAccess();
            await AppStateStatic.UserStateChanged.PublishAsync();
        }

        private async Task ProcessUserAccess()
        {
            try
            {
                //register new user or new login (need to be last step on this method)
                if (!AppStateStatic.IsAuthenticated) return;
                if (Navigation.Uri.Contains("/legal/", StringComparison.OrdinalIgnoreCase)) return;

                if (AppStateStatic.Principal == null)
                {
                    Navigation.NavigateTo($"/{Culture}/auth/register-user");
                }
                else
                {
                    await RegisterLogin();
                }
            }
            catch (Exception ex)
            {
                ex.ProcessException(Snackbar, Logger);
            }
        }

        private async Task RegisterLogin()
        {
            var minInterval = TimeSpan.FromHours(12);
            var now = DateTimeOffset.UtcNow;

            if (AppStateStatic.LastAccess != null && now - AppStateStatic.LastAccess < minInterval)
            {
                return;
            }

            var platform = await AppStateStatic.GetPlatform(JsRuntime, CancellationToken.None);
            var country = await AppStateStatic.GetCountry(IpInfoApi, JsRuntime, CancellationToken.None);

            await LoginApi.Add(platform, country, CancellationToken.None);

            AppStateStatic.LastAccess = now;
        }

        private async Task ApplyDarkMode(CancellationToken cancellationToken)
        {
            var darkMode = await AppStateStatic.GetDarkMode(JsRuntime, Cts.Token);

            if (darkMode == null && _mudThemeProvider != null)
            {
                var system = await _mudThemeProvider.GetSystemDarkModeAsync();
                darkMode = system;

                await JsRuntime.Utils().SetStorage("dark-mode", darkMode ?? false, JavascriptContext.Default.Boolean, cancellationToken);
            }

            AppStateStatic.ChangeDarkMode(darkMode ?? false);
        }

        private async Task RedirectToRequestConsentForChina(CancellationToken cancellationToken)
        {
            if (!AppStateStatic.IsAuthenticated && !Navigation.Uri.Contains("/legal/", StringComparison.OrdinalIgnoreCase))
            {
                var country = await AppStateStatic.GetCountry(IpInfoApi, JsRuntime, cancellationToken);

                if (string.Equals(country, "CN", StringComparison.OrdinalIgnoreCase))
                {
                    var consent = await JsRuntime.Utils().GetStorage("consent", JavascriptContext.Default.Boolean, cancellationToken);

                    if (!consent)
                    {
                        Navigation.NavigateTo($"/{Culture}/legal/ask-consent");
                    }
                }
            }
        }

        private async Task AskUSerForReview(CancellationToken cancellationToken)
        {
            var accesses = await JsRuntime.Utils().GetStorage("session-accesses", JavascriptContext.Default.HashSetDateTime, cancellationToken) ?? [];
            var hasPreviousAccess = accesses.Count > 0;
            var lastAccess = hasPreviousAccess ? accesses.Max() : (DateTime?)null;
            bool isTooSoon = false;

            if (lastAccess != null)
            {
                var hoursSinceLast = (DateTime.UtcNow - lastAccess.Value).TotalHours;
                isTooSoon = hoursSinceLast < 24;
            }

            var reviewed = await JsRuntime.Utils().GetStorage("store-reviewed", JavascriptContext.Default.Boolean, cancellationToken);

            accesses.Add(DateTime.UtcNow); //simulate the new access
            bool isOddAccess = accesses.Count % 2 == 1;

            if (accesses.Count >= 3 && isOddAccess && !reviewed && !isTooSoon)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(5000, cancellationToken); //delay of 5 seconds
                    await DialogService.AskReviewPopup();
                }, cancellationToken);
            }
        }

        private async Task RegisterSessionAccesses(CancellationToken cancellationToken)
        {
            var accesses = await JsRuntime.Utils().GetStorage("session-accesses", JavascriptContext.Default.HashSetDateTime, cancellationToken) ?? [];
            var hasPreviousAccess = accesses.Count > 0;
            var lastAccess = hasPreviousAccess ? accesses.Max() : (DateTime?)null;
            bool isTooSoon = false;

            if (lastAccess != null)
            {
                var hoursSinceLast = (DateTime.UtcNow - lastAccess.Value).TotalHours;
                isTooSoon = hoursSinceLast < 2;
            }

            if (!isTooSoon)
            {
                accesses.Add(DateTime.UtcNow);

                if (accesses.Count > 10) //keep only the last 10 records
                {
                    accesses = [.. accesses.OrderDescending().Take(10)];
                }

                await JsRuntime.Utils().SetStorage("session-accesses", accesses, JavascriptContext.Default.HashSetDateTime, cancellationToken);
            }
        }

        private async Task ShowOnBoardingPopup(CancellationToken cancellationToken)
        {
            if (Navigation.Uri.Contains("printscreen", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var onboarding = await JsRuntime.Utils().GetStorage("onboarding-popup", JavascriptContext.Default.Boolean, cancellationToken);

            //show only once
            if (!onboarding)
            {
                await DialogService.OnboardingPopup(Culture);
                await JsRuntime.Utils().SetStorage("onboarding-popup", value: true, JavascriptContext.Default.Boolean, cancellationToken);
            }
        }

        protected async Task ShowNotificationError(string message)
        {
            if (!message.CanShowSnackbar()) return;

            Snackbar.Add(message, Severity.Error);

            await JsRuntime.Utils().PlayBeep(220, 400, "square", CancellationToken.None);
            await JsRuntime.Utils().Vibrate([200, 100, 200], CancellationToken.None);
        }

        [JSInvokable]
        public static void RegistrationSuccessful()
        {
            _ = BufferedEvent.Invoke(nameof(RegistrationSuccessful));
        }

        [JSInvokable]
        public static void AppleVerify(string receipt)
        {
            _ = BufferedEvent.Invoke(nameof(AppleVerify), receipt);
        }

        [JSInvokable]
        public static void ShowError(string error)
        {
            _ = BufferedEvent.Invoke(nameof(ShowError), error);
        }

        [JSInvokable]
        public static void ClerkAuthChanged(string? token)
        {
            _ = BufferedEvent.Invoke(nameof(ClerkAuthChanged), token);
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