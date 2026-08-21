using SD.Shared.Models.Subscription;
using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.Cosmos.Anonymous
{
    public class PaymentPublicApi(IHttpClientFactory factory) : ApiCosmos<PaymentConfigurations>(factory, ApiType.Anonymous, key: null, [], ApiContext.Default.PaymentConfigurations)
    {
        public async Task<PaymentConfigurations?> GetConfigurations(PaymentProvider provider, CancellationToken cancellationToken)
        {
            return await GetAsync($"public/payment/configurations?provider={provider}", setNewVersion: false, state: null, cancellationToken);
        }

        public async Task<bool> StripeValidateSession(string id, CancellationToken cancellationToken)
        {
            return await GetBoolAsync($"public/stripe/validate-session/{id}", cancellationToken);
        }
    }
}