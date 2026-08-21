using SD.Shared.Models.Auth;
using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.Cosmos.Anonymous
{
    public class PublicLoginApi(IHttpClientFactory factory) : ApiCosmos<AuthLogin>(factory, ApiType.Anonymous, key: null, [], ApiContext.Default.AuthLogin)
    {
        public async Task SendEmail(string? email, string? reference, CancellationToken cancellationToken)
        {
            await PostAsync($"public/login/email?email={email}&reference={(reference ?? "error")}", null, null, cancellationToken);
        }

        public async Task<string?> StatusEmail(string? reference, CancellationToken cancellationToken)
        {
            return await GetStringAsync($"public/login/status?reference={reference ?? "error"}", cancellationToken);
        }
    }
}