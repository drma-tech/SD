﻿namespace SD.WEB.Modules.Subscription.Core
{
    public class IpInfoApi(IHttpClientFactory factory) : ApiExternal(factory)
    {
        public async Task<string?> GetCountry()
        {
            try
            {
                return await GetValueAsync("https://ipinfo.io/country");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class IpInfoServerApi(IHttpClientFactory factory) : ApiCore(factory, null, ApiType.Anonymous)
    {
        public async Task<string?> GetCountry()
        {
            try
            {
                return await GetValueAsync("public/country");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}