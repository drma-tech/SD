using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaymentAuthApi(IHttpClientFactory factory) : ApiCosmos<AuthSubscription>(factory, ApiType.Authenticated, null)
    {
        public async Task CreateCustomer()
        {
            await PostAsync(Endpoint.CreateCustomer, null, null);
        }

        private struct Endpoint
        {
            public const string CreateCustomer = "paddle/customer";
        }
    }
}