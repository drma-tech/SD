using SD.Shared.Models.Auth;
using SD.WEB.Api.Core;

namespace SD.WEB.Api.Module.Cosmos.Admin
{
    public class PrincipalAdminApi(IHttpClientFactory factory) : ApiCosmos<AuthPrincipal>(factory, ApiType.Anonymous, "principal_import", [], ApiContext.Default.AuthPrincipal)
    {
        public async Task<IEnumerable<AuthPrincipal>> GetAll(CancellationToken cancellationToken)
        {
            return await GetListAsync("principal/get-all", states: [], cancellationToken);
        }

        public async Task Migrate(string? oldId, string? newId, CancellationToken cancellationToken)
        {
            await PutAsync($"principal/migrate/{oldId}/{newId}", null, ApiContext.Default.AuthPrincipal, states: [], cancellationToken);
        }
    }
}