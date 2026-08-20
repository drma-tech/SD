using Microsoft.AspNetCore.Components;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Components
{
    public partial class SubscriptionPlans
    {
        [Parameter][EditorRequired] public PaymentProvider Provider { get; set; }
        [Parameter][EditorRequired] public AccountCycle Cycle { get; set; }
        [Parameter][EditorRequired] public AuthPrincipal? Client { get; set; }

        private PaymentConfigurations? Config { get; set; }
        private bool _processing;

        protected override async Task LoadStaticDataAsync()
        {
            Config = await ConfigurationApi.GetConfigurations(Provider, Cts.Token);
        }

        public static string? GetPrice(AccountProduct product, AccountCycle cycle)
        {
            return (product, cycle) switch
            {
                (AccountProduct.Premium, AccountCycle.Monthly) => "$1.99",
                (AccountProduct.Premium, AccountCycle.Yearly) => "$19.99",
                _ => "$0.00",
            };
        }

        protected async Task OpenCheckout()
        {
            try
            {
                _processing = true;

                if (AppStateStatic.IsAuthenticated)
                {
                    var priceId = Config?.GetPriceId(AccountProduct.Premium, Cycle);

                    if (priceId.Empty())
                    {
                        await ShowWarning($"Price not available for {AccountProduct.Premium.GetFieldSettings().Name}");
                        _processing = false; StateHasChanged();
                        return;
                    }

                    if (Provider == PaymentProvider.Paddle)
                    {
                        await ShowWarning($"Provider not available: {Provider.GetFieldSettings().Name}");
                        _processing = false; StateHasChanged();
                    }
                    else if (Provider == PaymentProvider.Apple)
                    {
                        await JsRuntime.Payments().AppleOpenCheckout(priceId, Cts.Token);
                    }
                    else if (Provider == PaymentProvider.Google)
                    {
                        await JsRuntime.Payments().GoogleOpenCheckout(priceId, "type", Cts.Token);
                    }
                    else if (Provider == PaymentProvider.Stripe)
                    {
                        if (Client != null && Client.StripeCustomerId.Empty())
                        {
                            Client = await PaymentAuthApi.StripeCustomer(Cts.Token);
                        }

                        //create session and redirect to checkout
                        await JsRuntime.Payments().StripeOpenCheckout(priceId, Cts.Token);
                    }
                    else
                    {
                        await ShowWarning($"Provider not implemented: {Provider.GetFieldSettings().Name}");
                        _processing = false; StateHasChanged();
                    }
                }
                else
                {
                    await ShowWarning(Translations.Module.Subscription.YouMustLoggedSubscribe);
                    _processing = false; StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
                _processing = false; StateHasChanged();
            }
            finally
            {
                await Task.Delay(5000, Cts.Token);
                _processing = false;
            }
        }
    }
}
