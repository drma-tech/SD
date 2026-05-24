using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaymentConfigurationApi(IHttpClientFactory factory) : ApiCosmos<PaymentConfigurations>(factory, ApiType.Anonymous, null, ApiContext.Default.PaymentConfigurations)
{
    public async Task<PaymentConfigurations?> GetConfigurations(PaymentProvider provider, CancellationToken cancellationToken)
    {
        return await GetAsync(Endpoint.Configurations(provider), false, cancellationToken);
    }

    private struct Endpoint
    {
        public static string Configurations(PaymentProvider provider) => $"public/payment/configurations?provider={provider}";
    }
}