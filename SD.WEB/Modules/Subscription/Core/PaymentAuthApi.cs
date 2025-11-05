using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaymentAuthApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Authenticated, null)
    {
        public async Task CreateCustomer()
        {
            await PostAsync(Endpoint.CreateCustomer, null, null);
        }

        public async Task AppleVerify(string receipt)
        {
            await PostAsync(Endpoint.AppleVerify, null, receipt);
        }

        private struct Endpoint
        {
            public const string CreateCustomer = "paddle/customer";
            public const string AppleVerify = "apple/verify";
        }
    }
}