using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using SD.API.Repository.Core;
using SD.Shared.Core.Models;
using SD.Shared.Models.Auth;
using SD.Shared.Models.Subscription;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;

namespace SD.API.Functions
{
    public class PaddleFunction(IRepository repo, IConfiguration configuration)
    {
        //endpoints to authorize
        //Live:    https://34.232.58.13,https://34.195.105.136,https://34.237.3.244,https://35.155.119.135,https://52.11.166.252,https://34.212.5.7
        //Sandbox: https://34.194.127.46,https://54.234.237.108,https://3.208.120.145,https://44.226.236.210,https://44.241.183.62,https://100.20.172.113

        [Function("GetSubscription")]
        public async Task<RootSubscription?> GetSubscription(
           [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];

                var endpoint = configuration.GetValue<string>("Paddle_Endpoint");
                var key = configuration.GetValue<string>("Paddle_Key");

                using var http = new HttpClient();

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}");

                var response = await http.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode) throw new NotificationException(response.ReasonPhrase);

                return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("GetSubscriptionUpdate")]
        public async Task<RootSubscription?> GetSubscriptionUpdate(
          [HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/paddle/subscription/update")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var id = req.GetQueryParameters()["id"];

                var endpoint = configuration.GetValue<string>("Paddle_Endpoint");
                var key = configuration.GetValue<string>("Paddle_Key");

                using var http = new HttpClient();

                http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

                using var request = new HttpRequestMessage(HttpMethod.Get, $"{endpoint}subscriptions/{id}/update-payment-method-transaction");

                var response = await http.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode) throw new NotificationException(response.ReasonPhrase);

                return await response.Content.ReadFromJsonAsync<RootSubscription>(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }

        [Function("PostSubscription")]
        public async Task PostSubscription(
            [HttpTrigger(AuthorizationLevel.Anonymous, Method.POST, Route = "public/paddle/subscription")] HttpRequestData req, CancellationToken cancellationToken)
        {
            try
            {
                var validSignature = await req.ValidPaddleSignature(configuration["Paddle_Signature"], cancellationToken);

                if (!validSignature) throw new NotificationException("wrong paddle signature");

                var body = await req.GetPublicBody<RootEvent>(cancellationToken) ?? throw new NotificationException("body null");
                if (body.data == null) throw new NotificationException("body.data null");

                var result = await repo.Query<ClientePrincipal>(x => body.data.customer_id == (x.ClientePaddle != null ? x.ClientePaddle.CustomerId : ""), null, DocumentType.Principal, cancellationToken);

                if (result == null) return;
                var client = result.FirstOrDefault() ?? throw new NotificationException("client null");
                if (client.ClientePaddle == null) throw new NotificationException("client.ClientePaddle null");

                client.ClientePaddle.SubscriptionId = body.data.id;

                foreach (var item in body.data.items)
                {
                    if (item == null) throw new NotificationException("item null");
                    if (item.price == null) throw new NotificationException("item.price null");

                    var localItem = client.ClientePaddle.Items.Find(f => f.ProductId == item.price.product_id) ?? throw new NotificationException("localItem null");
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

        [Function("Configurations")]
        public Configurations Configurations([HttpTrigger(AuthorizationLevel.Anonymous, Method.GET, Route = "public/paddle/configurations")] HttpRequestData req)
        {
            try
            {
                var config = new Configurations
                {
                    Token = configuration.GetValue<string>("Paddle_Token"),
                    ProductStandard = configuration.GetValue<string>("Paddle_ProductStandard"),
                    ProductPremium = configuration.GetValue<string>("Paddle_ProductPremium"),
                    PriceStandardMonth = configuration.GetValue<string>("Paddle_PriceStandardMonth"),
                    PriceStandardYear = configuration.GetValue<string>("Paddle_PriceStandardYear"),
                    PricePremiumMonth = configuration.GetValue<string>("Paddle_PricePremiumMonth"),
                    PricePremiumYear = configuration.GetValue<string>("Paddle_PricePremiumYear")
                };

                return config;
            }
            catch (Exception ex)
            {
                req.ProcessException(ex);
                throw new UnhandledException(ex.BuildException());
            }
        }
    }
}