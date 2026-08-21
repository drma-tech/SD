using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.RegularExpressions;

namespace SD.WEB.Modules.Auth
{
    public partial class LoginPage
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? ReturnUrl { get; set; }

        private string? email;
        private string? otp;
        private bool emailProbablySent;
        private MudTextField<string>? txtOTP;

        private async Task LoginWithGoogle() => await SignIn("google");

        private async Task LoginWithApple() => await SignIn("apple");

        private async Task LoginWithMicrosoft() => await SignIn("microsoft");

        private async Task LoginWithEmail() => await SignIn("email", email);

        private SD.Shared.Enums.Platform? Platform { get; set; }
        private bool _processingInProgress;
        private string? lastUsedProvider;

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            Platform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);
            lastUsedProvider = await JsRuntime.Utils().GetStorage("auth-last-used", JavascriptContext.Default.String, Cts.Token);
            return true;
        }

        private async Task SignIn(string? provider = null, string? email = null)
        {
            var success = false;
            var reference = Guid.NewGuid().ToString("N");
            var isEmail = string.Equals(provider, "email", StringComparison.OrdinalIgnoreCase);
            var isMicrosoft = string.Equals(provider, "microsoft", StringComparison.OrdinalIgnoreCase);

            try
            {
                _processingInProgress = true;

                await JsRuntime.Utils().SetStorage("auth", AuthProvider.Supabase, JavascriptContext.Default.AuthProvider, Cts.Token);
                await JsRuntime.Utils().SetStorage("auth-last-used", provider, JavascriptContext.Default.String, Cts.Token);

                if (isEmail)
                {
                    if (!IsValidEmail(email ?? ""))
                    {
                        await ShowError(Translations.Module.Auth.EnterEmail);
                        _processingInProgress = false; StateHasChanged();
                        return;
                    }

                    await PublicLoginApi.SendEmail(email, reference, Cts.Token);
                    success = true;
                }
                else
                {
                    if (isMicrosoft) provider = "azure";
                    await JsRuntime.Supabase().SignInAsync(provider!, ReturnUrl, Cts.Token);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
                _processingInProgress = false; StateHasChanged();
            }
            finally
            {
                if (success)
                {
                    await Task.Delay(isEmail ? 6000 : 25000, Cts.Token);
                    if (isEmail) emailProbablySent = true;
                }

                _processingInProgress = false;
                StateHasChanged();

                if (isEmail && success)
                {
                    await ShowInfo(Translations.Module.Auth.CodeSent.CustomFormat(email));
                    await txtOTP!.FocusAsync();
                    _ = MonitorEmailStatus(reference);
                }
            }
        }

        private async Task MonitorEmailStatus(string reference)
        {
            try
            {
                for (int i = 0; i < 12 && !_processingInProgress; i++)
                {
                    var message = await PublicLoginApi.StatusEmail(reference, Cts.Token);

                    if (message.NotEmpty())
                    {
                        await ShowError(message);
                        emailProbablySent = false;
                        StateHasChanged();
                        break;
                    }

                    if (_processingInProgress || Cts.IsCancellationRequested) break;

                    await Task.Delay(5000, Cts.Token);
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
        private static partial Regex EmailRegex();

        private static bool IsValidEmail(string? email)
        {
            if (email.Empty()) return false;

            return EmailRegex().IsMatch(email);
        }

        private async Task ConfirmCode()
        {
            try
            {
                _processingInProgress = true;

                if (otp.Empty())
                {
                    await ShowError(Translations.Module.Auth.EnterCode);
                    _processingInProgress = false; StateHasChanged();
                    return;
                }

                await JsRuntime.Supabase().ConfirmCode(email!, otp!, Cts.Token);

                Navigation.NavigateTo($"/{Culture}");
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
                _processingInProgress = false; StateHasChanged();
            }
            finally
            {
                await Task.Delay(5000, Cts.Token);
                _processingInProgress = false;
            }
        }

        private static readonly HashSet<string> KnownDomains = new(StringComparer.OrdinalIgnoreCase) { "gmail.com", "hotmail.com", "outlook.com", "yahoo.com" };

        public static string? SuggestEmailCorrection(string? email)
        {
            if (email.Empty()) return null;

            var parts = email.Split('@');
            if (parts.Length != 2)
                return null;

            var local = parts[0];
            var domain = parts[1];

            if (KnownDomains.Contains(domain))
                return null;

            // Find closest match
            var closest = KnownDomains
                .Select(d => new { Domain = d, Distance = SD.Shared.Core.Helper.StringHelper.Levenshtein(domain, d) })
                .OrderBy(x => x.Distance)
                .First();

            // Threshold to avoid bad suggestions
            if (closest.Distance <= 2)
                return $"Did you mean {local}@{closest.Domain}?";

            return null;
        }
    }
}