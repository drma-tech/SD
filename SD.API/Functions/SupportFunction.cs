using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.Shared.Models.Support;
using System.Text.Json;

namespace SD.API.Functions
{
    public class SupportFunction(CosmosRepository repo)
    {
        [Function("country")]
        public async Task<HttpResponseData> GetCountry([HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/country/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            var userIp = req.Headers.GetValues("X-Forwarded-For").FirstOrDefault()?.Split(',')[0].Trim();

            string geoApiUrl = $"http://ip-api.com/json/{userIp}";
            var response = await ApiStartup.HttpClient.GetStringAsync(geoApiUrl);
            var obj = JsonSerializer.Deserialize<GeoLocationResponse>(response);

            return await req.CreateResponse(obj?.CountryCode, ttlCache.one_day, cancellationToken);
        }

        public sealed class GeoLocationResponse
        {
            public string? Country { get; set; }
            public string? CountryCode { get; set; }
        }

        [Function("UpdatesGet")]
        public async Task<HttpResponseData> UpdatesGet(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/updates/get")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var doc = await repo.ListAll<UpdateModel>(DocumentType.Update, cancellationToken);

                return await req.CreateResponse(doc, ttlCache.one_day, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("UpdatesAdd")]
        public async Task<UpdateModel> UpdatesAdd(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "adm/updates/add")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<UpdateModel>(cancellationToken);
                var dbItem = await repo.Get<UpdateModel>(DocumentType.Update, item.Id, cancellationToken);

                if (dbItem != null)
                {
                    dbItem.Title = item.Title;
                    dbItem.Description = item.Description;

                    return await repo.Upsert(dbItem, cancellationToken);
                }
                else
                {
                    return await repo.Upsert(item, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("UpdatesDelete")]
        public async Task UpdatesDelete(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.PUT, Route = "adm/updates/delete")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<UpdateModel>(cancellationToken);

                await repo.Delete(item, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("TicketGetList")]
        public async Task<List<TicketModel>> TicketGetList(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/ticket/get-list")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var userId = req.GetUserId();

                return await repo.Query<TicketModel>(m => m.TicketStatus != TicketStatus.New || m.IdUserOwner == userId, DocumentType.Ticket, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("TicketGetAll")]
        public async Task<List<TicketModel>> TicketGetAll(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "adm/ticket/get-all")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                return await repo.ListAll<TicketModel>(DocumentType.Ticket, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }

        [Function("TicketInsert")]
        public async Task<TicketModel?> TicketInsert(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Ticket/Insert")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var item = await req.GetPublicBody<TicketModel>(cancellationToken);

                return await repo.Upsert(item, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw;
            }
        }
    }
}