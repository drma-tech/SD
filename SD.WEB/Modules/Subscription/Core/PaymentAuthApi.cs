using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaymentPublicApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Anonymous, null)
    {
        public async Task<bool> StripeValidateSession(string id)
        {
            return await GetAsync<bool>(Endpoint.StripeValidateSession(id));
        }

        private struct Endpoint
        {
            public static string StripeValidateSession(string id) => $"public/stripe/validate-session/{id}";
        }
    }

    public class PaymentAuthApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Authenticated, null)
    {
        public async Task AppleVerify(string receipt)
        {
            await PostAsync(Endpoint.AppleVerify, receipt);
        }

        public async Task<AuthPrincipal?> StripeCustomer(bool isUserAuthenticated)
        {
            if (!isUserAuthenticated) return null;

            return await GetAsync<AuthPrincipal>(Endpoint.StripeCustomer);
        }

        public async Task<string?> StripePortalLink()
        {
            return await GetStringAsync(Endpoint.StripePortalLink);
        }

        private struct Endpoint
        {
            public const string AppleVerify = "apple/verify";
            public const string StripeCustomer = "stripe/customer";
            public const string StripePortalLink = "stripe/portal-link";
        }
    }
}