using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;

namespace SD.API.Functions
{
    public class MySuggestionsFunction
    {
        private readonly IRepository _repo;

        public MySuggestionsFunction(IRepository repo)
        {
            _repo = repo;
        }

        [Function("MySuggestionsGet")]
        public async Task<MySuggestions?> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "MySuggestions/Get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                return await _repo.Get<MySuggestions>(DocumentType.MySuggestions + ":" + userId, new PartitionKey(userId), cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("MySuggestionsSync")]
        public async Task<MySuggestions?> Sync(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "MySuggestions/Sync/{MediaType}")] HttpRequestData req,
            string MediaType, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                var obj = await _repo.Get<MySuggestions>(DocumentType.MySuggestions + ":" + userId, new PartitionKey(userId), cancellationToken);
                var body = await req.GetPublicBody<MySuggestions>(cancellationToken);

                if (obj == null)
                {
                    obj = body;

                    obj.Initialize(userId);
                }
                else
                {
                    obj = body;

                    obj.Update();
                }

                var type = (MediaType)Enum.Parse(typeof(MediaType), MediaType);

                if (type == Shared.Enums.MediaType.movie)
                {
                    obj.MovieSyncDate = DateTime.Now;
                }
                else
                {
                    obj.ShowSyncDate = DateTime.Now;
                }

                return await _repo.Upsert(obj, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("MySuggestionsAdd")]
        public async Task<MySuggestions?> Add(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "MySuggestions/Add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var body = await req.GetBody<MySuggestions>(cancellationToken);

                return await _repo.Upsert(body, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}