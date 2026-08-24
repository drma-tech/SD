using Microsoft.AspNetCore.Components;
using MudBlazor;
using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Subscription
{
    public partial class SubscriptionPopup
    {
        [CascadingParameter] private IMudDialogInstance? MudDialog { get; set; }

        private AuthPrincipal? Client { get; set; }
        private List<PaymentProvider> Providers { get; set; } = [];

        private AccountCycle Cycle { get; set; } = AccountCycle.Yearly;
        private SD.Shared.Enums.Platform? Platform { get; set; }
        private PaymentProvider Provider { get; set; } = PaymentProvider.Stripe;
        private string? CurrentCountry { get; set; }
        private bool BlockedCountry { get; set; }

        //https://www.xolo.io/zz-en/faq/xolo-leap/category/get-started/article/do-you-accept-customers-from-all-countries
        private readonly Dictionary<string, string> _XoloBlockedCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            { "AF", "Afghanistan" },
            { "BY", "Belarus" },
            { "CE", "Crimea" },
            { "CU", "Cuba" },
            { "IR", "Iran" },
            { "IQ", "Iraq" },
            { "MM", "Myanmar" },
            { "KP", "North Korea" },
            { "RU", "Russia" },
            { "SY", "Syria" },
            { "VE", "Venezuela" },
        };

        //https://docs.stripe.com/payments/managed-payments/how-it-works#restricted-countries
        private readonly Dictionary<string, string> _StripeBlockedCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            { "SH", "Saint Helena, Ascension and Tristan da Cunha" },
            { "CN", "China" },
            { "CU", "Cuba" },
            { "IR", "Iran" },
            { "XK", "Kosovo" },
            { "KP", "North Korea" },
            { "RU", "Russia" },
            { "SY", "Syria" },
        };

        protected override void OnInitialized()
        {
            BufferedEvent.Register(nameof(RegistrationSuccessful), RegistrationSuccessful);
            BufferedEvent.Register(nameof(AppleVerify), async (string receipt) => await AppleVerify(receipt));
        }

        protected override async Task<bool> LoadInteropDataAsync(Microsoft.JSInterop.IJSRuntime JsRuntime)
        {
            CurrentCountry = await IpInfoApi.GetCountry(Cts.Token);

            if (CurrentCountry.NotEmpty())
            {
                BlockedCountry = _XoloBlockedCountries.ContainsKey(CurrentCountry);

                if (BlockedCountry) return true;
            }

            Providers.Clear();

            Platform = await AppStateStatic.GetPlatform(JsRuntime, Cts.Token);

            if (Platform == SD.Shared.Enums.Platform.ios)
            {
                if (string.Equals(CurrentCountry, "US", StringComparison.OrdinalIgnoreCase))
                {
                    Providers.Add(PaymentProvider.Apple);
                    Providers.Add(PaymentProvider.Stripe);
                }
                else
                {
                    Providers.Add(PaymentProvider.Apple);
                }
            }
            else
            {
                Providers.Add(PaymentProvider.Stripe);
                Providers.Add(PaymentProvider.Microsoft);
                Providers.Add(PaymentProvider.Google);
                Providers.Add(PaymentProvider.Apple);
            }

            Provider = Providers[0]; //todo: when implemented, select native provider by default

            return true;
        }

        protected override async Task LoadAuthenticatedDataAsync(CancellationToken token)
        {
            Client = await PrincipalApi.Get(setNewVersion: true, token);

            var sub = Client?.GetActiveSubscription();

            if (sub?.Provider != null && sub.Cycle != null)
            {
                Provider = sub.Provider.Value;
                Cycle = sub.Cycle.Value;
            }
        }

        private async Task RegistrationSuccessful()
        {
            try
            {
                var freshClient = await PrincipalApi.Get(setNewVersion: true, Cts.Token) ?? throw new NotificationException("Client null");
                var sub = freshClient?.GetActiveSubscription();

                if (sub != null && sub?.Product != null)
                {
                    Client = freshClient;
                    StateHasChanged();

                    await ShowSuccess(Translations.Module.Subscription.RegistrationSuccessful);
                }
                else
                {
                    Logger.Warning("RegistrationSuccessful called but subscription is not active.");
                    throw new NotificationException("Something went wrong with your subscription. Please contact support.");
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task AppleVerify(string receipt)
        {
            try
            {
                await PaymentAuthApi.AppleVerify(receipt, Cts.Token);

                var freshClient = await PrincipalApi.Get(setNewVersion: true, Cts.Token) ?? throw new NotificationException("Client null");
                var sub = freshClient?.GetActiveSubscription();

                if (sub != null && sub?.Product != null)
                {
                    Client = freshClient;
                    StateHasChanged();

                    await ShowSuccess(Translations.Module.Subscription.RegistrationSuccessful);
                }
                else
                {
                    throw new NotificationException("Something went wrong with your subscription. Please contact support.");
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }
    }
}
