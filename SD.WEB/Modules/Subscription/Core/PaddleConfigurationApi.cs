using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaddleConfigurationApi(IHttpClientFactory factory) : ApiCosmos<PaddleConfigurations>(factory, null)
{
    public async Task<PaddleConfigurations?> GetConfigurations()
    {
        return await GetAsync(Endpoint.Configurations, null);
    }

    public async Task<string?> GetCountry()
    {
        try
        {
            var response = await Http.GetAsync(new Uri("https://ipinfo.io/country"));
            return (await response.Content.ReadAsStringAsync()).Trim();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private struct Endpoint
    {
        public const string Configurations = "public/paddle/configurations";
    }
}