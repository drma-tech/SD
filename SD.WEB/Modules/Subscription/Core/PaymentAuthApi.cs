using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaymentPublicApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Anonymous, null, ApiContext.Default.AuthSubscription)
    {
        public async Task<bool> StripeValidateSession(string id, CancellationToken cancellationToken)
        {
            return await GetAsync<bool>(Endpoint.StripeValidateSession(id), true, cancellationToken);
        }

        private struct Endpoint
        {
            public static string StripeValidateSession(string id) => $"public/stripe/validate-session/{id}";
        }
    }

    public class PaymentAuthApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Authenticated, null, ApiContext.Default.AuthSubscription)
    {
        public async Task AppleVerify(string receipt, CancellationToken cancellationToken)
        {
            await PostAsync(Endpoint.AppleVerify, receipt, ApiContext.Default.String, cancellationToken);
        }

        public async Task<AuthPrincipal?> StripeCustomer(bool isUserAuthenticated, CancellationToken cancellationToken)
        {
            if (!isUserAuthenticated) return null;

            return await GetAsync<AuthPrincipal>(Endpoint.StripeCustomer, true, cancellationToken);
        }

        public async Task<string?> StripePortalLink(CancellationToken cancellationToken)
        {
            return await GetStringAsync(Endpoint.StripePortalLink, cancellationToken);
        }

        private struct Endpoint
        {
            public const string AppleVerify = "apple/verify";
            public const string StripeCustomer = "stripe/customer";
            public const string StripePortalLink = "stripe/portal-link";
        }
    }
}