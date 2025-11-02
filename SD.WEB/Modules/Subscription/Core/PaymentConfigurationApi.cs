using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaymentConfigurationApi(IHttpClientFactory factory) : ApiCosmos<PaymentConfigurations>(factory, ApiType.Anonymous, null)
{
    public async Task<PaymentConfigurations?> GetConfigurations(PaymentProvider provider)
    {
        return await GetAsync(Endpoint.Configurations(provider), null);
    }

    private struct Endpoint
    {
        public static string Configurations(PaymentProvider provider) => $"public/payment/configurations?provider={provider}";
    }
}