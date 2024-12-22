using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core
{
    public class PaddleConfigurationApi(IHttpClientFactory factory) : ApiCosmos<Configurations>(factory, null)
    {
        private struct Endpoint
        {
            public const string configurations = "public/paddle/configurations";
        }

        public async Task<Configurations?> GetConfigurations()
        {
            return await GetAsync(Endpoint.configurations, null);
        }
    }
}