using SD.Shared.Models.Subscription;

namespace SD.WEB.Modules.Subscription.Core;

public class PaddleConfigurationApi(IHttpClientFactory factory) : ApiCosmos<Configurations>(factory, null)
{
    public async Task<Configurations?> GetConfigurations()
    {
        return await GetAsync(Endpoint.configurations, null);
    }

    public async Task<string?> GetCountry()
    {
        try
        {
            var response = await _http.GetAsync(new Uri("https://ipinfo.io/country"));
            return (await response.Content.ReadAsStringAsync()).Trim();
        }
        catch (Exception)
        {
            return null;
        }
    }

    private struct Endpoint
    {
        public const string configurations = "public/paddle/configurations";
    }
}