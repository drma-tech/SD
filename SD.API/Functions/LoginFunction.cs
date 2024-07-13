using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Models.Auth;

namespace SD.API.Functions
{
    public class LoginFunction(IRepository repo)
    {
        //[OpenApiOperation("LoginAdd", "Azure (Cosmos DB)")]
        //[OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(void))]
        [Function("LoginAdd")]
        public async Task Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "login/add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var platform = req.GetQueryParameters()["platform"];
                if (platform.Empty()) platform = "webapp";
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");
                var login = await repo.Get<ClienteLogin>(DocumentType.Login + ":" + userId, cancellationToken);

                if (login == null)
                {
                    var newLogin = new ClienteLogin { UserId = userId, Logins = [DateTimeOffset.Now], Platforms = [platform!] };
                    newLogin.Initialize(userId);

                    await repo.Upsert(newLogin, cancellationToken);
                }
                else
                {
                    if (Array.Exists(login.Platforms, a => a == platform))
                    {
                        await repo.PatchItem<ClienteLogin>(nameof(DocumentType.Login) + ":" + userId,
                            [
                                PatchOperation.Add("/logins/-", DateTimeOffset.Now),
                            ], cancellationToken);
                    }
                    else
                    {
                        login.Logins = login.Logins.Union([DateTimeOffset.Now]).ToArray();
                        login.Platforms = login.Platforms.Union([platform]).ToArray();
                        await repo.Upsert(login, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}