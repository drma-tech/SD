using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;
using System.Net;

namespace SD.API.Functions
{
    public class LoginFunction
    {
        private readonly IRepository _repo;

        public LoginFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("LoginAdd")]
        public async Task Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Login/Add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();
                if (string.IsNullOrEmpty(userId)) throw new InvalidOperationException("unauthenticated user");
                var login = await _repo.Get<ClienteLogin>(DocumentType.Login + ":" + userId, new PartitionKey(userId), cancellationToken);

                if (login == null)
                {
                    var newLogin = new ClienteLogin { UserId = userId, Logins = new DateTimeOffset[] { DateTimeOffset.Now } };
                    newLogin.Initialize(userId);

                    await _repo.Upsert(newLogin, cancellationToken);
                }
                else
                {
                    await _repo.PatchItem<ClienteLogin>(nameof(DocumentType.Login) + ":" + userId, new PartitionKey(userId), new List<PatchOperation> { PatchOperation.Add("/logins/-", DateTimeOffset.Now) }, cancellationToken);
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