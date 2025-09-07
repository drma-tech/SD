using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaddleConfigurationApi(IHttpClientFactory factory) : ApiCosmos<PaddleConfigurations>(factory, null)
{
    public async Task<PaddleConfigurations?> GetConfigurations()
    {
        return await GetAsync(Endpoint.Configurations, null);
    }

    private struct Endpoint
    {
        public const string Configurations = "public/paddle/configurations";
    }
}