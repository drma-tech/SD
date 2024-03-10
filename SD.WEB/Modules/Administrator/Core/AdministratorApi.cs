using Microsoft.Extensions.Caching.Memory;
using SD.Shared.Models.Support;

namespace SD.WEB.Modules.Auth.Core
{
    public class AdministratorApi(IHttpClientFactory factory, IMemoryCache memoryCache) : ApiServices(factory, memoryCache)
    {
        private struct Endpoint
        {
            public const string GetEmails = "adm/emails";
            public const string EmailUpdate = "adm/emails/update";
            public const string SendEmail = "adm/send-email";
        }

        public async Task<List<EmailDocument>> GetEmails()
        {
            return await GetAsync<List<EmailDocument>>(Endpoint.GetEmails, false) ?? [];
        }

        public async Task SendEmail(SendEmail inbound)
        {
            await PostAsync(Endpoint.SendEmail, false, inbound);
        }

        public async Task EmailUpdate(EmailDocument email)
        {
            await PostAsync(Endpoint.EmailUpdate, false, email);
        }
    }
}