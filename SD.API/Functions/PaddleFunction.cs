using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;

namespace SD.API.Functions
{
    public class PaddleFunction(IRepository repo)
    {
        //Live:    https://34.232.58.13,https://34.195.105.136,https://34.237.3.244,https://35.155.119.135,https://52.11.166.252,https://34.212.5.7
        //Sandbox: https://34.194.127.46,https://54.234.237.108,https://3.208.120.145,https://44.226.236.210,https://44.241.183.62,https://100.20.172.113

        [Function("Subscription")]
        public async Task Subscription(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "Paddle/Subscription")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var body = await req.GetPublicBody<SubscriptionPaddle>(cancellationToken);
                if (body == null) throw new NotificationException("body null");
                if (body.data == null) throw new NotificationException("body.data null");

                var result = await repo.Query<ClientePrincipal>(x => body.data.customer_id == (x.ClientePaddle != null ? x.ClientePaddle.CustomerId : ""), null, DocumentType.Principal, cancellationToken);

                if (result == null) throw new NotificationException("result null");
                var client = result.FirstOrDefault();
                if (client == null) throw new NotificationException("client null");
                if (client.ClientePaddle == null) throw new NotificationException("client.ClientePaddle null");

                foreach (var item in body.data.items)
                {
                    if (item == null) throw new NotificationException("item null");
                    if (item.price == null) throw new NotificationException("item.price null");

                    var localItem = client.ClientePaddle.Items.Find(f => f.ProductId == item.price.product_id);
                    if (localItem == null) throw new NotificationException("localItem null");

                    localItem.Active = (body.data.status == "active" || body.data.status == "trialing") && (item.status == "active" || item.status == "trialing");
                }

                await repo.Upsert(client, cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}