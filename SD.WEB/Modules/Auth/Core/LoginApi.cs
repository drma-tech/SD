using SD.Shared.Models.Auth;

namespace SD.WEB.Modules.Auth.Core;

public class LoginApi(IHttpClientFactory factory) : ApiCosmos<ClienteLogin>(factory, null)
{
    public async Task Add(string platform)
    {
        var ip = "";

        try
        {
            //TODO: TypeError: Failed to fetch
            var response = await _http.GetAsync(new Uri("https://ipinfo.io/ip"));
            ip = await response.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            ip = "0.0.0.0";
        }

        await PostAsync<ClienteLogin>(Endpoint.Add(platform, ip?.Trim()), null, null);
    }

    private struct Endpoint
    {
        public static string Add(string platform, string? ip)
        {
            return $"login/add?platform={platform}&ip={ip}";
        }
    }
}