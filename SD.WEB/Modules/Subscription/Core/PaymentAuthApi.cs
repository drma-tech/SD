using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaymentAuthApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Authenticated, null)
    {
        public async Task AppleVerify(string receipt)
        {
            await PostAsync(Endpoint.AppleVerify, null, receipt);
        }

        public async Task<AuthPrincipal?> StripeCustomer()
        {
            return await GetAsync<AuthPrincipal>(Endpoint.StripeCustomer);
        }

        private struct Endpoint
        {
            public const string AppleVerify = "apple/verify";
            public const string StripeCustomer = "stripe/customer";
        }
    }
}